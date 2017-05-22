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
            Map.Layer layer = layersListBox.SelectedItem as Map.Layer;
            return layer == null? null : layer.Path;

        }

        protected override Control GetEditControl(ITypeDescriptorContext context, object currentValue)
        {
            layersListBox.BorderStyle = BorderStyle.None;
            layersListBox.Items.Clear();

            if (Map.Instance != null)
            {
                List<Map.Layer> layers = new List<Map.Layer>();
                layers.Add(Map.Instance.RootLayer);
                layers.AddRange(Map.Instance.RootLayer.ChildrenDescent);

                layers.Any(_layer => {
                    layersListBox.Items.Add(_layer);
                    return false;
                });
            }

            layersListBox.SelectedIndex = layersListBox.FindString(Convert.ToString(currentValue));
            layersListBox.Height = layersListBox.PreferredHeight;
            return layersListBox;
        }
    }

    public class LayerBean
    {
        public LayerBean(Map.Layer layer)
        {
            this.Layer = layer; 
        }

        public Map.Layer Layer { get; private set; }

        public override string ToString()
        {
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
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
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
