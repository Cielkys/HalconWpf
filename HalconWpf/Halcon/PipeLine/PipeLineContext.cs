using System.Collections.Generic;
using HalconDotNet;
using HalconWpf.Models;

namespace HalconWpf.Halcon.Pipeline
{
    /// <summary>
    /// 流程上下文：步骤之间传递数据的通道。
    /// 由 HalconService 在执行流程前构建，步骤只读图像、写中间结果与输出参数。
    /// </summary>
    public class PipelineContext
    {
        public PipelineContext(HImage image, HSmartWindowControlWPF window)
        {
            Image = image;
            Window = window;
            Outputs = new List<OutputParameter>();
        }

        /// <summary>当前处理的图像（步骤只读）。</summary>
        public HImage Image { get; }

        /// <summary>显示窗口（叠加中间结果用）。</summary>
        public HSmartWindowControlWPF Window { get; }

        /// <summary>当前区域：上一步产出、下一步消费；还没有则为 null。</summary>
        public HObject? CurrentRegion { get; set; }

        /// <summary>输出参数，执行完交给参数表格。</summary>
        public List<OutputParameter> Outputs { get; }
    }
}
