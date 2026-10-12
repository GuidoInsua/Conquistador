using UnityEngine;

public class DisparoEnemigo : MonoBehaviour
{
    [SerializeField] Transform posicionArma; 
    [SerializeField] GameObject balaPrefab; 
    
    private const float intervaloDisparo = 6f;
    private float tiempoDesdeUltimoDisparo = 1f;
    private EnemigoAlien enemigoAlien;

    void Start()
    {
        enemigoAlien = GetComponent<EnemigoAlien>();
    }
    void Update()
    {
        tiempoDesdeUltimoDisparo += Time.deltaTime;

        if (tiempoDesdeUltimoDisparo >= intervaloDisparo)
        {
            tiempoDesdeUltimoDisparo = 0;
            enemigoAlien.cambiarAnimacion("Alien01_Disparo");
        }
    }

    public void shoot()
    {   
        if (balaPrefab != null && posicionArma != null)
        {
            Instantiate(balaPrefab, posicionArma.position, posicionArma.rotation);
        }
        else
        {
            Debug.LogWarning("Falta asignar el prefab de la bala o la posición del arma.");
        }
        enemigoAlien.cambiarAnimacion("Alien01_Idle");
    }
}
