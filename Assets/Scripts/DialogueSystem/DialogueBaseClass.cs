using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueBaseClass : MonoBehaviour
    {
        public bool finished { get; private set; }

        protected IEnumerator WriteText(string input, Text textHolder, Color textColor, Font textFont, float delay, AudioClip sound, float delayBetweenLines)
        {
            textHolder.color = textColor;
            textHolder.font = textFont;

            for (int i = 0; i < input.Length; i++)
            {
                // Мгновенный пропуск при нажатии пробела
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    textHolder.text = input;
                    break;
                }

                textHolder.text += input[i];
                SoundManager.instance.PlaySound(sound);
                yield return new WaitForSeconds(delay);
            }

            yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Space));
            finished = true;
        }
    }
}