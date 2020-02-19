import React, { useEffect, useState } from 'react';
import { message, Card, Table } from 'antd';
import { PageHeaderWrapper } from '@ant-design/pro-layout';
import { removeAsync, loadAsync } from './service';
import { UserModel } from './model.d';

const Index: React.FC = () => {
    const [data, setData] = useState<UserModel[]>();
    const [selectedRowKeys, setSelectedRowKeys] = useState<number[]>([]);

    const columns = [
        { title: '名称', dataIndex: 'userName'},
        { title: '真实姓名', dataIndex: 'realName'},
        { title: '真实姓名', dataIndex: 'password'},
    ];
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

export default Index;

