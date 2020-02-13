import request from '@/utils/request';
import { Result, DataResult } from '@/models/result.d';
import { UserAliasModel } from './model.d';

/**
 * 获取用户别名。
 */
export async function loadAsync(): Promise<DataResult<UserAliasModel>> {
    return request<DataResult<UserAliasModel>>('/api/useralias');
}

/**
 * 获取用户别名。
 * @param params 查询实例。
 */
export async function getAsync(params: {userId: number, roleId: number}): Promise<DataResult<UserAliasModel>> {
    return request<DataResult<UserAliasModel>>('/api/useralias/get');
}

/**
 * 添加用户别名。
 * @param model {UserAliasModel} 用户别名实例。
 */
export async function createAsync(model: UserAliasModel): Promise<Result> {
    return request<Result>('/api/useralias/create', {
        method: 'POST',
        data: model
    });
}

/**
 * 删除用户别名。
 * @param params 主键实例。
 */
export async function removeAsync(params: {userId: number, roleId: number}): Promise<Result> {
    return request<Result>('/api/useralias/remove', {
        method: 'POST',
        data: params
    });
}

/**
 * 更新用户别名。
 * @param model {UserAliasModel} 用户别名实例。
 */
export async function updateAsync(model: UserAliasModel): Promise<Result> {
    return request<Result>('/api/useralias/update', {
        method: 'POST',
        data: model
    });
}
