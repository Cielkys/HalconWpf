using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HalconWpf.Halcon;
using HalconWpf.Halcon.Pipeline;
using HalconWpf.Halcon.Steps;

namespace HalconWpf.ViewModels
{
    /// <summary>
    /// 流程面板视图模型：管理流程列表（添加/删除/执行）。
    /// 执行时按列表顺序把每条流程的步骤交给 Halcon 服务。
    /// </summary>
    public partial class FlowViewModel : ObservableObject
    {
        private readonly IHalconService _halcon;
        private readonly OutputParamsViewModel _outputs;
        private readonly System.Action<string> _status;

        public FlowViewModel(IHalconService halcon, OutputParamsViewModel outputs, System.Action<string> status)
        {
            _halcon = halcon;
            _outputs = outputs;
            _status = status;
            Flows.CollectionChanged += (s, e) => RunCommand.NotifyCanExecuteChanged();
        }

        /// <summary>左侧列表绑定的流程集合。</summary>
        public ObservableCollection<VisionFlow> Flows { get; } = new();

        /// <summary>当前选中的流程。</summary>
        [ObservableProperty]
        private VisionFlow? _selectedFlow;

        /// <summary>当前选中的步骤（参数面板绑定它编辑参数）。</summary>
        [ObservableProperty]
        private IVisionStep? _selectedStep;

        /// <summary>新建一条空流程（名字自动编号，不重名）。</summary>
        [RelayCommand]
        private void AddFlow()
        {
            int n = Flows.Count + 1;
            while (Flows.Any(f => f.Name == $"流程{n}"))
            {
                n++;
            }

            var flow = new VisionFlow($"流程{n}");
            Flows.Add(flow);
            SelectedFlow = flow;
            _status($"已创建流程「{flow.Name}」，用顶部\"创建流程\"菜单添加步骤");
        }

        /// <summary>向选中流程添加指定类型步骤（没有流程就先建一条）。</summary>
        [RelayCommand]
        private void AddStep(string kind)
        {
            if (SelectedFlow == null)
            {
                AddFlow();
            }

            var flow = SelectedFlow!;
            IVisionStep step = kind switch
            {
                "Threshold" => new ThresholdStep(128, 255),
                "Connection" => new ConnectionStep(),
                "SelectShape" => new SelectShapeStep("area", "and", 50, 999999),
                _ => throw new ArgumentException($"未知步骤类型：{kind}"),
            };

            flow.AddStep(step);
            RunCommand.NotifyCanExecuteChanged();
            _status($"已向「{flow.Name}」添加步骤：{step.Name}");
        }

        /// <summary>删除选中流程。</summary>
        [RelayCommand(CanExecute = nameof(HasSelectedFlow))]
        private void RemoveFlow()
        {
            if (SelectedFlow == null)
            {
                return;
            }

            string name = SelectedFlow.Name;
            Flows.Remove(SelectedFlow);
            _status($"已删除流程「{name}」");
        }

        private bool HasSelectedFlow => SelectedFlow != null;

        /// <summary>删除指定步骤（步骤行尾 ✕ 调用；按归属流程找，不依赖当前选中）。</summary>
        [RelayCommand]
        private void RemoveStep(IVisionStep? step)
        {
            if (step == null)
            {
                return;
            }

            VisionFlow? flow = Flows.FirstOrDefault(f => f.Steps.Contains(step));
            if (flow == null)
            {
                return;
            }

            flow.Steps.Remove(step);
            if (SelectedStep == step)
            {
                SelectedStep = null;
            }

            RunCommand.NotifyCanExecuteChanged();
            _status($"已从「{flow.Name}」移除步骤：{step.Name}");
        }

        partial void OnSelectedFlowChanged(VisionFlow? value)
        {
            RemoveFlowCommand.NotifyCanExecuteChanged();
            RunCommand.NotifyCanExecuteChanged();
            SelectedStep = null; // 换流程时清掉旧步骤选中，避免参数面板编辑到别处的步骤
        }

        /// <summary>执行选中流程（选中哪条跑哪条）。</summary>
        [RelayCommand(CanExecute = nameof(CanRun))]
        private void Run()
        {
            if (SelectedFlow == null)
            {
                return;
            }

            try
            {
                var outputs = _halcon.RunPipeline(SelectedFlow.Steps);
                if (outputs.Count > 0)
                {
                    _outputs.SetParameters(outputs);
                }

                _status($"「{SelectedFlow.Name}」执行完成：{SelectedFlow.Steps.Count} 个步骤");
            }
            catch (System.Exception ex)
            {
                _status(ex.Message);
            }
        }

        private bool CanRun => SelectedFlow != null && SelectedFlow.Steps.Count > 0;
    }
}
