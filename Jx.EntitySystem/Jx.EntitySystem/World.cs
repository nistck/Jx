﻿using System;
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
                return World.instance;
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
            if (World.instance != null)
            {
                Log.Fatal("\"World\" already created.");
            }
            World.instance = this;
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

        protected internal override void OnPostCreate(bool loaded)
        {
            base.OnPostCreate(loaded);
            base.SubscribeToTickEvent();
        }

        protected internal override void OnDestroy()
        {
            World.instance = null;
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
            TextBlock textBlock2 = textBlock.FindChild("customSerializationValues");
            if (textBlock2 != null)
            {
                foreach (TextBlock current in textBlock2.Children)
                {
                    string name = current.Name;
                    string attribute = current.GetAttribute("type");
                    string attribute2 = current.GetAttribute("value");
                    string text = string.Format("World: Custom serialization value \"{0}\"", name);
                    Type type = null;
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    for (int i = 0; i < assemblies.Length; i++)
                    {
                        Assembly assembly = assemblies[i];
                        type = assembly.GetType(attribute);
                        if (type != null)
                        {
                            break;
                        }
                    }
                    object value;
                    if (type == null)
                    {
                        Log.Warning("Entity System: Serialization error. The class type is not found \"{0}\" ({1}).", attribute, text);
                    }
                    else if (Ci.GetLoadStringValue(type, attribute2, text, out value))
                    {
                        SetCustomSerializationValue(name, value);
                    }
                }
            }
            return true;
        }

        private void SaveCustomSerializationValues(TextBlock textBlock)
        {
            if (customSerializationValues.Count == 0)
            {
                return;
            }
            TextBlock textBlock2 = textBlock.AddChild("customSerializationValues");
            foreach (KeyValuePair<string, object> current in customSerializationValues)
            {
                string key = current.Key;
                object value = current.Value;
                if (value != null)
                {
                    Type type = value.GetType();
                    string errorString = string.Format("World: Custom serialization value \"{0}\"", key);
                    string saveValueString = Ci.GetSaveValueString(type, value, errorString);
                    TextBlock textBlock3 = textBlock2.AddChild(key);
                    textBlock3.SetAttribute("type", type.FullName);
                    textBlock3.SetAttribute("value", saveValueString);
                }
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
