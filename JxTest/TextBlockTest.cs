using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Jx.IO;

namespace JxTest
{
    [TestClass]
    public class TextBlockTest
    {
        [TestMethod]
        public void TestLoadAndSave()
        {
            TextBlock textBlock = new TextBlock();

            string id = "09234";
            string name = "你的名字";

            textBlock.SetAttribute("id", id);
            textBlock.SetAttribute("name", name);

            string section = "管理器";
            TextBlock ctb = textBlock.AddChild(section);
            string section_id = "sec_928023874";
            string section_name = "中文系统简要设计";
            ctb.SetAttribute("id", section_id);
            ctb.SetAttribute("name", section_name);

            string rowsName = "rows";
            TextBlock rowsBlock = ctb.AddChild(rowsName);
            int rowCount = 8;

            rowsBlock.SetAttribute("cnt", Convert.ToString(rowCount));
            for(int i = 0; i < rowCount; i ++)
            {
                string row_name = string.Format("row_{0}", i);
                string row_value = string.Format("值_{0}", i);
                rowsBlock.SetAttribute(row_name, row_value);
            }

            string textResult = textBlock.DumpToString();

            string errorMessage = "";
            TextBlock block = TextBlock.Parse(textResult, out errorMessage);

            Assert.IsNotNull(block);
            // Level 0
            string _id = block.GetAttribute("id");
            Assert.IsTrue(id == _id);

            string _name = block.GetAttribute("name");
            Assert.IsTrue(name == _name);

            TextBlock _tbRnd = block.FindChild(Guid.NewGuid().ToString());
            Assert.IsNull(_tbRnd);

            TextBlock _ctb = block.FindChild(section);
            Assert.IsNotNull(_ctb);

            string _section_id = _ctb.GetAttribute("id");
            Assert.IsTrue(section_id == _section_id);

            string _section_name = _ctb.GetAttribute("name");
            Assert.IsTrue(section_name == _section_name);

            TextBlock _rowsBlock = _ctb.FindChild(rowsName);
            Assert.IsNotNull(_rowsBlock);

            string _cnt = _rowsBlock.GetAttribute("cnt");
            int nCnt = -1;
            int.TryParse(_cnt, out nCnt);
            Assert.IsTrue(nCnt == rowCount);

            for(int i = 0; i < nCnt; i++)
            {
                string _row_name = string.Format("row_{0}", i);
                string _row_value = string.Format("值_{0}", i);
                Assert.IsTrue(_rowsBlock.IsAttributeExist(_row_name));

                string __row_value = _rowsBlock.GetAttribute(_row_name);
                Assert.IsTrue(__row_value == _row_value);
            }
        } 
    }
}
