namespace Hearth.Prism.Toolkit
{
    /// <summary>
    /// 导航特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class RegisterNavigationAttribute : Attribute
    {
        /// <summary>
        /// 导航名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 导航视图类型
        /// </summary>
        public Type? ViewType { get; set; }

        public RegisterNavigationAttribute(string name)
        {
            Name = name;
        }

        public RegisterNavigationAttribute(string name, Type? viewType)
        {
            Name = name;
            ViewType = viewType;
        }
    }
}
