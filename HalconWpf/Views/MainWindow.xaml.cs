using System;
using System.Windows;
using HalconWpf.ViewModels;

namespace HalconWpf
{
    /// <summary>
    /// 主窗口：仅负责创建视图模型并绑定 DataContext，不含业务逻辑。
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        protected override void OnClosed(EventArgs e)
        {
            _viewModel.Dispose();
            base.OnClosed(e);
        }
    }
}
