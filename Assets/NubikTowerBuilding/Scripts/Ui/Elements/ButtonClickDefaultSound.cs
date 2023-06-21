using KlopoffGames.Core.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace NubikTowerBuilding.Ui.Elements
{
    public class ButtonClickDefaultSound : MonoBehaviour, IPointerClickHandler
    {
        [Inject] private AudioManager _audio;
        [SerializeField] private string sfxName = "ClickGeneric1";

        private Selectable _selectable;

        private void Start()
        {
            _selectable = GetComponent<Selectable>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_selectable.interactable)
            {
                return;
            }
            
            _audio.PlaySound(sfxName, false,1f, 1f);    
        }
    }
}
