using System;

namespace Jx.Engine.Attributes
{
    public interface IAssociatedComponentsAttribute
    {
        Type[] ComponentTypes { get; }
    }
}