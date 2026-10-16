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
// Desc: Saca esbirros del boss, de a varios, cada vez que el cuerpo pasa a otro estado de daño.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class InvocadorEsbirros : MonoBehaviour
{
    [Tooltip("Prefab con EnemigoAlien y Esbirro.")]
    [SerializeField] private EnemigoAlien esbirroPrefab;
    [Tooltip("Cada vez que cambia de estado de daño salen esbirros; al destruirse mueren todos.")]
    [SerializeField] private ParteBoss cuerpo;
    [SerializeField] private int cantidadPorFase = 6;
    [SerializeField] private float intervalo = 0.5f;

    private JefeCangrejo jefe;
    private IObjectPool<EnemigoAlien> pool;
    private readonly List<EnemigoAlien> activos = new List<EnemigoAlien>();

    private void Awake()
    {
        jefe = GetComponent<JefeCangrejo>();

        if (esbirroPrefab == null || esbirroPrefab.GetComponent<Esbirro>() == null || cuerpo == null || jefe == null)
        {
            Debug.LogError("InvocadorEsbirros: falta el prefab (con Esbirro), el cuerpo o el JefeCangrejo.");
            enabled = false;
            return;
        }

        pool = new ObjectPool<EnemigoAlien>(
            CrearEsbirro,
            esbirro =>
            {
                esbirro.gameObject.SetActive(true);
                esbirro.ResetEnemy();
                activos.Add(esbirro);
            },
            esbirro =>
            {
                esbirro.gameObject.SetActive(false);
                activos.Remove(esbirro);
            },
            esbirro =>
            {
                if (esbirro != null)
                    Destroy(esbirro.gameObject);
            });

        cuerpo.CambioEstado += _ => StartCoroutine(Lanzar());
        cuerpo.Destruida += MatarTodos;
    }

    // Sin padre: si fueran hijos del boss se moverían con él y se apagarían al morir.
    private EnemigoAlien CrearEsbirro()
    {
        EnemigoAlien esbirro = Instantiate(esbirroPrefab);
        esbirro.SetPool(pool);
        return esbirro;
    }

    private IEnumerator Lanzar()
    {
        for (int i = 0; i < cantidadPorFase; i++)
        {
            EnemigoAlien esbirro = pool.Get();
            esbirro.GetComponent<Esbirro>().Iniciar(cuerpo.transform.position, i % 2 == 0 ? -1f : 1f, jefe.Jugador);
            yield return new WaitForSeconds(intervalo);
        }
    }

    private void MatarTodos()
    {
        StopAllCoroutines();

        // Copia: al terminar la animación de muerte cada uno se saca de la lista.
        foreach (EnemigoAlien esbirro in activos.ToArray())
            esbirro.TakeDamage(float.MaxValue);
    }
}
