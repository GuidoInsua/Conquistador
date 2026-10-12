using UnityEngine;

/// <summary>
/// PowerUp que aumenta la velocidad del jugador cuando colisiona con él
/// Aparece cuando un enemigo muere (50% de probabilidad)
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

        // Asegurarse de que tiene la etiqueta correcta
        gameObject.tag = "PowerUp";
    }

    void Update()
    {
        // Destruir el PowerUp si sale de la pantalla
        if (SalidoDePatalla())
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

            Debug.Log($"¡PowerUp recogido! Velocidad incrementada en {speedBoostPercentage * 100}% durante {boostDuration} segundos.");
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

        Debug.Log($"PowerUp spawneado en posición {position}");
    }

    private bool SalidoDePatalla()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
            return false;

        // Convertir posición de mundo a posición de pantalla
        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);

        // Si está fuera del rango de pantalla (con pequeño buffer)
        return screenPos.x < -50 || screenPos.x > Screen.width + 50 ||
               screenPos.y < -50 || screenPos.y > Screen.height + 50;
    }
}
