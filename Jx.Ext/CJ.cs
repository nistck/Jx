using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace Jx.Ext
{
    public static class CJ
    {
        private static string GetCSharpStringOf(Type type)
        {
            CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("C#");
            CodeTypeReferenceExpression expression = new CodeTypeReferenceExpression(new CodeTypeReference(type));
            string result;
            using (StringWriter stringWriter = new StringWriter())
            {
                codeDomProvider.GenerateCodeFromExpression(expression, stringWriter, new CodeGeneratorOptions());
                result = stringWriter.GetStringBuilder().ToString();
            }
            return result;
        }

        public static string TypeToCSharpString(Type type)
        {
            string result;
            try
            {
                result = CJ.GetCSharpStringOf(type);
            }
            catch
            {
                string text = type.FullName.Replace("+", ".");
                result = text;
            }
            return result;
        }
    }
}
