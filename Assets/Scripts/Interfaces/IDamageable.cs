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
// Desc: Interfaz para todo lo que puede recibir daño.

/// <summary>
/// Interface para cualquier objeto que puede recibir daño
/// Úsala en enemigos, jefe, estructuras, etc.
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Recibe daño
    /// </summary>
    /// <param name="cantidad">Cantidad de daño a recibir</param>
    void TakeDamage(float cantidad);

    /// <summary>
    /// Obtiene la salud actual (opcional, útil para UI)
    /// </summary>
    float GetSalud();
}
