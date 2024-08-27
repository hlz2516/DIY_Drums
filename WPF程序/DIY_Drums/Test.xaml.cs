﻿using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Windows;

namespace DIY_Drums
{
    /// <summary>
    /// Test.xaml 的交互逻辑
    /// </summary>
    public partial class Test : Window
    {
        ObservableCollection<string> notes = new ObservableCollection<string>();

        public Test()
        {
            InitializeComponent();
            //获取本机所有可用串口
            string[] portNames = SerialPort.GetPortNames();
            this.cbbPort1.ItemsSource = new List<string>(portNames);
            //获取audios目录下所有的文件名
            string wavDir = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\audios";
            var fileNames = Directory.GetFiles(wavDir);
            foreach (var name in fileNames)
            {
                notes.Add(Path.GetFileNameWithoutExtension(name));
            }
            this.note1.ItemsSource = notes;
            this.note2.ItemsSource = notes;
            this.note3.ItemsSource = notes;
            this.note4.ItemsSource = notes;
            this.note5.ItemsSource = notes;
            this.note6.ItemsSource = notes;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.note1.SelectedValue = Config.Default.A0;
            this.note2.SelectedValue = Config.Default.A1;
            this.note3.SelectedValue = Config.Default.A2;
            this.note4.SelectedValue = Config.Default.A3;
            this.note5.SelectedValue = Config.Default.A4;
            this.note6.SelectedValue = Config.Default.A5;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            Config.Default.A0 = note1.SelectedValue?.ToString();
            Config.Default.A1 = note2.SelectedValue?.ToString();
            Config.Default.A2 = note3.SelectedValue?.ToString();
            Config.Default.A3 = note4.SelectedValue?.ToString();
            Config.Default.A4 = note5.SelectedValue?.ToString();
            Config.Default.A5 = note6.SelectedValue?.ToString();
            Config.Default.Com = cbbPort1.SelectedValue?.ToString();
            Config.Default.Save();
            MessageBox.Show("保存成功", "提示");
        }
    }
}
