using System;
using Xunit;
using IndieGameFramework.Core;

namespace IndieGameFramework.Core.Tests
{
    /// <summary>
    /// GameManager 测试
    /// </summary>
    public class GameManagerTests
    {
        [Fact]
        public void Instance_ShouldNotBeNull()
        {
            // Arrange & Act
            var instance = GameManager.Instance;
            
            // Assert
            Assert.NotNull(instance);
        }

        [Fact]
        public void Initialize_ShouldSetStateToMenu()
        {
            // Arrange
            var manager = GameManager.Instance;
            
            // Act
            manager.Initialize();
            
            // Assert
            Assert.Equal(GameState.Menu, manager.CurrentState);
        }

        [Fact]
        public void StartGame_ShouldSetStateToPlaying()
        {
            // Arrange
            var manager = GameManager.Instance;
            manager.Initialize();
            
            // Act
            manager.StartGame();
            
            // Assert
            Assert.Equal(GameState.Playing, manager.CurrentState);
            Assert.True(manager.IsPlaying);
        }

        [Fact]
        public void Pause_ShouldSetStateToPaused()
        {
            // Arrange
            var manager = GameManager.Instance;
            manager.Initialize();
            manager.StartGame();
            
            // Act
            manager.Pause();
            
            // Assert
            Assert.Equal(GameState.Paused, manager.CurrentState);
            Assert.True(manager.IsPaused);
        }

        [Fact]
        public void Resume_ShouldSetStateToPlaying()
        {
            // Arrange
            var manager = GameManager.Instance;
            manager.Initialize();
            manager.StartGame();
            manager.Pause();
            
            // Act
            manager.Resume();
            
            // Assert
            Assert.Equal(GameState.Playing, manager.CurrentState);
        }

        [Fact]
        public void GameOver_ShouldSetStateToGameOver()
        {
            // Arrange
            var manager = GameManager.Instance;
            manager.Initialize();
            manager.StartGame();
            
            // Act
            manager.GameOver();
            
            // Assert
            Assert.Equal(GameState.GameOver, manager.CurrentState);
        }

        [Fact]
        public void StateChangedEvent_ShouldFire()
        {
            // Arrange
            var manager = GameManager.Instance;
            manager.Initialize();
            bool eventFired = false;
            GameState oldState = GameState.None;
            GameState newState = GameState.None;
            
            manager.OnStateChanged += (from, to) =>
            {
                eventFired = true;
                oldState = from;
                newState = to;
            };
            
            // Act
            manager.StartGame();
            
            // Assert
            Assert.True(eventFired);
            Assert.Equal(GameState.Menu, oldState);
            Assert.Equal(GameState.Playing, newState);
        }
    }

    /// <summary>
    /// EventBus 测试
    /// </summary>
    public class EventBusTests
    {
        [Fact]
        public void Instance_ShouldNotBeNull()
        {
            var instance = EventBus.Instance;
            Assert.NotNull(instance);
        }

        [Fact]
        public void Publish_ShouldInvokeSubscriber()
        {
            // Arrange
            var bus = EventBus.Instance;
            bus.Initialize();
            
            bool eventReceived = false;
            int receivedValue = 0;
            
            bus.Subscribe<TestEvent>(e =>
            {
                eventReceived = true;
                receivedValue = e.Value;
            });
            
            // Act
            bus.Publish(new TestEvent { Value = 42 });
            
            // Assert
            Assert.True(eventReceived);
            Assert.Equal(42, receivedValue);
        }

        [Fact]
        public void Unsubscribe_ShouldStopReceivingEvents()
        {
            // Arrange
            var bus = EventBus.Instance;
            bus.Initialize();
            
            int callCount = 0;
            
            Action<TestEvent> handler = e => callCount++;
            bus.Subscribe(handler);
            
            // Act - 第一次发布
            bus.Publish(new TestEvent { Value = 1 });
            
            // 取消订阅
            bus.Unsubscribe(handler);
            
            // 第二次发布
            bus.Publish(new TestEvent { Value = 2 });
            
            // Assert
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void MultipleSubscribers_ShouldAllReceive()
        {
            // Arrange
            var bus = EventBus.Instance;
            bus.Initialize();
            
            int subscriber1Count = 0;
            int subscriber2Count = 0;
            
            bus.Subscribe<TestEvent>(e => subscriber1Count++);
            bus.Subscribe<TestEvent>(e => subscriber2Count++);
            
            // Act
            bus.Publish(new TestEvent { Value = 1 });
            bus.Publish(new TestEvent { Value = 2 });
            
            // Assert
            Assert.Equal(2, subscriber1Count);
            Assert.Equal(2, subscriber2Count);
        }

        [Fact]
        public void GetSubscriberCount_ShouldReturnCorrectCount()
        {
            // Arrange
            var bus = EventBus.Instance;
            bus.Initialize();
            
            bus.Subscribe<TestEvent>(e => { });
            bus.Subscribe<TestEvent>(e => { });
            
            // Act
            var count = bus.GetSubscriberCount<TestEvent>();
            
            // Assert
            Assert.Equal(2, count);
        }
    }

    /// <summary>
    /// 测试用事件类
    /// </summary>
    public class TestEvent
    {
        public int Value { get; set; }
    }

    /// <summary>
    /// ObjectPool 测试
    /// </summary>
    public class ObjectPoolTests
    {
        [Fact]
        public void Instance_ShouldNotBeNull()
        {
            var instance = ObjectPool.Instance;
            Assert.NotNull(instance);
        }

        [Fact]
        public void Get_ShouldReturnNewObject()
        {
            // Arrange
            var pool = ObjectPool.Instance;
            pool.Initialize();
            pool.Register(() => new TestObject(), null, 5, 10);
            
            // Act
            var obj = pool.Get<TestObject>();
            
            // Assert
            Assert.NotNull(obj);
        }

        [Fact]
        public void Return_ShouldMakeObjectAvailable()
        {
            // Arrange
            var pool = ObjectPool.Instance;
            pool.Initialize();
            pool.Register(() => new TestObject(), null, 5, 10);
            
            // Act
            var obj1 = pool.Get<TestObject>();
            pool.Return(obj1);
            var obj2 = pool.Get<TestObject>();
            
            // Assert - 应该复用了同一个对象
            Assert.Same(obj1, obj2);
        }

        [Fact]
        public void GetStats_ShouldReturnCorrectStats()
        {
            // Arrange
            var pool = ObjectPool.Instance;
            pool.Initialize();
            pool.Register(() => new TestObject(), null, 5, 10);
            
            // Act
            pool.Get<TestObject>();
            pool.Get<TestObject>();
            var stats = pool.GetStats<TestObject>();
            
            // Assert
            Assert.Equal(2, stats.Active);
            Assert.Equal(3, stats.Inactive);
            Assert.Equal(5, stats.Total);
        }
    }

    public class TestObject
    {
        public int Id { get; set; } = Guid.NewGuid().GetHashCode();
    }

    /// <summary>
    /// SaveSystem 测试
    /// </summary>
    public class SaveSystemTests
    {
        [Fact]
        public void Instance_ShouldNotBeNull()
        {
            var instance = SaveSystem.Instance;
            Assert.NotNull(instance);
        }

        [Fact]
        public void SaveAndLoad_ShouldWork()
        {
            // Arrange
            var saveSystem = SaveSystem.Instance;
            saveSystem.Initialize();
            var testData = new TestSaveData { Name = "Test", Score = 100 };
            var slotName = "test_slot_" + Guid.NewGuid();
            
            // Act
            var saved = saveSystem.Save(slotName, testData);
            var loaded = saveSystem.Load<TestSaveData>(slotName);
            
            // Assert
            Assert.True(saved);
            Assert.NotNull(loaded);
            Assert.Equal("Test", loaded.Name);
            Assert.Equal(100, loaded.Score);
            
            // Cleanup
            saveSystem.Delete(slotName);
        }

        [Fact]
        public void Exists_ShouldReturnTrueAfterSave()
        {
            // Arrange
            var saveSystem = SaveSystem.Instance;
            saveSystem.Initialize();
            var slotName = "test_slot_" + Guid.NewGuid();
            
            // Act
            saveSystem.Save(slotName, new TestSaveData());
            var exists = saveSystem.Exists(slotName);
            
            // Assert
            Assert.True(exists);
            
            // Cleanup
            saveSystem.Delete(slotName);
        }

        [Fact]
        public void Delete_ShouldRemoveSave()
        {
            // Arrange
            var saveSystem = SaveSystem.Instance;
            saveSystem.Initialize();
            var slotName = "test_slot_" + Guid.NewGuid();
            saveSystem.Save(slotName, new TestSaveData());
            
            // Act
            var deleted = saveSystem.Delete(slotName);
            var exists = saveSystem.Exists(slotName);
            
            // Assert
            Assert.True(deleted);
            Assert.False(exists);
        }

        [Fact]
        public void QuickSaveAndLoad_ShouldWork()
        {
            // Arrange
            var saveSystem = SaveSystem.Instance;
            saveSystem.Initialize();
            var testData = new TestSaveData { Name = "Quick", Score = 999 };
            
            // Act
            saveSystem.QuickSave(testData);
            var loaded = saveSystem.QuickLoad<TestSaveData>();
            
            // Assert
            Assert.NotNull(loaded);
            Assert.Equal("Quick", loaded.Name);
            Assert.Equal(999, loaded.Score);
            
            // Cleanup
            saveSystem.Delete("_quicksave");
        }
    }

    [Serializable]
    public class TestSaveData
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }

    /// <summary>
    /// SceneLoader 测试
    /// </summary>
    public class SceneLoaderTests
    {
        [Fact]
        public void Instance_ShouldNotBeNull()
        {
            var instance = SceneLoader.Instance;
            Assert.NotNull(instance);
        }

        [Fact]
        public void RegisterScene_ShouldAddScene()
        {
            // Arrange
            var loader = SceneLoader.Instance;
            
            // Act
            loader.RegisterScene("TestScene", "/path/to/scene");
            
            // Assert
            Assert.True(loader.IsSceneRegistered("TestScene"));
        }

        [Fact]
        public void GetRegisteredScenes_ShouldReturnAllScenes()
        {
            // Arrange
            var loader = SceneLoader.Instance;
            loader.RegisterScene("Scene1", "/path/1");
            loader.RegisterScene("Scene2", "/path/2");
            
            // Act
            var scenes = loader.GetRegisteredScenes();
            
            // Assert
            Assert.Contains("Scene1", scenes);
            Assert.Contains("Scene2", scenes);
        }

        [Fact]
        public void LoadScene_ShouldFireEvents()
        {
            // Arrange
            var loader = SceneLoader.Instance;
            loader.RegisterScene("TestScene", "/path/to/scene");
            
            bool loadStartFired = false;
            bool loadCompleteFired = false;
            
            loader.OnSceneLoadStart += _ => loadStartFired = true;
            loader.OnSceneLoadComplete += _ => loadCompleteFired = true;
            
            // Act
            loader.LoadScene("TestScene", async: false);
            
            // Assert
            Assert.True(loadStartFired);
            Assert.True(loadCompleteFired);
            Assert.Equal("TestScene", loader.CurrentScene);
        }
    }
}
