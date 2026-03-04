namespace Hearth.Prism.Toolkit
{
    /// <summary>
    /// 对话框特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class RegisterDialogAttribute : Attribute
    {
        /// <summary>
        /// 对话框名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 对话框视图类型
        /// </summary>
        public Type? ViewType { get; set; }

        public RegisterDialogAttribute(string name)
        {
            Name = name;
        }

        public RegisterDialogAttribute(string name, Type? viewType)
        {
            Name = name;
            ViewType = viewType;
        }
    }
}
