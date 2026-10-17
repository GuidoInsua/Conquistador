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
// Desc: Base de los power-ups: se recoge al tocar al jugador y cada tipo define su efecto.

using UnityEngine;

/// <summary>
/// Base de todos los power-ups. Cada tipo hereda y define qué le hace al jugador en Aplicar.
/// Aparecen cuando un enemigo muere (la probabilidad y las opciones las define EnemigoAlien).
/// </summary>
public abstract class PowerUp : MonoBehaviour
{
    [SerializeField] private ParticleSystem collectEffect; // Partículas opcionales al ser recogido

    private bool isCollected = false;

    void Start()
    {
        if (GetComponent<Collider2D>() == null)
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            Recoger(collision.gameObject);
        }
    }

    /// <summary>
    /// Efecto del power-up sobre el jugador que lo recogió.
    /// </summary>
    protected abstract void Aplicar(GameObject jugador);

    private void Recoger(GameObject jugador)
    {
        isCollected = true;

        Aplicar(jugador);

        // Reproducir efecto de partículas si existe
        if (collectEffect != null)
        {
            ParticleSystem effect = Instantiate(collectEffect, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, 2f); // Destruir después de 2 segundos
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Con cierta probabilidad spawnea uno de los power-ups posibles, elegido según su peso.
    /// </summary>
    /// <param name="opciones">Power-ups posibles y el peso de cada uno</param>
    /// <param name="position">Posición donde spawnear</param>
    /// <param name="probability">Probabilidad (0-1) de que aparezca alguno</param>
    public static void SpawnPowerUp(OpcionPowerUp[] opciones, Vector3 position, float probability = 0.5f)
    {
        if (Random.value > probability)
        {
            return;
        }

        GameObject powerUpPrefab = ElegirPrefab(opciones, Random.value);

        if (powerUpPrefab == null)
        {
            Debug.LogWarning("No hay power-ups válidos para spawnear. Revisá las opciones del enemigo.");
            return;
        }

        Instantiate(powerUpPrefab, position, Quaternion.identity);
    }

    /// <summary>
    /// Elige un prefab según el peso de cada opción. valor va de 0 a 1 (0 = primera opción, 1 = última).
    /// Ignora las opciones sin prefab o con peso 0. Devuelve null si no queda ninguna.
    /// </summary>
    public static GameObject ElegirPrefab(OpcionPowerUp[] opciones, float valor)
    {
        if (opciones == null)
        {
            return null;
        }

        // Las opciones vacías o sin prefab cuentan con peso 0, así nunca salen.
        float[] pesos = new float[opciones.Length];
        for (int i = 0; i < opciones.Length; i++)
        {
            if (opciones[i] != null && opciones[i].Prefab != null)
            {
                pesos[i] = opciones[i].Peso;
            }
            else
            {
                pesos[i] = 0f;
            }
        }

        int indice = SorteoPonderado.ElegirIndice(pesos, valor);
        if (indice < 0)
        {
            return null;
        }

        return opciones[indice].Prefab;
    }
}
