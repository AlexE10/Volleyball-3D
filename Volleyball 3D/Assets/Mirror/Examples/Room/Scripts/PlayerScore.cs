using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.NetworkRoom
{
    public class PlayerScore : NetworkBehaviour
    {
        [SyncVar]
        public int index;

        [SyncVar]
        public uint score;


        void OnGUI()
        {
            GUI.Box(new Rect(10f + (index * 110), 10f, 100f, 25f), $"P{index}: {score:0000000}");
            //Debug.Log("Indexul " + index);
            //scoreText.text = score.ToString();
        }

        public void UpdateScore(bool value)
        {
            if (value)
            {
                this.score++;
            }
        }
    }
}
