using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.IO;
public class IntroWebgl : MonoBehaviour
{
	VideoPlayer video;
	private void Awake()
	{
		video = GetComponent<VideoPlayer>();
		video.url = Path.Combine(Application.streamingAssetsPath, "Intro Stream.mov");
	}
	private IEnumerator Start()
	{
		yield return StartCoroutine(LoadIntro());
		yield return StartCoroutine(Wait(4.35f));
		SceneManager.LoadScene(sceneBuildIndex: 1);
	}
	private IEnumerator Wait(float time)
	{
		yield return new WaitForSeconds(time);
	}
	private IEnumerator LoadIntro()
    {
		while (video.isPrepared)
			yield return null;
		video.Play();
    }
    private void Update()
    {
   //     if (Input.anyKeyDown)
			//SceneManager.LoadScene(sceneBuildIndex: 1);
	}
}
