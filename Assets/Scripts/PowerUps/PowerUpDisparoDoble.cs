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
// Desc: Power-up que hace que el jugador dispare dos balas en paralelo por un tiempo.

using UnityEngine;

public class PowerUpDisparoDoble : PowerUp
{
    [SerializeField] private float duracion = 20f; // Duración en segundos

    protected override void Aplicar(GameObject jugador)
    {
        if (jugador.TryGetComponent(out DispararJugador disparo))
        {
            disparo.ActivarDisparoDoble(duracion);
        }
        else
        {
            Debug.LogWarning("No se encontró DispararJugador en el jugador.");
        }
    }
}
