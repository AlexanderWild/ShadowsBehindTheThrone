using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
	public class AudioStore : MonoBehaviour
	{
		public float effectVolume = 1.0f;

		public AudioClip back;
		public AudioClip click;
		public AudioClip confirm;

		private AudioSource source;

		void Awake()
		{
			source = GetComponent<AudioSource>();
		}

		void playEffect(AudioClip c)
		{
			source.PlayOneShot(c, effectVolume);
		}

		public void playBack() { playEffect(back); }
		public void playClick() { playEffect(click); }
		public void playConfirm() { playEffect(confirm); }
	}
}
