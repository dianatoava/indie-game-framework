using System;
using System.Collections.Generic;

namespace IndieGameFramework.Core
{
    /// <summary>
    /// 游戏状态枚举
    /// </summary>
    public enum GameState
    {
        None,
        Initializing,
        Menu,
        Playing,
        Paused,
        GameOver,
        Victory
    }

    /// <summary>
    /// 游戏管理器 - 单例，管理游戏状态和流程
    /// </summary>
    public class GameManager
    {
        #region 单例模式

        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameManager();
                }
                return _instance;
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 游戏状态变化事件
        /// </summary>
        public event Action<GameState, GameState> OnStateChanged;

        #endregion

        #region 属性

        /// <summary>
        /// 当前游戏状态
        /// </summary>
        public GameState CurrentState { get; private set; } = GameState.None;

        /// <summary>
        /// 游戏是否正在运行
        /// </summary>
        public bool IsPlaying => CurrentState == GameState.Playing;

        /// <summary>
        /// 游戏是否暂停
        /// </summary>
        public bool IsPaused => CurrentState == GameState.Paused;

        #endregion

        #region 生命周期

        /// <summary>
        /// 初始化游戏
        /// </summary>
        public void Initialize()
        {
            SetState(GameState.Initializing);
            
            // 初始化核心系统
            EventBus.Instance.Initialize();
            ObjectPool.Instance.Initialize();
            SaveSystem.Instance.Initialize();
            
            SetState(GameState.Menu);
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            if (CurrentState == GameState.None || CurrentState == GameState.Menu)
            {
                SetState(GameState.Playing);
            }
        }

        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void Pause()
        {
            if (CurrentState == GameState.Playing)
            {
                SetState(GameState.Paused);
            }
        }

        /// <summary>
        /// 恢复游戏
        /// </summary>
        public void Resume()
        {
            if (CurrentState == GameState.Paused)
            {
                SetState(GameState.Playing);
            }
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        public void GameOver()
        {
            SetState(GameState.GameOver);
        }

        /// <summary>
        /// 游戏胜利
        /// </summary>
        public void Victory()
        {
            SetState(GameState.Victory);
        }

        /// <summary>
        /// 重启游戏
        /// </summary>
        public void Restart()
        {
            SetState(GameState.None);
            Initialize();
        }

        #endregion

        #region 私有方法

        private void SetState(GameState newState)
        {
            var oldState = CurrentState;
            if (oldState != newState)
            {
                CurrentState = newState;
                OnStateChanged?.Invoke(oldState, newState);
            }
        }

        #endregion
    }
}
