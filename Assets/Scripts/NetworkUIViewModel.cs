using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace NGOTutorial
{
    public class NetworkUIViewModel : MonoBehaviour
    {
        private VisualElement _root, _logUI;
        private Button _hostBtn, _serverBtn, _clientBtn, _toggleLogBtn;

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            if (_root is null)
            {
                Debug.Log("NetworkUIViewModel: can't find root visual element. No UI elements will be binded.");
                return;
            }
            
            _hostBtn = _root.Q<Button>("HostBtn");
            _serverBtn = _root.Q<Button>("ServerBtn");
            _clientBtn = _root.Q<Button>("ClientBtn");
            _toggleLogBtn = _root.Q<Button>("ToggleLogBtn");
            _logUI = _root.Q<VisualElement>("LogUI");
            
            _hostBtn.clicked += StartHost;
            _serverBtn.clicked += StartServer;
            _clientBtn.clicked += StartClient;
            _toggleLogBtn.clicked += ToggleLog;
        }
        
        private void OnDisable()
        {
            _hostBtn.clicked -= StartHost;
            _serverBtn.clicked -= StartServer;
            _clientBtn.clicked -= StartClient;
            _toggleLogBtn.clicked -= ToggleLog;
        }
        
        private void ToggleLog()
        {
            if (_logUI.style.display == DisplayStyle.None)
            {
                _logUI.style.display = DisplayStyle.Flex;
            }
            else
            {
                _logUI.style.display = DisplayStyle.None;
            }
        }
        
        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
        }
        
        public void StartServer()
        {
            NetworkManager.Singleton.StartServer();
        }
        
        public void StartClient()
        {
            NetworkManager.Singleton.StartClient();
        }
        
    }
}