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
// Desc: Abanico de balas que lanza una pinza del boss mientras sigue viva.

using UnityEngine;

public class DisparoPinza : MonoBehaviour
{
    [SerializeField] private GameObject balaPrefab;
    [Tooltip("De dónde salen las balas. Las balas bajan según su rotación.")]
    [SerializeField] private Transform puntaDisparo;
    [SerializeField] private int cantidadBalas = 5;
    [Tooltip("Grados entre la primera y la última bala.")]
    [SerializeField] private float anguloCono = 70f;

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

    // La llama el boss. Una pinza destruida ya no dispara.
    public void Disparar()
    {
        if (!enabled || !pinza.EstaViva)
            return;

        BalaEnemiga.DispararAbanico(balaPrefab, puntaDisparo.position, puntaDisparo.rotation, cantidadBalas, anguloCono);
    }
}
