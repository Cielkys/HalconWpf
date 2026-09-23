using System;
using HalconDotNet;
using HalconWpf.Halcon.Pipeline;
using HalconWpf.Models;

namespace HalconWpf.Halcon.Steps
{
    /// <summary>
    /// 图形筛选步骤：按任意特征的数值范围筛选区域，输出筛选后数量。
    /// </summary>
    public class SelectShapeStep : IVisionStep
    {
        public SelectShapeStep(string features, string operate, double min, double max)
        {
            Features = features;
            Operate = operate;
            Min = min;
            Max = max;
        }

        /// <summary>筛选特征名</summary>
        public string Features { get; set; }

        /// <summary>特征组合关系</summary>
        public string Operate { get; set; }

        /// <summary>特征下限</summary>
        public double Min { get; set; }

        /// <summary>特征上限</summary>
        public double Max { get; set; }

        public string Name => "图形筛选";

        public void Execute(PipelineContext ctx)
        {
            if (ctx.CurrentRegion == null)
            {
                throw new InvalidOperationException("没有可筛选的区域（需先执行连通分割）。");
            }

            HOperatorSet.SelectShape(ctx.CurrentRegion, out HObject selected, Features, Operate, Min, Max);
            ctx.CurrentRegion.Dispose();
            ctx.CurrentRegion = selected;

            HOperatorSet.CountObj(selected, out HTuple count);
            ctx.Outputs.Add(new OutputParameter("selected_count", count.D));

            ctx.Window.HalconWindow.SetColor("red");
            ctx.Window.HalconWindow.DispObj(selected);
            ctx.Window.HalconWindow.FlushBuffer();
        }
    }
}
