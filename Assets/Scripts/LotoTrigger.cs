using UnityEngine;

/// <summary>
/// LEGACY / basit yardımcı: bir parçaya fareyle tıklayınca bir mesaj nesnesini açar.
/// Not: Asıl etkileşim sistemi artık ChargingStationPart + InfoPanelController üzerinden
/// yürüyor. Bu script sadece sahnede eski referans kırılmasın diye düzeltilmiş halde
/// bırakıldı. Yeni parçalar için ChargingStationPart kullanın.
///
/// Gereksinim: GameObject üzerinde bir Collider olmalı.
/// </summary>
[RequireComponent(typeof(Collider))]
public class LotoTrigger : MonoBehaviour
{
    [Tooltip("Tıklanınca aktif edilecek mesaj/uyarı nesnesi (örn. bir UI paneli).")]
    public GameObject lotoMessage;

    // Unity'nin OnMouseDown'u parametresizdir. Eski koddaki parametreli imza
    // hiçbir zaman tetiklenmiyordu; düzeltildi.
    private void OnMouseDown()
    {
        if (lotoMessage != null)
            lotoMessage.SetActive(true);
    }
}
