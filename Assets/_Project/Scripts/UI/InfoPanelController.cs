using System.Collections;
using TMPro;
using UnityEngine;
using VoltVR.Core;

namespace VoltVR.UI
{
    /// <summary>
    /// Ekrandaki bilgi panelini yönetir. SimulationManager'ın olaylarını dinler
    /// ve adım yönergesi, tıklanan parçanın açıklaması, geri bildirim ve ilerleme
    /// yazılarını günceller.
    ///
    /// Kurulum: Sahnede bir Canvas oluştur, içine TextMeshPro yazılar koy ve
    /// bu scriptteki alanlara sürükle-bırak ile bağla. Boş bıraktığın alanlar
    /// sorun çıkarmaz (null-güvenli).
    /// </summary>
    public class InfoPanelController : MonoBehaviour
    {
        [Header("UI Referansları (TextMeshPro)")]
        public TMP_Text stepTitleText;     // Adım başlığı
        public TMP_Text instructionText;   // Ne yapılacağı yönergesi
        public TMP_Text partInfoText;      // Tıklanan parçanın bilgisi
        public TMP_Text feedbackText;      // Başarı / uyarı mesajı
        public TMP_Text progressText;      // "Adım 2 / 4"

        [Header("Geri Bildirim Rengi")]
        public Color successColor = new Color(0.3f, 0.9f, 0.4f);
        public Color warningColor = new Color(1f, 0.4f, 0.3f);

        private void OnEnable()
        {
            var m = SimulationManager.Instance;
            m.OnStepChanged += HandleStepChanged;
            m.OnPartInspected += HandlePartInspected;
            m.OnFeedback += HandleFeedback;
            m.OnSimulationCompleted += HandleCompleted;
        }

        private void Start()
        {
            // Manager bizden önce başlamış olabilir; mevcut adımı tazele.
            SimulationManager.Instance.BroadcastCurrentState();
        }

        private void OnDisable()
        {
            // Oyun kapanırken yeni instance yaratmamak için Instance'ı yeniden çağırmıyoruz.
            var m = SimulationManager.Instance;
            if (m == null) return;
            m.OnStepChanged -= HandleStepChanged;
            m.OnPartInspected -= HandlePartInspected;
            m.OnFeedback -= HandleFeedback;
            m.OnSimulationCompleted -= HandleCompleted;
        }

        private void HandleStepChanged(SimulationStep step, int index, int total)
        {
            SetText(stepTitleText, step.title);
            SetText(instructionText, step.instruction);
            SetText(progressText, $"Adım {index + 1} / {total}");
        }

        private void HandlePartInspected(string info)
        {
            SetText(partInfoText, info);
        }

        private void HandleFeedback(string message)
        {
            if (feedbackText == null) return;
            feedbackText.text = message;
            // Uyarı mı başarı mı? Basit bir renk seçimi.
            bool isWarning = message.Contains("⚠") || message.Contains("değil") || message.Contains("TEHLİKE");
            feedbackText.color = isWarning ? warningColor : successColor;
        }

        private void HandleCompleted()
        {
            SetText(stepTitleText, "✔ SİMÜLASYON TAMAMLANDI");
            SetText(instructionText, "Tüm güvenlik adımları doğru sırayla uygulandı ve şarj başlatıldı.");
            SetText(progressText, "Bitti");
        }

        private static void SetText(TMP_Text field, string value)
        {
            if (field != null) field.text = value;
        }
    }
}
