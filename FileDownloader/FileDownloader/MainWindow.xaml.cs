using FileDownloader.Model;
using FileDownloader.Tools;
using System;
using System.Windows;
using System.Windows.Forms;

namespace FileDownloader
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public DownloadInfo DownloadInfo { get; set; }
        public DisplayModel dispalyModel = null;
        public System.Timers.Timer UpdateProgressTimer;
        public System.Timers.Timer UpdateSpeedTimer;

        public MainWindow()
        {
            InitializeComponent();
            dispalyModel = new DisplayModel();
            this.DataContext = dispalyModel;
            // this.UrlTextBox.Text = @"http://ucan.25pp.com/Wandoujia_web_seo_baidu_homepage.apk";
        }

        /// <summary>
        /// 初始化定时器等
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateProgressTimer = new System.Timers.Timer()
            {
                Interval = 20
            };
            UpdateSpeedTimer = new System.Timers.Timer()
            {
                Interval = 1000
            };
            UpdateProgressTimer.Elapsed += UpdateProgressTimer_Elapsed;
            UpdateProgressTimer.Start();

            UpdateSpeedTimer.Elapsed += UpdateSpeedTimer_Elapsed;
            UpdateSpeedTimer.Start();
        }

        /// <summary>
        /// 更新速度信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateSpeedTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Helper.downloadInfo == null) return;
            this.dispalyModel.UpdateSpeed();
        }
        /// <summary>
        /// 更新进度信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateProgressTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Helper.downloadInfo == null) return;
            this.dispalyModel.UpdateProgress();
        }

        private async void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = this.UrlTextBox.Text.Trim();
                if (!Helper.IsURL(url))
                {
                    System.Windows.MessageBox.Show("输入的URL不合法，请重新输入！");
                    this.UrlTextBox.Text = string.Empty;
                    this.UrlTextBox.Focus();
                    return;
                }

                if (Helper.downloadInfo != null && !Helper.downloadInfo.IsComplate)
                {
                    return;
                }
                string path = SelectFolder();
                if (string.IsNullOrEmpty(path))
                    return;

                Progress.Visibility = Visibility.Visible;
                await Helper.DownloadAsync(url, path);

                System.Windows.MessageBox.Show("下载完成！");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                // throw;
            }
            
        }

        /// <summary>
        /// 文件保存路径选择
        /// </summary>
        /// <returns></returns>
        private string SelectFolder()
        {
            FolderBrowserDialog m_Dialog = new FolderBrowserDialog();

            var result = m_Dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
                return null;
            return m_Dialog.SelectedPath.Trim();
        }

        
    }
}
