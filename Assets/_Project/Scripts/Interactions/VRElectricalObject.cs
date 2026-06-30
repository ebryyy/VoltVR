using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VoltVR.Core;

namespace VoltVR.Interaction
{
    // Bu script, XRGrabInteractable sýnýfýndan miras alýr. Yani takýldýðý nesne hem tutulabilir olur hem de bu özel kodlarý çalýþtýrýr.
    public class VRElectricalObject : XRGrabInteractable
    {
        // Unity 6 XRI 3.x mimarisinde bir nesne tutulduðunda "OnSelectEntered" fonksiyonu tetiklenir.
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args); // XRI'ýn kendi temel tutma fonksiyonunu çalýþtýr

            // ÝÞ GÜVENLÝÐÝ KONTROLÜ:
            // Eðer simülasyonda güç henüz kesilmediyse VE teknisyen bu nesneye dokunduysa...
            if (!SimulationManager.Instance.IsPowerCut)
            {
                // Ark patlamasýný tetikle! Nesnenin tam olduðu pozisyonda patlama olacak.
                SimulationManager.Instance.TriggerArcBlast(transform.position);
            }
            else
            {
                Debug.Log($"{gameObject.name} güvenli bir þekilde tutuldu.");
            }
        }
    }
}