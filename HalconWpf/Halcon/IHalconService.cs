using System;
using System.Collections.Generic;
using HalconDotNet;
using HalconWpf.Halcon.Pipeline;
using HalconWpf.Models;

namespace HalconWpf.Halcon
{
    /// <summary>
    /// Halcon 操作接口：阈值分割、连通域等所有算法后续都加在这里。
    /// ViewModel 只依赖本接口调用 Halcon，不接触具体实现。
    /// </summary>
    public interface IHalconService : IDisposable
    {
        /// <summary>是否已加载图片。</summary>
        bool HasImage { get; }

        /// <summary>由视图在 Loaded 时交入显示窗口。</summary>
        void AttachWindow(HSmartWindowControlWPF window);

        /// <summary>加载图片并显示（自动适应窗口）。返回文件名与尺寸。</summary>
        (string Name, int Width, int Height) Load(string path);

        /// <summary>图像适应窗口显示。</summary>
        void FitWindow();

        /// <summary>显示一帧相机图像（可从后台线程调用；首帧或尺寸变化自动适应窗口）。</summary>
        void DisplayFrame(HObject frame);

        /// <summary>按顺序执行步骤，返回输出参数。步骤失败会抛出并带上步骤名。</summary>
        IReadOnlyList<OutputParameter> RunPipeline(IReadOnlyList<IVisionStep> steps);
    }
}
