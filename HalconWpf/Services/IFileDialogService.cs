namespace HalconWpf.Services
{
    /// <summary>
    /// 文件对话框抽象：供 ViewModel 调用，不依赖具体 UI 实现。
    /// </summary>
    public interface IFileDialogService
    {
        /// <summary>弹出打开图片对话框，返回所选路径；取消返回 null。</summary>
        string? ShowOpenImage();
    }
}
