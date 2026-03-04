using Microsoft.Extensions.DependencyInjection;

namespace Hearth.Prism.Toolkit
{
    /// <summary>
    /// 服务特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class RegisterServiceAttribute : Attribute
    {
        /// <summary>
        /// 服务注册键
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 服务注册类型
        /// </summary>
        public Type? ServiceType { get; set; }

        /// <summary>
        /// 服务生命周期
        /// </summary>
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;

        /// <summary>
        /// 服务注册特性
        /// </summary>
        public RegisterServiceAttribute()
        {
        }

        /// <summary>
        /// 服务注册特性
        /// </summary>
        /// <param name="name">服务注册名称</param>
        public RegisterServiceAttribute(string name)
        {
            Name = name;
        }
    }
}