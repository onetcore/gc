# gc
自动化代码生成器。

1. 将gc.exe目录设置为环境目录
   ```ssh
    set path=%path%;gc目录;
   ```
2. 调用gc命令进行生成，参数如下
    1. --f|--file：指定配置文件，配置文件为`gc.json`；
    2. --o|--out: 输出文件夹路径；

    注：如果没有指定则在当前目录下的`gc.json`文件。

# gc.json定义

```json
{
  "namespace": "Yd",//命名空间
  "using": [ "System" ]//默认每个类引用命名空间
}
```

# 类型定义

```json
{
  "name": "User",
  "desc": "用户实体",
  "using": [ ],
  "namespace": "Yd",
  "base": [],
  "props": [
    {
      "name": "id",
      "desc": "唯一Id",
      "type": "int",
      "identity": true
    },
    {
      "name": "userName",
      "desc": "名称",
      "size": 64,
      "unique": true
    },
    {
      "name": "realName",
      "desc": "真实姓名",
      "size": 64
    },
    {
      "name": "password",
      "desc": "真实姓名",
      "size": 128
    }
  ]
}
```
每一个类单独定义一个json文件，生成的文件将包含服务端的netcore代码，以及客户端的typescript代码。

## 类型参数说明

1. name：指定类型名称，如果不指定将使用文件名称；
2. desc: 描述信息，一般用于注释使用；
3. using: 引用命名空间，类的命名空间将会附加项目配置`gc.json`中的`using`配置；
4. namespace: 指定命名空间，如果不指定将会使用`gc.json`中的命名空间`namespace`和文件夹名称组合来定义类型的命名空间；
5. base: 指定类继承的类型或者接口，接口必须使用`I`开头；

## 属性参数说明

1. name: 指定属性名称；
2. desc: 描述信息，一般用于注释使用；
3. type: netcore中的类型，如果没指定默认为`string`类型；
4. size: 长度大小，主要用于数据库迁移中使用，`string`类型如果没有配置将使用`nvarchar(max)`；
5. map: 默认为`true`，如果设置为`false`将不迁移到数据库中；
6. update: 默认为`true`，如果设置为`false`更新实体的时候将忽略当前属性；
7. scope: 默认为`public`，其他配置将不迁移到数据库中；
8. identity: 自增长属性；
9. key: 主键；
10. unique: 唯一键；
11. fkey: 外键：
    1. main：主键类型名称，代码生成只会生成名称，不会自动引入命名空间；
    2. key: 主键名称；
    3. update: 当主键更新时触发的操作，默认为不操作；
    4. delete: 当主键被删除时候出发的操作，默认为级联删除；

## 外键出发选项说明

```csharp
    /// <summary>
    /// 外键相关操作。
    /// </summary>
    public enum ReferentialAction
    {
        /// <summary>
        /// 无操作。
        /// </summary>
        NoAction,
        /// <summary>
        /// 约束。
        /// </summary>
        Restrict,
        /// <summary>
        /// 级联。
        /// </summary>
        Cascade,
        /// <summary>
        /// 设为空。
        /// </summary>
        SetNull,
        /// <summary>
        /// 设为默认值。
        /// </summary>
        SetDefault
    }
```