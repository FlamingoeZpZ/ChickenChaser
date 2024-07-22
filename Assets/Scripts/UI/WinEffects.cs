using Characters;
using Unity.Cinemachine;
using UnityEngine;

public class WinEffects : MonoBehaviour
{

    void OnEnable()
    {
        PlayerChicken.onPlayerEscaped += OnGameWon;
    }

    private void OnDisable()
    {
        PlayerChicken.onPlayerEscaped -= OnGameWon;
    }

    private void OnGameWon(Vector3 _)
    {
        Debug.Log("Who am I?", gameObject);
        GetComponent<CinemachineCamera>().enabled = true;
    }
}
