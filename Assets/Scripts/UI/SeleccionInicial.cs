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
// Desc: Al mostrarse un menú deja marcada su primera opción, para manejarlo con flechas y Enter.

using UnityEngine;
using UnityEngine.EventSystems;

public class SeleccionInicial : MonoBehaviour
{
    [Tooltip("La opción que queda marcada al abrir este menú.")]
    [SerializeField] private GameObject primeraOpcion;

    private void Awake()
    {
        if (primeraOpcion == null)
        {
            Debug.LogError("SeleccionInicial: falta asignar la primera opción.");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        Seleccionar();
    }

    // Al cargar la escena el EventSystem puede prenderse después que este menú.
    private void Start()
    {
        Seleccionar();
    }

    // Si un clic con el mouse dejó todo sin marcar, las flechas vuelven a funcionar.
    private void Update()
    {
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
        {
            Seleccionar();
        }
    }

    private void Seleccionar()
    {
        if (enabled && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(primeraOpcion);
        }
    }
}
