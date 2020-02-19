import React, {{ useEffect }} from 'react';
import {{ Form, message, Input, Button, Card, Checkbox }} from 'antd';
import {{ PageHeaderWrapper }} from '@ant-design/pro-layout';
import { createAsync } from './service';
import { UserModel } from './model.d';

const formItemLayout = {
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
};

const CreateForm: React.FC = () => {
    const [form] = Form.useForm();

    const handleSubmit = async (values: UserModel) => {
        const hide = message.loading('正在添加...');
        const result = await createAsync(values);
        hide();
        if (result.status) {
            message.success('成功添加');
            return true;
        }
        message.error(result.message);
        return false;
    };

    return (
        <PageHeaderWrapper>
            <Card>
                <Form {...formItemLayout} form={form} onFinish={handleSubmit}>
                    <Form.Item name="userName" label="名称">
                        <Input placeholder="请输入" />
                    </Form.Item>
                    <Form.Item name="realName" label="真实姓名">
                        <Input placeholder="请输入" />
                    </Form.Item>
                    <Form.Item name="password" label="真实姓名">
                        <Input placeholder="请输入" />
                    </Form.Item>
                    <Form.Item
                        {...submitFormLayout}
                        style={{
                            marginTop: 32,
                        }}>
                        <Button htmlType="submit" type="primary">提交</Button>
                    </Form.Item>
                </Form>
            </Card>
        </PageHeaderWrapper>
    );
}

export default CreateForm;

