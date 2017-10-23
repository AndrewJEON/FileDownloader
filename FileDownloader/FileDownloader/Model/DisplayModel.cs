using FileDownloader.Tools;

namespace FileDownloader.Model
{
    public class DisplayModel : BindableBase
    {
        private double _progressVal;
        /// <summary>
        /// 文件下载进度值
        /// </summary>
        public double ProgressVal
        {
            get { return _progressVal; }
            set { this.SetProperty(ref _progressVal, value); }
        }

        private string _dispalySize;
        /// <summary>
        /// 文件显示大小
        /// </summary>
        public string DisplaySize
        {
            get { return _dispalySize; }
            set { SetProperty(ref this._dispalySize, value); }
        }

        private string _remainTime;
        /// <summary>
        /// 文件剩余下载时间
        /// </summary>
        public string RemainTime
        {
            get { return _remainTime; }
            set { SetProperty(ref this._remainTime, value); }
        }

        private string _curSpeed;
        /// <summary>
        /// 当前文件下载速度
        /// </summary>
        public string CurSpeed
        {
            get { return _curSpeed; }
            set { SetProperty(ref this._curSpeed, value); }
        }

        private long _beforeSize;
        /// <summary>
        /// 前一次更新时已下载文件的大小
        /// </summary>
        public long BeforeSize
        {
            get { return _beforeSize; }
            set { _beforeSize = value; }
        }

        /// <summary>
        /// 将进度条数据更新
        /// </summary>
        public void UpdateProgress()
        {
            this.ProgressVal = Helper.GetProgress();
            
            this.DisplaySize = (Helper.downloadInfo.DownloadSize / (1024.0 * 1024.0)).ToString("f2") 
                                + "/" + Helper.GetDisplaySize() + "M";
            
        }

        /// <summary>
        /// 更新速度和剩余时间
        /// </summary>
        public void UpdateSpeed()
        {
            double speed = Helper.GetSpeed(BeforeSize);
            this.CurSpeed = (speed > 0 ? speed.ToString("f2") : 0.ToString())+ "kb/s";
            if (speed - 0 < 0.001 && !string.IsNullOrEmpty(RemainTime))
                RemainTime = RemainTime;
            else
            {
                double time = Helper.GetRemainTime(speed);
                this.RemainTime = string.Format("{0}:{1}", time > 0 ? (((int)time) / 60).ToString("d2") : 0.ToString("d2"),
                                                            time > 0 ? (((int)time) % 60).ToString("d2") : 0.ToString("d2"));
            }
            
            this.BeforeSize = Helper.downloadInfo.DownloadSize;
        }

    }
}
