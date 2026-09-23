using System;
using HalconDotNet;
using HalconWpf.Halcon.Pipeline;
using HalconWpf.Models;

namespace HalconWpf.Halcon.Steps
{
    /// <summary>
    /// 连通分割步骤：把上一步的区域拆成独立连通域，输出目标数量。
    /// </summary>
    public class ConnectionStep : IVisionStep
    {
        public string Name => "连通分割";

        public void Execute(PipelineContext ctx)
        {
            if (ctx.CurrentRegion == null)
            {
                throw new InvalidOperationException("没有可连通的区域（需先执行阈值分割）。");
            }

            HOperatorSet.Connection(ctx.CurrentRegion, out HObject connected);
            ctx.CurrentRegion.Dispose();
            ctx.CurrentRegion = connected;

            HOperatorSet.CountObj(connected, out HTuple count);
            ctx.Outputs.Add(new OutputParameter("blob_count", count.D));

            ctx.Window.HalconWindow.SetColor("cyan");
            ctx.Window.HalconWindow.DispObj(connected);
            ctx.Window.HalconWindow.FlushBuffer();
        }
    }
}
