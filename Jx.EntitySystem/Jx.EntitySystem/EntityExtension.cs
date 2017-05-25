using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.EntitySystem
{
    public static class EntityExtension
    {
        public static List<EntityTypes.ClassInfo> GetClassInfo(this EntityType entityType)
        {
            List<EntityTypes.ClassInfo> result = new List<EntityTypes.ClassInfo>();
            if (entityType == null)
                return result;
            for (EntityTypes.ClassInfo classInfo = entityType.ClassInfo; classInfo != null; classInfo = classInfo.BaseClassInfo)
                result.Add(classInfo);
            return result;
        }


        /// <summary>
        /// <para>手动创建Entity</para>
        /// <para>例子：</para>
        /// <para>    [ManualTypeCreate]</para>
        /// <para>    public class SimpleEntityType : EntityType {</para>
        /// <para>    }</para>
        /// <para>    </para>
        /// <para>    public class SimpleEntity : Entity {</para>
        /// <para>        SimpleEntityType _type = null;</para>
        /// <para>        public new SimpleEntityType Type { get { return _type; } }</para>
        /// <para>    }</para>
        /// <para>    </para>
        /// <para>使用方法：</para>
        /// <para>    SimpleEntity entity = ManualCreate&lt;SimpleEntityType, SimpleEntity&gt;("SimpleEntity", "", networkType)</para>
        /// <para>    其中，"SimpleEntity", 与 class SimpleEntity 对应， 且 class SimpleEntityType 的命名必须是 class SimpleEntity + "Type" (即： SimpleEntityType)</para>
        /// 
        /// </summary>
        /// <typeparam name="TElementType">EntityType类型</typeparam>
        /// <typeparam name="TElement">Entity类型</typeparam>
        /// <param name="typeName">Entity类型名称</param>
        /// <param name="entityClassName"></param>
        /// <param name="parentEntity"></param>
        /// <returns></returns>
        public static TElement ManualCreate<TElementType, TElement>(
            string typeName, string entityClassName, Entity parentEntity)
            where TElementType : EntityType, new()
            where TElement : Entity, new()
        {
            if (Entities.Instance == null || parentEntity == null)
                return default(TElement);

            TElementType elementType = (TElementType)EntityTypes.Instance.GetByName(typeName);
            if (elementType == null)
            {
                elementType = (TElementType)EntityTypes.Instance.ManualCreateType(typeName,
                        EntityTypes.Instance.GetClassInfoByEntityClassName(entityClassName)
                    );
            } 
            TElement result = (TElement)Entities.Instance.Create(elementType, parentEntity);
            return result;
        }

        /// <summary>
        /// <para>手动创建Entity</para>
        /// <para>例子：</para>
        /// <para>    [ManualTypeCreate]</para>
        /// <para>    public class SimpleEntityType : EntityType {</para>
        /// <para>    }</para>
        /// <para>    </para>
        /// <para>    public class SimpleEntity : Entity {</para>
        /// <para>        SimpleEntityType _type = null;</para>
        /// <para>        public new SimpleEntityType Type { get { return _type; } }</para>
        /// <para>    }</para>
        /// <para>    </para>
        /// <para>使用方法：</para>
        /// <para>    SimpleEntity entity = ManualCreate&lt;SimpleEntityType, SimpleEntity&gt;(networkType)</para>
        /// <para>    其中，"SimpleEntity", 与 class SimpleEntity 对应， 且 class SimpleEntityType 的命名必须是 class SimpleEntity + "Type" (即： SimpleEntityType)</para>
        /// 
        /// </summary>
        /// <typeparam name="TEntityType"></typeparam>
        /// <typeparam name="TEntity"></typeparam> 
        /// <returns></returns>
        public static TEntity ManualCreate<TEntityType, TEntity>(Entity parentEntity)
            where TEntityType : EntityType, new()
            where TEntity : Entity, new()
        {
            if (Entities.Instance == null || parentEntity == null)
                return default(TEntity);

            TEntityType entityType = (TEntityType)EntityTypes.Instance.GetByName(typeof(TEntity).Name); 
            TEntity entity = (TEntity)Entities.Instance.Create(entityType, parentEntity);
            return entity;
        }
    }
}
