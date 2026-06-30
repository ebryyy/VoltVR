using UnityEngine;
using UnityEngine.InputSystem; // Yeni Input System
using VoltVR.Interaction;

namespace VoltVR.Core
{
    /// <summary>
    /// MASAÜSTÜ tıklama sistemi. Main Camera'ya eklenir.
    ///
    /// Neden bu var: Proje "yeni Input System" kullandığı için Unity'nin eski
    /// OnMouseDown mekanizması bazı kurulumlarda sessizce çalışmaz. Bu script
    /// fare tıklamasını yeni Input System ile alır, kameradan ışın (ray) atar
    /// ve denk gelen ChargingStationPart'ı tetikler. HER tıklamada Console'a
    /// ne olduğunu yazar; böylece sorun varsa hemen görürüz.
    /// </summary>
    public class DesktopClickInteractor : MonoBehaviour
    {
        [Tooltip("Boş bırakılırsa otomatik bu nesnedeki ya da Main Camera kullanılır.")]
        public Camera cam;
        public float maxDistance = 100f;

        private void Awake()
        {
            if (cam == null) cam = GetComponent<Camera>();
            if (cam == null) cam = Camera.main;
        }

        private void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;
            if (!mouse.leftButton.wasPressedThisFrame) return; // sadece tıklama anı

            if (cam == null)
            {
                Debug.LogError("[Tıklama] Kamera bulunamadı! Main Camera'nın 'MainCamera' tag'i var mı?");
                return;
            }

            Vector2 screenPos = mouse.position.ReadValue();
            Ray ray = cam.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
            {
                // Collider parçanın çocuğunda olsa bile parent'taki script'i bul.
                var part = hit.collider.GetComponentInParent<ChargingStationPart>();
                if (part != null)
                {
                    Debug.Log($"[Tıklama] ✔ '{part.partName}' (id: {part.partId}) tıklandı.");
                    part.Interact();
                }
                else
                {
                    Debug.Log($"[Tıklama] '{hit.collider.name}' nesnesine değdi ama üzerinde ChargingStationPart yok.");
                }
            }
            else
            {
                Debug.Log("[Tıklama] Hiçbir collider'a değmedi → parçalarda Collider eksik olabilir (ya da yanlış yere tıklandı).");
            }
        }
    }
}
