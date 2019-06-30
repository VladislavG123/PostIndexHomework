using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PostIndex
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345));
            socket.Listen(100);
            Thread serverThread = new Thread(ServerThreadProcedure);
            serverThread.Start(socket);
        }

        private async static void ServerThreadProcedure(object obj)
        {
            Socket serverSocket = obj as Socket;
            Socket clientSocket = serverSocket.Accept();
            while (true)
            {
                byte[] buffer = new byte[4 * 1024];
                int receiveSize = clientSocket.Receive(buffer);
                if (receiveSize == 0)
                {
                    continue;
                }
                string postcode = Encoding.UTF8.GetString(buffer);

                var info = await GetPostIndexInfo(postcode);

                string message = "";
                foreach (var item in info.Data)
                {
                    message += item.Address + "\n";
                }
                clientSocket.Send(Encoding.UTF8.GetBytes(message));
            }
        }

        private static Task<PostIndexData> GetPostIndexInfo(string postcode)
        {
            return Task.Run(() =>
            {
                WebRequest request = WebRequest.Create($"https://api.post.kz/api/byPostcode/{postcode}?from=0");

                WebResponse response = request.GetResponse();
                string json = "";
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line = "";
                        while ((line = reader.ReadLine()) != null)
                        {
                            json += line;
                        }
                    }
                }
                response.Close();

                return JsonConvert.DeserializeObject<PostIndexData>(json);
            });
        }
    }
}
