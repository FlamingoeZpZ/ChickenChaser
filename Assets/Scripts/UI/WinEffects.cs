using Characters;
using Cinemachine;
using UnityEngine;

public class WinEffects : MonoBehaviour
{

    private CinemachineVirtualCamera _cam;

    private void Awake()
    {
        _cam = GetComponent<CinemachineVirtualCamera>();
    }

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
        _cam.enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
