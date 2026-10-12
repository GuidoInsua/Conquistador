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
    
    private const float intervaloDisparo = 6f;
    private float tiempoDesdeUltimoDisparo = 1f;
    private EnemigoAlien enemigoAlien;

    void Start()
    {
        enemigoAlien = GetComponent<EnemigoAlien>();
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
