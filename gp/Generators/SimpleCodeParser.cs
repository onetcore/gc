using System;
using System.Collections.Generic;
using System.IO;

namespace gp
{
    /// <summary>
    /// 简单代码分析器，只用于简单实体类分析属性使用。
    /// </summary>
    public class SimpleCodeParser
    {
        /// <summary>
        /// 初始化类<see cref="SimpleCodeParser"/>。
        /// </summary>
        /// <param name="code">代码。</param>
        public SimpleCodeParser(string code)
        {
            using (var reader = new StringReader(code))
            {
                Initialize(reader);
            }
        }

        private void Initialize(StringReader reader)
        {
            var code = reader.ReadLine();
            while (code != null)
            {
                code = code.Trim();
                if (code.Length > 1)
                    Parse(code, reader);
                code = reader.ReadLine();
            }
        }

        private void ReadUtil(StringReader reader, Func<string, bool> func)
        {
            var code = reader.ReadLine();
            while (code != null)
            {
                code = code.Trim();
                if (code.Length > 1)
                {
                    //返回true结束当前操作，否则继续
                    if (func(code))
                        return;
                }
                code = reader.ReadLine();
            }
        }

        /// <summary>
        /// 引用命名空间。
        /// </summary>
        public List<string> Usings { get; } = new List<string>();

        private const string PublicClass = "public class ";
        private const string Namespace = "namespace ";
        private const string Using = "using ";
        private string _namespace;
        /// <summary>
        /// 当前类列表。
        /// </summary>
        public List<Class> Classes { get; } = new List<Class>();
        private void Parse(string code, StringReader reader)
        {
            switch (code[0])
            {
                case '/'://注释
                    ParseComment(code, reader);
                    return;
                case '[':
                    ParseAttribute(code, reader);
                    return;
            }
            if (code.StartsWith(Using))
                Usings.Add(code);
            else if (code.StartsWith(Namespace))
                _namespace = code.Trim('{', ' ');
            else if (code.StartsWith(PublicClass))
            {
                code = code.Substring(PublicClass.Length);
                var className = ReadName(code);
                var current = new Class(Usings, _namespace, _attributes, className, _description);
                _attributes.Clear();
                _description = null;
                Classes.Add(current);
                Parse(reader, current);
            }
        }

        private void Parse(StringReader reader, Class current)
        {
            var code = reader.ReadLine();
            while (code != null)
            {
                code = code.Trim();
                if (code.Length > 1)
                {
                    if (code.StartsWith(PublicClass) || code.StartsWith(Namespace))
                        Parse(code, reader);
                    else
                        Parse(code, reader, current);
                }
                code = reader.ReadLine();
            }
        }

        private const string Public = "public ";
        private const string PublicVirtual = "public virtual ";
        private const string PublicOverride = "public override ";
        private void Parse(string code, StringReader reader, Class current)
        {
            switch (code[0])
            {
                case '/'://注释
                    ParseComment(code, reader);
                    return;
                case '[':
                    ParseAttribute(code, reader);
                    return;
            }
            if (code.StartsWith(Using))
                Usings.Add(code);
            else if (SubstringStartsWith(ref code, Public) || SubstringStartsWith(ref code, PublicVirtual) || SubstringStartsWith(ref code, PublicOverride))
            {
                var type = ReadName(code);
                code = code.Substring(type.Length + 1).Trim();
                var name = ReadName(code);
                if (name == code)
                {
                    var ignored = false;
                    ReadUtil(reader, cur =>
                    {
                        if (cur.StartsWith("(")) //方法忽略
                        {
                            ignored = true;
                            return true;
                        }
                        if (cur.StartsWith("{"))//属性
                        {
                            return true;
                        }

                        return false;
                    });
                    if (ignored)//方法忽略
                        return;
                }
                code = code.Substring(name.Length + 1).Trim();
                if (code.StartsWith("{"))
                {
                    var property = new Property(current, _attributes, name, type, _description);
                    current[name] = property;
                    _attributes.Clear();
                    _description = null;
                }
            }

        }

        private bool SubstringStartsWith(ref string code, string starts)
        {
            if (code.StartsWith(starts))
            {
                code = code.Substring(starts.Length).Trim();
                return true;
            }

            return false;
        }

        private string ReadName(string code)
        {
            code = code.TrimStart();
            var name = "";
            for (int i = 0; i < code.Length; i++)
            {
                if (char.IsLetter(code[i]))
                    name += code[i];
                else
                    return name;
            }

            return name;
        }

        //特性
        private readonly List<string> _attributes = new List<string>();
        private void ParseAttribute(string code, StringReader reader)
        {
            if (code.EndsWith("]")) _attributes.Add(code);
            else ReadUtil(reader, current =>
            {
                code += current;
                if (current.EndsWith("]"))
                {
                    _attributes.Add(code);
                    return true;
                }
                return false;
            });
        }

        //注释
        private string _description;
        private void ParseComment(string code, StringReader reader)
        {
            //其他注释不关心
            if (!code.StartsWith("///"))
                return;
            code = code.Replace(" ", string.Empty).ToLower();
            //不是描述不关心
            if (!code.StartsWith("///<summary>"))
                return;
            _description = null;
            ReadUtil(reader, current =>
            {
                if (!current.StartsWith("///"))
                    return false;
                current = current.Replace(" ", string.Empty).ToLower();
                if (current.StartsWith("///</summary>"))
                    return true;
                if (_description == null)
                {
                    _description = current
                        .Replace("获取或设置", string.Empty)
                        .Replace("获取", string.Empty)
                        .Replace("设置", string.Empty)
                        .Trim('/', ' ', '.', '。');
                    return false;
                }
                throw new Exception(@"注释书写不规范，必须按照如下格式来书写：
    /// <summary>
    /// 描述信息
    /// </summary>
");
            });
        }
    }
}