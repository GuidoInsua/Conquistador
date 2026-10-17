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
// Desc: Volumen de la música y de los efectos que elige el jugador; se guarda entre partidas.

using UnityEngine;

public static class VolumenAudio
{
    private const string claveMusica = "VolumenMusica";
    private const string claveEfectos = "VolumenEfectos";

    // Avisa a la música que ya está sonando para que cambie al momento.
    public static event System.Action Cambio;

    public static float Musica
    {
        get => PlayerPrefs.GetFloat(claveMusica, 1f);
        set => Guardar(claveMusica, value);
    }

    public static float Efectos
    {
        get => PlayerPrefs.GetFloat(claveEfectos, 1f);
        set => Guardar(claveEfectos, value);
    }

    private static void Guardar(string clave, float valor)
    {
        PlayerPrefs.SetFloat(clave, Mathf.Clamp01(valor));
        Cambio?.Invoke();
    }
}
