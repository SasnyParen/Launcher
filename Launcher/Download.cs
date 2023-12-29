using Launcher;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Tommy;

public class DownloadFile
{
    public string url;
    public string fileName;
    public DownloadFile(string surl, string sfileName)
    {
        url = surl;
        fileName = sfileName;
    }
}
public class DownloadManager
{
    private String DownloadPath = "A:\\Test";
    private WebClient webClient = new WebClient();
    private List<DownloadFile> files = new List<DownloadFile>();
    private DownloadFile CurrentFile;
    private TomlTable DownloadFiles = new TomlTable();
    public int filesdownload, filesall;
    public WebClient GetWebClient { get { return webClient; } }
    public DownloadFile GetDownloadFile { get { return CurrentFile; } }
    public DownloadManager()
    {
        webClient.DownloadFileCompleted += (s, e) =>
        {
            CheckDownload();
        };

    }
    private void CheckDownload()
    {
        TomlTable settings = DataManager.GetTomlData();
        TextBlock myTextBlock = (TextBlock)Application.Current.MainWindow.FindName("TextDownload");
        if (!settings["DownloadFiles"].HasKey(CurrentFile.fileName))
            DownloadFiles.Add(CurrentFile.fileName, true);

        files.Remove(CurrentFile);

        if (files.Count != 0)
        {

            CurrentFile = files[0];
            filesdownload++;
            myTextBlock.Text = "Чтение кэша...";
            if (!settings["DownloadFiles"].HasKey(CurrentFile.fileName))
            {
                myTextBlock.Text = "Начинаем скачивать...";
                Download(CurrentFile.url, CurrentFile.fileName);
            }
            else
            {
                myTextBlock.Text = "Файл уже скачан...";
                files.Remove(CurrentFile);
                CheckDownload();
            }

        }
        else
        {
            try
            {
                if (MainWindow.GAME_PATH.EndsWith("hl2.exe"))
                {
                    string start_args = "";
                    CheckBox checkbox1 = (CheckBox)Application.Current.MainWindow.FindName("FullScreen"); if (checkbox1.IsChecked == true) start_args = start_args + "-fullscreen ";
                    CheckBox checkbox2 = (CheckBox)Application.Current.MainWindow.FindName("MultiCore"); if (checkbox1.IsChecked == true) start_args = start_args + "+gmod_mcore_test 1 ";
                    CheckBox checkbox3 = (CheckBox)Application.Current.MainWindow.FindName("Reflects"); if (checkbox1.IsChecked == true) start_args = start_args + "+r_threaded_renderables 0 ";
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = MainWindow.GAME_PATH;
                    startInfo.Arguments = start_args;
                    Process.Start(startInfo);
                    myTextBlock.Text = "Игра запущена";
                }
                else
                {
                    myTextBlock.Text = "Ошибка запуска: Игра указана не верно";
                }
            }
            catch (Exception ex)
            {

                myTextBlock.Text = "Ошибка запуска: Игра указана не верно";
            }
            if (settings.HasKey(("DownloadFiles")))
            {
               
            }
            else
            {
                settings.Add("DownloadFiles", DownloadFiles);
                DataManager.Save();
            }
            
            
        }
    }
    private void Download(string url, string filename)
    {
        Uri uri = new Uri(url);
        webClient.DownloadFileAsync(uri, DownloadPath + "\\" + filename);
    }
    public DownloadManager AddQuery(string url, string fileName)
    {
        files.Add(new DownloadFile(url, fileName));
        return this;
    }    
    public void StartDownload()
    {
        DownloadPath = MainWindow.GAME_PATH.TrimEnd("hl2.exe".ToCharArray())+ "garrysmod/lua/bin";
        Trace.WriteLine(DownloadPath);
        DirectoryInfo di = Directory.CreateDirectory(DownloadPath);
        DownloadPath = DownloadPath + "/";
        CurrentFile = files[0];
        filesdownload = 1; filesall = files.Count;
        Download(CurrentFile.url, CurrentFile.fileName);
    }
}