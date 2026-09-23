using System.Collections.ObjectModel;

namespace HalconWpf.Halcon.Pipeline
{
    /// <summary>
    /// 一条成品流程（如"Blob检测"）= 名字 + 有序步骤集合。
    /// 左侧列表显示的就是它，执行时按 Steps 顺序跑。
    /// </summary>
    public class VisionFlow
    {
        public VisionFlow(string name)
        {
            Name = name;
            Steps = new ObservableCollection<IVisionStep>();
        }

        /// <summary>流程名（如"Blob检测"）。</summary>
        public string Name { get; }

        /// <summary>内部积木，顺序 = 执行顺序。</summary>
        public ObservableCollection<IVisionStep> Steps { get; }

        /// <summary>往流程里添加一个步骤。</summary>
        public void AddStep(IVisionStep step) => Steps.Add(step);
    }
}
