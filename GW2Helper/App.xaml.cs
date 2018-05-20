using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;

namespace GW2Helper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
        }
        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "log.txt")) File.Delete(AppDomain.CurrentDomain.BaseDirectory + "log.txt");
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "log.txt", e.Message + Environment.NewLine + e.Source + Environment.NewLine + e.StackTrace);
        }
    }
}
