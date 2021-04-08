using System.Text.RegularExpressions;

namespace gp
{
    /// <summary>
    /// c#定义
    /// </summary>
    public static class CSharpConstants
    {
        /// <summary>
        /// 关键词列表。
        /// </summary>
        public static readonly string[] Keywords = {
            "abstract", "as", "async", "await",
            "base", "break",
            "case", "catch", "checked", "class", "const", "continue",
            "default", "delegate", "do",
            "else", "enum", "event", "explicit", "extern",
            "finally", "fixed", "for", "foreach",
            "goto",
            "if", "implicit", "in", "interface", "internal", "is",
            "lock",
            "namespace", "new",
            "operator", "out", "override",
            "params", "private", "protected", "public",
            "readonly", "ref", "return",
            "sealed", "sizeof", "stackalloc", "static", "struct", "switch",
            "this", "throw", "try", "typeof",
            "unchecked", "unsafe", "using",
            "virtual", "void", "volatile",
            "while",
            "add", "alias", "ascending",
            "descending", "dynamic",
            "from", "get", "global", "group", "into", "join", "let", "orderby", "partial", "remove", "select", "set", "value", "var", "yield"
        };

        /// <summary>
        /// 代码块关键词。
        /// </summary>
        public static readonly string[] BlockKeywords = { "catch", "class", "do", "else", "finally", "for", "foreach", "if", "struct", "switch", "try", "while" };

        /// <summary>
        /// 定义关键词。
        /// </summary>
        public static readonly string[] DefineKeywords = { "class", "interface", "namespace", "struct", "var" };

        /// <summary>
        /// 类型。
        /// </summary>
        public static readonly string[] Types =
        {
            "Action", "Boolean", "Byte", "Char", "DateTime", "DateTimeOffset", "Decimal", "Double", "Func", "Guid",
            "Int16", "Int32", "Int64", "Object", "SByte", "Single", "String", "Task", "TimeSpan", "UInt16", "UInt32",
            "UInt64", "bool", "byte", "char", "decimal", "double", "short", "int", "long", "object", "sbyte", "float",
            "string", "ushort", "uint", "ulong"
        };

        /// <summary>
        /// 原子关键词。
        /// </summary>
        public static readonly string[] Atoms = { "true", "false", "null" };

        /// <summary>
        /// 数字表达式。
        /// </summary>
        public static readonly Regex NumbeRegex = new Regex(@"^(?:0x[a-f\d]+|0b[01]+|(?:\d+\.?\d*|\.\d+)(?:e[-+]?\d+)?)(u|ll?|l|f)?", RegexOptions.IgnoreCase);

        /// <summary>
        /// 操作符表达式。
        /// </summary>
        public static readonly Regex OperatorRegex = new Regex(@"[+\-*&%=<>!?|\/]");

        /// <summary>
        /// 标点符号表达式。
        /// </summary>
        public static readonly Regex PunctuationRegex = new Regex(@"[\[\]{}\(\),;\:\.]");
    }
}