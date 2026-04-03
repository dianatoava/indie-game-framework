using System;
using System.IO;
using System.Text.Json;

namespace IndieGameFramework.Core
{
    /// <summary>
    /// 存档系统 - 游戏数据持久化
    /// </summary>
    public class SaveSystem
    {
        #region 单例模式

        private static SaveSystem _instance;
        public static SaveSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SaveSystem();
                }
                return _instance;
            }
        }

        #endregion

        #region 常量

        private const string SAVE_EXTENSION = ".json";
        private const string BACKUP_EXTENSION = ".bak";

        #endregion

        #region 属性

        /// <summary>
        /// 存档目录
        /// </summary>
        public string SaveDirectory { get; set; }

        /// <summary>
        /// 是否启用备份
        /// </summary>
        public bool EnableBackup { get; set; } = true;

        /// <summary>
        /// 是否启用加密（简单 XOR）
        /// </summary>
        public bool EnableEncryption { get; set; } = false;

        #endregion

        #region 生命周期

        /// <summary>
        /// 初始化存档系统
        /// </summary>
        public void Initialize()
        {
            // 默认存档目录
            SaveDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "IndieGameFramework",
                "Saves"
            );

            Directory.CreateDirectory(SaveDirectory);
        }

        #endregion

        #region 保存/加载

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="slotName">存档槽名称</param>
        /// <param name="data">数据对象</param>
        /// <returns>是否成功</returns>
        public bool Save<T>(string slotName, T data)
        {
            try
            {
                var filePath = GetSavePath(slotName);

                // 备份旧存档
                if (EnableBackup && File.Exists(filePath))
                {
                    File.Copy(filePath, filePath + BACKUP_EXTENSION, true);
                }

                // 序列化
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // 加密（可选）
                if (EnableEncryption)
                {
                    json = SimpleEncrypt(json);
                }

                // 写入文件
                File.WriteAllText(filePath, json);

                Console.WriteLine($"[SaveSystem] Saved: {slotName}");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SaveSystem] Save failed: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="slotName">存档槽名称</param>
        /// <returns>数据对象，失败返回 default</returns>
        public T Load<T>(string slotName)
        {
            try
            {
                var filePath = GetSavePath(slotName);

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"[SaveSystem] Save not found: {slotName}");
                    return default;
                }

                // 读取文件
                var json = File.ReadAllText(filePath);

                // 解密（可选）
                if (EnableEncryption)
                {
                    json = SimpleDecrypt(json);
                }

                // 反序列化
                var data = JsonSerializer.Deserialize<T>(json);

                Console.WriteLine($"[SaveSystem] Loaded: {slotName}");
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SaveSystem] Load failed: {e.Message}");
                return default;
            }
        }

        /// <summary>
        /// 删除存档
        /// </summary>
        public bool Delete(string slotName)
        {
            try
            {
                var filePath = GetSavePath(slotName);
                
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                var backupPath = filePath + BACKUP_EXTENSION;
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }

                Console.WriteLine($"[SaveSystem] Deleted: {slotName}");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SaveSystem] Delete failed: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 检查存档是否存在
        /// </summary>
        public bool Exists(string slotName)
        {
            return File.Exists(GetSavePath(slotName));
        }

        #endregion

        #region 快速存档

        private const string QUICK_SAVE_SLOT = "_quicksave";

        /// <summary>
        /// 快速保存
        /// </summary>
        public bool QuickSave<T>(T data)
        {
            return Save(QUICK_SAVE_SLOT, data);
        }

        /// <summary>
        /// 快速加载
        /// </summary>
        public T QuickLoad<T>()
        {
            return Load<T>(QUICK_SAVE_SLOT);
        }

        #endregion

        #region 私有方法

        private string GetSavePath(string slotName)
        {
            // 清理非法字符
            var cleanName = slotName.Replace("/", "_").Replace("\\", "_");
            return Path.Combine(SaveDirectory, cleanName + SAVE_EXTENSION);
        }

        private string SimpleEncrypt(string text)
        {
            // 简单 XOR 加密（仅用于防止直接查看）
            var chars = text.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)(chars[i] ^ 0x42);
            }
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(chars));
        }

        private string SimpleDecrypt(string encrypted)
        {
            var bytes = Convert.FromBase64String(encrypted);
            var chars = System.Text.Encoding.UTF8.GetChars(bytes);
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)(chars[i] ^ 0x42);
            }
            return new string(chars);
        }

        #endregion
    }

    #region 存档数据结构

    /// <summary>
    /// 游戏进度数据
    /// </summary>
    [Serializable]
    public class GameProgress
    {
        public string SaveName { get; set; }
        public DateTime SaveTime { get; set; }
        public int PlayTimeSeconds { get; set; }
        public string CurrentScene { get; set; }
        public PlayerData Player { get; set; }
        public object WorldState { get; set; }
    }

    /// <summary>
    /// 玩家数据
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public string PlayerName { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; set; }
        public int Score { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
    }

    #endregion
}
