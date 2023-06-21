using UnityEngine;
using UnityEngine.EventSystems;

namespace NubikTowerBuilding.Ui
{
    public class UiInputTrigger : MonoBehaviour, IPointerDownHandler,
        IPointerUpHandler, IPointerEnterHandler,
        IPointerExitHandler,
        IPointerClickHandler,
        IPointerMoveHandler
    {
        public delegate void DownDelegate(PointerEventData eventData);
        public delegate void UpDelegate(PointerEventData eventData);
        public delegate void EnterDelegate(PointerEventData eventData);
        public delegate void ExitDelegate(PointerEventData eventData);
        public delegate void ClickDelegate(PointerEventData eventData);
        public delegate void MoveDelegate(PointerEventData eventData);

        public event DownDelegate OnDown;
        public event UpDelegate OnUp;
        public event EnterDelegate OnEnter;
        public event ExitDelegate OnExit;
        public event ClickDelegate OnClick;
        public event MoveDelegate OnMove;
        
        public bool IsDown { get; private set; }
        public bool IsOver { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsDown = true;
            OnDown?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsDown = false;
            OnUp?.Invoke(eventData);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsOver = true;
            OnEnter?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsOver = false;
            OnExit?.Invoke(eventData);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            OnMove?.Invoke(eventData);
        }
    }
}