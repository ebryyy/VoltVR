using UnityEngine;
using VoltVR.Core;

namespace VoltVR.Interaction
{
    /// <summary>
    /// Şarj istasyonundaki tıklanabilir bir parça (şalter, eldiven, voltaj kalemi,
    /// şarj kablosu, kapak vb.). Her parçaya bu script eklenir ve Inspector'dan
    /// id / isim / açıklama doldurulur.
    ///
    /// Hem MASAÜSTÜ (fare ile tıklama) hem de VR (Interact() çağrısı) destekler.
    /// Demo VR gözlüğü olmadan da çalışsın diye fare tıklaması ana giriş yoludur.
    ///
    /// Gereksinim: GameObject üzerinde bir Collider olmalı (fare tıklaması için).
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ChargingStationPart : MonoBehaviour
    {
        [Header("Parça Kimliği")]
        [Tooltip("Senaryo adımlarındaki requiredPartId ile eşleşmeli. " +
                 "Hazır senaryo id'leri: gloves, breaker, voltmeter, charge_cable")]
        public string partId = "breaker";

        public string partName = "Ana Şalter";

        [TextArea(2, 5)]
        public string description = "Yüksek voltaj hattını açıp kapatan ana şalter.";

        [Header("Güvenlik")]
        [Tooltip("İşaretliyse: enerji kesilmeden bu parçaya dokunmak ark patlamasını tetikler.")]
        public bool isLiveHazard = false;

        [Header("Görsel (opsiyonel)")]
        [Tooltip("Fareyle üzerine gelince parçayı vurgular.")]
        public bool highlightOnHover = true;
        public Color highlightColor = new Color(1f, 0.85f, 0.2f);

        private Renderer _renderer;
        private Color _baseColor;
        private bool _hasColor;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer != null && _renderer.material.HasProperty("_BaseColor"))
            {
                _baseColor = _renderer.material.color;
                _hasColor = true;
            }
        }

        // --- MASAÜSTÜ: fare etkileşimi ---
        // OnMouseDown/Enter/Exit, sahnede Collider + kamerada PhysicsRaycaster
        // (ya da sadece bir Collider) varsa otomatik çalışır.
        private void OnMouseDown() => Interact();

        private void OnMouseEnter()
        {
            if (highlightOnHover && _hasColor)
                _renderer.material.color = highlightColor;
        }

        private void OnMouseExit()
        {
            if (highlightOnHover && _hasColor)
                _renderer.material.color = _baseColor;
        }

        /// <summary>
        /// Ortak etkileşim noktası. Fare buradan çağırır; VR tarafında da
        /// XR Interactable'ın "Select Entered" olayına bu metod bağlanabilir.
        /// </summary>
        public void Interact()
        {
            SimulationManager.Instance.ReportInteraction(this);
        }
    }
}
