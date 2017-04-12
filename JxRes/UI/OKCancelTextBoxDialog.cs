using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JxRes.UI
{
    public partial class OKCancelTextBoxDialog : Form
    {
        public OKCancelTextBoxDialog(string s1, string s2, string s3, Func<string, bool> callback)
        {
            InitializeComponent();
        }

        public string TextBoxText
        {
            get { return ">TextBoxText>"; }
        }
    }
}
