using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterName : MonoBehaviour
{
    public Camera mainCamera;
    void Start()
    {
        // Eğer kamera atanmamışsa, ana kamerayı otomatik olarak bul.
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        // Kamera ile nesne arasındaki yön vektörünü al
        Vector3 direction = mainCamera.transform.position - transform.position;

        // Y ekseninde dönmeyi engellemek için Y eksenini sıfırla
        direction.y = 0;

        // Nesneyi bu yeni yön vektörüne doğru döndür
        transform.rotation = Quaternion.LookRotation(-direction);
    }
}
