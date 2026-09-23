using System;
using System.Collections.Generic;
using System.IO;
using HalconDotNet;
using HalconWpf.Halcon.Pipeline;
using HalconWpf.Models;

namespace HalconWpf.Halcon
{
    /// <summary>
    /// Halcon 服务：持有窗口与图像，负责加载、显示、适应窗口等全部 Halcon 操作。
    /// 窗口由视图通过 AttachWindow 交入；服务只持有，不负责释放（归控件所有）。
    /// </summary>
    public class HalconService : IHalconService
    {
        private HSmartWindowControlWPF? _window;
        private HImage? _image;

        /// <summary>是否已加载图片。</summary>
        public bool HasImage => _image != null;

        /// <summary>由视图在 Loaded 时交入显示窗口。</summary>
        public void AttachWindow(HSmartWindowControlWPF window)
        {
            _window = window;
        }

        /// <summary>加载图片并显示（自动适应窗口）。返回文件名与尺寸。</summary>
        public (string Name, int Width, int Height) Load(string path)
        {
            var image = new HImage(path);
            HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);

            _image?.Dispose();
            _image = image;

            FitWindow();
            return (Path.GetFileName(path), width.I, height.I);
        }

        /// <summary>图像适应窗口显示。</summary>
        public void FitWindow()
        {
            if (_window == null || _image == null)
            {
                return;
            }

            _window.SetFullImagePart(_image);
            Display();
        }

        /// <summary>重绘当前图像。</summary>
        public void Display()
        {
            if (_window == null || _image == null)
            {
                return;
            }

            _window.HalconWindow.ClearWindow();
            _window.HalconWindow.DispObj(_image);
            _window.HDisplayCurrentObject = _image;
            _window.HalconWindow.FlushBuffer();
        }

        /// <summary>按顺序执行步骤：重绘底图 → foreach 执行 → 返回输出参数。</summary>
        public IReadOnlyList<OutputParameter> RunPipeline(IReadOnlyList<IVisionStep> steps)
        {
            if (_image == null || _window == null)
            {
                throw new InvalidOperationException("尚未加载图片。");
            }

            Display();
            var ctx = new PipelineContext(_image, _window);
            foreach (IVisionStep step in steps)
            {
                try
                {
                    step.Execute(ctx);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"步骤「{step.Name}」失败：{ex.Message}", ex);
                }
            }

            return ctx.Outputs;
        }

        public void Dispose()
        {
            _image?.Dispose();
            _image = null;
            GC.SuppressFinalize(this);
        }
    }
}
