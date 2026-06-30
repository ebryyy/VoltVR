using System;
using System.Collections.Generic;
using UnityEngine;
using VoltVR.Interaction;

namespace VoltVR.Core
{
    /// <summary>
    /// Tek bir simülasyon adımını temsil eder. Inspector'dan doldurulur,
    /// böylece programcı olmayan ekip üyesi de senaryoyu düzenleyebilir.
    /// </summary>
    [Serializable]
    public class SimulationStep
    {
        public string title;                      // Örn: "Adım 2: Enerjiyi Kes"
        [TextArea(2, 4)] public string instruction; // Panelde gösterilecek yönerge
        public string requiredPartId;             // Bu adımı geçmek için tıklanması gereken parçanın id'si
    }

    /// <summary>
    /// Simülasyonun beyni. Adımları sırayla yürütür, parça tıklamalarını
    /// değerlendirir ve UI'a olaylar (event) yayınlar.
    ///
    /// ÖNEMLİ: Bu sınıf tamamen masaüstü (fare ile) çalışacak şekilde tasarlandı.
    /// VR gözlüğü olmadan da demo açılıp tıklanabilir. VR sadece bonus.
    /// </summary>
    public class SimulationManager : Singleton<SimulationManager>
    {
        [Header("Senaryo Adımları (sırayla yürütülür)")]
        public List<SimulationStep> steps = new List<SimulationStep>();

        // --- UI'ın dinleyeceği olaylar (event) ---
        public event Action<SimulationStep, int, int> OnStepChanged; // (adım, indeks, toplam)
        public event Action<string> OnPartInspected;                 // tıklanan parçanın açıklaması
        public event Action<string> OnFeedback;                      // başarı / uyarı mesajı
        public event Action OnSimulationCompleted;                   // tüm adımlar bitti
        public event Action<Vector3> OnArcBlast;                     // güvenlik ihlali konumu

        // --- Global durum bayrakları (eski kodla uyumlu kalsın diye korundu) ---
        public bool IsGlovesEquipped { get; set; }
        public bool IsPowerCut { get; set; }
        public bool IsVoltageVerified { get; set; }

        private int _currentIndex = -1;
        public bool IsCompleted => _currentIndex >= steps.Count;
        public SimulationStep CurrentStep =>
            (_currentIndex >= 0 && _currentIndex < steps.Count) ? steps[_currentIndex] : null;

        private void Start()
        {
            // Inspector boşsa, demo yine de çalışsın diye varsayılan senaryoyu yükle.
            if (steps == null || steps.Count == 0)
                LoadDefaultScenario();

            GoToStep(0);
        }

        /// <summary>UI bileşenleri geç açılırsa mevcut durumu tekrar yayınlamak için.</summary>
        public void BroadcastCurrentState()
        {
            if (CurrentStep != null)
                OnStepChanged?.Invoke(CurrentStep, _currentIndex, steps.Count);
        }

        private void GoToStep(int index)
        {
            _currentIndex = index;

            if (index < steps.Count)
            {
                OnStepChanged?.Invoke(steps[index], index, steps.Count);
                Debug.Log($"[Simülasyon] {steps[index].title}");
            }
            else
            {
                OnSimulationCompleted?.Invoke();
                OnFeedback?.Invoke("Tüm güvenlik adımları tamamlandı. Şarj güvenle başlatıldı.");
                Debug.Log("[Simülasyon] TAMAMLANDI.");
            }
        }

        /// <summary>
        /// Tıklanabilir parçalar (ChargingStationPart) tıklandığında burayı çağırır.
        /// </summary>
        public void ReportInteraction(ChargingStationPart part)
        {
            // 1) Her tıklamada parçayı tanıt (bilgi paneli için).
            OnPartInspected?.Invoke($"<b>{part.partName}</b>\n{part.description}");

            if (IsCompleted) return;

            var step = CurrentStep;

            // 2) Güvenlik kuralı: enerji kesilmeden canlı/tehlikeli parçaya dokunma!
            if (part.isLiveHazard && !IsPowerCut)
            {
                TriggerArcBlast(part.transform.position);
                return;
            }

            // 3) Doğru parça mı? Doğruysa adımı geç, değilse yönlendir.
            if (part.partId == step.requiredPartId)
            {
                ApplySideEffects(part.partId);
                OnFeedback?.Invoke($"✔ {step.title} tamamlandı.");
                GoToStep(_currentIndex + 1);
            }
            else
            {
                OnFeedback?.Invoke($"Sıradaki adım bu değil. Yapman gereken: {step.title}");
            }
        }

        /// <summary>Bazı parçalar tıklanınca global durumu değiştirir.</summary>
        private void ApplySideEffects(string partId)
        {
            switch (partId)
            {
                case "gloves": IsGlovesEquipped = true; break;
                case "breaker": IsPowerCut = true; break;
                case "voltmeter": IsVoltageVerified = true; break;
            }
        }

        /// <summary>Kural ihlali: enerji kesilmeden müdahale → ark patlaması.</summary>
        public void TriggerArcBlast(Vector3 explosionPosition)
        {
            Debug.LogError("KRİTİK HATA: Ark patlaması! Enerji kesilmeden müdahale edildi.");
            OnArcBlast?.Invoke(explosionPosition);
            OnFeedback?.Invoke("⚠ TEHLİKE: Önce ana şalteri indirip enerjiyi kesmelisin! (Ark patlaması)");
        }

        /// <summary>
        /// Inspector boş bırakılırsa kullanılan hazır 4 adımlı senaryo.
        /// EV şarj istasyonunda güvenli müdahale + şarj başlatma akışı.
        /// </summary>
        private void LoadDefaultScenario()
        {
            steps = new List<SimulationStep>
            {
                new SimulationStep {
                    title = "Adım 1: Güvenlik Eldiveni",
                    instruction = "Yalıtkan güvenlik eldivenlerini giy. (Eldivenlere tıkla)",
                    requiredPartId = "gloves"
                },
                new SimulationStep {
                    title = "Adım 2: Enerjiyi Kes (LOTO)",
                    instruction = "Ana şalteri indirerek yüksek voltaj hattının enerjisini kes.",
                    requiredPartId = "breaker"
                },
                new SimulationStep {
                    title = "Adım 3: Voltaj Kontrolü",
                    instruction = "Voltaj kalemiyle ölçüm yaparak enerjinin gerçekten kesildiğini doğrula.",
                    requiredPartId = "voltmeter"
                },
                new SimulationStep {
                    title = "Adım 4: Şarjı Başlat",
                    instruction = "Artık güvenli. Şarj kablosunu bağlayıp şarjı başlat.",
                    requiredPartId = "charge_cable"
                }
            };
        }
    }
}
