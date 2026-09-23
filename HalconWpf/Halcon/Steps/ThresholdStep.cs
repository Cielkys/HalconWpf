using HalconDotNet;
using HalconWpf.Halcon.Pipeline;
using HalconWpf.Models;

namespace HalconWpf.Halcon.Steps
{
    /// <summary>
    /// 阈值分割步骤：按灰度范围分割出区域，叠加显示并输出面积。
    /// </summary>
    public class ThresholdStep : IVisionStep
    {
        public ThresholdStep(double min, double max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>灰度下限（参数面板可编辑）。</summary>
        public double Min { get; set; }

        /// <summary>灰度上限（参数面板可编辑）。</summary>
        public double Max { get; set; }

        public string Name => "阈值分割";

        public void Execute(PipelineContext ctx)
        {
            HOperatorSet.Threshold(ctx.Image, out HObject region, Min, Max);
            ctx.CurrentRegion?.Dispose();
            ctx.CurrentRegion = region;

            HOperatorSet.AreaCenter(region, out HTuple row, out HTuple column, out HTuple area);
            ctx.Outputs.Add(new OutputParameter("threshold_area", area.D));

            ctx.Window.HalconWindow.SetColor("green");
            ctx.Window.HalconWindow.DispObj(region);
            ctx.Window.HalconWindow.FlushBuffer();
        }
    }
}
