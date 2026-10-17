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
// Desc: Saca esbirros del boss en tandas cada cierto tiempo, mientras pelea.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class InvocadorEsbirros : MonoBehaviour
{
    [Tooltip("Prefab con EnemigoAlien y Esbirro.")]
    [SerializeField] private EnemigoAlien esbirroPrefab;
    [Tooltip("Salen desde su centro. Al destruirse mueren todos.")]
    [SerializeField] private ParteBoss cuerpo;
    [SerializeField] private float tiempoEntreTandas = 5f;
    [Tooltip("Uno por costado, alternando.")]
    [SerializeField] private int cantidadPorTanda = 2;
    [Tooltip("Tiempo entre esbirro y esbirro dentro de una tanda.")]
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

        cuerpo.Destruida += MatarTodos;
    }

    // La llama el boss cuando llega a su lugar de pelea.
    public void Comenzar()
    {
        if (enabled)
            StartCoroutine(Tandas());
    }

    private IEnumerator Tandas()
    {
        while (true)
        {
            yield return new WaitForSeconds(tiempoEntreTandas);
            yield return Lanzar();
        }
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
        for (int i = 0; i < cantidadPorTanda; i++)
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
            esbirro.Expirar();
    }
}
