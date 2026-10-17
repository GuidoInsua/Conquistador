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
// Desc: Power-up que aumenta la velocidad del jugador por un tiempo.

using UnityEngine;

public class PowerUpVelocidad : PowerUp
{
    [SerializeField] private float speedBoostPercentage = 0.2f; // 20% de aumento
    [SerializeField] private float boostDuration = 10f; // Duración en segundos

    protected override void Aplicar(GameObject jugador)
    {
        // TryGetComponent busca el componente: si lo encuentra devuelve true y lo deja en la variable de "out".
        if (jugador.TryGetComponent(out MovimientoJugador movimiento))
        {
            movimiento.ApplySpeedBoost(speedBoostPercentage, boostDuration);
        }
        else
        {
            Debug.LogWarning("No se encontró MovimientoJugador en el jugador.");
        }
    }
}
