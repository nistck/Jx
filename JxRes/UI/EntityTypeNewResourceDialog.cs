using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Jx;
using Jx.EntitySystem;

namespace JxRes.UI
{
    public partial class EntityTypeNewResourceDialog : Form
    {
        public static EntityTypes.ClassInfo LastEntityTypeChoosed { get; private set; }

        public EntityTypeNewResourceDialog(string directory)
        {
            InitializeComponent();
        }

        public string TypeName { get; private set; }

        public EntityTypes.ClassInfo TypeClass { get; private set; }
        

        private void EntityTypeNewResourceDialog_Load(object sender, EventArgs e)
        {
            int index = 0; 
            foreach (EntityTypes.ClassInfo classInfo in EntityTypes.Instance.Classes)
            {
                if (typeof(LogicComponentType).IsAssignableFrom(classInfo.TypeClassType) || classInfo.TypeClassType.IsAbstract)
                    continue;
                cboTypes.Items.Add(new JClassInfo(classInfo));
                if (LastEntityTypeChoosed != null && classInfo.Equals(LastEntityTypeChoosed))
                    index = cboTypes.Items.Count - 1;
            }
            cboTypes.SelectedIndex = index;
        }

        private class JClassInfo
        {
            public JClassInfo(EntityTypes.ClassInfo classInfo)
            {
                this.Class = classInfo;
            }

            public EntityTypes.ClassInfo Class { get; private set; }
            public override int GetHashCode()
            {
                if (Class == null)
                    return 0; 
                return Class.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                JClassInfo c = obj as JClassInfo;
                if (c == null || c.Class == null || Class == null)
                    return false;

                return c.Class.Equals(Class);
            }

            public override string ToString()
            {
                if (Class == null)
                    return "<未知类型>";

                object[] attrs = Class.EntityClassType.GetCustomAttributes(typeof(NameAttribute), false);
                if( attrs != null )
                {
                    NameAttribute nameAttr = attrs.OfType<NameAttribute>().FirstOrDefault();
                    if( nameAttr != null )
                    {
                        string text = string.Format("{0} ({1})", nameAttr.Name, Class.EntityClassType.Name);
                        return text;
                    }
                }

                return Class.ToString();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            string typeName = textName.Text.Trim(); 
            if( typeName == "" )
            {
                MessageBox.Show("请输入EntityType名称", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; 
            }
            if( CurrentClass == null )
            {
                MessageBox.Show("请选择EntityType类型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; 
            }

            TypeName = typeName;
            TypeClass = CurrentClass.Class;
            LastEntityTypeChoosed = TypeClass;
            DialogResult = DialogResult.OK;
            Close();
        }

        private JClassInfo CurrentClass
        {
            get { return cboTypes.SelectedItem as JClassInfo; }
        }
    }
}
