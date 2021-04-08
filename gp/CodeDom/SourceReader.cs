using gp.Properties;

namespace gp
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    ///   实现 <see cref="T:System.IO.TextReader" /> ，使其从字符串读取。
    /// </summary>
    [ComVisible(true)]
    [Serializable]
    public class SourceReader : TextReader
    {
        private string _source;
        private int _position;
        private int _length;

        /// <summary>
        ///   新实例初始化 <see cref="SourceReader" /> 读取指定的字符串中的类。
        /// </summary>
        /// <param name="source">
        ///   字符串 <see cref="SourceReader" /> 应进行初始化。
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="source" /> 参数为 <see langword="null" />。
        /// </exception>
        public SourceReader(string source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _length = source.Length;
        }

        /// <summary>
        ///   关闭 <see cref="T:System.IO.StringReader" />。
        /// </summary>
        public override void Close()
        {
            Dispose(true);
        }

        /// <summary>
        ///   释放由 <see cref="T:System.IO.StringReader" /> 占用的非托管资源，还可以另外再释放托管资源。
        /// </summary>
        /// <param name="disposing">
        ///   若要释放托管资源和非托管资源，则为 <see langword="true" />；若仅释放非托管资源，则为 <see langword="false" />。
        /// </param>
        protected override void Dispose(bool disposing)
        {
            _source = null;
            _position = 0;
            _length = 0;
            base.Dispose(disposing);
        }

        /// <summary>返回下一个可用字符，但不使用它。</summary>
        /// <returns>读取，表示下一个字符的整数或-1，如果没有更多的可用字符或者流不支持查找。</returns>
        /// <exception cref="T:System.ObjectDisposedException">
        ///   当前读取器已关闭。
        /// </exception>
        public override int Peek()
        {
            if (_position == _length)
                return -1;
            return _source[_position];
        }

        /// <summary>返回下一个可用字符，但不使用它。</summary>
        /// <returns>读取，表示下一个字符的整数或'\0'，如果没有更多的可用字符或者流不支持查找。</returns>
        /// <exception cref="T:System.ObjectDisposedException">
        ///   当前读取器已关闭。
        /// </exception>
        public virtual char Current => _position == _length ? char.MinValue : _source[_position];

        /// <summary>输入字符串中读取下一个字符并将字符位置提升一个字符。</summary>
        /// <returns>下一步中的字符基础字符串，或者如果没有更多的可用字符则为-1。</returns>
        /// <exception cref="T:System.ObjectDisposedException">
        ///   当前读取器已关闭。
        /// </exception>
        public override int Read()
        {
            if (_position == _length)
                return -1;
            return _source[_position++];
        }

        /// <summary>输入字符串中读取下一个字符并将字符位置提升一个字符。</summary>
        /// <returns>下一步中的字符基础字符串，或者如果没有更多的可用字符则为'\0'。</returns>
        /// <exception cref="T:System.ObjectDisposedException">
        ///   当前读取器已关闭。
        /// </exception>
        public virtual char ReadChar()
        {
            if (_position == _length)
                return char.MinValue;
            return _source[_position++];
        }

        /// <summary>
        ///   在输入字符串中读取的字符块，并通过将字符位置提升 <paramref name="count" />。
        /// </summary>
        /// <param name="buffer">
        ///   此方法返回时，包含指定的字符数组，该数组的 <paramref name="index" /> 和 (<paramref name="index" /> + <paramref name="count" /> - 1) 之间的值由从当前源中读取的字符替换。
        /// </param>
        /// <param name="index">缓冲区中的起始索引。</param>
        /// <param name="count">要读取的字符数。</param>
        /// <returns>
        ///   读入缓冲区的总字符数。
        ///    这可以是小于的字符数请求如果许多字符当前不可用，或零个如果已到达基础字符串的末尾。
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="buffer" /> 为 <see langword="null" />。
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///   缓冲区长度减去 <paramref name="index" /> 小于 <paramref name="count" />。
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="index" /> 或 <paramref name="count" /> 为负数。
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///   当前读取器已关闭。
        /// </exception>
        public override int Read(char[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (buffer.Length - index < count)
                throw new ArgumentException(Resources.SourceReader_OutOfIndex);
            var size = _length - _position;
            if (size > 0)
            {
                if (size > count)
                    size = count;
                _source.CopyTo(_position, buffer, index, size);
                _position += size;
            }

            return size;
        }

        /// <summary>读取从当前位置到字符串结尾的所有字符并将它们作为一个字符串返回。</summary>
        /// <returns>来自当前位置到基础字符串末尾的内容。</returns>
        /// <exception cref="T:System.OutOfMemoryException">
        ///   没有足够的内存来为返回的字符串分配缓冲区。
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///   当前读取器已关闭。
        /// </exception>
        public override string ReadToEnd()
        {
            var text = _position != 0 ? _source.Substring(_position, _length - _position) : _source;
            _position = _length;
            return text;
        }

        /// <summary>从当前字符串中读取一行字符并返回数据作为字符串。</summary>
        /// <returns>
        ///   从当前字符串的下一行或 <see langword="null" /> 如果已到达字符串的结尾。
        /// </returns>
        /// <exception cref="T:System.ObjectDisposedException">
        ///   当前读取器已关闭。
        /// </exception>
        /// <exception cref="T:System.OutOfMemoryException">
        ///   没有足够的内存来为返回的字符串分配缓冲区。
        /// </exception>
        public override string ReadLine()
        {
            int pos;
            for (pos = _position; pos < _length; ++pos)
            {
                char ch = _source[pos];
                switch (ch)
                {
                    case '\n':
                    case '\r':
                        string str = _source.Substring(_position, pos - _position);
                        _position = pos + 1;
                        if (ch == '\r' && _position < _length && _source[_position] == '\n')
                            ++_position;
                        return str;
                    default:
                        continue;
                }
            }

            if (pos <= _position)
                return (string)null;
            var text = _source.Substring(_position, pos - _position);
            _position = pos;
            return text;
        }

        /// <summary>从当前字符串的异步读取一行字符并返回字符串形式的数据。</summary>
        /// <returns>
        ///   表示异步读取操作的任务。
        ///    值参数包含来自字符串读取器的下一行或 <see langword="null" /> 如果已读取所有字符。
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   下一行中的字符数大于 <see cref="F:System.Int32.MaxValue" />。
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///   字符串读取器已被释放。
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        ///   以前的读取操作当前正在使用读取器。
        /// </exception>
        [ComVisible(false)]
        public override Task<string> ReadLineAsync()
        {
            return Task.FromResult(ReadLine());
        }

        /// <summary>异步读取所有字符从当前位置到字符串的末尾，并将它们作为一个字符串返回。</summary>
        /// <returns>
        ///   表示异步读取操作的任务。
        ///    值参数包含从当前位置到字符串结尾的字符的字符串。
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   字符数大于 <see cref="F:System.Int32.MaxValue" />。
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///   字符串读取器已被释放。
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        ///   以前的读取操作当前正在使用读取器。
        /// </exception>
        [ComVisible(false)]
        public override Task<string> ReadToEndAsync()
        {
            return Task.FromResult(ReadToEnd());
        }

        /// <summary>异步从当前字符串中读取指定的最大字符数并将数据写入缓冲区中，指定索引处开始。</summary>
        /// <param name="buffer">
        ///   此方法返回时，包含指定的字符数组，该数组的 <paramref name="index" /> 和 (<paramref name="index" /> + <paramref name="count" /> - 1) 之间的值由从当前源中读取的字符替换。
        /// </param>
        /// <param name="index">
        ///   在 <paramref name="buffer" /> 中开始写入的位置。
        /// </param>
        /// <param name="count">
        ///   要读取的最大字符数。
        ///    如果在指定的数目的字符写入到缓冲区之前已到达字符串的末尾，该方法返回。
        /// </param>
        /// <returns>
        ///   表示异步读取操作的任务。
        ///   值参数的值包含读入缓冲区的总字节数。
        ///    结果值可能小于请求的字节数的如果当前可用字节数小于所请求的数目，或者如果已到达字符串结尾，它可以是 0 （零）。
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="buffer" /> 为 <see langword="null" />。
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="index" /> 或 <paramref name="count" /> 为负数。
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="index" /> 和 <paramref name="count" /> 的总和大于缓冲区长度。
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///   字符串读取器已被释放。
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        ///   以前的读取操作当前正在使用读取器。
        /// </exception>
        [ComVisible(false)]
        public override Task<int> ReadBlockAsync(char[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count));
            if (buffer.Length - index < count)
                throw new ArgumentException(Resources.SourceReader_OutOfIndex);
            return Task.FromResult(ReadBlock(buffer, index, count));
        }

        /// <summary>异步从当前字符串中读取指定的最大字符数并将数据写入缓冲区中，指定索引处开始。</summary>
        /// <param name="buffer">
        ///   此方法返回时，包含指定的字符数组，该数组的 <paramref name="index" /> 和 (<paramref name="index" /> + <paramref name="count" /> - 1) 之间的值由从当前源中读取的字符替换。
        /// </param>
        /// <param name="index">
        ///   在 <paramref name="buffer" /> 中开始写入的位置。
        /// </param>
        /// <param name="count">
        ///   要读取的最大字符数。
        ///    如果在指定的数目的字符写入到缓冲区之前已到达字符串的末尾，该方法返回。
        /// </param>
        /// <returns>
        ///   表示异步读取操作的任务。
        ///   值参数的值包含读入缓冲区的总字节数。
        ///    结果值可能小于请求的字节数的如果当前可用字节数小于所请求的数目，或者如果已到达字符串结尾，它可以是 0 （零）。
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="buffer" /> 为 <see langword="null" />。
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="index" /> 或 <paramref name="count" /> 为负数。
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="index" /> 和 <paramref name="count" /> 的总和大于缓冲区长度。
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///   字符串读取器已被释放。
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        ///   以前的读取操作当前正在使用读取器。
        /// </exception>
        [ComVisible(false)]
        public override Task<int> ReadAsync(char[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count));
            if (buffer.Length - index < count)
                throw new ArgumentException(Resources.SourceReader_OutOfIndex);
            return Task.FromResult(Read(buffer, index, count));
        }

        /// <summary>
        /// 读取直到遇到<paramref name="end"/>字符，读取的字符不包含<paramref name="end"/>字符。
        /// 需要判断结束符号，就是<seealso cref="Current"/>。
        /// </summary>
        /// <param name="end">结束字符。</param>
        /// <returns>返回当前读取到的字符串。</returns>
        public virtual string ReadUntil(char end = ' ')
        {
            var builder = new StringBuilder();
            while (_position < _length)
            {
                var current = Current;
                if (end == current)
                    return builder.ToString();
                builder.Append(current);
                _position++;
            }

            return builder.ToString();
        }

        /// <summary>
        /// 读取直到遇到<paramref name="ends"/>字符，读取的字符不包含<paramref name="ends"/>字符。
        /// 需要判断结束符号，就是<seealso cref="Current"/>。
        /// </summary>
        /// <param name="ends">分隔符。</param>
        /// <returns>返回当前读取到的字符串。</returns>
        public virtual string ReadUntil(params char[] ends)
        {
            var builder = new StringBuilder();
            while (_position < _length)
            {
                var current = Current;
                if (ends.Any(x => x == current))
                    return builder.ToString();
                builder.Append(current);
                _position++;
            }

            return builder.ToString();
        }

        /// <summary>
        /// 读取字符串直到<paramref name="end"/>。
        /// </summary>
        /// <param name="end">结束字符串。</param>
        /// <param name="includeEnd">是否包含<paramref name="end"/>字符串。</param>
        /// <returns>返回当前读取的字符串。</returns>
        public virtual string ReadUntil(string end, bool includeEnd = true)
        {
            var builder = new StringBuilder();
            while (!IsNext(end))
            {
                var current = ReadChar();
                if (current == char.MinValue)
                    break;
                builder.Append(current);
            }

            if (includeEnd)
            {
                builder.Append(end);
                Offset(end.Length);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 判断接下来的字符是否为<paramref name="end"/>。
        /// </summary>
        /// <param name="end">结束字符。</param>
        /// <returns>返回判断结果。</returns>
        public virtual bool IsNext(char end)
        {
            return Current == end;
        }

        /// <summary>
        /// 判断接下来的字符串是否为<paramref name="end"/>。
        /// </summary>
        /// <param name="end">结束字符串。</param>
        /// <returns>返回判断结果。</returns>
        public virtual bool IsNext(string end)
        {
            if (_length - _position < end.Length)
                return false;
            for (var i = 0; i < end.Length; i++)
            {
                if (_source[i + _position] != end[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 跳过空白字符。
        /// </summary>
        /// <returns>返回当前源码读取实例。</returns>
        public virtual SourceReader EscapeWhiteSpace()
        {
            while (_position < _length)
            {
                var current = Current;
                if (char.IsWhiteSpace(current))
                    _position++;
                else
                    break;
            }

            return this;
        }

        /// <summary>
        /// 查找字符。
        /// </summary>
        /// <param name="value">要查找的字符。</param>
        /// <returns>返回字符位置。</returns>
        public int IndexOf(char value)
        {
            return _source.IndexOf(value, _position);
        }

        /// <summary>
        /// 查找字符串。
        /// </summary>
        /// <param name="value">要查找的字符串。</param>
        /// <param name="comparison">字符对比选项。</param>
        /// <returns>返回字符串位置。</returns>
        public int IndexOf(string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return _source.IndexOf(value, _position, comparison);
        }

        /// <summary>
        /// 是否为引用字符串"",@"",@$"",''。
        /// </summary>
        /// <param name="quote">引用字符。</param>
        /// <param name="escape">返回转义字符。</param>
        /// <returns>返回判断结果。</returns>
        public virtual bool IsQuote(out char quote, out char escape)
        {
            if (IsNext("@$\"") || IsNext("@\""))
            {
                quote = '"';
                escape = '"';
                return true;
            }

            if (Peek() == '"')
            {
                quote = '"';
                escape = '\\';
                return true;
            }

            if (Peek() == '\'')
            {
                quote = '\'';
                escape = '\\';
                return true;
            }

            quote = '\0';
            escape = '\0';
            return false;
        }

        /// <summary>
        /// 读取引用字符块"",@"",@$"",''，调用之前需要先调用<seealso cref="IsQuote"/>方法进行判断。
        /// 读取的字符串包含结束字符<paramref name="quote"/>。
        /// </summary>
        /// <param name="quote">引用字符。</param>
        /// <param name="escape">转义字符。</param>
        /// <returns>返回引用字符块。</returns>
        public virtual string ReadQuote(char quote, char escape)
        {
            var builder = new StringBuilder();
            builder.Append(ReadUntil(quote));//如果是@"",@$""，附加@或者@$字符串。
            builder.Append(ReadChar());//第一个引号
            while (_position < _length)
            {
                var current = ReadChar();//读取一个字符
                if (current == escape)
                {//如果是转义符，不过下面一个字符，直接添加
                    builder.Append(current);
                    current = ReadChar();
                    if (current == int.MinValue) //已经结束了
                        return builder.ToString();
                    builder.Append(current);
                    continue;
                }

                if (current == quote)
                {//引用结束
                    builder.Append(current);
                    break;
                }
                builder.Append(current);
            }

            return builder.ToString();
        }

        /// <summary>
        /// 读取直到遇到字符<paramref name="end"/>，读取的字符不包含<paramref name="end"/>字符。
        /// 需要判断结束符号，就是<seealso cref="Current"/>。
        /// </summary>
        /// <param name="end">结束字符。</param>
        /// <returns>返回当前字符串。</returns>
        public virtual string ReadQuoteUntil(char end)
        {
            var builder = new StringBuilder();
            while (_position < _length)
            {
                if (IsQuote(out var quote, out var escape))
                    builder.Append(ReadQuote(quote, escape));
                else
                {
                    //读取注释
                    builder.Append(ReadComment());
                    var current = Current;
                    if (end == current)
                        return builder.ToString();
                    builder.Append(current);
                    _position++;
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// 读取直到遇到<paramref name="ends"/>字符，读取的字符不包含<paramref name="ends"/>字符。
        /// 需要判断结束符号，就是<seealso cref="Current"/>。
        /// </summary>
        /// <param name="ends">分隔符。</param>
        /// <returns>返回当前读取到的字符串。</returns>
        public virtual string ReadQuoteUntil(params char[] ends)
        {
            var builder = new StringBuilder();
            while (_position < _length)
            {
                if (IsQuote(out var quote, out var escape))
                    builder.Append(ReadQuote(quote, escape));
                else
                {
                    //读取注释
                    builder.Append(ReadComment());
                    var current = Current;
                    if (ends.Any(x => x == current))
                        return builder.ToString();
                    builder.Append(current);
                    _position++;
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// 读取字符串直到<paramref name="end"/>。
        /// </summary>
        /// <param name="end">结束字符串。</param>
        /// <returns>返回当前读取的字符串。</returns>
        public virtual string ReadQuoteUntil(string end)
        {
            var builder = new StringBuilder();
            while (!IsNext(end))
            {
                if (IsQuote(out var quote, out var escape))
                    builder.Append(ReadQuote(quote, escape));
                else
                {
                    //读取注释
                    builder.Append(ReadComment());
                    var current = ReadChar();
                    if (current == char.MinValue)
                        break;
                    builder.Append(current);
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 读取字符串直到<paramref name="separater"/>。
        /// </summary>
        /// <param name="separater">分隔符。</param>
        /// <param name="end">结束字符。</param>
        /// <returns>返回当前读取的字符串。</returns>
        public virtual string ReadQuoteUntil(string separater, char end)
        {
            var builder = new StringBuilder();
            while (CanRead)
            {
                //遇到分隔符返回
                if (IsNext(separater))
                    return builder.ToString();

                if (IsQuote(out var quote, out var escape))
                    builder.Append(ReadQuote(quote, escape));
                else
                {
                    //读取注释
                    builder.Append(ReadComment());
                    var current = Current;
                    if (current == end)
                        return builder.ToString();
                    Offset();
                    builder.Append(current);
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 读取字符串直到<paramref name="separater"/>。
        /// </summary>
        /// <param name="separater">分隔符。</param>
        /// <param name="ends">结束字符。</param>
        /// <returns>返回当前读取的字符串。</returns>
        public virtual string ReadQuoteUntil(string separater, params char[] ends)
        {
            var builder = new StringBuilder();
            while (CanRead)
            {
                //遇到分隔符返回
                if (IsNext(separater))
                    return builder.ToString();

                if (IsQuote(out var quote, out var escape))
                    builder.Append(ReadQuote(quote, escape));
                else
                {
                    //读取注释
                    builder.Append(ReadComment());
                    var current = Current;
                    if (ends.Any(x => x == current))
                        return builder.ToString();
                    Offset();
                    builder.Append(current);
                }

                EscapeWhiteSpace();
            }
            return builder.ToString();
        }

        /// <summary>
        /// 读取层级字符块。
        /// </summary>
        /// <param name="start">开始字符。</param>
        /// <param name="end">结束字符。</param>
        /// <returns>返回层级字符块。</returns>
        public virtual string ReadQuoteBlock(char start, char end)
        {
            if (!IsNext(start))
                throw new Exception(string.Format(Resources.SourceReader_StartCharNotMatch, start));
            var deep = 0;
            var builder = new StringBuilder();
            while (_position < _length)
            {
                // 是否为引用字符串
                if (IsQuote(out var quote, out var escape))
                {
                    builder.Append(ReadQuote(quote, escape));
                    continue;
                }

                //读取注释
                builder.Append(ReadComment());

                var current = Current;
                if (current == start)
                {//开始字符深度增加
                    deep++;
                }
                else if (current == end)
                {//结束字符深度减少
                    deep--;
                    if (deep <= 0)
                    {//结束后返回字符串
                        _position++;
                        builder.Append(current);
                        return builder.ToString();
                    }
                }

                builder.Append(current);
                _position++;
            }

            return builder.ToString();
        }

        /// <summary>
        /// 读取注释。
        /// </summary>
        /// <returns>返回注释代码。</returns>
        public virtual string ReadComment()
        {
            if (Current == '/' && _position + 1 < _length)
            {
                var next = _source[_position + 1];
                if (next == '*')
                    return ReadUntil("*/");
                if (next == '/')
                    return ReadLine() + "\r\n";
            }

            return null;
        }

        /// <summary>
        /// 读取直到遇到<paramref name="end"/>字符，读取的字符必须包含在<paramref name="chars"/>中。
        /// </summary>
        /// <param name="end">结束符。</param>
        /// <param name="chars">读取的字符。</param>
        /// <returns>返回当前读取到的字符串。</returns>
        public virtual string ReadChars(char end, char[] chars)
        {
            var builder = new StringBuilder();
            var index = _position;
            while (index < _length)
            {
                var current = _source[index];
                if (current == end)
                {//结束符
                    _position = index + 1;
                    builder.Append(current);
                    return builder.ToString();
                }

                //拥有未包含的字符
                if (chars.All(x => x != current))
                    return null;
                builder.Append(current);
                index++;
            }

            return null;
        }

        /// <summary>
        /// 是否可读。
        /// </summary>
        public bool CanRead => _position < _length;

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            if (CanRead)
                return _source.Substring(_position);
            return string.Empty;
        }

        /// <summary>
        /// 设置偏移量。
        /// </summary>
        /// <param name="offset">偏移量。</param>
        public void Offset(int offset = 1)
        {
            _position += offset;
        }
    }
}