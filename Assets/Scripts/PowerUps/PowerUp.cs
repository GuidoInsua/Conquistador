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
// Desc: Power-up que da un boost de velocidad al jugador cuando lo recoge.

using UnityEngine;

/// <summary>
/// PowerUp que aumenta la velocidad del jugador cuando colisiona con él
/// Aparece cuando un enemigo muere (la probabilidad la define EnemigoAlien)
/// </summary>
public class PowerUp : MonoBehaviour
{
    [SerializeField] private float speedBoostPercentage = 0.2f; // 20% de aumento
    [SerializeField] private float boostDuration = 10f; // Duración en segundos
    [SerializeField] private ParticleSystem collectEffect; // Partículas opcionales al ser recogido

    private bool isCollected = false;
    private Collider2D powerUpCollider;

    void Start()
    {
        powerUpCollider = GetComponent<Collider2D>();

        if (powerUpCollider == null)
        {
            Debug.LogWarning("PowerUp no tiene Collider2D. No será interactivo.");
        }

        if (!CompareTag("PowerUp"))
        {
            Debug.LogWarning("PowerUp no tiene la etiqueta 'PowerUp'. Revisá el prefab.");
        }
    }

    void Update()
    {
        // Destruir el PowerUp si sale de la pantalla
        if (Pantalla.EstaFuera(transform.position))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Detecta colisión con el jugador
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected)
            return;

        // Verificar si el objeto que colisiona es el jugador
        if (collision.CompareTag("Player"))
        {
            ApplyBoost(collision.gameObject);
        }
    }

    /// <summary>
    /// Aplica el boost de velocidad al jugador
    /// </summary>
    private void ApplyBoost(GameObject player)
    {
        isCollected = true;

        // Obtener el script de movimiento del jugador
        MovimientoJugador playerMovement = player.GetComponent<MovimientoJugador>();

        if (playerMovement != null)
        {
            // Aplicar el boost de velocidad
            playerMovement.ApplySpeedBoost(speedBoostPercentage, boostDuration);
        }
        else
        {
            Debug.LogWarning("No se encontró MovimientoJugador en el jugador.");
        }

        // Reproducir efecto de partículas si existe
        if (collectEffect != null)
        {
            ParticleSystem effect = Instantiate(collectEffect, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, 2f); // Destruir después de 2 segundos
        }

        // Destruir el PowerUp
        Destroy(gameObject);
    }

    /// <summary>
    /// Spawea un PowerUp en la posición especificada con probabilidad
    /// </summary>
    /// <param name="powerUpPrefab">Prefab del PowerUp a spawnear</param>
    /// <param name="position">Posición donde spawnear</param>
    /// <param name="probability">Probabilidad (0-1) de que aparezca. Default 0.5 = 50%</param>
    public static void SpawnPowerUp(GameObject powerUpPrefab, Vector3 position, float probability = 0.5f)
    {
        // Verificar probabilidad
        if (Random.value > probability)
            return; // No spawnear

        if (powerUpPrefab == null)
        {
            Debug.LogWarning("PowerUp prefab es null. No se puede spawnear PowerUp.");
            return;
        }

        // Instanciar el PowerUp en la posición
        Instantiate(powerUpPrefab, position, Quaternion.identity);
    }

}
