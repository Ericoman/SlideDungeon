using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlobalControllers
{
    public class SceneController : MonoBehaviour
    {
        private static SceneController _instance;
        public static SceneController Instance => _instance;

        private bool _isNewGame;
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
        }

        private void SceneManagerOnsceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            if (scene.buildIndex == 1)
            {
                if (!_isNewGame)
                {
                    StartCoroutine(WaitAndLoadState());
                }
            }
        }

        public void StartGame(bool newGame = false)
        {
            _isNewGame = newGame;
            SceneManager.LoadScene(1);
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene(0);
        }

        IEnumerator WaitAndLoadState()
        {
            yield return new WaitForEndOfFrame();
            GameManager.Instance.LoadLastState();
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
