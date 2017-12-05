﻿using System;
using Jx.Engine.Component;
using Jx.Engine.Entity;

namespace Jx.Engine.Extensions
{
    public static class EntityExtension
    {
        public static IComponent GetComponent<TComponentType>(this IEntity entity)
        {
            var componentTypeName = typeof (TComponentType);

            if (!entity.HasComponent(componentTypeName))
            {
                throw new ArgumentException("Entity does not have component of type: " + componentTypeName);
            }

            return entity.Components[componentTypeName];
        }

        public static bool HasComponent<TComponentType>(this IEntity entity)
        {
            return entity.HasComponent(typeof (TComponentType));
        }

        public static bool RemoveComponent<TComponentType>(this IEntity entity)
        {
            return entity.RemoveComponent(typeof (TComponentType));
        }
    }
}