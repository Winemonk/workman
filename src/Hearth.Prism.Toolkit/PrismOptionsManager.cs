using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearth.Prism.Toolkit
{
    public sealed class PrismOptionsManager<TOptions> : IOptions<TOptions> where TOptions : class
    {
        private readonly IOptionsFactory<TOptions> _factory;

        private volatile object _syncObj;

        private volatile TOptions _value;

        public TOptions Value
        {
            get
            {
                TOptions value = _value;
                if (value != null)
                {
                    return value;
                }

                lock (_syncObj ?? Interlocked.CompareExchange(ref _syncObj, new object(), null) ?? _syncObj)
                {
                    return _value ?? (_value = _factory.Create(Options.DefaultName));
                }
            }
        }

        public PrismOptionsManager(IOptionsFactory<TOptions> factory)
        {
            _factory = factory;
        }
    }
}
