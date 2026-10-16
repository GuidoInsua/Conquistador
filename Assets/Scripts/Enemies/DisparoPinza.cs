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
// Desc: Abanico de balas que lanza una pinza del boss de a una, con una bola en la punta que marca de dónde salen.

using System.Collections;
using UnityEngine;

public class DisparoPinza : MonoBehaviour
{
    [SerializeField] private GameObject balaPrefab;
    [Tooltip("De dónde salen las balas. Las balas bajan según su rotación.")]
    [SerializeField] private Transform puntaDisparo;
    [SerializeField] private int cantidadBalas = 5;
    [Tooltip("Grados entre la primera y la última bala.")]
    [SerializeField] private float anguloCono = 70f;
    [Tooltip("Tiempo entre bala y bala.")]
    [SerializeField] private float intervaloBalas = 0.15f;

    [Header("Aviso")]
    [Tooltip("Se prende en la punta mientras dispara. Si queda vacío, no se muestra nada.")]
    [SerializeField] private GameObject bola;
    [Tooltip("Cuánto se ve la bola antes de la primera bala.")]
    [SerializeField] private float duracionAviso = 0.3f;

    private ParteBoss pinza;

    private void Awake()
    {
        pinza = GetComponent<ParteBoss>();

        if (balaPrefab == null || puntaDisparo == null || pinza == null)
        {
            Debug.LogError("DisparoPinza: falta la bala, la punta de disparo o el ParteBoss de la pinza.");
            enabled = false;
        }
    }

    // La corre el boss. Una pinza destruida ya no dispara, ni aunque se rompa a mitad del abanico.
    public IEnumerator Disparar()
    {
        if (!enabled || !pinza.EstaViva)
            yield break;

        if (bola != null)
            bola.SetActive(true);

        yield return new WaitForSeconds(duracionAviso);

        for (int i = 0; i < cantidadBalas && pinza.EstaViva; i++)
        {
            float angulo = BalaEnemiga.AnguloAbanico(i, cantidadBalas, anguloCono);
            Instantiate(balaPrefab, puntaDisparo.position, puntaDisparo.rotation * Quaternion.Euler(0f, 0f, angulo));
            yield return new WaitForSeconds(intervaloBalas);
        }

        if (bola != null)
            bola.SetActive(false);
    }
}
