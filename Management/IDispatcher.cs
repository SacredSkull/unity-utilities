using System;

namespace UnityUtilities.Management
{
    public interface IDispatcher {
        void Post(Action<object> action, object state = null);
    }
}