using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HalconDotNet;
using HalconWpf.ViewModels;

namespace HalconWpf.Views
{
    /// <summary>
    /// Halcon 显示视图：Loaded 时把窗口交给 IHalconService，
    /// 只转发鼠标输入，不包含渲染逻辑。
    /// </summary>
    public partial class HalconView : UserControl
    {
        private HalconViewModel? _viewModel;

        public HalconView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as HalconViewModel;
            _viewModel?.Service.AttachWindow(HalconWindow);
        }

        private void HalconWindow_HMouseMove(object sender, HSmartWindowControlWPF.HMouseEventArgsWPF e)
        {
            _viewModel?.UpdateMousePosition(e.Row, e.Column);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            _viewModel?.ClearMousePosition();
        }
    }
}
