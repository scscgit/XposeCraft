using UnityEngine;

/// <summary>
/// Toggle support for Menu window.
/// </summary>
public class MenuController : MonoBehaviour
{
	public GameObject menu;
	public bool MenuAtStart;

	UIController ui;
	Animator animator;

	void Start()
	{
		this.ui = menu.GetComponent<UIController>();
		this.animator = menu.GetComponent<Animator>();

		if (!MenuAtStart)
		{
			// Pausing the animator, disabling the Menu
			animator.enabled = false;
			// Last started animation was Show, we explicitly set Hide
			ui.Hide();
		}
	}

	void Update()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			animator.enabled = true;
			var show = ui.isShow;
			Debug.logger.Log("Escape button pressed, " + (show ? "closing" : "opening") + " menu");
			if (show)
			{
				ui.Hide();
			}
			else if (!show)
			{
				ui.Show();
			}
		}
	}
}
