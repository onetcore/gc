using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gp
{
    /// <summary>
    /// c#文件。
    /// </summary>
    public class FileElement : IEnumerable<ICodeElement>, ICodeElement
    {
        private int _id;
        private readonly List<ICodeElement> _elements = new List<ICodeElement>();
        /// <summary>
        /// 添加代码元素。
        /// </summary>
        /// <param name="element">元素实例。</param>
        public void AddElement(ICodeElement element)
        {
            element.Index = ++_id;
            _elements.Add(element);
        }

        private IEnumerable<string> _usings;
        /// <summary>
        /// 判断是否引入空间。
        /// </summary>
        /// <param name="ns">命名空间。</param>
        /// <returns>返回判断结果。</returns>
        public bool IsUsing(string ns) =>
            (_usings ??= _elements.OfType<UsingElement>().Select(x => x.Namespace).ToList()).Any(x => x.Equals(ns));

        /// <summary>
        /// 初始化类<see cref="FileElement"/>。
        /// </summary>
        /// <param name="source">文件代码。</param>
        public FileElement(string source)
        {
            using var reader = new SourceReader(source);
            reader.EscapeWhiteSpace();
            while (reader.CanRead)
            {
                switch (reader.Current)
                {
                    case 'u'://using
                        {
                            var keyword = reader.ReadUntil();
                            if (keyword == "using")
                            {
                                AddElement(new UsingElement(reader.ReadUntil(';').Trim(), this));
                                reader.Offset();
                                break;
                            }

                            throw new Exception($"不支持：{keyword}关键词！");
                        }
                    case 'n'://namespace
                        {
                            var keyword = reader.ReadUntil();
                            if (keyword == "namespace")
                            {
                                var @namespace = new NamespaceElement(reader.ReadUntil('{').Trim(), this);
                                @namespace.Init(reader);
                                AddElement(@namespace);
                                break;
                            }
                            throw new Exception($"不支持：{keyword}关键词！");
                        }
                    case '/':
                        {
                            if (reader.IsNext("/*"))
                                AddElement(new CommentElement(reader.ReadUntil("*/"), this));
                            else if (reader.IsNext("//"))
                                AddElement(new CommentElement(reader.ReadLine()?.Trim() + "\r\n", this));
                            else
                                throw new Exception("非法字符串！");
                        }
                        break;
                    case '[':
                        AddElement(new AttributeElement(reader.ReadQuoteBlock('[', ']'), this));
                        break;
                }
                reader.EscapeWhiteSpace();
            }
        }

        /// <summary>
        /// 读取文件中的c#代码。
        /// </summary>
        /// <param name="path">代码路径。</param>
        /// <returns>返回c#代码实例。</returns>
        public static FileElement FromFile(string path)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var fr = new StreamReader(fs, Encoding.UTF8);
            return new FileElement(fr.ReadToEnd());
        }

        /// <summary>
        /// 保存到文件中。
        /// </summary>
        /// <param name="path">文件路径。</param>
        public void SaveAs(string path)
        {
            using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            using var sw = new StreamWriter(fs, Encoding.UTF8);
            sw.Write(ToString());
        }

        /// <summary>
        /// 保存到文件中。
        /// </summary>
        /// <param name="path">文件路径。</param>
        public async Task SaveAsAsync(string path)
        {
            using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            using var sw = new StreamWriter(fs, Encoding.UTF8);
            await sw.WriteAsync(ToString());
        }

        /// <summary>返回一个循环访问集合的枚举器。</summary>
        /// <returns>用于循环访问集合的枚举数。</returns>
        public IEnumerator<ICodeElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        /// <summary>返回循环访问集合的枚举数。</summary>
        /// <returns>
        ///   一个可用于循环访问集合的 <see cref="T:System.Collections.IEnumerator" /> 对象。
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return string.Join("\r\n", _elements);
        }

        /// <summary>
        /// 排序Id。
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 类型。
        /// </summary>
        public CodeType Type => CodeType.File;

        /// <summary>
        /// 父级元素。
        /// </summary>
        public ICodeElement Parent => null;

        /// <summary>
        /// 获取所有类型列表。
        /// </summary>
        public IEnumerable<TElement> GetTypes<TElement>() where TElement : TypeElement
        {
            return this.OfType<NamespaceElement>().SelectMany(x => x.OfType<TElement>());
        }
    }
}