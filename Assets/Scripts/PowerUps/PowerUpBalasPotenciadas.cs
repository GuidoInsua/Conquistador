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
// Desc: Power-up que hace las balas del jugador más anchas y más fuertes por un tiempo.

using UnityEngine;

public class PowerUpBalasPotenciadas : PowerUp
{
    [SerializeField] private float multiplicadorAncho = 2f;
    [SerializeField] private float multiplicadorDano = 2f;
    [SerializeField] private float duracion = 20f; // Duración en segundos

    protected override void Aplicar(GameObject jugador)
    {
        // TryGetComponent busca el componente: si lo encuentra devuelve true y lo deja en la variable de "out".
        if (jugador.TryGetComponent(out DispararJugador disparo))
        {
            disparo.ActivarBalasPotenciadas(multiplicadorAncho, multiplicadorDano, duracion);
        }
        else
        {
            Debug.LogWarning("No se encontró DispararJugador en el jugador.");
        }
    }
}
