using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BytesRoad.Net.Ftp;

namespace RapidSharpTestTools.TelnetFtp
{
    public enum ETypeItem
    {
        Any = 0,
        Directory = 1,
        File = 3,
        Driver = 4,
    }


    public class TelnetFtp
    {
        #region Private
        /// <summary>
        /// Максимальное количество прыжков для определения типа ссылки
        /// </summary>
        private const int MaxEnterLinkFtp = 10;
        private bool IsLinux { get; }
        private readonly FtpClient _ftp;
        private TelnetClient _telnet;
        private bool _loginTelnet;
        private bool _loginFtp;
        private const int TimeoutFtp = 30000;
        private int OffsetFtpItemPath => IsLinux ? 56 : 47;
        #endregion

        public string Host { get; }
        public string Username { get; }
        public string Password { get; }
        /// <summary>
        /// Пароль суперпользователя root
        /// </summary>
        public string PasswordSU { get; }
        public string UsernameFTP { get; }
        public string PasswordFTP { get; }

        public const string ErrorTelnet = "ОШИБКА: Подключение Telnet";

        /// <summary>
        /// Для QNX достаточно задать host, user, pass (стандартно: 10.0.0.10, "root", "1")
        /// Для Linux задать host, user, pass, passSU (стандартно: 10.0.0.10, "user", "user", "root")
        /// </summary>
        /// <param name="host"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="isQNX"></param>
        public TelnetFtp(string host, string user, string pass, bool isQNX = false) :
              this(host, user, pass, "")
        {
            IsLinux = !isQNX;
            UsernameFTP = user;
            PasswordFTP = pass;
        }

        public TelnetFtp(string host, string user, string pass, string passSU = "", string passFTP = "")
        {
            IsLinux = true;
            Host = host;
            Username = user;
            Password = pass;
            PasswordSU = passSU;
            _telnet = new TelnetClient(Host);

            UsernameFTP = string.IsNullOrWhiteSpace(passSU) ? "root" : passSU;
            PasswordFTP = string.IsNullOrWhiteSpace(passFTP) ? "root" : passFTP;
            _ftp = new FtpClient();
        }

        public bool Reconnect()
        {
            _loginFtp = _loginTelnet = false;
            _telnet = new TelnetClient(Host);
            return true;
        }

        public string Execute(string command, int timeout = 5000)
        {
            if (!ПодготовкаTelnet())
                return ErrorTelnet;
            _telnet.SendCommand(command);
            return WaitReadAnswer(timeout);
        }

        public void ExecuteWithoutAnswer(string command)
        {
            if (ПодготовкаTelnet())
                _telnet.SendCommand(command);
        }

        public bool PutFile(string remoteFilename, Stream file, short chmod = 0666)
        {
            if (!ПодготовкаFtp())
                return false;
            try
            {
                _ftp.PutFile(TimeoutFtp, remoteFilename, file);
                return ChmodFile(remoteFilename, chmod);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool GetFile(string remoteFilename, Stream file)
        {
            if (!ПодготовкаFtp())
                return false;

            try
            {
                _ftp.GetFile(TimeoutFtp, file, remoteFilename);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool RenameFile(string srcPath, string dstPath)
        {
            if (!ПодготовкаFtp())
                return false;

            try
            {
                _ftp.RenameFile(TimeoutFtp, srcPath, dstPath);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool ExistFile(string remoteFilename)
        {
            IDictionary<string, ETypeItem> items;
            remoteFilename = remoteFilename.TrimEnd('/');
            if (!СписокФайловПапок(remoteFilename.Substring(0, remoteFilename.LastIndexOf('/')), out items))
                return false;
            ETypeItem typeItem;
            return items.TryGetValue(remoteFilename, out typeItem) && typeItem == ETypeItem.File;
        }

        public bool GetChmodFile(string remoteFilename, out short chmod)
        {
            chmod = 0;
            if (!ПодготовкаFtp())
                return false;
            try
            {
                var res = _ftp.SendCommand(TimeoutFtp, $"STAT {remoteFilename}");
                var permissions = res.RawString.Substring(res.RawString.IndexOf('\n') + 2, 9);
                int result = 0;
                string part = permissions.Substring(0, 3);
                switch (part)
                {
                    case "---":
                        break;
                    case "--x":
                        result += 100;
                        break;
                    case "-w-":
                        result += 200;
                        break;
                    case "-wx":
                        result += 300;
                        break;
                    case "r--":
                        result += 400;
                        break;
                    case "r-x":
                        result += 500;
                        break;
                    case "rw-":
                        result += 600;
                        break;
                    case "rwx":
                        result += 700;
                        break;
                    default:
                        Debug.WriteLine("SORE");
                        break;
                }
                part = permissions.Substring(3, 3);
                switch (part)
                {
                    case "---":
                        break;
                    case "--x":
                        result += 10;
                        break;
                    case "-w-":
                        result += 20;
                        break;
                    case "-wx":
                        result += 30;
                        break;
                    case "r--":
                        result += 40;
                        break;
                    case "r-x":
                        result += 50;
                        break;
                    case "rw-":
                        result += 60;
                        break;
                    case "rwx":
                        result += 70;
                        break;
                    default:
                        Debug.WriteLine("SORE");
                        break;
                }
                part = permissions.Substring(6, 3);
                switch (part)
                {
                    case "---":
                        break;
                    case "--x":
                        result += 1;
                        break;
                    case "-w-":
                        result += 2;
                        break;
                    case "-wx":
                        result += 3;
                        break;
                    case "r--":
                        result += 4;
                        break;
                    case "r-x":
                        result += 5;
                        break;
                    case "rw-":
                        result += 6;
                        break;
                    case "rwx":
                        result += 7;
                        break;
                    default:
                        Debug.WriteLine("SORE");
                        break;
                }
                chmod = (short)result;
            }
            catch (Exception e)
            {
                WriteCodeFTP(e);
                return false;
            }
            return true;
        }

        public bool DeleteFile(string remoteFilename)
        {
            if (!ПодготовкаFtp())
                return false;

            try
            {
                _ftp.DeleteFile(TimeoutFtp, remoteFilename);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool CreateFolder(string remoteFilename)
        {
            if (!ПодготовкаFtp())
                return false;

            try
            {
                var res = _ftp.CreateDirectory(TimeoutFtp, remoteFilename);
            }
            catch (FtpErrorException e)
            {
                Debug.WriteLine(e.StackTrace);
                return false;
            }
            return true;
        }
        public bool GetFileList(string remotePath, out IList<string> files)
        {
            IDictionary<string, ETypeItem> typedItems;
            if (СписокФайловПапок(remotePath, out typedItems))
            {
                files = typedItems.Where(i => i.Value == ETypeItem.File).Select(i => i.Key).ToList();
                return true;
            }
            files = null;
            return false;
        }

        public bool GetFolderList(string remotePath, out IList<string> folders)
        {
            IDictionary<string, ETypeItem> typedItems;
            if (СписокФайловПапок(remotePath, out typedItems))
            {
                folders = typedItems.Where(i => i.Value == ETypeItem.Directory).Select(i => i.Key).ToList();
                return true;
            }
            folders = null;
            return false;
        }

        public bool GetItemList(string remotePath, out IList<string> items)
        {
            IDictionary<string, ETypeItem> typedItems;
            if (СписокФайловПапок(remotePath, out typedItems))
            {
                items = typedItems.Select(i => i.Key).ToList();
                return true;
            }
            items = null;
            return false;
        }

        private bool СписокФайловПапок(string remotePath, out IDictionary<string, ETypeItem> items, string filterItem = "", int countEnter = 0)
        {
            if (countEnter > MaxEnterLinkFtp)
            {
                items = null;
                return false;
            }

            items = new Dictionary<string, ETypeItem>();
            if (!ПодготовкаFtp())
                return false;

            try
            {
                if (!remotePath.EndsWith("/"))
                    remotePath += "/";
                var res = _ftp.GetDirectoryList(TimeoutFtp, remotePath);

                foreach (var item in res)
                {   //Linux
                    var dir = FtpItem2Path(item);
                    if (dir == string.Empty)
                        continue;
                    var fullPath = remotePath + GetItemPath(dir);
                    if (filterItem != string.Empty && filterItem != fullPath)
                        continue;
                    var typeItem = GetTypeItem(item);
                    var linkPath = GetLinkPath(dir);
                    if (typeItem == ETypeItem.Any && linkPath != string.Empty)
                    {
                        if (!linkPath.StartsWith("/"))
                            linkPath = remotePath + linkPath;
                        IDictionary<string, ETypeItem> linkItems;
                        if (СписокФайловПапок(linkPath.Substring(0, linkPath.LastIndexOf("/", StringComparison.Ordinal)),
                          out linkItems, linkPath, countEnter + 1))
                        {
                            ETypeItem linkTypeItem;
                            if (linkItems.TryGetValue(linkPath, out linkTypeItem))
                                typeItem = linkTypeItem;
                        }
                    }
                    items.Add(fullPath, typeItem);
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return false;
            }
        }

        string FtpItem2Path(FtpItem item)
        {
            if (item.RawString.Length < OffsetFtpItemPath)
                return string.Empty;
            var dir = item.RawString.Substring(OffsetFtpItemPath);
            return dir == "." || dir == ".." ? string.Empty : dir;
        }

        string GetLinkPath(string item)
        {
            var link = item.IndexOf(" -> ", StringComparison.Ordinal);
            return link > 0 ? item.Substring(link + 4) : string.Empty;
        }

        string GetItemPath(string item)
        {
            var link = item.IndexOf(" -> ", StringComparison.Ordinal);
            return link > 0 ? item.Substring(0, link) : item;
        }



        public bool ExistFolder(string remoteFilename)
        {
            IDictionary<string, ETypeItem> items;
            remoteFilename = remoteFilename.TrimEnd('/');
            if (!СписокФайловПапок(remoteFilename.Substring(0, remoteFilename.LastIndexOf('/')), out items))
                return false;
            ETypeItem typeItem;
            return items.TryGetValue(remoteFilename, out typeItem);
        }

        public bool DeleteFolder(string remoteFilename)
        {
            if (!ПодготовкаFtp())
                return false;
            try
            {
                _ftp.DeleteDirectory(TimeoutFtp, remoteFilename);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool ChmodFile(string remoteFilename, short chmod)
        {
            if (!ПодготовкаFtp())
                return false;

            try
            {
                var res = _ftp.SendCommand(TimeoutFtp, $"SITE CHMOD {chmod} {remoteFilename}");
            }
            catch (Exception e)
            {
                WriteCodeFTP(e);
                return false;
            }
            return true;
        }

        public string ReadAnswer()
        {
            return WaitReadAnswer();
        }

        #region "private"
        bool ПодготовкаTelnet()
        {
            if (_loginTelnet)
                return true;

            var isConnect = EnterAndWait("login:");
            if (!isConnect)
                return false;

            _telnet.SendCommand(Username);
            var isPassword = EnterAndWait("password:");
            if (!isPassword)
                return false;

            _telnet.SendCommand(Password);
            if (IsLinux)
            {
                if (!string.IsNullOrWhiteSpace(PasswordSU))
                {
                    var isEnterUser = EnterAndWait("$");
                    if (!isEnterUser)
                        return false;

                    _telnet.SendCommand("su");
                    isPassword = EnterAndWait("password:");
                    if (!isPassword)
                        return false;

                    _telnet.SendCommand(PasswordSU);
                }
            }

            var isEnterCmd = EnterAndWait("#");
            if (!isEnterCmd)
                return false;

            return _loginTelnet = true;
        }

        private bool EnterAndWait(string question)
        {
            var sw = new Stopwatch();
            var login = _telnet.ReadAnswer(10);
            sw.Start();
            var alive = true;
            while (!login.TrimEnd().ToLower().EndsWith(question.ToLower()))
            {
                if (sw.Elapsed.TotalSeconds < 5)
                    login = _telnet.ReadAnswer(10);
                else
                {
                    Debug.WriteLine(login.Trim().Trim('\0') + " Ожидали вопрос: " + question);
                    alive = false;
                    break;
                }
            }
            sw.Stop();
            return alive;
        }

        private string WaitReadAnswer(int timeoutMs = 5000)
        {
            var sw = new Stopwatch();
            var readed = _telnet.ReadAnswer(10);
            var answer = readed;
            sw.Start();
            while (!readed.TrimEnd().EndsWith("#"))
            {
                if (sw.Elapsed.TotalMilliseconds >= timeoutMs)
                    break;
                readed = _telnet.ReadAnswer(100);
                answer += readed;
            }
            return answer;
        }


        bool ПодготовкаFtp(int timeoutFtp = TimeoutFtp)
        {
            if (!_loginFtp)
            {
                _ftp.PassiveMode = true;
                var res = _ftp.Connect(timeoutFtp, Host, 21);
                Debug.WriteLine("FTP Response code: " + res.Code);
                if (res.Code != 220)
                    return false;

                _ftp.Login(timeoutFtp, UsernameFTP, PasswordFTP);
                _loginFtp = true;
            }
            return true;
        }

        void WriteCodeFTP(Exception code)
        {
            Debug.WriteLine("FTP Response code: " + code);
        }
        private ETypeItem GetTypeItem(FtpItem item)
        {
            switch (item.ItemType)
            {
                case FtpItemType.Directory:
                    return ETypeItem.Directory;
                case FtpItemType.File:
                    return ETypeItem.File;
                default:
                    switch (item.RawString[0])
                    {
                        case 'd':
                            return ETypeItem.Directory;
                        case '-':
                            return ETypeItem.File;
                        case 'n':
                            return ETypeItem.Driver;
                    }
                    return ETypeItem.Any;
            }
        }
        #endregion
    }

}
