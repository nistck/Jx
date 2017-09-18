using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Jx.UI.Editors;
using Jx.MapSystem;
using System.Globalization;

namespace Jx.EntitiesCommon.Editors
{
    /// <summary>
    /// 在MapObject中引用
    /// </summary>
    public class EditorLayerEditor : BasePropertyEditor
    {
        private ListBox layersListBox;

        public EditorLayerEditor()
        {
            layersListBox = new ListBox();
            layersListBox.Click += LayersListBox_Click;
        }

        private void LayersListBox_Click(object sender, EventArgs e)
        {
            CloseDropdownWindow();
        }

        protected override object EndEdit(Control editControl, ITypeDescriptorContext context, object value)
        {
            LayerBean layerBean = layersListBox.SelectedItem as LayerBean;
            return layerBean == null? null : layerBean.Layer;

        }

        protected override Control GetEditControl(ITypeDescriptorContext context, object currentValue)
        {
            layersListBox.BorderStyle = BorderStyle.None;
            layersListBox.Items.Clear();

            if (Map.Instance != null)
            {
                List<Map.EditorLayer> layers = new List<Map.EditorLayer>();
                layers.Add(Map.Instance.RootEditorLayer);
                layers.AddRange(Map.Instance.RootEditorLayer.ChildrenDescent);

                // NULL
                layersListBox.Items.Add(new LayerBean(null));
                layers.Select(_layer => new LayerBean(_layer)).Any(_layer => {
                    layersListBox.Items.Add(_layer);
                    return false;
                });
            }

            layersListBox.SelectedIndex = layersListBox.FindString(Convert.ToString(currentValue));
            layersListBox.Height = layersListBox.PreferredHeight;
            return layersListBox;
        }
    }

    [TypeConverter(typeof(LayerBeanTypeConverter))]
    public class LayerBean
    {
        public LayerBean(Map.EditorLayer layer)
        {
            this.Layer = layer; 
        }

        public Map.EditorLayer Layer { get; private set; }

        public override string ToString()
        {
            if (Layer == null)
                return "<无>"; 

            string t = "";
            if (Layer != null && Layer.Indent > 0)
                t = new string(' ', Layer.Indent);

            string result = string.Format("{0}{1}", t, Layer.Name);
            return result; 
        }
    }

    public class LayerBeanTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
