using System;
using System.Collections.Generic;

namespace IndieGameFramework.Core
{
    /// <summary>
    /// 场景加载器 - 管理场景切换
    /// </summary>
    public class SceneLoader
    {
        #region 单例模式

        private static SceneLoader _instance;
        public static SceneLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SceneLoader();
                }
                return _instance;
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 场景加载开始事件
        /// </summary>
        public event Action<string> OnSceneLoadStart;

        /// <summary>
        /// 场景加载完成事件
        /// </summary>
        public event Action<string> OnSceneLoadComplete;

        #endregion

        #region 字段

        private readonly Dictionary<string, SceneDefinition> _scenes = new();
        private string _currentScene;
        private string _previousScene;

        #endregion

        #region 属性

        /// <summary>
        /// 当前场景名称
        /// </summary>
        public string CurrentScene => _currentScene;

        /// <summary>
        /// 上一个场景名称
        /// </summary>
        public string PreviousScene => _previousScene;

        /// <summary>
        /// 是否正在加载场景
        /// </summary>
        public bool IsLoading { get; private set; }

        #endregion

        #region 内部类

        private class SceneDefinition
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public Action OnEnter { get; set; }
            public Action OnExit { get; set; }
            public bool IsAdditive { get; set; }
        }

        #endregion

        #region 注册场景

        /// <summary>
        /// 注册场景
        /// </summary>
        /// <param name="name">场景名称</param>
        /// <param name="path">场景路径</param>
        /// <param name="onEnter">进入场景回调</param>
        /// <param name="onExit">离开场景回调</param>
        public void RegisterScene(
            string name,
            string path,
            Action onEnter = null,
            Action onExit = null)
        {
            _scenes[name] = new SceneDefinition
            {
                Name = name,
                Path = path,
                OnEnter = onEnter,
                OnExit = onExit,
                IsAdditive = false
            };
        }

        /// <summary>
        /// 注册叠加场景（可同时加载多个）
        /// </summary>
        public void RegisterAdditiveScene(string name, string path)
        {
            _scenes[name] = new SceneDefinition
            {
                Name = name,
                Path = path,
                IsAdditive = true
            };
        }

        #endregion

        #region 加载场景

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="async">是否异步加载</param>
        public void LoadScene(string sceneName, bool async = true)
        {
            if (!_scenes.ContainsKey(sceneName))
            {
                Console.WriteLine($"[SceneLoader] Scene not found: {sceneName}");
                return;
            }

            if (IsLoading)
            {
                Console.WriteLine("[SceneLoader] Already loading a scene");
                return;
            }

            IsLoading = true;
            _previousScene = _currentScene;

            // 退出当前场景
            if (_currentScene != null && _scenes.ContainsKey(_currentScene))
            {
                _scenes[_currentScene].OnExit?.Invoke();
            }

            OnSceneLoadStart?.Invoke(sceneName);

            if (async)
            {
                LoadSceneAsync(sceneName);
            }
            else
            {
                LoadSceneSync(sceneName);
            }
        }

        /// <summary>
        /// 重新加载当前场景
        /// </summary>
        public void ReloadScene()
        {
            if (!string.IsNullOrEmpty(_currentScene))
            {
                LoadScene(_currentScene);
            }
        }

        /// <summary>
        /// 返回上一个场景
        /// </summary>
        public void GoBack()
        {
            if (!string.IsNullOrEmpty(_previousScene))
            {
                LoadScene(_previousScene);
            }
        }

        #endregion

        #region 私有方法

        private void LoadSceneSync(string sceneName)
        {
            var scene = _scenes[sceneName];

            // 模拟加载（实际 Unity 项目中这里调用 SceneManager.LoadScene）
            Console.WriteLine($"[SceneLoader] Loading scene: {scene.Name} ({scene.Path})");
            
            // 进入新场景
            _currentScene = sceneName;
            scene.OnEnter?.Invoke();
            
            IsLoading = false;
            OnSceneLoadComplete?.Invoke(sceneName);
        }

        private async void LoadSceneAsync(string sceneName)
        {
            var scene = _scenes[sceneName];

            Console.WriteLine($"[SceneLoader] Async loading scene: {scene.Name}");
            
            // 模拟异步加载
            await System.Threading.Tasks.Task.Delay(100);
            
            _currentScene = sceneName;
            scene.OnEnter?.Invoke();
            
            IsLoading = false;
            OnSceneLoadComplete?.Invoke(sceneName);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取已注册场景列表
        /// </summary>
        public List<string> GetRegisteredScenes()
        {
            return new List<string>(_scenes.Keys);
        }

        /// <summary>
        /// 检查场景是否已注册
        /// </summary>
        public bool IsSceneRegistered(string sceneName)
        {
            return _scenes.ContainsKey(sceneName);
        }

        #endregion
    }
}
