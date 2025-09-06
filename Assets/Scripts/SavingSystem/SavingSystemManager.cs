using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Rooms;
using UnityEditor;
using UnityEngine;

namespace SavingSystem
{
    public class SavingSystemManager : MonoBehaviour
    {
        [SerializeField]
        private List<Rooms.RoomManager> rooms;
        [SerializeField]
        private TileInstancer tileInstancer;
        [SerializeField]
        private int maxFiles = 4;
        [SerializeField]
        private bool encrypt = true;
        
        private bool _newGame = true;
        public bool IsNewGame => _newGame;
        
        private const string BASE_FILENAME = "save";
        private const string SAVE_FILES_DIRECTORY = "Saves";
        private const string SAVE_FILE_EXTENSION = "mfs";

#if UNITY_EDITOR
        [MenuItem("Tools/InstantSave")]
        public static void InstantSave()
        {
            SavingSystemManager manager = FindFirstObjectByType<SavingSystemManager>();
            manager.Save();
        }
        [MenuItem("Tools/InstantLoad")]
        public static void InstantLoad()
        {
            SavingSystemManager manager = FindFirstObjectByType<SavingSystemManager>();
            manager.Load();
        }
#endif
        public void Save()
        {
            if (maxFiles <= 0)
            {
                Debug.LogError("Save manager misconfigured maxFiles <= 0");
                return;
            }
            
            SavedData savedData = new SavedData();
            
            savedData.playTutorial = GameManager.Instance.playTutorial;
            
            List<RoomContext> roomContexts = new List<RoomContext>();
            foreach (Rooms.RoomManager room in rooms)
            {
                roomContexts.Add(room.GetCurrentContext());
            }
            
            savedData.rooms = roomContexts;

            savedData.tiles = tileInstancer.GetTileContexts().ToList();

            SaveAsync(savedData).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Debug.LogError(t.Exception);
                }
                else
                {
                    Debug.Log("Game saved succesfully");
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        private async Task SaveAsync(SavedData savedData)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Include;
            
            string filePath = GetFilePathWithRotation();

            if (encrypt)
            {
                byte[] cipherBytes = await Task.Run(() =>
                {
                    byte[] plainBytes;
                    using (var ms = new MemoryStream())
                    using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, savedData);
                        writer.Flush();
                        sw.Flush();
                        plainBytes = ms.ToArray();
                    }

                    byte[] encryptedBytes = EncryptionUtils.Encrypt(plainBytes);
                    return EncryptionUtils.Compress(encryptedBytes);
                });
                
                await File.WriteAllBytesAsync(filePath, cipherBytes);
            }
            else
            {
                await Task.Run(() =>
                {
                    using (StreamWriter sw = new StreamWriter(filePath))
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, savedData);
                    }
                });
            } 
        }

        public void Load()
        {
            string filePath = GetMostRecentSaveFilePath();

            LoadAsync(filePath,LoadDataInGame).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Debug.LogError(t.Exception);
                }
                else
                {
                    Debug.Log("Game loaded succesfully");
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            
        }

        private void LoadDataInGame(SavedData savedData)
        {
            GameManager.Instance.playTutorial = savedData.playTutorial;

            foreach (RoomContext roomContext in savedData.rooms)
            {
                Rooms.RoomManager room = rooms.FirstOrDefault(x=> x.GetRoomDataSO().id == roomContext.roomId);
                if (room != null)
                {
                    room.SetContext(roomContext);
                }
            }
            
            tileInstancer.SetTilecontexts(savedData.tiles.ToArray());
            
            _newGame = false;
        }
        private async Task LoadAsync(string filePath,Action<SavedData> onLoaded = null)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Include;

            SavedData savedData = null;
                
            if (encrypt)
            {
                await Task.Run(() =>
                {
                    byte[] plainBytes = File.ReadAllBytes(filePath);
                    plainBytes = EncryptionUtils.Decompress(plainBytes);
                    plainBytes = EncryptionUtils.Decrypt(plainBytes);
                    
                    using (var ms = new MemoryStream(plainBytes))
                    using (var sr = new StreamReader(ms, Encoding.UTF8))
                    using (var reader = new JsonTextReader(sr))
                    {
                        savedData = serializer.Deserialize<SavedData>(reader);
                    }
                });
            }
            else
            {
                await Task.Run(() =>
                {
                    using (var sr = new StreamReader(filePath))
                    using (var reader = new JsonTextReader(sr))
                    {
                        savedData = serializer.Deserialize<SavedData>(reader);
                    }
                });
            }
            onLoaded?.Invoke(savedData);
        }

        private string GetMostRecentSaveFilePath()
        {
            string dir = Path.Combine(Application.persistentDataPath,SAVE_FILES_DIRECTORY);
            Directory.CreateDirectory(dir);

            // Get current files matching our pattern
            string[] files = Directory.GetFiles(dir, $"{BASE_FILENAME}_*.{SAVE_FILE_EXTENSION}");
            files = files.OrderBy(f => File.GetLastWriteTime(f)).ToArray();
            
            string oldestFile = files.Last();
            return oldestFile;
        }
        private string GetFilePathWithRotation()
        {
            string dir = Path.Combine(Application.persistentDataPath,SAVE_FILES_DIRECTORY);
            Directory.CreateDirectory(dir);

            // Get current files matching our pattern
            string[] files = Directory.GetFiles(dir, $"{BASE_FILENAME}_*.{SAVE_FILE_EXTENSION}");
            if (files.Length < maxFiles)
            {
                int nextIndex = files.Length;
                return Path.Combine(dir, $"{BASE_FILENAME}_{nextIndex}.{SAVE_FILE_EXTENSION}");
            }
            
            files = files.OrderBy(f => File.GetLastWriteTime(f)).ToArray();
            int firstValidFileIndex = 0;
            for (int i = 0; i < files.Length - maxFiles; i++)
            {
                File.Delete(files[i]);
                firstValidFileIndex = i+1;
            }
            
            string oldestFile = files[firstValidFileIndex];
            return oldestFile;
        }


        public void RegisterRoom(Rooms.RoomManager room)
        {
            rooms.Add(room);
        }

        public void UnregisterRoom(Rooms.RoomManager room)
        {
            rooms.Remove(room);
        }
    }
}
