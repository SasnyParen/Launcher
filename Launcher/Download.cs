using Launcher;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

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
    public int filesdownload, filesall;
    public WebClient GetWebClient { get { return webClient; } }
    public DownloadFile GetDownloadFile { get { return CurrentFile; } }
    public DownloadManager()
    {
        webClient.DownloadFileCompleted += (s, e) =>
        {
            files.Remove(CurrentFile);
            
            if (files.Count != 0)
            {
                CurrentFile = files[0];
                filesdownload++;
                Download(CurrentFile.url, CurrentFile.fileName);
            }
            else
            {
                Process.Start(MainWindow.GAME_PATH);
            }
        };
        
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