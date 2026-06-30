using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VoltVR.Core;

namespace VoltVR.Interaction
{
    public class VRBreakerSwitch : XRSimpleInteractable
    {
        private bool _isSwitchDown = false;

        protected override void OnActivated(ActivateEventArgs args)
        {
            base.OnActivated(args);
            TriggerSwitch(); // Asýl iþi yapan fonksiyonu çaðýrýyoruz
        }

        // --- MOUSE ÝLE TEST ETMEK ÝÇÝN EKLENEN KISIM ---
        [ContextMenu("Þalteri Ýndir (Test)")]
        public void TriggerSwitch()
        {
            if (!_isSwitchDown)
            {
                _isSwitchDown = true;
                transform.localRotation = Quaternion.Euler(45f, 0f, 0f);
                SimulationManager.Instance.IsPowerCut = true;
                Debug.Log("ÞALTER ÝNDÝRÝLDÝ: Yüksek voltaj hattý baþarýyla kesildi.");
            }
        }
    }
}