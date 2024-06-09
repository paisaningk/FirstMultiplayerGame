using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using utilities;

namespace Lobby
{
    public class LoadingCanvasController : MonoBehaviour
    {
        public Animator animator;
        public Button cancelBtn;
        private NetworkRunnerController networkRunnerController;

        private void Start()
        {
            networkRunnerController = GlobalManager.Instance.NetworkRunnerController;
            networkRunnerController.OnStartedRunnerConnection += OnStartedRunnerConnection;
            networkRunnerController.OnPlayerJoinedSuccessfully += OnPlayerJoinedSuccessfully;

            gameObject.SetActive(false);
            
            cancelBtn.onClick.AddListener(networkRunnerController.ShutDownRunner);
        }

        [Button]
        private void Test()
        {
            Debug.Log(GlobalManager.Instance);
        }

        private void OnDestroy()
        {
            networkRunnerController.OnStartedRunnerConnection -= OnStartedRunnerConnection;
            networkRunnerController.OnPlayerJoinedSuccessfully -= OnPlayerJoinedSuccessfully;
        }

        private void OnPlayerJoinedSuccessfully()
        {
            animator.Play("Out");
            StartCoroutine(gameObject.CloseGameObjectWhenAnimEnd(animator, false));
        }
        
        private void OnStartedRunnerConnection()
        {
            gameObject.SetActive(true);
            animator.Play("In");
            StartCoroutine(gameObject.CloseGameObjectWhenAnimEnd(animator));
        }
    }
}