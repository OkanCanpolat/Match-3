
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioClip[] candyDestroySounds;
    private AudioSource audioSource;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        #endregion
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayCandyDestroySound()
    {
        int index = Random.Range(0, candyDestroySounds.Length);
        audioSource.PlayOneShot(candyDestroySounds[index]);
    }

}
