using System.Net.Sockets;
using System.Text;

namespace RapidSharpTestTools.TelnetFtp
{
    /// <summary>
    /// Класс telnet-клиента для подключению к серверу
    /// </summary>
    public class TelnetClient
    {
        readonly TcpClient tcpSocket;

        /// <summary>
        /// Инициализирует подключение по протоколу Telnet.
        /// </summary>
        /// <param name="Hostname">Имя хоста (IP).</param>
        /// <param name="Port">Порт. По умолчанию 23.</param>
        public TelnetClient(string Hostname, int Port = 23)
        {
            try
            {
                tcpSocket = new TcpClient(Hostname, Port);
            }
            catch (SocketException socketException)
            {
                System.Diagnostics.Debug.WriteLine("Не удалось подключиться к данному хосту. Нажмите любую клавишу...");
            }
        }
        /// <summary>
        /// Возвращает статус, есть ли подключение на данный момент.
        /// </summary>
        public bool IsConnected => tcpSocket.Connected;

        enum Verbs
        {
            WILL = 251,
            WONT = 252,
            DO = 253,
            DONT = 254,
            IAC = 255
        }
        enum Options
        {
            SGA = 3
        }
        private void ParseTelnet(StringBuilder sb)
        {
            var stream = tcpSocket.GetStream();
            while (tcpSocket.Available > 0)
            {
                int input = stream.ReadByte();
                switch (input)
                {
                    case -1:
                        break;
                    case (int)Verbs.IAC:
                        // interpret as command
                        int inputverb = stream.ReadByte();
                        if (inputverb == -1)
                            break;
                        switch (inputverb)
                        {
                            case (int)Verbs.IAC:
                                //literal IAC = 255 escaped, so append char 255 to string
                                sb.Append(inputverb);
                                break;
                            case (int)Verbs.DO:
                            case (int)Verbs.DONT:
                            case (int)Verbs.WILL:
                            case (int)Verbs.WONT:
                                // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                                int inputoption = stream.ReadByte();
                                if (inputoption == -1)
                                    break;
                                stream.WriteByte((byte)Verbs.IAC);
                                if (inputoption == (int)Options.SGA)
                                    stream.WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WILL : (byte)Verbs.DO);
                                else
                                    stream.WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WONT : (byte)Verbs.DONT);
                                stream.WriteByte((byte)inputoption);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        sb.Append((char)input);
                        break;
                }
            }
        }
        /// <summary>
        /// Отправляет команду удаленному серверу.
        /// </summary>
        /// <param name="command">Консольная команда.</param>
        public void SendCommand(string command)
        {
            if (!tcpSocket.Connected)
                return;
            byte[] buf = System.Text.ASCIIEncoding.ASCII.GetBytes((command + '\n').Replace("\0xFF", "\0xFF\0xFF"));
            tcpSocket.GetStream().Write(buf, 0, buf.Length);
        }
        /// <summary>
        /// Получает ответ на отправленную команду.
        /// </summary>
        /// <returns></returns>
        public string ReadAnswer(int timeoutMs = 2000)
        {
            if (!tcpSocket.Connected)
                return null;
            StringBuilder sb = new StringBuilder();
            do
            {
                ParseTelnet(sb);
                System.Threading.Thread.Sleep(timeoutMs);
            } while (tcpSocket.Available > 0);
            return sb.ToString();
        }
    }
}