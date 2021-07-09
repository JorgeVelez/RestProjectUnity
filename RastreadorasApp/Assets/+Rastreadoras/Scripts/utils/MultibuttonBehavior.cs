using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultibuttonBehavior : MonoBehaviour {

	bool fromHideIfClickOutside=false;
	void Start () {

		Button[] buttons = GetComponentsInChildren<Button>();
		foreach (Button bt in buttons)
		{
			bt.onClick.AddListener(dissapear);
		}
		
	}

	void Update()
	{
		HideIfClickedOutside(gameObject);
	}

	public void toggle()
	{
		if (!gameObject.activeSelf)
			gameObject.SetActive(true);

		fromHideIfClickOutside = false;
	}

	public void hide()
	{
		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void dissapear () {
		gameObject.SetActive(false);
	}

	private void HideIfClickedOutside(GameObject panel)
	{
		if (Input.GetMouseButtonUp(0) && panel.activeSelf &&
			!RectTransformUtility.RectangleContainsScreenPoint(
				panel.GetComponent<RectTransform>(),
				Input.mousePosition,
				Camera.main))
		{
			panel.SetActive(false);
			fromHideIfClickOutside = true;
		}
	}
}
