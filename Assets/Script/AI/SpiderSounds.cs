using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderSounds : MonoBehaviour
{

    public AudioClip[] audioClips; // Les clips audio � jouer
    public float[] probabilities; // Probabilit�s pour chaque clip (incluant une option de silence)
    public float interval = 2f;   // Intervalle entre chaque tentative de lecture

    private AudioSource audioSource;

    void Start()
    {
        if (audioClips.Length + 1 != probabilities.Length)
        {
            Debug.LogError("Le tableau des probabilit�s doit inclure une probabilit� pour le silence !");
            return;
        }

        // Normaliser les probabilit�s pour �viter des erreurs
        NormalizeProbabilities();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("Un AudioSource est n�cessaire sur cet objet !");
            return;
        }

        // D�marrer la lecture r�p�t�e
        InvokeRepeating(nameof(PlayRandomSound), interval, interval);
    }

    void PlayRandomSound()
    {
        if (probabilities.Length == 0) return;

        int clipIndex = GetRandomIndexByProbability();

        // Si l'index correspond au dernier �l�ment (le silence), ne rien jouer
        if (clipIndex == probabilities.Length - 1)
        {
            return;
        }

        // Jouer un clip
        if (clipIndex >= 0 && clipIndex < audioClips.Length)
        {
            audioSource.PlayOneShot(audioClips[clipIndex]);
        }
    }

    int GetRandomIndexByProbability()
    {
        float randomValue = Random.value; // Valeur entre 0 et 1
        float cumulative = 0f;

        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulative += probabilities[i];
            if (randomValue <= cumulative)
            {
                return i;
            }
        }

        return -1; // Si aucun index n'est trouv� (ne devrait pas arriver)
    }

    void NormalizeProbabilities()
    {
        float total = 0f;
        foreach (var p in probabilities)
        {
            total += p;
        }

        if (total > 0)
        {
            for (int i = 0; i < probabilities.Length; i++)
            {
                probabilities[i] /= total;
            }
        }
        else
        {
            Debug.LogError("La somme des probabilit�s doit �tre sup�rieure � z�ro !");
        }
    }

    public void StopPlaying()
    {
        CancelInvoke(nameof(PlayRandomSound));
    }
}

