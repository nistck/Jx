using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Jx.Ext;
using Jx.FileSystem;

namespace Jx.EntitySystem
{
    [AllowToCreateTypeBasedOnThisClass(false)]
    public class WorldType : EntityType
    {
        public WorldType()
        {
            base.NetworkType = EntityNetworkTypes.Synchronized;
            base.AllowEmptyName = true;
            base.CreatableInMapEditor = false;
        }
    }

    public class World : Entity
    {
        private static World instance;
        private EngineRandom random = new EngineRandom();
        private Dictionary<string, object> customSerializationValues = new Dictionary<string, object>();

        [Entity.TypeFieldAttribute]
        private WorldType _type = null;

        public new WorldType Type
        {
            get
            {
                return this._type;
            }
        }

        public static World Instance
        {
            get
            {
                return instance;
            }
        }

        [Browsable(false)]
        public EngineRandom Random
        {
            get
            {
                return this.random;
            }
        }

        public World()
        {
            if (instance != null)
            {
                Log.Fatal("\"World\" already created.");
            }
            instance = this;
        }

        protected override bool OnLoad(TextBlock block)
        {
            return base.OnLoad(block) && this.LoadCustomSerializationValues(block);
        }

        protected override void OnSave(TextBlock block)
        {
            base.OnSave(block);
 
            this.SaveCustomSerializationValues(block);
        }

        protected override void OnPostCreate(bool loaded)
        {
            base.OnPostCreate(loaded);
            base.SubscribeToTickEvent();
        }

        protected override void OnDestroy()
        {
            instance = null;
            base.OnDestroy();
        }

        protected override void OnDeleteSubscribedToDeletionEvent(Entity entity)
        {
            base.OnDeleteSubscribedToDeletionEvent(entity);
            this.ClearCustomSerializationValue(entity);
        }

        internal void DeleteEntitiesQueuedForDeletion()
        {
            Entities.Instance.DeleteEntitiesQueuedForDeletion();
            while (true)
            {
                Entity entity = this;
                while (true)
                {
                    using (LinkedList<Entity>.Enumerator enumerator = entity.Children.GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                        {
                            Entity current = enumerator.Current;
                            entity = current;
                            continue;
                        }
                    }
                    break;
                }
                if (entity == this)
                {
                    break;
                }
                entity.SetForDeletion(false);
                Entities.Instance.DeleteEntitiesQueuedForDeletion();
            }
        }

        public object GetCustomSerializationValue(string name)
        {
            object result;
            if (this.customSerializationValues.TryGetValue(name, out result))
            {
                return result;
            }
            return null;
        }

        
        public void SetCustomSerializationValue(string name, object value)
        {
            if (!StringUtils.IsCorrectIdentifierName(name))
            {
                Log.Fatal("World: SetCustomSerializationValue: Incorrect identifier name \"{0}\".", name);
                return;
            }
            this.customSerializationValues[name] = value;
            Entity entity = value as Entity;
            if (entity != null)
            {
                base.SubscribeToDeletionEvent(entity);
            }
        }
        public void ClearCustomSerializationValue(string name)
        {
            object obj;
            if (!this.customSerializationValues.TryGetValue(name, out obj))
            {
                return;
            }
            this.customSerializationValues.Remove(name);
            Entity entity = obj as Entity;
            if (entity != null)
            {
                base.UnsubscribeToDeletionEvent(entity);
            }
        }
        public void ClearAllCustomSerializationValues()
        {
            while (true)
            {
                using (Dictionary<string, object>.KeyCollection.Enumerator enumerator = this.customSerializationValues.Keys.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        string current = enumerator.Current;
                        this.ClearCustomSerializationValue(current);
                        continue;
                    }
                }
                break;
            }
        }

        private bool LoadCustomSerializationValues(TextBlock textBlock)
        {
            TextBlock customValuesBlock = textBlock.FindChild("customSerializationValues");
            if (customValuesBlock == null)
                return true;

            foreach (TextBlock current in customValuesBlock.Children)
            {
                string name = current.Name;
                string typeName = current.GetAttribute("type");
                string valueString = current.GetAttribute("value");
                string text = string.Format("World: Custom serialization value \"{0}\"", name);
                Type type = null;
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < assemblies.Length; i++)
                {
                    Assembly assembly = assemblies[i];
                    type = assembly.GetType(typeName);
                    if (type != null)
                        break;
                }

                object value;
                if (type == null)
                    Log.Warning("Entity System: Serialization error. The class type is not found \"{0}\" ({1}).", typeName, text);
                else if (EntityHelper.ConvertFromString(type, valueString, text, out value))
                    SetCustomSerializationValue(name, value);
            }
            return true;
        }

        private void SaveCustomSerializationValues(TextBlock textBlock)
        {
            if (customSerializationValues.Count == 0)
                return;
            
            TextBlock customValuesBlock = textBlock.AddChild("customSerializationValues");
            foreach (KeyValuePair<string, object> current in customSerializationValues)
            {
                string key = current.Key;
                object value = current.Value;
                if (value == null)
                    continue;
                Type type = value.GetType();
                string errorString = string.Format("World: Custom serialization value \"{0}\"", key);
                string saveValueString = EntityHelper.ConvertToString(type, value, errorString);
                TextBlock customValueBlock = customValuesBlock.AddChild(key);
                customValueBlock.SetAttribute("type", type.FullName);
                customValueBlock.SetAttribute("value", saveValueString);
            }
            ClearAllCustomSerializationValues();
        }

        private void ClearCustomSerializationValue(Entity entity)
        {
            IL_00:
            while (this.customSerializationValues.Count != 0)
            {
                foreach (KeyValuePair<string, object> current in customSerializationValues)
                {
                    if (current.Value == entity)
                    {
                        this.ClearCustomSerializationValue(current.Key);
                        goto IL_00;
                    }
                }
                break;
            }
        }
    }
}
