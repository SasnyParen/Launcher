using System.IO;
using System.Net.WebSockets;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using Tommy;
using File = System.IO.File;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WebSocketController CurrentConnect = new WebSocketController();
        public void StartDownloadFiles()
        {
            DownloadManager downloadFiles = new DownloadManager();
            downloadFiles.AddQuery("https://media.discordapp.net/attachments/881430976926973952/1178381352442265600/image.png", "test.png")
                .AddQuery("https://github.com/fluffy-servers/gmod-discord-rpc/releases/download/1.2/gmcl_gdiscord_win32.dll", "gmcl_gdiscord_win32.dll")
                .AddQuery("https://media.discordapp.net/attachments/881430976926973952/1178364363690746027/image.png", "test1.png")
                .AddQuery("https://media.discordapp.net/attachments/881430976926973952/1178286218283327588/1000.png", "test2.png");
            downloadFiles.StartDownload();
            downloadFiles.GetWebClient.DownloadProgressChanged += (s, e) =>
            {
                downloadbar.Value = e.ProgressPercentage;
                TextDownload.Text = "Скачивание: " + downloadFiles.GetDownloadFile.fileName + " ("+ downloadFiles.filesdownload+"/"+ downloadFiles.filesall+")";
            };
        }
        public MainWindow()
        {
            DataManager.ReadTomlFile();
            CurrentConnect.Connect();
            InitializeComponent();
            CurrentConnect.GetNews();
        }
        private void Tollbar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void PlayButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
     
                StartDownloadFiles();
           
        }
        private void LeftPanel_Main_MousePress(object sender, MouseEventArgs e)
        {
            MainGrid.Visibility = Visibility.Visible;
            NewsGrid.Visibility = Visibility.Hidden;
        }
        private void LeftPanel_News_MousePress(object sender, MouseEventArgs e)
        {
            MainGrid.Visibility = Visibility.Hidden;
            NewsGrid.Visibility = Visibility.Visible;
        }
    }
}