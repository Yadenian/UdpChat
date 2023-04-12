using System.Net;
using System.Net.Sockets;
using System.Text;


IPAddress localAddress = IPAddress.Parse("127.0.0.1");
Console.Write("Введите свое УНИКАЛЬНОЕ имя: ");
string? username = Console.ReadLine();

if (username == null)
{
    Console.WriteLine("Ocho slona");
    return;
}

var uniq = await CheckUserAsync();

if (uniq)
{
    Console.WriteLine("Вы правильно написали.");
    await ShowPreviousMessagesAsync();
}
else
{
    Console.WriteLine("Вы тут впервые");

    using (StreamWriter stream = new StreamWriter("Users.txt", true))
    {
        string chatmessage = username;
        stream.WriteLine(chatmessage);
    }

    using (FileStream stream = File.Create($"{username}User.txt")) { }
}

Console.Write("Введите порт для приема сообщений: ");
if (!int.TryParse(Console.ReadLine(), out var localPort)) return;
Console.Write("Введите порт для отправки сообщений: ");
if (!int.TryParse(Console.ReadLine(), out var remotePort)) return;
Console.WriteLine();


Task.Run(ReceiveMessageAsync);

await SendMessageAsync();


async Task SendMessageAsync()
{
    using Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    Console.WriteLine("Для отправки сообщений введите сообщение и нажмите Enter");
    
    while (true)
    {
        var message = Console.ReadLine(); 

        if (string.IsNullOrWhiteSpace(message)) break;

        message = $"{username}: {message}";
        byte[] data = Encoding.UTF8.GetBytes(message);

        await sender.SendToAsync(data, new IPEndPoint(localAddress, remotePort));
        using (StreamWriter stream = new StreamWriter($"{username}User.txt", true))
        {
            await stream.WriteLineAsync(message);
        }
    }
}

async Task ReceiveMessageAsync()
{
    byte[] data = new byte[65535];

    using Socket receiver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

    receiver.Bind(new IPEndPoint(localAddress, localPort));
    while (true)
    {

        var result = await receiver.ReceiveFromAsync(data, new IPEndPoint(IPAddress.Any, 0));
        var message = Encoding.UTF8.GetString(data, 0, result.ReceivedBytes);

        Console.WriteLine(message);

        using (StreamWriter stream = new StreamWriter($"{username}User.txt", true))
        {
            await stream.WriteLineAsync(message);
        }
    }
}

async Task<bool> CheckUserAsync()
{
    try
    {
        using (StreamReader stream = new StreamReader("Users.txt"))
        {
            string? line;
            while ((line = await stream.ReadLineAsync()) != null)
            {
                if (line == username)
                {
                    return true;
                }
            }
        }
    }
    catch(Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }

    return false;
}

async Task ShowPreviousMessagesAsync()
{
    using (StreamReader stream = new StreamReader($"{username}User.txt"))
    {
        string? line;
        while ((line = await stream.ReadLineAsync()) != null)
        {
            Console.WriteLine(line);
        }
    }
}