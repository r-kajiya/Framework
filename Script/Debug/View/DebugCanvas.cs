using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class DebugCanvas : MonoBehaviour
    {
        const int MAX_LOG = 100;

        [SerializeField]
        GameObject _root = null;

        [SerializeField]
        Transform _logGrid = null;

        [SerializeField]
        GameObject _logGridCellPrefab = null;

        void Awake()
        {
            _root.SetActive(false);
        }

        public void OnRootToggle()
        {
            _root.SetActive(!_root.activeSelf);
        }

        public void ClearLog()
        {
            for (int i = 0; i < _logGrid.childCount; i++)
            {
                GameObject.Destroy(_logGrid.GetChild(i).gameObject);
            }
        }

        public void Log(string log)
        {
            IfNeededClearLog();
            InternalLog(log, Color.white);
        }

        public void LogWarning(string log)
        {
            IfNeededClearLog();
            InternalLog(log, Color.yellow);
        }

        public void LogError(string log)
        {
            IfNeededClearLog();
            InternalLog(log, Color.red);
        }

        public void ClearPlayerData()
        {
            string[] filePaths = Directory.GetFiles(Application.persistentDataPath);
            foreach (string filePath in filePaths){
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        public void OpenAnimalBook()
        {
            
        }

        void InternalLog(string log, Color color)
        {
            if (_logGrid == null)
            {
                return;
            }
            
            var logGridCell = GameObject.Instantiate(_logGridCellPrefab, _logGrid);
            var text = logGridCell.GetComponentInChildren<Text>();
            var image = logGridCell.GetComponent<Image>();
            text.text = log;
            image.color = color;
        }

        void IfNeededClearLog()
        {
            if (_logGrid == null)
            {
                return;
            }
            
            if (_logGrid.childCount > MAX_LOG)
            {
                ClearLog();
            }
        }
    }
}
