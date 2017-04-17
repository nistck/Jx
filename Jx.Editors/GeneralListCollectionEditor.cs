// Copyright (C) NeoAxis Group Ltd. This is part of NeoAxis 3D Engine SDK.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design;

using Jx;
using Jx.EntitySystem;

namespace Jx.EntitiesCommon.Editors
{
    public class GeneralListCollectionEditor : CollectionEditor
    {
        //Values of item's properties.
        //We need save values of properties because need restore values when user cancel changes.
        Dictionary<object, List<Tuple<PropertyInfo, object>>> propertyCopies;

        //

        public GeneralListCollectionEditor(Type type)
            : base(type) { }

        object GetOwner()
        {
            EntityTypeCustomTypeDescriptor typeDescriptor = Context.Instance as EntityTypeCustomTypeDescriptor;
            if (typeDescriptor != null)
                return typeDescriptor.EntityType;

            EntityCustomTypeDescriptor entityDescriptor = Context.Instance as EntityCustomTypeDescriptor;
            if (entityDescriptor != null)
                return entityDescriptor.Entity;

            return Context.Instance;
        }

        object GetList()
        {
            object owner = GetOwner();
            PropertyInfo listProperty = owner.GetType().GetProperty(Context.PropertyDescriptor.Name);
            object list = listProperty.GetValue(owner, null);
            return list;
        }

        int GetListCount(object list)
        {
            PropertyInfo property = list.GetType().GetProperty("Count");
            return (int)property.GetValue(list, null);
        }

        object GetListItem(object list, int index)
        {
            PropertyInfo property = list.GetType().GetProperty("Item");
            object item = property.GetValue(list, new object[] { index });
            return item;
        }

        protected override CollectionForm CreateCollectionForm()
        {
            object list = GetList();

            //copy object's properties
            propertyCopies = new Dictionary<object, List<Tuple<PropertyInfo, object>>>();

            int listCount = GetListCount(list);
            for (int n = 0; n < listCount; n++)
            {
                object listItem = GetListItem(list, n);

                List<Tuple<PropertyInfo, object>> pairList = new List<Tuple<PropertyInfo, object>>();

                foreach (PropertyInfo property in listItem.GetType().GetProperties())
                {
                    if (!property.CanWrite)
                        continue;
                    BrowsableAttribute[] browsableAttributes = (BrowsableAttribute[])property.
                        GetCustomAttributes(typeof(BrowsableAttribute), true);
                    if (browsableAttributes.Length != 0)
                    {
                        bool browsable = true;
                        foreach (BrowsableAttribute browsableAttribute in browsableAttributes)
                        {
                            if (!browsableAttribute.Browsable)
                            {
                                browsable = false;
                                break;
                            }
                        }
                        if (!browsable)
                            continue;
                    }

                    object value = property.GetValue(listItem, null);
                    pairList.Add(new Tuple<PropertyInfo, object>(property, value));
                }

                propertyCopies.Add(listItem, pairList);
            }

            CollectionForm form = base.CreateCollectionForm();
            form.FormClosed += FormClosed;
            return form;
        }

        void FormClosed(object sender, FormClosedEventArgs e)
        {
            CollectionForm form = (CollectionForm)sender;

            if (form.DialogResult == DialogResult.Cancel)
            {
                //restore properties
                foreach (KeyValuePair<object, List<Tuple<PropertyInfo, object>>> keyValuePair in propertyCopies)
                {
                    object listItem = keyValuePair.Key;
                    List<Tuple<PropertyInfo, object>> pairList = keyValuePair.Value;

                    foreach (Tuple<PropertyInfo, object> pair in pairList)
                    {
                        PropertyInfo property = pair.Item1;
                        object value = pair.Item2;
                        property.SetValue(listItem, value, null);
                    }
                }
            }
        }
    }
}
