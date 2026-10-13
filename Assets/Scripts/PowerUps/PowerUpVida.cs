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
// Desc: Power-up que le devuelve vida al jugador.

using UnityEngine;

public class PowerUpVida : PowerUp
{
    [SerializeField] private int vidaExtra = 10;

    protected override void Aplicar(GameObject jugador)
    {
        if (jugador.TryGetComponent(out VidaJugador vida))
        {
            vida.actualizarVida(vidaExtra);
        }
        else
        {
            Debug.LogWarning("No se encontró VidaJugador en el jugador.");
        }
    }
}
