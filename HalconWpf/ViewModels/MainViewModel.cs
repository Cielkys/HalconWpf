using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HalconWpf.Halcon;
using HalconWpf.Halcon.Camera;
using HalconWpf.Models;
using HalconWpf.Services;

namespace HalconWpf.ViewModels
{
    /// <summary>
    /// 主窗口视图模型：组合根，组合 Halcon 与输出参数子视图模型，维护状态栏文本。
    /// </summary>
    public partial class MainViewModel : ObservableObject, System.IDisposable
    {
        private readonly IFileDialogService _fileDialog;
        private readonly IHalconService _halcon;

        /// <summary>状态栏状态文本。</summary>
        [ObservableProperty]
        private string _statusText = "就绪";

        public MainViewModel() : this(new FileDialogService())
        {
        }

        public MainViewModel(IFileDialogService fileDialog)
        {
            _fileDialog = fileDialog;
            _halcon = new HalconService();
            OutputParams = new OutputParamsViewModel();
            Halcon = new HalconViewModel(_halcon);
            Flow = new FlowViewModel(_halcon, OutputParams, message => StatusText = message);
            Camera = new CameraViewModel(new CameraService(_halcon));
        }

        /// <summary>Halcon 子视图模型。</summary>
        public HalconViewModel Halcon { get; }

        /// <summary>相机子视图模型。</summary>
        public CameraViewModel Camera { get; }

        /// <summary>输出参数子视图模型。</summary>
        public OutputParamsViewModel OutputParams { get; }

        /// <summary>流程子视图模型（与 Halcon 共用同一个服务实例）。</summary>
        public FlowViewModel Flow { get; }

        /// <summary>顶部工具栏"打开图片"：选文件 → Halcon 服务加载 → 回填图片信息与尺寸参数。</summary>
        [RelayCommand]
        private void OpenImage()
        {
            string? path = _fileDialog.ShowOpenImage();
            if (path == null)
            {
                return;
            }

            try
            {
                var (name, width, height) = _halcon.Load(path);
                ApplyImage(name, width, height);
            }
            catch (Exception ex)
            {
                StatusText = $"打开图片失败: {ex.Message}";
            }
        }

        /// <summary>把取到的图回填状态栏、适应窗口按钮与尺寸参数。</summary>
        private void ApplyImage(string name, int width, int height)
        {
            Halcon.ImageInfo = $"{name}   {width} x {height}";
            Halcon.FitWindowCommand.NotifyCanExecuteChanged();
            OutputParams.SetParameters(new[]
            {
                new OutputParameter("image_width", width),
                new OutputParameter("image_height", height),
            });
        }

        public void Dispose()
        {
            Halcon.Dispose();
        }
    }
}
