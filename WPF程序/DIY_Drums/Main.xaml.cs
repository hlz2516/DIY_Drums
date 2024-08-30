using DIY_Drums.Helpers;
using NAudio.Wave;
using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Input;

namespace DIY_Drums
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private SerialPort com;
        private CachedSound[] sounds;
        private AudioPlaybackEngine engine;

        public Main()
        {
            InitializeComponent();
            sounds = new CachedSound[6];
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var drivers = AsioOut.GetDriverNames();
            if (drivers.Length == 0)
            {
                MessageBox.Show("没有检测到ASIO驱动程序，请到ASIO官方网站进行下载安装！", "提示");
                return;
            }
            engine = new AudioPlaybackEngine(drivers[0]);
            string comName = Config.Default.Com;
            if (string.IsNullOrEmpty(comName))
            {
                MessageBox.Show("没有设置对应串口，请进入设置窗口进行设置！", "提示");
                return;
            }
            try
            {
                for (int i = 0; i < 6; i++)
                {
                    string value = Config.Default[$"A{i}"]?.ToString();
                    string wavLoc = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\audios\\empty.wav";
                    if (!string.IsNullOrEmpty(value))
                    {
                        wavLoc = AppDomain.CurrentDomain.BaseDirectory + $"\\Resources\\audios\\{value}.wav";
                    }
                    sounds[i] = new CachedSound(wavLoc);
                }
                com = new SerialPort(comName, 31250);
                com.Open();
                com.DataReceived += Com_DataReceived;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Initialized failed!");
                MessageBox.Show("初始化失败，请查看日志文件logs.txt检查原因", "错误");
            }
        }

        private void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int val = -1;
            while (com.BytesToRead > 0)
            {
                val = com.ReadByte();
            }
            switch (val)
            {
                case 0:
                    engine.PlaySound(sounds[0]);
                    break;
                case 1:
                    engine.PlaySound(sounds[1]);
                    break;
                case 2:
                    engine.PlaySound(sounds[2]);
                    break;
                case 3:
                    engine.PlaySound(sounds[3]);
                    break;
                case 4:
                    engine.PlaySound(sounds[4]);
                    break;
                case 5:
                    engine.PlaySound(sounds[5]);
                    break;
                default:
                    break;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Q:
                    engine.PlaySound(sounds[0]);
                    break;
                case Key.W:
                    engine.PlaySound(sounds[1]);
                    break;
                case Key.E:
                    engine.PlaySound(sounds[2]);
                    break;
                case Key.R:
                    engine.PlaySound(sounds[3]);
                    break;
                case Key.T:
                    engine.PlaySound(sounds[4]);
                    break;
                case Key.Y:
                    engine.PlaySound(sounds[5]);
                    break;
                default:
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            engine.Dispose();
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            var setupForm = new Settings();
            var res = setupForm.ShowDialog();
            if (res == true)
            {
                var driver = Config.Default.Driver;
                if (string.IsNullOrEmpty(driver))
                {
                    MessageBox.Show("没有检测到ASIO驱动程序，请到ASIO官方网站进行下载安装！", "提示");
                    return;
                }
                for (int i = 0; i < 6; i++)
                {
                    string value = Config.Default[$"A{i}"]?.ToString();
                    string wavLoc = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\audios\\empty.wav";
                    if (!string.IsNullOrEmpty(value))
                    {
                        wavLoc = AppDomain.CurrentDomain.BaseDirectory + $"\\Resources\\audios\\{value}.wav";
                    }
                    sounds[i] = new CachedSound(wavLoc);
                }
                string comName = Config.Default.Com;
                if (string.IsNullOrEmpty(comName))
                {
                    MessageBox.Show("没有设置对应串口，请进入设置窗口进行设置！", "提示");
                    return;
                }
                try
                {
                    engine.Dispose();
                    engine = new AudioPlaybackEngine(driver);
                    com = new SerialPort(comName, 31250);
                    com.Open();
                    com.DataReceived += Com_DataReceived;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Initialized failed!");
                }
            }
        }
    }
}
