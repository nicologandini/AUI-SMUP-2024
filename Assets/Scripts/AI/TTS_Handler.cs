using System.Threading.Tasks;
using UnityEngine;

namespace SMUP.AI {
    public class TTS_Handler : MonoBehaviour
    {
        [SerializeField] private AI_TTS tts;

        private bool _isTalking = false;

        private void Start() {
            TalkText("ciao, come va123, lungo testo di prova 123. Ciao che stai facendo?");
            TalkText("Prova 1,2,3, prova di testo subito dopo ad un altro");
        }

        public async Task TalkText(string text) {
            if(_isTalking) {return;}

            _isTalking = true;
            await tts.TextToSpeech(text);
            _isTalking = false;
        }
    }
}