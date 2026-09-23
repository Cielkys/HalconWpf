using System.Collections.Generic;

namespace HalconWpf.Models
{
    /// <summary>
    /// 一个输出参数（可为数组，如 area[]、row[]、col[]）。
    /// </summary>
    public class OutputParameter
    {
        public OutputParameter(string name, params double[] values)
        {
            Name = name;
            Values = values ?? new double[0];
        }

        /// <summary>参数名，如 area、row、col。</summary>
        public string Name { get; }

        /// <summary>参数值数组。</summary>
        public IReadOnlyList<double> Values { get; }
    }
}
