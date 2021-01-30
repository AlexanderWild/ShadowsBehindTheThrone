using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class UIMusic : MonoBehaviour
    {
        public bool doneLoading = false;

        public World world;
        public AudioSource source;
        public AudioClip playing;
        public AudioClip titleTheme;
        public List<AudioClip> loadedMusic = new List<AudioClip>();
        public int[] order;
        public int index;

        public void loadMusic()
        {
            if (Directory.Exists("advdata") && Directory.Exists("advdata" + World.separator + "music"))
            {
                foreach (string filename in Directory.GetFiles("advdata" + World.separator + "music"))
                {
                    if (filename.Contains(".wav") || filename.Contains(".ogg"))
                    {
                        World.log("Found music file: " + filename);
                        DirectoryInfo dir = new DirectoryInfo(".");
                        string target = "file://" + dir.FullName + World.separator + filename;
                        World.log(target);
                        WWW www = new WWW(target);//You'd be a fool NOT to treat a local file as if it were a web page
                        while (!www.isDone)
                        {
                            //Wait till whatever the fuck does whatever the fuck
                        }
                        AudioClip clip = www.GetAudioClip();
                        //clip.LoadAudioData();
                        if (filename.Contains("TITLE.wav"))
                            titleTheme = clip;
                        else
                            loadedMusic.Add(clip);
                    }
                }
            }
            doneLoading = true;
            shuffle();
        }

        public void shuffle()
        {
            if (loadedMusic.Count == 0) { order = new int[0]; return; }

            order = new int[loadedMusic.Count];
            for (int i = 0; i < order.Length; i++)
            {
                order[i] = i;
            }
            for (int i = 0; i < order.Length; i++)
            {
                int q = Eleven.random.Next(order.Length);
                int held = order[q];
                order[q] = order[i];
                order[i] = held;
            }
        }


        public void playTest()
        {
            source.clip = loadedMusic[0];
            source.clip = world.audioStore.activateFishmen;
            source.Play();
        }

        public void playTheme()
        {
            source.clip = titleTheme;
            source.Play();
        }

        private static IEnumerator fadeSource(AudioSource s, float seconds)
        {
            float start = s.volume;

            float max = seconds / Time.deltaTime;
            for (float n = 0f; n <= max; n += 1f)
            {
                s.volume = Mathf.Lerp(start, 0, n / max);
                yield return null;
            }

            s.Stop();
            s.clip = null;
        }

        public void stopTheme()
        {
            StartCoroutine(fadeSource(source, 2f));
        }

        public void Update()
        {
            if (source.clip != titleTheme)
                source.volume = World.musicVolume/100f;

            if (!doneLoading) { return; }
            if (loadedMusic.Count == 0) { return; }
            if (!PopupIOOptions.hasLoadedOpts) { return; }
            if (source.isPlaying) { return; }
            if (World.musicVolume == 0) { return; }

            if (index >= order.Length)
            {
                shuffle();
                index = 0;
            }

            source.clip = loadedMusic[order[index]];
            source.Play();
            //source.PlayOneShot(loadedMusic[order[index]]);

            World.log("Playing music. Index " + index + " of " + loadedMusic.Count);

            index += 1;
        }
    }
}
