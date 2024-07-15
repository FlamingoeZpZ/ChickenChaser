using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class CustomButton : Selectable
    {
        // Event delegates triggered on click.
        [FormerlySerializedAs("onReleased")]
        [SerializeField]
        
        private Button.ButtonClickedEvent m_OnReleased = new ();
        
        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private Button.ButtonClickedEvent m_OnClick = new();

        public Button.ButtonClickedEvent onReleased
        {
            get => m_OnReleased;
            set => m_OnReleased = value;
        }
        
        public Button.ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }
        
        private void Release()
        {
            //if (!IsActive() || !IsInteractable()) return;

            UISystemProfilerApi.AddMarker("Button.onReleased", this);
            m_OnReleased.Invoke();
        }
        
      

        private void Press()
        {
            //if (!IsActive() || !IsInteractable()) return;
            
            UISystemProfilerApi.AddMarker("Button.onClick", this);
            m_OnClick.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            Release();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            Press();
        }
    }
}