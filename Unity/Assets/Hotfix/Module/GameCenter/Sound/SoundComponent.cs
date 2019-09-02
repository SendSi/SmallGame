using ETModel;
using UnityEngine;
using System;

namespace ETHotfix
{
    [ObjectSystem]
    public class SoundComponentAwakeSystem : AwakeSystem<SoundComponent>
    {
        public override void Awake(SoundComponent self)
        {
            self.Awake();
        }
    }

    /// <summary>
    /// 游戏音效管理组件
    /// 示例 SoundComponent.Instance.PlaySFX("sfx_bet");
    /// 示例 SoundComponent.Instance.PlayBGM("bgm_lobby");
    /// </summary>
    public class SoundComponent : Component
    {
        /// <summary>
        /// 游戏消息音效
        /// </summary>
        public const string SFX_POKER_EVENT = "sfx_poker_event";
        /// <summary>
        /// 游戏滚轮音效
        /// </summary>
        public const string SFX_GAME_WHEEL_TICK = "sfx_game_wheel_tick";
        /// <summary>
        /// 更换牌桌音效
        /// </summary>
        public const string SFX_DESK_SHIFT = "sfx_desk_shift";
        /// <summary>
        /// 用户加注筹码音效
        /// </summary>
        public const string SFX_DESK_POST_RAISE = "sfx_desk_post_raise";
        /// <summary>
        /// 轮到该用户操作音效
        /// </summary>
        public const string SFX_DESK_PLAYER_TURN = "sfx_desk_player_turn";
        /// <summary>
        /// 用户弃牌音效
        /// </summary>
        public const string SFX_DESK_PLAYER_FOLD = "sfx_desk_player_fold";
        /// <summary>
        /// 用户让牌音效
        /// </summary>
        public const string SFX_DESK_PLAYER_CHECK = "sfx_desk_player_check";
        /// <summary>
        /// 给用户发牌音效
        /// </summary>
        public const string SFX_DESK_NEW_CARD = "sfx_desk_new_card";
        /// <summary>
        /// 收取筹码音效
        /// </summary>
        public const string SFX_DESK_MOVE_CHIPS = "sfx_desk_move_chips";
        /// <summary>
        /// 开牌音效
        /// </summary>
        public const string SFX_DESK_CHAT = "sfx_desk_chat";
        /// <summary>
        /// 大盲注音效
        /// </summary>
        public const string SFX_DESK_BET_SECOND = "sfx_desk_bet_second";
        /// <summary>
        /// 小盲注音效
        /// </summary>
        public const string SFX_DESK_BET_FIRST = "sfx_desk_bet_first";
        /// <summary>
        /// 5秒提示
        /// </summary>
        public const string SFX_ACTION_ALERT = "sfx_action_alert";
        /// <summary>
        /// 大厅BGM
        /// </summary>
        public const string BGM_LOBBY = "bgm_lobby";
        /// <summary>
        /// 牌局BGM
        /// </summary>
        public const string BGM_GAME = "bgm_game";
        /// <summary>
        /// 滚轮
        /// </summary>
        public const string SFX_EFF_1 = "sfx_eff_1";
        /// <summary>
        /// 撒钱
        /// </summary>
        public const string SFX_EFF_2 = "sfx_eff_2";

        public const string defaultSoundAssetBundle = "Sound";

        public static SoundComponent Instance;
        public AudioManager audioManager;
        private ReferenceCollector rc;

        public async void Awake()
        {
            Instance = this;
            audioManager = AudioManager.Instance;
            // 项目音效资源少，直接加载全部。
            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            // resourcesComponent.LoadBundle("sound.unity3d");
            await resourcesComponent.LoadBundleAsync("sound.unity3d");
            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset("sound.unity3d", "Sound");
            GameObject sound = UnityEngine.Object.Instantiate(bundleGameObject);
            sound.name = "SoundComponent";
            sound.transform.parent = GameObject.Find("Global").transform;
            rc = sound.GetComponent<ReferenceCollector>();
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            resourcesComponent.UnloadBundle("sound.unity3d");
            base.Dispose();
        }

        #region 音乐封装
        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public void StopBGM()
        {
            audioManager.StopBGM();
        }

        public void StopAllSFX()
        {
            audioManager.StopAllSFX();
        }

        public void PlaySFX(string _clipName, Action _callback = null)
        {
            AudioClip clip = rc.Get<AudioClip>(_clipName);
            if (clip != null)
            {
                PlayClip(clip, _callback);
            }
            else
            {
                //Log.Error($"SoundComponent :: PlaySound 没有此音效 => {_clipName}");
            }
        }

        public void PlayBGM(string _clipName, float volume = 1f)
        {
            AudioClip clip = rc.Get<AudioClip>(_clipName);
            if (clip != null)
            {
                PlayBGM(clip, volume);
            }
            else
            {
                Log.Error($"SoundComponent :: PlaySound 没有此音效 => {_clipName}");
            }
        }

        public void EnableSFX(bool enable)
        {
            if (null != audioManager)
            {
                audioManager.IsSoundOn = enable;
                audioManager.SaveSFXPreferences();
            }
        }

        public void EnableBGM(bool enable)
        {
            if (null != audioManager)
            {
                audioManager.IsMusicOn = enable;
                audioManager.SaveBGMPreferences();
            }
        }

        public void SFXVolume(float volume)
        {
            if (null != audioManager)
            {
                audioManager.SoundVolume = volume;
                audioManager.SaveSFXPreferences();
            }
        }

        public void BGMVolume(float volume)
        {
            if (null != audioManager)
            {
                audioManager.MusicVolume = volume;
                audioManager.SaveBGMPreferences();
            }
        }

        #endregion

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="_callback"></param>
        private void PlayClip(AudioClip _clip, Action _callback = null)
        {
            if (_clip != null)
            {
                audioManager.PlayOneShot(_clip, _callback);
            }
        }

        /// <summary>
        /// 播放背景音效
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="_volume"></param>
        private void PlayBGM(AudioClip _clip, float _volume)
        {
            if (_clip != null)
            {
                audioManager.PlayBGM(_clip, MusicTransition.CrossFade, 1, _volume);
            }
        }
    }
}
