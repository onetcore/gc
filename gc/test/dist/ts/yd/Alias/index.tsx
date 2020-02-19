import React, { useEffect, useState } from 'react';
import { message, Card, Table } from 'antd';
import { PageHeaderWrapper } from '@ant-design/pro-layout';
import { removeAsync, loadAsync } from './service';
import { UserAliasModel } from './model.d';

const Index: React.FC = () => {
    const [data, setData] = useState<UserAliasModel[]>();
    const [selectedRowKeys, setSelectedRowKeys] = useState<number[]>([]);

    const columns = [
        { title: '用户Id', dataIndex: 'userId'},
        { title: '用户名称', dataIndex: 'name'},
        { title: '角色Id', dataIndex: 'roleId'},
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

