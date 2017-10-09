using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{

	// Use this for initialization
	public Sprite FullHeart;
	public Sprite HalfHeart;
	public Sprite EmpyHeart;
	private List<Image> _items = new List<Image>();
	public int HeartAmount;
	public GameObject Template;
	public GameObject Bar;
	public void SetHP(float hp)
	{
		hp = Mathf.Max(hp, 0);
		float divider = 100 / HeartAmount;
		int fullHearts = (int)(hp / divider);
		int heartIndex = 0;
		for (int i = 0; i < fullHearts; i++)
		{
			
			_items[i].sprite = FullHeart;
			heartIndex++;
		}

		float halfHp = hp - (fullHearts * divider) - (divider/2);
		if (halfHp > 0)
		{
			_items[heartIndex].sprite = HalfHeart;
			heartIndex++;
		}
		for (int i = heartIndex; i < HeartAmount; i++)
		{
			_items[i].sprite = EmpyHeart;
		}
	}

	void Start()
	{
		for (int i = 0; i < HeartAmount; i++)
		{
			var go = Instantiate(Template);
			go.transform.parent = Bar.transform;
			go.GetComponent<RectTransform>().localScale = Vector3.one;
			go.GetComponent<Image>().sprite = FullHeart;
			_items.Add(go.GetComponent<Image>());
		}
		Template.SetActive(false);
	}

	// Update is called once per frame

	void Update()
	{

	}

}
