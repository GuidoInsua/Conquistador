// ##########################################################################
// ## o ########## o ##                                                     #
// ### \ ######## / ###                                                     #
// #### \ ###### / ####       CONQUISTADOR                                  #
// ##### :-'""'-: #####       ------------                                  #
// ## .-'  ____  `-. ##       Autor:   Guido Insua                          #
// # ( (  (_()_)  ) ) #       Mail:    insua.guido@gmail.com                #
// ## `-.   ^^   .-' ##       Materia: Programacion 1 2do Cuatri 2026       #
// ##### `._==_.' #####                                                     #
// ###### __)(___ #####                                                     #
// ##########################################################################
// Desc: Hace que el alien dispare cada cierto tiempo con su animación.

using UnityEngine;

public class DisparoEnemigo : MonoBehaviour
{
    [SerializeField] Transform posicionArma; 
    [SerializeField] GameObject balaPrefab; 
    
    [SerializeField] float intervaloDisparo = 6f;

    [Header("Disparo en cono")]
    [SerializeField] int cantidadBalas = 1;
    [SerializeField] float anguloCono = 0f; // Grados entre la primera y la última bala

    private float tiempoDesdeUltimoDisparo = 1f;
    private EnemigoAlien enemigoAlien;

    void Awake()
    {
        enemigoAlien = GetComponent<EnemigoAlien>();

        if (enemigoAlien == null)
        {
            Debug.LogError("DisparoEnemigo: falta el componente EnemigoAlien.");
            enabled = false;
        }
    }

    // El enemigo vuelve del pool: el temporizador empieza de nuevo.
    void OnEnable()
    {
        tiempoDesdeUltimoDisparo = 1f;
    }

    void Update()
    {
        // Muriendo: no pisar la animación de muerte, o Morir() nunca se llama.
        if (enemigoAlien.GetSalud() <= 0)
            return;

        tiempoDesdeUltimoDisparo += Time.deltaTime;

        if (tiempoDesdeUltimoDisparo >= intervaloDisparo)
        {
            tiempoDesdeUltimoDisparo = 0;
            enemigoAlien.cambiarAnimacion(EnemigoAlien.animDisparo);
        }
    }

    public void shoot()
    {
        // Evento de la animación: si el alien ya está muriendo, no disparar ni pisar la muerte.
        if (enemigoAlien == null || enemigoAlien.GetSalud() <= 0)
            return;

        if (balaPrefab != null && posicionArma != null)
        {
            for (int i = 0; i < cantidadBalas; i++)
            {
                float angulo = cantidadBalas > 1
                    ? Mathf.Lerp(-anguloCono / 2f, anguloCono / 2f, i / (float)(cantidadBalas - 1))
                    : 0f;

                Instantiate(balaPrefab, posicionArma.position, posicionArma.rotation * Quaternion.Euler(0f, 0f, angulo));
            }
        }
        else
        {
            Debug.LogWarning("Falta asignar el prefab de la bala o la posición del arma.");
        }
        enemigoAlien.cambiarAnimacion(EnemigoAlien.animIdle);
    }
}
