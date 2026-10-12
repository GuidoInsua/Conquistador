using UnityEngine;

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
