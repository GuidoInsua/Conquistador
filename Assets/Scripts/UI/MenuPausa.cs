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
// Desc: Pausa con Escape: congela el juego y muestra continuar, opciones, menú principal y salir.

using UnityEngine;
using UnityEngine.InputSystem;

public class MenuPausa : MonoBehaviour
{
    [Tooltip("Empieza apagado; se prende al pausar.")]
    [SerializeField] private GameObject canvasPausa;
    [Tooltip("Los botones de la pausa, para volver a ellos desde opciones.")]
    [SerializeField] private GameObject botones;
    [SerializeField] private GameObject panelOpciones;
    [Tooltip("Si alguna está a la vista (ganaste o perdiste), Escape no pausa.")]
    [SerializeField] private GameObject[] pantallasFinales = new GameObject[0];

    private bool isPausado;

    private void Awake()
    {
        if (canvasPausa == null || botones == null || panelOpciones == null)
        {
            Debug.LogError("MenuPausa: falta el canvas de pausa, los botones o el panel de opciones.");
            enabled = false;
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            AlternarPausa();
    }

    private void AlternarPausa()
    {
        if (isPausado)
        {
            Continuar();
            return;
        }

        foreach (GameObject pantalla in pantallasFinales)
        {
            if (pantalla != null && pantalla.activeSelf)
                return;
        }

        isPausado = true;
        canvasPausa.SetActive(true);
        botones.SetActive(true);
        panelOpciones.SetActive(false);
        Time.timeScale = 0f;
    }

    // También la llama el botón Continuar.
    public void Continuar()
    {
        isPausado = false;
        canvasPausa.SetActive(false);
        Time.timeScale = 1f;
    }
}
