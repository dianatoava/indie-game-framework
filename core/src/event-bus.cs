using System;
using System.Collections.Generic;

namespace IndieGameFramework.Core
{
    /// <summary>
    /// 全局事件总线 - 解耦系统间通信
    /// </summary>
    public class EventBus
    {
        #region 单例模式

        private static EventBus _instance;
        public static EventBus Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EventBus();
                }
                return _instance;
            }
        }

        #endregion

        #region 字段

        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        #endregion

        #region 生命周期

        /// <summary>
        /// 初始化事件总线
        /// </summary>
        public void Initialize()
        {
            _subscribers.Clear();
        }

        /// <summary>
        /// 清空所有订阅
        /// </summary>
        public void Clear()
        {
            _subscribers.Clear();
        }

        #endregion

        #region 订阅/取消订阅

        /// <summary>
        /// 订阅事件
        /// </summary>
        public void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
            {
                _subscribers[type] = new List<Delegate>();
            }
            _subscribers[type].Add(handler);
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        public void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (_subscribers.ContainsKey(type))
            {
                _subscribers[type].Remove(handler);
            }
        }

        #endregion

        #region 发布事件

        /// <summary>
        /// 发布事件
        /// </summary>
        public void Publish<T>(T eventData)
        {
            var type = typeof(T);
            if (_subscribers.ContainsKey(type))
            {
                var handlers = _subscribers[type];
                foreach (var handler in handlers)
                {
                    try
                    {
                        ((Action<T>)handler)(eventData);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[EventBus] Error in handler: {e.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// 发布简单事件（无数据）
        /// </summary>
        public void Publish<T>() where T : new()
        {
            Publish(new T());
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取订阅者数量
        /// </summary>
        public int GetSubscriberCount<T>()
        {
            var type = typeof(T);
            return _subscribers.ContainsKey(type) ? _subscribers[type].Count : 0;
        }

        #endregion
    }

    #region 常用事件类型

    /// <summary>
    /// 游戏启动事件
    /// </summary>
    public class GameStartedEvent { }

    /// <summary>
    /// 游戏暂停事件
    /// </summary>
    public class GamePausedEvent { }

    /// <summary>
    /// 游戏恢复事件
    /// </summary>
    public class GameResumedEvent { }

    /// <summary>
    /// 游戏结束事件
    /// </summary>
    public class GameOverEvent
    {
        public int Score { get; set; }
        public string Reason { get; set; }
    }

    /// <summary>
    /// 场景加载事件
    /// </summary>
    public class SceneLoadedEvent
    {
        public string SceneName { get; set; }
    }

    #endregion
}
