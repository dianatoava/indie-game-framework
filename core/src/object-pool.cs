using System;
using System.Collections.Generic;

namespace IndieGameFramework.Core
{
    /// <summary>
    /// 对象池 - 复用对象，避免频繁创建销毁
    /// </summary>
    public class ObjectPool
    {
        #region 单例模式

        private static ObjectPool _instance;
        public static ObjectPool Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ObjectPool();
                }
                return _instance;
            }
        }

        #endregion

        #region 内部类

        /// <summary>
        /// 池子项
        /// </summary>
        private class PoolItem
        {
            public object Item { get; set; }
            public bool IsActive { get; set; }
        }

        /// <summary>
        /// 对象池定义
        /// </summary>
        private class ObjectPoolDefinition
        {
            public Type Type { get; set; }
            public Func<object> Factory { get; set; }
            public Action<object> ResetAction { get; set; }
            public List<PoolItem> Items { get; set; } = new();
            public int MaxSize { get; set; }
        }

        #endregion

        #region 字段

        private readonly Dictionary<string, ObjectPoolDefinition> _pools = new();

        #endregion

        #region 生命周期

        /// <summary>
        /// 初始化对象池
        /// </summary>
        public void Initialize()
        {
            _pools.Clear();
        }

        /// <summary>
        /// 清空所有池子
        /// </summary>
        public void Clear()
        {
            _pools.Clear();
        }

        #endregion

        #region 注册池子

        /// <summary>
        /// 注册对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="factory">工厂方法</param>
        /// <param name="resetAction">重置动作（可选）</param>
        /// <param name="initialSize">初始大小</param>
        /// <param name="maxSize">最大大小</param>
        public void Register<T>(
            Func<T> factory,
            Action<T> resetAction = null,
            int initialSize = 10,
            int maxSize = 100)
        {
            var key = typeof(T).Name;
            
            var poolDef = new ObjectPoolDefinition
            {
                Type = typeof(T),
                Factory = () => factory(),
                ResetAction = obj => resetAction?.Invoke((T)obj),
                MaxSize = maxSize
            };

            // 预创建初始对象
            for (int i = 0; i < initialSize; i++)
            {
                poolDef.Items.Add(new PoolItem
                {
                    Item = factory(),
                    IsActive = false
                });
            }

            _pools[key] = poolDef;
        }

        #endregion

        #region 获取/归还对象

        /// <summary>
        /// 从池子获取对象
        /// </summary>
        public T Get<T>()
        {
            var key = typeof(T).Name;
            
            if (!_pools.ContainsKey(key))
            {
                throw new InvalidOperationException($"Pool for {key} not registered");
            }

            var poolDef = _pools[key];

            // 查找非活跃对象
            foreach (var poolItem in poolDef.Items)
            {
                if (!poolItem.IsActive)
                {
                    poolItem.IsActive = true;
                    return (T)poolItem.Item;
                }
            }

            // 池子已满，创建新对象
            if (poolDef.Items.Count < poolDef.MaxSize)
            {
                var newItem = poolDef.Factory();
                poolDef.Items.Add(new PoolItem
                {
                    Item = newItem,
                    IsActive = true
                });
                return (T)newItem;
            }

            // 超出最大限制，返回第一个非活跃对象
            var firstInactive = poolDef.Items[0];
            firstInactive.IsActive = true;
            return (T)firstInactive.Item;
        }

        /// <summary>
        /// 归还对象到池子
        /// </summary>
        public void Return<T>(T obj)
        {
            var key = typeof(T).Name;
            
            if (!_pools.ContainsKey(key))
            {
                return;
            }

            var poolDef = _pools[key];

            // 查找并重置对象
            foreach (var poolItem in poolDef.Items)
            {
                if (poolItem.Item != null && poolItem.Item.Equals(obj))
                {
                    poolDef.ResetAction?.Invoke(obj);
                    poolItem.IsActive = false;
                    return;
                }
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取池子统计信息
        /// </summary>
        public PoolStats GetStats<T>()
        {
            var key = typeof(T).Name;
            
            if (!_pools.ContainsKey(key))
            {
                return null;
            }

            var poolDef = _pools[key];
            int active = 0;
            int inactive = 0;

            foreach (var item in poolDef.Items)
            {
                if (item.IsActive) active++;
                else inactive++;
            }

            return new PoolStats
            {
                Type = key,
                Total = poolDef.Items.Count,
                Active = active,
                Inactive = inactive,
                MaxSize = poolDef.MaxSize
            };
        }

        /// <summary>
        /// 清空指定池子
        /// </summary>
        public void ClearPool<T>()
        {
            var key = typeof(T).Name;
            if (_pools.ContainsKey(key))
            {
                _pools[key].Items.Clear();
            }
        }

        #endregion
    }

    /// <summary>
    /// 池子统计信息
    /// </summary>
    public class PoolStats
    {
        public string Type { get; set; }
        public int Total { get; set; }
        public int Active { get; set; }
        public int Inactive { get; set; }
        public int MaxSize { get; set; }

        public override string ToString()
        {
            return $"Pool<{Type}>: {Active}/{Total} active (max: {MaxSize})";
        }
    }
}
