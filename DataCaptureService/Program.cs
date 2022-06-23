using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DataCaptureService
{
    class Program
    {
        static string filePath = @"C:\Users\Mane_Stepanyan\source\repos\Message-Queue\DataCaptureService\Data\f{0}.txt";
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
                for(int i = 1; i <= 5; i++)
                {
                    byte[] buffer = null;
                    using (FileStream fs = new FileStream(string.Format(filePath, i), FileMode.Open, FileAccess.Read))
                    {
                        buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, (int)fs.Length);
                    }
                    var msg = BytesToString(buffer);
                    messageQueue.Send(msg);
                }
                
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
        static string BytesToString(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}
