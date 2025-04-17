using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Audio.Effects;
using YukkuriMovieMaker.Plugin.Effects;

namespace YMM4SamplePlugin.AudioEffect
{
    [AudioEffect("サンプル音声エフェクト", ["サンプル"], [])]
    public class SampleAudioEffect : AudioEffectBase
    {
        public override string Label => "サンプル音声エフェクト";

        [Display(GroupName = "サンプル", Name = "音量", Description = "音量を調整します")]
        [AnimationSlider("F0", "%", 0, 100)]
        public Animation Volume { get; } = new Animation(100, 0, 100);

        [Display(GroupName = "グループ名", Name = "テキスト", Description = "項目の説明")]
        [TextEditor(AcceptsReturn = true)]
        public string Text { get => text; set => Set(ref text, value); }
        string text = string.Empty;

        /// <summary>
        /// トグルが押されたらテキストを送信
        /// </summary>
        [Display(GroupName = "グループ名", Name = "トグル", Description = "送信スイッチ")]
        [ToggleSlider]
        public bool Toggle
        {
            get => toggle;
            set
            {
                if (Set(ref toggle, value)) // 値が変化したときのみ
                {
                    if (!string.IsNullOrWhiteSpace(Text?.Trim()))
                    {
                        _ = AutoConsoleSender.SendTextAsync(Text); // ONでもOFFでも送信
                    }
                }
            }
        }
        bool toggle = false;
        public override IAudioEffectProcessor CreateAudioEffect(TimeSpan duration)
        {
            return new SampleAudioEffectProcessor(this, duration);
        }

        public override IEnumerable<string> CreateExoAudioFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            var fps = exoOutputDescription.VideoInfo.FPS;
            return
            [
                $"_name=音量\r\n" +
                $"_disable={(IsEnabled ? 1 : 0)}\r\n" +
                $"音量={Volume.ToExoString(keyFrameIndex, "F1", fps)}\r\n"
            ];
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Volume];
    }
}
