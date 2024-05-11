using Characters;
using UnityEngine;
using Utilities;

namespace Game
{
    public class ChickenTrap : MonoBehaviour
    {
        [SerializeField] private float decayTime = .8f;
        private float _currentDecayTime;
        private Chicken _chicken;
        private Material _myMaterial;

        private void Awake()
        {
            _chicken = transform.GetChild(0).GetComponent<Chicken>();
            _myMaterial = GetComponent<MeshRenderer>().material;
            _chicken.enabled = false;
        }

        private void OnTriggerStay(Collider other)
        {
            if (1 << other.gameObject.layer == StaticUtilities.PlayerLayer)
            {
                _currentDecayTime += Time.deltaTime;
                _myMaterial.SetFloat(StaticUtilities.FillMatID, _currentDecayTime / decayTime);
                if (_currentDecayTime >= decayTime)
                {
                    FreeChicken();
                }
            }
        }

        private void FreeChicken()
        {
            _chicken.transform.parent = null;
            _chicken.enabled = true;
            Destroy(gameObject);
        }
    }
}
