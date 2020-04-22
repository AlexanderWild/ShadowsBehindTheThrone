using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
	public class AudioStore : MonoBehaviour
	{
		public float effectVolume = 1.0f;

		public AudioClip breath;
		public AudioClip scratch;

		public AudioClip click;
		public AudioClip clickInfo;
		public AudioClip clickSelect;

		public AudioClip activate;
		public AudioClip activateFlesh;
		public AudioClip activateFishmen;

		private AudioSource source;

		void Awake()
		{
			source = GetComponent<AudioSource>();
		}

		void playEffect(AudioClip c)
		{
			source.PlayOneShot(c, effectVolume);
		}

		public void playBreath() { playEffect(breath); }
		public void playScratch() { playEffect(scratch); }

		public void playClick() { playEffect(click); }
		public void playClickInfo() { playEffect(clickInfo); }
		public void playClickSelect() { playEffect(clickSelect); }

		public void playActivate() { playEffect(activate); }
		public void playActivateFlesh() { playEffect(activateFlesh); }
		public void playActivateFishmen() { playEffect(activateFishmen); }
	}
}
