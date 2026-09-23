namespace HalconWpf.Models
{
    /// <summary>
    /// 输出参数表格的一行（参数名 = 值）。
    /// </summary>
    public class ParameterRow
    {
        public ParameterRow(string element, string value)
        {
            Element = element;
            Value = value;
        }

        /// <summary>参数名，如 area。</summary>
        public string Element { get; }

        /// <summary>格式化后的值（数组用逗号分隔）。</summary>
        public string Value { get; }
    }
}
