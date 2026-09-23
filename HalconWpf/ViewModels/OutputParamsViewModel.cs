using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HalconWpf.Models;

namespace HalconWpf.ViewModels
{
    /// <summary>
    /// 输出参数面板视图模型：负责参数行的展开与清空。
    /// </summary>
    public partial class OutputParamsViewModel : ObservableObject
    {
        /// <summary>DataGrid 绑定的行集合。</summary>
        public ObservableCollection<ParameterRow> Rows { get; } = new();

        /// <summary>显示一组输出参数</summary>
        public void SetParameters(IEnumerable<OutputParameter> parameters)
        {
            Rows.Clear();
            foreach (OutputParameter parameter in parameters)
            {
                string value = string.Join(", ", parameter.Values.Select(v => v.ToString("0.###")));
                Rows.Add(new ParameterRow(parameter.Name, value));
            }
        }

        /// <summary>清空显示</summary>
        [RelayCommand]
        public void Clear()
        {
            Rows.Clear();
        }
    }
}
