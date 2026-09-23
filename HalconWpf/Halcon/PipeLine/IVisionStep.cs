namespace HalconWpf.Halcon.Pipeline
{
    /// <summary>
    /// 流程步骤契约：所有步骤（阈值分割、连通域等）实现本接口。
    /// 步骤之间不直接调用，数据全部经 PipelineContext 传递。
    /// </summary>
    public interface IVisionStep
    {
        /// <summary>步骤名（列表显示 + 异常定位用）。</summary>
        string Name { get; }

        /// <summary>执行本步骤，读写上下文。</summary>
        void Execute(PipelineContext ctx);
    }
}
