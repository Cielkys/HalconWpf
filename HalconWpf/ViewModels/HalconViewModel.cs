using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HalconWpf.Halcon;

namespace HalconWpf.ViewModels
{
    /// <summary>
    /// Halcon 业务视图模型：图片状态与命令。
    /// Halcon 操作全部通过 IHalconService 接口调用，不含具体 Halcon 实现。
    /// </summary>
    public partial class HalconViewModel : ObservableObject, IDisposable
    {
        private readonly IHalconService _halcon;

        /// <summary>状态栏图片信息（文件名 + 尺寸）。</summary>
        [ObservableProperty]
        private string _imageInfo = string.Empty;

        /// <summary>状态栏鼠标位置文本。</summary>
        [ObservableProperty]
        private string _mousePositionText = string.Empty;

        public HalconViewModel(IHalconService halcon)
        {
            _halcon = halcon;
        }

        /// <summary>Halcon 服务（视图 Loaded 时把窗口交入）。</summary>
        public IHalconService Service => _halcon;

        [RelayCommand(CanExecute = nameof(CanAdjustDisplay))]
        private void FitWindow()
        {
            _halcon.FitWindow();
        }

        private bool CanAdjustDisplay() => _halcon.HasImage;

        /// <summary>视图反馈鼠标位置（row, col）。</summary>
        public void UpdateMousePosition(double row, double col)
        {
            MousePositionText = $"row: {row:0}  col: {col:0}";
        }

        /// <summary>视图反馈鼠标离开图像区域。</summary>
        public void ClearMousePosition()
        {
            MousePositionText = string.Empty;
        }

        public void Dispose()
        {
            _halcon.Dispose();
        }
    }
}
