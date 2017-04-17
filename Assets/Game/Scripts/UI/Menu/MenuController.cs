using UnityEngine;
using UnityEngine.Serialization;

namespace XposeCraft.UI.Menu
{
    /// <summary>
    /// Toggle and action support for Menu window.
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        // To prevent disable when using the toggle, this script must not be attached to that Menu or any of its children
        [FormerlySerializedAs("menu")] public GameObject Menu;

        public bool MenuAtStart;
        public bool ToggleOnEscape = true;

        private UIController _ui;
        private Animator _animator;

        public void QuitGame()
        {
            Application.Quit();
            Debug.Log("The application was quit");
        }

        public void ToggleDisplay()
        {
            _animator.enabled = true;
            var show = _ui.isShow;
            if (show)
            {
                _ui.Hide();
            }
            else
            {
                _ui.Show();
            }
        }

        private void Start()
        {
            _ui = Menu.GetComponent<UIController>();
            _animator = Menu.GetComponent<Animator>();

            if (!MenuAtStart)
            {
                // Pausing the animator, disabling the Menu
                _animator.enabled = false;
                // Last started animation was Show, we explicitly set Hide
                _ui.Hide();
            }
        }

        private void Update()
        {
            if (ToggleOnEscape && Input.GetButtonDown("Cancel"))
            {
                ToggleDisplay();
            }
        }
    }
}
