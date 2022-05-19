using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace gp
{
    /// <summary>
    /// 代码元素基类。
    /// </summary>
    public abstract class CodeElement : ICodeElement, IEnumerable<ICodeElement>
    {
        /// <summary>
        /// 父级元素。
        /// </summary>
        public ICodeElement Parent { get; }

        private List<CommentElement> _comments;
        /// <summary>
        /// 评论列表。
        /// </summary>
        public List<CommentElement> Comments
        {
            get
            {
                if (_comments == null)
                {
                    _comments = new List<CommentElement>();
                    if (Parent is CodeElement parent)
                    {
                        for (var i = Index - 1; i >= 0; i--)
                        {
                            var element = parent._elements[i];
                            if (element is CommentElement comment)
                                _comments.Add(comment);
                            else if (element is AttributeElement)
                                continue;
                            else
                                break;
                        }

                        _comments.Reverse();
                    }
                }

                return _comments;
            }
        }

        /// <summary>
        /// 注释内容。
        /// </summary>
        public string Comment => string.Join(", ", Comments
                    .Select(x => x.ToString().Trim().Trim('.', '。', '!', '/', ' ')
                        .Replace("获取", string.Empty)
                        .Replace("设置", string.Empty)
                        .Replace("获取或设置", string.Empty)
                        .Replace("获取或者设置", string.Empty)
                    )
                    .Where(x => !x.EndsWith(">")));

        private List<AttributeElement> _attributes;
        /// <summary>
        /// 评论列表。
        /// </summary>
        public List<AttributeElement> Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = new List<AttributeElement>();
                    if (Parent is CodeElement parent)
                    {
                        for (var i = Index - 1; i > 0; i--)
                        {
                            var element = parent._elements[i];
                            if (element is AttributeElement attribute)
                                _attributes.Add(attribute);
                            else if (element is CommentElement)
                                continue;
                            else
                                break;
                        }

                        _attributes.Reverse();
                    }
                }

                return _attributes;
            }
        }

        /// <summary>
        /// 判断是否定义特性。
        /// </summary>
        /// <param name="attributeName">特性名称。</param>
        /// <returns>返回判断结果。</returns>
        public bool IsDefined(string attributeName)
        {
            return Attributes.Any(x => x.Name.Equals(attributeName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 获取特性实例。
        /// </summary>
        /// <param name="attributeName">特性名称。</param>
        /// <returns>返回特性实例。</returns>
        public AttributeElement GetAttribute(string attributeName)
        {
            return Attributes.SingleOrDefault(x => x.Name.Equals(attributeName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 排序Id。
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 类型。
        /// </summary>
        public abstract CodeType Type { get; }

        /// <summary>
        /// 读取当前代码块。
        /// </summary>
        /// <param name="reader">字符串读取实例。</param>
        public abstract void Init(SourceReader reader);

        private int _index;
        private readonly List<ICodeElement> _elements = new List<ICodeElement>();
        /// <summary>
        /// 初始化类<see cref="CodeElement"/>。
        /// </summary>
        /// <param name="parent">父级元素。</param>
        protected CodeElement(ICodeElement parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// 添加代码元素。
        /// </summary>
        /// <param name="element">元素实例。</param>
        public void AddElement(ICodeElement element)
        {
            element.Index = _index++;
            _elements.Add(element);
        }

        /// <summary>
        /// 读取注释。
        /// </summary>
        /// <param name="reader">源代码读取器。</param>
        protected void ReadComment(SourceReader reader)
        {
            if (reader.IsNext("/*"))
                AddElement(new CommentElement(reader.ReadUntil("*/"), this));
            else if (reader.IsNext("//"))
                AddElement(new CommentElement(reader.ReadLine()?.Trim(), this));
            else
                throw new Exception("非法字符串！");
        }

        /// <summary>
        /// 读取特性。
        /// </summary>
        /// <param name="reader">源代码读取器。</param>
        protected void ReadAttribute(SourceReader reader)
        {
            AddElement(new AttributeElement(reader.ReadQuoteBlock('[', ']'), this));
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
    }
}