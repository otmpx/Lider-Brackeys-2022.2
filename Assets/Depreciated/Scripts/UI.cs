using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI Instance;
    public Slider sanityBar;
    public float sanityValue;
    const float updateDur = 1f;
    const float initialUpdateDur = 2f;
    float timer;

    public GameObject progressBar;
    public GridLayoutGroup skullGrid;
    Image[] skulls;
    public GridLayoutGroup sigilGrid;

    Image[] sigils;
    public Sprite normalSkull;
    public Sprite babySkull;
    public Sprite bossSkull;
    Text enemyCounter;

    public Text fText;
    public Text graveText;
    public Text speedRunTimer;

    public bool toggleCinemaMode = false;

    private void Awake()
    {
        Instance = this;
        skulls = skullGrid.GetComponentsInChildren<Image>();
        sigils = sigilGrid.GetComponentsInChildren<Image>();
        enemyCounter = progressBar.GetComponentInChildren<Text>();
        foreach (var sigilIm in sigils)
            sigilIm.gameObject.SetActive(false);
        //sanityBar.value = LevelDirector.Instance.playerSanity;
        sanityValue = sanityBar.value;
        fText.gameObject.SetActive(false);
        graveText.gameObject.SetActive(false);
    }

    private void Update()
    {
#if !UNITY_EDITOR
      if (LevelDirector.Instance.currentRoomIndex == 0)
            InitialUILerp();
      else
#endif
        UILerp();

        //if (LevelDirector.Instance.isSpeedRunTimerActivated)
        //{
        //    var timer = LevelDirector.Instance.globalTimer;
        //    string minutes = Mathf.Floor(timer / 60).ToString("00");
        //    string seconds = Mathf.Floor(timer % 60).ToString("00");
        //    speedRunTimer.text = minutes + ":" + seconds;
        //}
        //else
        //{
        //    speedRunTimer.text = "";
        //}

        //for (int i = 0; i < LevelDirector.Instance.roomsExplored.Length; i++)
        //{
        //    if (i == 0)
        //    {
        //        //Baby rooom skull
        //        skulls[i].sprite = babySkull;
        //    }
        //    else if (i == LevelDirector.Instance.totalRooms - 1)
        //    {
        //        skulls[i].sprite = bossSkull;
        //    }
        //    else
        //    {
        //        skulls[i].sprite = normalSkull;
        //    }

        //    if (i <= LevelDirector.Instance.currentRoomIndex)
        //    {
        //        skulls[i].color = Color.white;
        //    }
        //    else
        //    {
        //        skulls[i].color = Color.grey;
        //    }
        //}

        if (toggleCinemaMode)
        {
            sanityBar.gameObject.SetActive(false);
            progressBar.SetActive(false);
            sigilGrid.gameObject.SetActive(false);
        }
        else
        {
            sanityBar.gameObject.SetActive(true);
            progressBar.SetActive(true);
            sigilGrid.gameObject.SetActive(true);
        }
    }

    //void InitialUILerp()
    //{
    //    timer += Time.deltaTime;
    //    sanityBar.value = Mathf.Clamp(Mathf.Lerp(0, LevelDirector.Instance.stats.maxSanity, timer / initialUpdateDur),
    //        0, LevelDirector.Instance.stats.maxSanity);
    //}

    void UILerp()
    {
        if (sanityBar.value != sanityValue)
        {
            timer += Time.deltaTime;
            sanityBar.value = Mathf.Lerp(sanityBar.value, sanityValue, timer / updateDur);
        }
        else
            timer = 0;
    }

    //public void UpdateSanityBarSize()
    //{
    //    // Used to update sanity bar UI
    //    float barSize = LevelDirector.Instance.stats.maxSanity * 2;
    //    RectTransform rt = sanityBar.GetComponent(typeof(RectTransform)) as RectTransform;
    //    rt.sizeDelta = new Vector2(barSize, 30);
    //    sanityBar.maxValue = LevelDirector.Instance.stats.maxSanity;
    //    sanityBar.value = LevelDirector.Instance.playerSanity;
    //    sanityValue = sanityBar.value;
    //}
}