using System;
using System.Net.Sockets;
using System.Text;
using System.Threading; // Для Thread.Sleep (если нужно)

public class TCPClient
{
    public static void Main(string[] args)
    {
        string serverAddress = "127.0.0.1"; // IP-адрес сервера
        int serverPort = 13000;              // Порт сервера

        try
        {
            // 1. Создание TcpClient
            TcpClient client = new TcpClient(serverAddress, serverPort);
            Console.WriteLine("Подключен к серверу.");

            // 2. Получение NetworkStream
            NetworkStream stream = client.GetStream();

            // Функция для отправки сообщений и получения ответа
            string SendMessageAndGetResponse(string messageToSend)
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(messageToSend);
                stream.Write(messageBytes, 0, messageBytes.Length);
                Console.WriteLine($"Отправлено серверу: {messageToSend}");

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Получено от сервера: {response}");
                return response;
            }

            // 3. Отправка сообщения "hello" (без \n)
            SendMessageAndGetResponse("hello");

            // 4. Отправка сообщения "exit" (без \n)
            SendMessageAndGetResponse("exit");

            // 5. Закрытие соединения (если сервер его еще не закрыл)
            stream.Close();
            client.Close();
            Console.WriteLine("Соединение закрыто.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Произошла ошибка: {e.Message}");
        }
    }
}