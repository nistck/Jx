﻿using System;

namespace Jx.Engine.Component
{
    public abstract class BaseComponent : IComponent
    {
        public Guid ID { get; } = Guid.NewGuid();
        public abstract IComponent Clone();
        public abstract void Reset();
    }
}