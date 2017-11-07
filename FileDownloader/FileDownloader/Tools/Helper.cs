using FileDownloader.Model;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileDownloader.Tools
{
    public static class Helper
    {
        public static System.Timers.Timer UpdateDownInfoTimer;
        public static DownloadInfo downloadInfo = null;
        public static Object locker = new object();

        static Helper()
        {
            UpdateDownInfoTimer = new System.Timers.Timer
            {
                Interval = 500
            };
            UpdateDownInfoTimer.Elapsed += UpdateDownInfoTimer_Elapsed;
            UpdateDownInfoTimer.Start();
        }

        /// <summary>
        /// 定时将下载信息写到本地
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UpdateDownInfoTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (downloadInfo == null || downloadInfo.IsComplate) return;
            lock (downloadInfo)
            {
                WriteDownloadInfo2Local(downloadInfo);
            }
        }

        #region 文件下载
        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="url">将要下载文件地址</param>
        /// <param name="path">文件保存位置</param>
        /// <returns></returns>
        public static async Task DownloadAsync(string url, string path)
        {
            string fileName = GetDownloadFileName(url);
            long fileSize = GetDownloadFileSize(url);
            string fileDownInfoPath = Path.Combine(path, fileName + ".tmp.downloadinfo");
            try
            {
                if (File.Exists(fileDownInfoPath))
                {
                    // 读取下载信息
                    downloadInfo = LoadDownloadInfoFromLocal(fileDownInfoPath);
                    if (downloadInfo == null) return;
                }
                else
                {
                    downloadInfo = new DownloadInfo(url, fileSize, fileName, path);
                }

                CutFile();

                // 开始下载
                Task[] tasks = new Task[downloadInfo.ThreadNum];
                for (int i = 0; i < tasks.Length; ++i)
                {
                    var index = i;
                    tasks[index] = DownloadBlockAsync(downloadInfo.FileBlocks[index]);
                    await Task.Delay(200);
                }
                await Task.WhenAll(tasks);

                await DownloadComplateAsync();
            }
            catch 
            {
                throw;
            }
            
        }

        /// <summary>
        /// 下载完成
        /// </summary>
        /// <returns></returns>
        public static Task DownloadComplateAsync()
        {
            var t = Task.Run(() =>
            {
                try
                {
                    MergerAllFiles(downloadInfo);
                    DeleteCacheFiles(downloadInfo);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                
            });
            return t;
        }

        /// <summary>
        /// 分块下载
        /// </summary>
        /// <param name="fileBlock"></param>
        /// <param name="url"></param>
        public static Task DownloadBlockAsync(FileBlock fileBlock)
        {
            return Task.Run(() =>
            {
                try
                {
                    HttpWebRequest httpRequest = WebRequest.Create(fileBlock.FileUrl) as HttpWebRequest;
                    httpRequest.AddRange(fileBlock.CurPos, fileBlock.EndPos);
                    HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
                    using (var httpStream = httpResponse.GetResponseStream())
                    {
                        httpStream.ReadTimeout = 2000;
                        using (FileStream fs = new FileStream(fileBlock.BlockPath, FileMode.OpenOrCreate))
                        {
                            int readBytes = 0;
                            byte[] buffer = new byte[1024];
                            fs.Seek(fileBlock.CurPos - fileBlock.StartPos, SeekOrigin.Begin);
                            while (true)
                            {
                                readBytes = httpStream.Read(buffer, 0, buffer.Length);
                                if (readBytes <= 0 || fileBlock.IsComplate)
                                {
                                    break;
                                }
                                fs.Write(buffer, 0, readBytes);
                                fileBlock.CurPos += readBytes;
                            }
                        }
                    }
                }
                catch (WebException ex)
                {
                    throw new Exception("网络错误！");
                }
                catch 
                {
                    throw;
                }
            });
        }
        /// <summary>
        /// 获取下载文件名称
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetDownloadFileName(string url)
        {
            string[] urlArr = url.Split(new char[] { '/' });
            if (urlArr == null || urlArr.Length <= 0) return null;
            return urlArr[urlArr.Length - 1];
        }

        /// <summary>
        /// 获取下载文件大小
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static long GetDownloadFileSize(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                if (request == null) return 0;
                request.Method = "HEAD";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                return response.ContentLength;
            }
            catch 
            {
                throw;
            }
        }

        /// <summary>
        /// 分割文件为文件块
        /// </summary>
        public static void CutFile()
        {
            int blockSize = (int)(downloadInfo.FileSize / downloadInfo.ThreadNum);
            int remainSize = (int)(downloadInfo.FileSize % downloadInfo.ThreadNum);
            for (int i = 0; i < downloadInfo.ThreadNum; ++i)
            {
                downloadInfo.FileBlocks.Add(
                    new FileBlock(i,
                              downloadInfo.FileUrl,
                              Path.Combine(downloadInfo.DicPath, downloadInfo.FileName),
                              i * blockSize,
                              (i == downloadInfo.ThreadNum - 1) ? downloadInfo.FileSize - 1 : (i + 1) * blockSize - 1));
            }
        }

        /// <summary>
        /// 将下载信息写到本地
        /// </summary>
        /// <param name="downloadInfo"></param>
        /// <param name="filePath"></param>
        public static void WriteDownloadInfo2Local(DownloadInfo downloadInfo)
        {
            try
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(DownloadInfo));
                using (MemoryStream ms = new MemoryStream())
                {
                    serializer.WriteObject(ms, downloadInfo);
                    string jsonString = string.Empty;
                    using (StreamReader reader = new StreamReader(ms))
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        jsonString = reader.ReadToEnd();
                    }
                    using (FileStream fs = new FileStream(downloadInfo.SavePath, FileMode.OpenOrCreate))
                    {
                        using (StreamWriter writer = new StreamWriter(fs))
                        {
                            fs.Seek(0, SeekOrigin.Begin);
                            writer.Write(jsonString);
                        }
                    }
                }
            }
            catch 
            {
                throw;
            }
            
        }

        /// <summary>
        /// 从本地将下载信息读取到实体类中
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DownloadInfo LoadDownloadInfoFromLocal(string filePath)
        {
            try
            {
                DownloadInfo downloadInfo = new DownloadInfo();
                string jsonString = string.Empty;
                using (var fs = File.Open(filePath, FileMode.Open))
                {
                    byte[] buffer = new byte[fs.Length];
                    int n = fs.Read(buffer, 0, buffer.Length);
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(DownloadInfo));
                    using (MemoryStream ms = new MemoryStream(buffer))
                    {
                        downloadInfo = serializer.ReadObject(ms) as DownloadInfo;
                    }
                }
                return downloadInfo;
            }
            catch 
            {
                throw;
            }
            
        }

        /// <summary>
        /// 将下载的缓存文件合并
        /// </summary>
        /// <param name="downloadInfo"></param>
        /// <returns></returns>
        public static bool MergerAllFiles(DownloadInfo downloadInfo)
        {
            try
            {
                using (var fs = File.Open(Path.Combine(downloadInfo.DicPath, downloadInfo.FileName), FileMode.OpenOrCreate))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        foreach (var fileBlock in downloadInfo.FileBlocks)
                        {
                            using (var itemFs = File.OpenRead(fileBlock.BlockPath))
                            {
                                byte[] buffer = new byte[itemFs.Length];
                                itemFs.Read(buffer, 0, buffer.Length);
                                bw.Write(buffer, 0, buffer.Length);
                            }
                        }
                    }
                }

                return true;
            }
            catch 
            {
                throw;
            }
            

        }

        /// <summary>
        /// 删除缓存文件
        /// </summary>
        /// <param name="downloadInfo"></param>
        public static void DeleteCacheFiles(DownloadInfo downloadInfo)
        {
            try
            {
                if (downloadInfo == null) return;
                File.Delete(downloadInfo.SavePath);
                
            }
            catch (Exception) {}
            try
            {
                foreach (var fileBlock in downloadInfo.FileBlocks)
                {
                    File.Delete(fileBlock.BlockPath);
                }
            }
            catch (Exception) { }
        }
        #endregion


        #region 显示
        /// <summary>
        /// 获取文件大小显示值 单位：M
        /// </summary>
        /// <returns></returns>
        public static string GetDisplaySize()
        {
            var size = downloadInfo.FileSize / (1024.0 * 1024.0);
            return size.ToString("f2");
        }

        /// <summary>
        /// 获取当前下载进度值
        /// </summary>
        /// <returns></returns>
        public static double GetProgress()
        {
            lock (locker)
            {
                return downloadInfo.DownloadSize * 1000.0 / downloadInfo.FileSize;
            }
        }

        /// <summary>
        /// 获取当前下载速度 单位 ： Kb/S
        /// </summary>
        /// <returns></returns>
        public static double GetSpeed(long befoerSize)
        {
            lock (locker)
            {
                return (downloadInfo.DownloadSize - befoerSize) / 1024.0;
            }
        }

        /// <summary>
        /// 获取剩余下载时间 单位： 秒
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static double GetRemainTime(double speed)
        {
            lock (locker)
            {
                speed = speed > 0 ? speed : 1;
                return (downloadInfo.FileSize - downloadInfo.DownloadSize) / (speed * 1024.0);
            }
        }
        #endregion

        #region 其他
        /// <summary>
        /// 判断url是否合法
        /// </summary>
        /// <param name="url">待验证的URL</param>
        /// <returns></returns>
        public static bool IsURL(string url)
        {
            var pattern = @"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(url);
        }
        #endregion
    }
}
