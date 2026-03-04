namespace Hearth.Prism.Toolkit
{
    /// <summary>
    /// 区域特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class RegisterRegionAttribute : Attribute
    {
        /// <summary>
        /// 区域名
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 区域视图类型
        /// </summary>
        public Type? ViewType { get; set; }

        public RegisterRegionAttribute(string name)
        {
            Name = name;
        }

        public RegisterRegionAttribute(string name, Type viewType)
        {
            Name = name;
            ViewType = viewType;
        }
    }
}
