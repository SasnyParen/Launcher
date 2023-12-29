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
using System.Diagnostics;
using Microsoft.Win32;
using System.Windows.Media.TextFormatting;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        public static bool IsConnectedToServer;
        WebSocketController CurrentConnect = new WebSocketController();
        TomlTable settings;
        public static string GAME_PATH;
        public void StartDownloadFiles()
        {
            if (!CheckTruePath()) {
                TextDownload.Text = "Ошибка запуска: Игра указана не верно";
                return;
            }
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
        public void UpdateGamePath(string path)
        {
            GAME_PATH = path;
            TextBoxGamePath.Text = path;
            if (settings.HasKey("GAME_PATH"))
            {
                settings["GAME_PATH"] = path;
                DataManager.Save();
            }
            else
            {
                settings.Add("GAME_PATH", path);
                Debug.WriteLine("Save ...");
                DataManager.Save();
            }
        }
        public void SettingsInitial()
        {
            if (settings.HasKey("GAME_PATH"))
            {
                GAME_PATH = settings["GAME_PATH"];
                TextBoxGamePath.Text = GAME_PATH;
            }
            else
            {
                GAME_PATH = GameFounder.FindPath();
                TextBoxGamePath.Text = GAME_PATH;
                settings.Add("GAME_PATH", GAME_PATH);
                Debug.WriteLine("Save ...");
                DataManager.Save();
            }
        }
        public static bool CheckTruePath()
        {
            bool containsSteamLibrary = GAME_PATH.Contains("SteamLibrary", StringComparison.OrdinalIgnoreCase);
            bool containsSteamApps = GAME_PATH.Contains("steamapps", StringComparison.OrdinalIgnoreCase);
            bool containsGarrysMod = GAME_PATH.Contains("GarrysMod", StringComparison.OrdinalIgnoreCase);
            return containsSteamLibrary && containsSteamApps && containsGarrysMod;
        }
        public MainWindow()
        {
            DataManager.Read();
            settings = DataManager.GetTomlData();
            InitializeComponent();
            CurrentConnect.Connect();
            CurrentConnect.GetNews();
            SettingsInitial();
        }
        private void Tollbar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void CloseWPF(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void PlayButton_MouseDown(object sender, RoutedEventArgs e)
        {
     
                StartDownloadFiles();
           
        }
        private void LeftPanel_Main_MousePress(object sender, RoutedEventArgs e)
        {

            MainGrid.Visibility = Visibility.Visible;
            NewsGrid.Visibility = Visibility.Hidden;
            SettingsGrid.Visibility = Visibility.Hidden;
        }
        private void LeftPanel_News_MousePress(object sender, RoutedEventArgs e)
        {
            MainGrid.Visibility = Visibility.Hidden;
            NewsGrid.Visibility = Visibility.Visible;
            SettingsGrid.Visibility = Visibility.Hidden;
        }
        private void LeftPanel_Setting_MousePress(object sender, RoutedEventArgs e)
        {
            MainGrid.Visibility = Visibility.Hidden;
            NewsGrid.Visibility = Visibility.Hidden;
            SettingsGrid.Visibility = Visibility.Visible;
        }
        private void SelectGamePath(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.ShowDialog();

            UpdateGamePath(openFileDialog1.FileName);

        }//AutoFindGame
        private void AutoFindGame(object sender, RoutedEventArgs e)
        {
            GAME_PATH = GameFounder.FindPath();
            UpdateGamePath(GAME_PATH);
        }//AutoFindGame
    }
}