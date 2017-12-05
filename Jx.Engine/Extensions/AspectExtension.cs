using System;
using Jx.Engine.Aspect;
using Jx.Engine.Component;

namespace Jx.Engine.Extensions
{
    public static class AspectExtension
    {
        public static T GetComponent<T>(this IAspect aspect) where T : class, IComponent
        {
            if (!aspect.Components.ContainsKey(typeof (T)))
            {
                throw new InvalidOperationException("Component type " + typeof (T).Name + "not found.");
            }

            return aspect.Components[typeof(T)] as T;
        }
    }
}