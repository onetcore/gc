using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gc
{
    /// <summary>
    /// 类配置。
    /// </summary>
    public class Class
    {
        /// <summary>
        /// 引用命名空间。
        /// </summary>
        public List<string> Using { get; set; }

        /// <summary>
        /// 基类，接口等。
        /// </summary>
        public List<string> Base { get; set; }

        /// <summary>
        /// 命名空间。
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// 类名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 属性。
        /// </summary>
        public List<Property> Props { get; set; }

        internal void AddUsing(string @namespace)
        {
            if (Using == null) Using = new List<string>();
            if (Using.Contains(@namespace)) return;
            Using.Add(@namespace);
        }

        /// <summary>
        /// 格式化输出类代码。
        /// </summary>
        /// <returns>返回代码字符串。</returns>
        public override string ToString()
        {
            var header = new StringBuilder();
            header.Append("namespace ").AppendLine(Namespace);
            header.AppendLine("{");
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(Desc))
            {
                builder.AppendLine("    /// <summary>");
                builder.Append("    /// ").Append(Desc).AppendLine("。");
                builder.AppendLine("    /// </summary>");
            }

            builder.Append("    public class ").Append(Name.Normalize());
            if (Base != null && Base.Count > 0)
            {
                builder.Append(" : ");
                var baseClass = Base.FirstOrDefault(x => x.StartsWith('I'));
                if (baseClass != null)
                {
                    Base.Remove(baseClass);
                    Base.Insert(0, baseClass);
                }

                builder.Append(string.Join(", ", Base));
            }

            builder.AppendLine();
            builder.AppendLine("    {");
            foreach (var property in Props)
            {
                property.Class = this;
                builder.AppendLine(property.ToString());
            }
            builder.AppendLine("    }");
            builder.AppendLine("}");

            if (Using != null && Using.Count > 0)
            {
                foreach (var @using in Using.Distinct())
                {
                    header.AppendLine($"    using {@using};");
                }

                header.AppendLine();
            }

            builder.Insert(0, header);
            return builder.ToString();
        }

        /// <summary>
        /// 数据库迁移类代码。
        /// </summary>
        /// <returns>返回数据库迁移类代码。</returns>
        public string ToDataMigration()
        {
            var builder = new StringBuilder();
            builder.Append("namespace ").AppendLine(Namespace);
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
            foreach (var property in Props)
            {
                if (!property.Map) continue;
                property.Class = this;
                builder.AppendLine(property.ToDataMigration());
            }

            var props = Props.Where(x => x.Unique);
            if (props.Any())
            {
                foreach (var property in props)
                {
                    builder.AppendLine($"                .UniqueConstraint(x => x.{property.Name.NormalizedName()})");
                }
            }

            var fkeys = Props.Where(x => x.FKey != null);
            if (fkeys.Any())
            {
                foreach (var property in fkeys)
                {
                    property.FKey.Property = property;
                    builder.AppendLine(property.FKey.ToString());
                }
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
 */", Desc).AppendLine();
            builder.Append("export interface ").Append(Name).AppendLine("Model {");
            foreach (var property in Props)
            {
                property.Class = this;
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
            foreach (var property in Props)
            {
                if (!property.Json || property.Identity)
                    continue;
                if (property.Type == "bool")
                    builder.AppendFormat(@"                    <Form.Item valuePropName=""checked"" name=""{0}"" label=""{1}"">
                        <Checkbox />
                    </Form.Item>", property.Name.NormalizedParameter(), property.Desc).AppendLine();
                else
                    builder.AppendFormat(@"                    <Form.Item name=""{0}"" label=""{1}"">
                        <Input placeholder=""请输入"" />
                    </Form.Item>", property.Name.NormalizedParameter(), property.Desc).AppendLine();
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
            foreach (var property in Props)
            {
                if (!property.Json || property.Identity)
                    continue;
                builder.AppendFormat("        {{ title: '{0}', dataIndex: '{1}'}},", property.Desc, property.Name.NormalizedParameter()).AppendLine();
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