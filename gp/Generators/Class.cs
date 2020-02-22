using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace gp
{
    /// <summary>
    /// 类型实例。
    /// </summary>
    public class Class : IEnumerable<Property>
    {
        internal Class(List<string> usings, string ns, List<string> attributes, string className, string description)
        {
            Usings = usings;
            Namespace = ns;
            Name = className;
            Description = description;
            Attributes.AddRange(attributes);
            attributes.Clear();
        }

        /// <summary>
        /// 引用命名空间。
        /// </summary>
        public List<string> Usings { get; }
        /// <summary>
        /// 当前命名空间。
        /// </summary>
        public string Namespace { get; }
        /// <summary>
        /// 类型特性。
        /// </summary>
        public List<string> Attributes { get; } = new List<string>();
        /// <summary>
        /// 类名称。
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 类注释。
        /// </summary>
        public string Description { get; }

        private readonly IDictionary<string, Property> _properties = new Dictionary<string, Property>(StringComparer.OrdinalIgnoreCase);
        /// <summary>返回一个循环访问集合的枚举器。</summary>
        /// <returns>用于循环访问集合的枚举数。</returns>
        public IEnumerator<Property> GetEnumerator()
        {
            return _properties.Values.GetEnumerator();
        }

        /// <summary>返回循环访问集合的枚举数。</summary>
        /// <returns>
        ///   一个可用于循环访问集合的 <see cref="T:System.Collections.IEnumerator" /> 对象。
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 索引获取属性。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>返回属性实例。</returns>
        public Property this[string name]
        {
            get
            {
                _properties.TryGetValue(name, out var property);
                return property;
            }
            set => _properties[name] = value;
        }

        /// <summary>
        /// 数据库迁移类代码。
        /// </summary>
        /// <returns>返回数据库迁移类代码。</returns>
        public string ToDataMigration()
        {
            var builder = new StringBuilder();
            builder.AppendLine(Namespace);
            builder.AppendLine("{")
                .AppendLine("    using Gentings.Data.Migrations;")
                .AppendLine();
            builder.AppendFormat(@"    /// <summary>
    /// 数据库迁移类。
    /// </summary>
    public class {0}DataMigration : DataMigration
    {{
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name=""builder"">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {{
            builder.CreateTable<{0}>(table => table", Name).AppendLine();
            foreach (var property in _properties.Values)
            {
                if (property.NotMapped) continue;
                builder.AppendLine(property.ToDataMigration());
            }
            builder.AppendLine("            );");
            builder.AppendLine("        }");
            builder.AppendLine("    }")
                .AppendLine("}");
            return builder.ToString();
        }


        /// <summary>
        /// 格式化为TypeScript脚本输出。
        /// </summary>
        /// <returns>返回TypeScript脚本。</returns>
        public string ToTypeScriptString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat(@"/**
 * {0}。
 */", Description).AppendLine();
            builder.Append("export interface ").Append(Name).AppendLine("Model {");
            foreach (var property in _properties.Values)
            {
                builder.AppendLine(property.ToTypeScriptString());
            }
            builder.AppendLine("}");
            return builder.ToString();
        }

        /// <summary>
        /// 将格式化为TypeScript表单脚本输出。
        /// </summary>
        /// <returns>返回TypeScript脚本。</returns>
        public string ToTypeScriptFormString(bool isCreated = true)
        {
            var action = isCreated ? "Create" : "Update";
            var actionLower = isCreated ? "create" : "update";
            var actionName = isCreated ? "添加" : "更新";
            var builder = new StringBuilder();
            //写入引用
            builder.AppendLine(@"import React, {{ useEffect }} from 'react';
import {{ Form, message, Input, Button, Card, Checkbox }} from 'antd';
import {{ PageHeaderWrapper }} from '@ant-design/pro-layout';");
            if (!isCreated)
            {
                builder.AppendLine("import { getPageQuery } from '@/utils/utils';");
                builder.AppendFormat(@"import {{ updateAsync, getAsync }} from './service';
import {{ {0}Model }} from './model.d';", Name.NormalizedName()).AppendLine().AppendLine();
            }
            else
                builder.AppendFormat(@"import {{ createAsync }} from './service';
import {{ {0}Model }} from './model.d';", Name.NormalizedName()).AppendLine().AppendLine();
            //写入布局。
            builder.AppendLine(@"const formItemLayout = {
    labelCol: {
        xs: {
            span: 24,
        },
        sm: {
            span: 7,
        },
    },
    wrapperCol: {
        xs: {
            span: 24,
        },
        sm: {
            span: 12,
        },
        md: {
            span: 10,
        },
    },
};
const submitFormLayout = {
    wrapperCol: {
        xs: {
            span: 24,
            offset: 0,
        },
        sm: {
            span: 10,
            offset: 7,
        },
    },
};").AppendLine();
            //写入头部
            builder.AppendFormat(@"const {0}Form: React.FC = () => {{
    const [form] = Form.useForm();

    const handleSubmit = async (values: {2}Model) => {{
        const hide = message.loading('正在{3}...');
        const result = await {1}Async(values);
        hide();
        if (result.status) {{
            message.success('成功{3}');
            return true;
        }}
        message.error(result.message);
        return false;
    }};", action, actionLower, Name.NormalizedName(), actionName).AppendLine().AppendLine();

            if (!isCreated)
            {
                //写入载入
                builder.AppendFormat(@"useEffect(() => {{
        const query = getPageQuery();
        const id = +query.id;
        getAsync(id).then(result => {{
            if (result.status) {{
                form.setFieldsValue(result.data);
            }}
            else {{
                message.error(result.message);
            }}
        }})
    }}, []);").AppendLine().AppendLine();
            }

            //写入呈现代码
            builder.AppendLine(@"    return (
        <PageHeaderWrapper>
            <Card>
                <Form {...formItemLayout} form={form} onFinish={handleSubmit}>");
            foreach (var property in _properties.Values)
            {
                if (!property.IgnoreJson || property.Identity)
                    continue;
                if (property.Type == "bool")
                    builder.AppendFormat(@"                    <Form.Item valuePropName=""checked"" name=""{0}"" label=""{1}"">
                        <Checkbox />
                    </Form.Item>", property.Name.NormalizedParameter(), property.Description).AppendLine();
                else
                    builder.AppendFormat(@"                    <Form.Item name=""{0}"" label=""{1}"">
                        <Input placeholder=""请输入"" />
                    </Form.Item>", property.Name.NormalizedParameter(), property.Description).AppendLine();
            }
            builder.AppendLine(@"                    <Form.Item
                        {...submitFormLayout}
                        style={{
                            marginTop: 32,
                        }}>
                        <Button htmlType=""submit"" type=""primary"">提交</Button>
                    </Form.Item>
                </Form>
            </Card>
        </PageHeaderWrapper>
    );
}");

            //写入导出
            builder.AppendFormat(@"
export default {0}Form;", action).AppendLine();
            return builder.ToString();
        }

        /// <summary>
        /// 表格列表页面字符串。
        /// </summary>
        /// <returns></returns>
        public string ToTypeScriptTableString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat(@"import React, {{ useEffect, useState }} from 'react';
import {{ message, Card, Table }} from 'antd';
import {{ PageHeaderWrapper }} from '@ant-design/pro-layout';
import {{ removeAsync, loadAsync }} from './service';
import {{ {0}Model }} from './model.d';

const Index: React.FC = () => {{
    const [data, setData] = useState<{0}Model[]>();
    const [selectedRowKeys, setSelectedRowKeys] = useState<number[]>([]);

    const columns = [", Name.NormalizedName()).AppendLine();
            foreach (var property in _properties.Values)
            {
                if (!property.IgnoreJson || property.Identity)
                    continue;
                builder.AppendFormat("        {{ title: '{0}', dataIndex: '{1}'}},", property.Description, property.Name.NormalizedParameter()).AppendLine();
            }
            builder.AppendLine(@"    ];
    const rowSelection = {
        selectedRowKeys,
        onChange: keys => {
            setSelectedRowKeys(keys);
        },
    };

    const handleRemoveSubmit = async () => {
        if (selectedRowKeys.length > 0) {
            const hide = message.loading('正在删除...');
            const result = await removeAsync(selectedRowKeys);
            hide();
            if (result.status) {
                message.success('删除成功');
                return true;
            }
            message.error(result.message);
            return false;
        }
        return false;
    };

    useEffect(() => {
        loadAsync().then(result => {
            if (result.status) {
                setData(result.data);
            }
            else {
                message.error(result.message);
            }
        })
    }, []);

    return (
        <PageHeaderWrapper>
            <Card>
                <Table rowSelection={rowSelection} columns={columns} dataSource={data} />
            </Card>
        </PageHeaderWrapper>
    );
};

export default Index;");
            return builder.ToString();
        }
    }
}