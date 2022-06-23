using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessingService
{
    class Program
    {
        static string filePath = @"C:\Users\Mane_Stepanyan\source\repos\Message-Queue\DataProcessingService\LocalFolder/WriteLines.txt";
        static void Main(string[] args)
        {
            MessageQueue messageQueue = null;
            string path = @".\Private$\DataCaptureQueue1";
            try
            {
               if (MessageQueue.Exists(path))
               {
                   messageQueue = new MessageQueue(path);
               }

               else
               {
                    MessageQueue.Create(path);
                    messageQueue = new MessageQueue(path);
               }

                messageQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(MyReceiveCompleted);
                messageQueue.BeginReceive();
                Console.ReadKey();
            }
            catch
            {
                throw;
            }
            finally
            {
                messageQueue.Dispose();
            }
        }
        private static void MyReceiveCompleted(Object source,
           ReceiveCompletedEventArgs asyncResult)
        {
            try
            {
                // Connect to the queue.
                MessageQueue mq = (MessageQueue)source;

                // End the asynchronous receive operation.
                Message m = mq.EndReceive(asyncResult.AsyncResult);

                m.Formatter = new XmlMessageFormatter(new String[] { "System.String, mscorlib" });
                byte[] msg = Encoding.ASCII.GetBytes(m.Body.ToString());
                //Console.WriteLine(msg);
                using (var stream = new FileStream(filePath, FileMode.Append))
                {
                    stream.Write(msg, 0, msg.Length);
                }
                File.AppendAllText(filePath, Environment.NewLine);
                // Restart the asynchronous receive operation.
                mq.BeginReceive();
            }
            catch (MessageQueueException)
            {
                // Handle sources of MessageQueueException.
            }
            return;
        }
    }
}
