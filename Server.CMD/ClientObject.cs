using Server.CMD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
 
namespace ConsoleServer
{
    public class ClientObject
    {
        public TcpClient client;
        public List<Message> Messages { get; set; }
        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
            Messages = ReadFromFile<List<Message>>() ?? new List<Message>();
        }

        public void Process()
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                BinaryReader br = new BinaryReader(stream);
                BinaryWriter wr = new BinaryWriter(stream);

                List<Message> selectedMessages;
                
                ReadDataFromClient(br, out selectedMessages);
                SendMessagesToClient(wr, selectedMessages);                
                    
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }

        private void ReadDataFromClient(BinaryReader stream,
            out List<Message> selectedMessages)
        {
            string fromUser = stream.ReadString();
            string toUser = stream.ReadString();
            string subject = stream.ReadString();
            string message = stream.ReadString();

            var dataMessage = new Message(toUser, fromUser, subject, message);
            Messages.Add(dataMessage);
            SaveToFile(Messages);

            selectedMessages = Messages.Where(u => u.To == fromUser).ToList();        
            
        }

        private void SendMessagesToClient(BinaryWriter streamWriter, List<Message> selectedMessages)
        {
            if (selectedMessages.Count > 0)
            {
                streamWriter.Write(selectedMessages.Count);
                foreach (var messageToSend in selectedMessages)
                {
                    string finalMessage = string.Format($"Subject: {messageToSend.Subject}\n " +
                        $"{messageToSend.Mes}\n" +
                        $"From: {messageToSend.From}\n");

                    Console.WriteLine(finalMessage);

                    streamWriter.Write(finalMessage);
                }
            }
            else
            {
                streamWriter.Write(1);
                streamWriter.Write("No messages");
            }
        }
        private void SaveToFile<T>(T value)
        {
            var formatter = new BinaryFormatter();

            using (var fs = new FileStream("messages.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, value);
            }
        }

        private T ReadFromFile<T>()
        {
            var formatter = new BinaryFormatter();

            using (var fs = new FileStream("messages.dat", FileMode.OpenOrCreate))
            {
                if (fs.Length > 0 && formatter.Deserialize(fs) is T values)
                {
                    return values;
                }
                else
                {
                    return default(T);
                }
            }
        }
    }
}