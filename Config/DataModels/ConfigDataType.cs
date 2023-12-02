namespace Config.DataModels
{
    /// <summary>
    /// 配置文件数据类型
    /// </summary>
    public class ConfigDataType
    {
        /// <summary>
        /// 项名
        /// </summary>
        //public required string Name { get; set; }
        /// <summary>
        /// 项类型
        /// </summary>
        public string? TypeName { get; set; }
        /// <summary>
        /// 项值
        /// </summary>
        public object? Value { get; set; }
        /// <summary>
        /// 获取动态类型的值
        /// </summary>
        public dynamic? DynamicValue()
        {
            if (Value == null)
            {
                return null;
            }
            if (TypeName == null)
            {
                return Value;
            }
            return Convert.ChangeType(Value, Type.GetType(TypeName)!);
        }
    }
}
