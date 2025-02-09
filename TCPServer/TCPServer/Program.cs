using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TCPServer
{
    private static TcpListener listener;
    private static int port = 13000;

    public static void Main(string[] args)
    {
        try
        {   //запуск сервера
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            listener = new TcpListener(ipAddress, port);
            listener.Start();
            Console.WriteLine($"Сервер запущен и слушает порт {port}");

            while (true)
            {
                Console.WriteLine("Ожидание входящего подключения...");
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Подключен клиент!");

                //обработка клиента в отдельном потоке
                Thread clientThread = new Thread(() => ProcessClient(client));
                clientThread.Start();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Произошла ошибка: {e.Message}");
        }
        finally
        {
            listener?.Stop();
            Console.WriteLine("Сервер остановлен.");
        }
    }

    private static void ProcessClient(TcpClient client)
    {
        NetworkStream stream = null;
        string clientAddress = client.Client.RemoteEndPoint?.ToString(); //получаем адрес клиента

        try
        {
            stream = client.GetStream();
            byte[] buffer = new byte[1024];
            string receivedMessage;

            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    Console.WriteLine($"Клиент {clientAddress} отключился.");
                    break; //клиент закрыл соединение
                }

                receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim(); //используем UTF8 и Trim()

                Console.WriteLine($"Получено от клиента {clientAddress}: {receivedMessage}");

                switch (receivedMessage.ToLower()) //приводим к нижнему регистру для сравнения
                {
                    case "hello":
                        string helloResponse = "Hello, Client!";
                        byte[] helloBytes = Encoding.UTF8.GetBytes(helloResponse);
                        stream.Write(helloBytes, 0, helloBytes.Length);
                        Console.WriteLine($"Отправлен ответ клиенту {clientAddress}: {helloResponse}");
                        break;

                    case "exit":
                        Console.WriteLine($"Клиент {clientAddress} запросил закрытие соединения.");
                        break; //выходим из цикла, что приведет к закрытию соединения

                    default:
                        string unknownResponse = $"Неизвестная команда: {receivedMessage}";
                        byte[] unknownBytes = Encoding.UTF8.GetBytes(unknownResponse);
                        stream.Write(unknownBytes, 0, unknownBytes.Length);
                        Console.WriteLine($"Отправлен ответ клиенту {clientAddress}: {unknownResponse}");
                        break;
                }

                if (receivedMessage.ToLower() == "exit")
                {
                    break; //закрываем соединение, если клиент отправил "exit"
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка обработки клиента {clientAddress}: {e.Message}");
        }
        finally
        {
            stream?.Close();
            client?.Close();
            Console.WriteLine($"Соединение с клиентом {clientAddress} закрыто.");
        }
    }
}