using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Zadejte MAC Adresu: ");
        string macAddress = Console.ReadLine();

        try
        {
            PoslatWakeOnLan(macAddress);
            Console.WriteLine($"Magic packet se úspěšně poslal na {macAddress}");
            LogToFile($"Magic packet se úspěšně poslal na {macAddress}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Nepodařilo se poslat magic packet: {ex.Message}");
            LogToFile($"Chyba: {ex.Message}");
        }
    }

    public static void PoslatWakeOnLan(string macAddress)
    {
        byte[] macBytes = VybratMacAdresu(macAddress);

        byte[] packet = new byte[102];
        for (int i = 0; i < 6; i++)
        {
            packet[i] = 0xFF;
        }
        for (int i = 1; i <= 16; i++)
        {
            Array.Copy(macBytes, 0, packet, i * 6, 6);
        }

        using (UdpClient client = new UdpClient())
        {
            client.Connect(IPAddress.Broadcast, 9);
            client.Send(packet, packet.Length);
        }
    }

    private static byte[] VybratMacAdresu(string macAddress)
    {
        string[] macSegments = macAddress.Split(':', '-');
        if (macSegments.Length != 6)
        {
            throw new ArgumentException("Nesprávný formát MAC Adresy");
        }

        byte[] macBytes = new byte[6];
        for (int i = 0; i < 6; i++)
        {
            macBytes[i] = Convert.ToByte(macSegments[i], 16);
        }

        return macBytes;
    }

    private static void LogToFile(string message)
    {
        string logFile = "log.txt";
        using (StreamWriter writer = File.AppendText(logFile))
        {
            writer.WriteLine($"{DateTime.Now} - {message}");
        }
    }
}
