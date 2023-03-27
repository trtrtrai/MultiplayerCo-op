using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Both.UIHolder
{
    public class OptionHolder : MonoBehaviour
    {
        public Button Music;
        public Image MusicBtnImg;
        public Sprite MusicOn;
        public Sprite MusicOff;

        public Button VieLang;
        public Button EngLang;
        public Button JapLang;

        public void SyncUI()
        {
            SyncMusic();

            SyncLanguage();
        }

        private void Awake()
        {
            Music.onClick.AddListener(Music_OnClick);
            VieLang.onClick.AddListener(() => GameOption.CurrentLanguage = "Vietnamese");
            EngLang.onClick.AddListener(() => GameOption.CurrentLanguage = "English");
            JapLang.onClick.AddListener(() => GameOption.CurrentLanguage = "Japanese");
        }

        private void Start()
        {
            SyncUI();
        }

        private void SyncMusic()
        {
            if (GameOption.VolumeTrigger)
            {
                AudioListener.volume = GameOption.Volume;
                MusicBtnImg.sprite = MusicOn;
            }
            else
            {
                MusicBtnImg.sprite = MusicOff;
            }
            AudioListener.pause = !GameOption.VolumeTrigger;
        }

        private void SyncLanguage()
        {
            switch (GameOption.CurrentLanguage)
            {
                case "English":
                    {
                        VieLang.interactable = true;
                        JapLang.interactable = true;
                        EngLang.interactable = false;
                        break;
                    }
                case "Vietnamese":
                    {
                        VieLang.interactable = false;
                        JapLang.interactable = true;
                        EngLang.interactable = true;
                        break;
                    }
                case "Japanese":
                    {
                        VieLang.interactable = true;
                        JapLang.interactable = false;
                        EngLang.interactable = true;
                        break;
                    }
            }
        }

        private void Music_OnClick()
        {
            GameOption.VolumeTrigger = !GameOption.VolumeTrigger;
            SyncMusic();
        }

        private void OnDestroy()
        {
            Music.onClick.RemoveAllListeners();
            VieLang.onClick.RemoveAllListeners();
            EngLang.onClick.RemoveAllListeners();
            JapLang.onClick.RemoveAllListeners();
        }
    }
}