using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroWindows : MonoBehaviour
{
	private void Start()
	{
		Invoke("Wait", 4.15f);
	}
	private void Wait()
	{
		SceneManager.LoadScene(sceneBuildIndex: 1);
	}
}
