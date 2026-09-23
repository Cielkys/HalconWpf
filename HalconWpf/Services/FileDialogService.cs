using Microsoft.Win32;

namespace HalconWpf.Services
{
    /// <summary>
    /// 基于 Win32 OpenFileDialog 的文件对话框服务实现。
    /// </summary>
    public class FileDialogService : IFileDialogService
    {
        public string? ShowOpenImage()
        {
            var dialog = new OpenFileDialog
            {
                Title = "打开图片",
                Filter = "图片文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.gif;*.pcx;*.all|所有文件|*.*"
            };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }
    }
}
