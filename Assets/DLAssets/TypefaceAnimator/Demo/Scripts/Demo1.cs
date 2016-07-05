using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Demo1 : MonoBehaviour
{
	public Text header;
	public TypefaceAnimator[] anims;
	int m_currentNum = 0;

	public int currentNum
	{
		get { return m_currentNum; }
		set {
			m_currentNum = value;
			if (m_currentNum >= anims.Length) m_currentNum = 0;
			else if (m_currentNum < 0) m_currentNum = anims.Length - 1;
		}
	}

	string headerText
	{
		get { return (currentNum + 1) + " / " + anims.Length; }
	}

	void Start ()
	{
		SwitchAnimation(m_currentNum);
		header.text = headerText;
	}

	void Update ()
	{
		if (Input.GetKeyDown("right")) OnChangeAnimation(1);
		if (Input.GetKeyDown("left")) OnChangeAnimation(-1);
	}

	void SwitchAnimation(int num)
	{
		for (int i = 0; i < anims.Length; i++)
		{
			anims[i].gameObject.SetActive(false);
		}
		
		anims[num].gameObject.SetActive(true);
	}

	public void OnChangeAnimation(int num)
	{
		currentNum += num;
		SwitchAnimation(currentNum);
		header.text = headerText;
	}
}
