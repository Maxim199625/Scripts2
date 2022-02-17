using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Visualizer : MonoBehaviour
{
	public float minHeight = 3.0f;
	public float maxHeight = 23.0f;
	public float updateSentivity = 10.0f;
	public Color minColor;
	public Color hightColor;

	[Space(15)]
	public AudioClip[] audioClips;
	//public bool loop = true;
	[Space(15), Range(64, 8192)]
	private int visualizerSimples = 64;

	private VisualizerObjectScript[] visualizerObjects;
	private Transform[] childsTr;
	private AudioSource audioSource;
	public float currentClipTime;
	private int currentClip;

	void Start()
	{
		visualizerObjects = GetComponentsInChildren<VisualizerObjectScript>();
		childsTr = new Transform[visualizerObjects.Length];
        for (int i = 0; i < childsTr.Length; i++)
        {
			childsTr[i] = visualizerObjects[i].transform;
        }
		audioSource = GetComponent<AudioSource>();
		InvokeRepeating("UpdateMusic", 0f, 0.05f);

		currentClip = PlayerPrefs.GetInt("CurrentClip");
		audioSource.PlayOneShot(audioClips[currentClip]);
		currentClipTime = audioClips[currentClip].length;
		StartCoroutine(NextClip());
	}

	IEnumerator NextClip()
    {
        while (true)
        {
			if (currentClip < audioClips.Length - 1)
				currentClip++;
			else
				currentClip = 0;
			
			PlayerPrefs.SetInt("CurrentClip", currentClip);
			yield return new WaitForSeconds(currentClipTime);
			
			audioSource.PlayOneShot(audioClips[currentClip]);
			currentClipTime = audioClips[currentClip].length;
		}
    }

    [System.Obsolete]
    void UpdateMusic()
	{
		float[] spectrumData = audioSource.GetSpectrumData(visualizerSimples, 0, FFTWindow.Rectangular);
		for (int i = 0; i < visualizerObjects.Length; i++)
		{
			Vector3 newSize = childsTr[i].localScale;
			float indensety = Mathf.Lerp(
				newSize.y, minHeight + (spectrumData[i] * (maxHeight - minHeight)), updateSentivity);

			newSize.y = Mathf.Clamp(indensety, minHeight, maxHeight);
			childsTr[i].localScale = newSize;

			//myRenderers[i].material.color = Color.Lerp(minColor, hightColor, indensety / 10);
			//myRenderers[i].material.SetColor("_EmissionColor", Color.Lerp(minColor, hightColor, newSize.y * newValue));
			//print(newSize.y * newValue);
		}
	}
}
