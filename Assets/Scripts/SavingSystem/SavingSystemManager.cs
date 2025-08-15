using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Rooms;
using UnityEditor;
using UnityEngine;

namespace SavingSystem
{
    public class SavingSystemManager : MonoBehaviour
    {
        [SerializeField]
        private Rooms.RoomManager[] rooms;
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
            
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Include;
            
            string filePath = GetFilePathWithRotation();

            if (encrypt)
            {
                byte[] plainBytes;
                using (var ms = new MemoryStream())
                using (StreamWriter sw = new StreamWriter(ms,Encoding.UTF8))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, savedData);
                    writer.Flush();
                    sw.Flush();
                    plainBytes = ms.ToArray();
                }

                byte[] cipherBytes = EncryptionUtils.Encrypt(plainBytes);
                cipherBytes = EncryptionUtils.Compress(cipherBytes);
                
                File.WriteAllBytes(filePath, cipherBytes);
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, savedData);
                }
            }

        }

        public void Load()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Include;

            SavedData savedData = null;
            string filePath = GetMostRecentSaveFilePath();
            

            if (encrypt)
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
            }
            else
            {
                using (var sr = new StreamReader(filePath))
                using (var reader = new JsonTextReader(sr))
                {
                    savedData = serializer.Deserialize<SavedData>(reader);
                }
            }
            
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
    }
}
