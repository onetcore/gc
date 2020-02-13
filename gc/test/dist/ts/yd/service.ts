import request from '@/utils/request';
import { Result, DataResult } from '@/models/result.d';
import { UserModel } from './model.d';

/**
 * 获取用户实体。
 */
export async function loadAsync(): Promise<DataResult<UserModel>> {
    return request<DataResult<UserModel>>('/api/user');
}

/**
 * 获取用户实体。
 * @param id {number} 唯一Id实例。
 */
export async function getAsync(id: number): Promise<DataResult<UserModel>> {
    return request<DataResult<UserModel>>('/api/user/get');
}

/**
 * 添加用户实体。
 * @param model {UserModel} 用户实体实例。
 */
export async function createAsync(model: UserModel): Promise<Result> {
    return request<Result>('/api/user/create', {
        method: 'POST',
        data: model
    });
}

/**
 * 删除用户实体。
 * @param ids {number[]} 唯一Id列表。
 */
export async function removeAsync(ids: number[]): Promise<Result> {
    return request<Result>('/api/user/remove', {
        method: 'POST',
        data: ids
    });
}

/**
 * 更新用户实体。
 * @param model {UserModel} 用户实体实例。
 */
export async function updateAsync(model: UserModel): Promise<Result> {
    return request<Result>('/api/user/update', {
        method: 'POST',
        data: model
    });
}
