using System;

namespace FileDownloader.Model
{
    [Serializable]
    public class FileBlock
    {
        #region 字段
        int _index;
        string _fileUrl;
        bool _isComplate;
        string _blockPath;
        long _downloadSize;
        long _startPos;
        long _curPos;
        long _endPos;
        #endregion

        #region 属性
        /// <summary>
        /// 下载块序号
        /// </summary>
        public int Index
        {
            get => _index;
            set => _index = value;
        }
        /// <summary>
        /// 块下载地址
        /// </summary>
        public string FileUrl
        {
            get => _fileUrl;
            set => _fileUrl = value;
        }
        /// <summary>
        /// 是否下载完成
        /// </summary>
        public bool IsComplate
        {
            get => _curPos >= _endPos;
        }
        /// <summary>
        /// 块存放路径
        /// </summary>
        public string BlockPath
        {
            get => _blockPath;
            set => _blockPath = value;
        }
        public long DownloadSize
        {
            get => CurPos - StartPos;
        }

        /// <summary>
        /// 起始位置
        /// </summary>
        public long StartPos
        {
            get => _startPos;
            set => _startPos = value;
        }
        /// <summary>
        /// 当前位置
        /// </summary>
        public long CurPos
        {
            get => _curPos;
            set => _curPos = value;
        }
        /// <summary>
        /// 结束位置
        /// </summary>
        public long EndPos
        {
            get => _endPos;
            set => _endPos = value;
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="index">序列号</param>
        /// <param name="fileUrl">文件下载地址</param>
        /// <param name="filePath">最终文件名</param>
        /// <param name="startPos">起始位置</param>
        /// <param name="endPos">终止位置</param>
        public FileBlock(int index, string fileUrl, string filePath, long startPos, long endPos)
        {
            this._index = index;
            this._fileUrl = fileUrl;
            this._blockPath = filePath + "." + index + ".tmp";
            this._curPos = this._startPos = startPos;
            this._endPos = endPos;

        }
    }
}
