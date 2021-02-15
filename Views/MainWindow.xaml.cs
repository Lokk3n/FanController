using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;


namespace FanController.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        public MainWindow()
        {
            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.BalloonTipText = "The app has been minimised. Click the tray icon to show.";
            m_notifyIcon.BalloonTipTitle = "Fan Controller";
            m_notifyIcon.Text = "Fan Controller";
            Stream iconStream = Application.GetResourceStream(new Uri("Icons/fan.ico", UriKind.Relative)).Stream;
            m_notifyIcon.Icon = new System.Drawing.Icon(iconStream);
            m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);
            InitializeComponent();
        }
        void OnInitialized(object sender, EventArgs args)
        {
            Console.WriteLine("Initialized");
            //Hide();
            //ShowTrayIcon(true);
        }
        void OnClose(object sender, CancelEventArgs args)
        {
            Console.WriteLine("Dispose");
            m_notifyIcon.Dispose();
            m_notifyIcon = null;
        }

        private WindowState m_storedWindowState = WindowState.Normal;
        void OnStateChanged(object sender, EventArgs args)
        {
            Console.WriteLine("OnStateChanged");
            if (WindowState == WindowState.Minimized)
            {
                Console.WriteLine("Minimalizacja");
                Hide();
                if (m_notifyIcon != null)
                    m_notifyIcon.ShowBalloonTip(2000);
            }
            else
                m_storedWindowState = WindowState;
        }
        void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            CheckTrayIcon();
            Console.WriteLine("...");
        }

        void m_notifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = m_storedWindowState;
        }
        void CheckTrayIcon()
        {
            ShowTrayIcon(!IsVisible);
        }

        void ShowTrayIcon(bool show)
        {
            if (show) Console.WriteLine("Show true");
            if (m_notifyIcon != null)
            {
                m_notifyIcon.Visible = true;
                Console.WriteLine("Show true true");
            }
        }
    }
}
