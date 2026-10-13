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
// Desc: Elige un índice al azar según el peso de cada opción (power-ups, tipos de enemigo).

using System;

public static class SorteoPonderado
{
    /// <summary>
    /// Elige una opción según su peso. Las opciones null o no válidas cuentan como peso 0.
    /// Devuelve null si no queda ninguna.
    /// </summary>
    public static T Elegir<T>(T[] opciones, Func<T, bool> esValida, Func<T, float> peso, float valor) where T : class
    {
        if (opciones == null)
            return null;

        float[] pesos = new float[opciones.Length];
        for (int i = 0; i < opciones.Length; i++)
            pesos[i] = opciones[i] != null && esValida(opciones[i]) ? peso(opciones[i]) : 0f;

        int indice = ElegirIndice(pesos, valor);
        return indice < 0 ? null : opciones[indice];
    }

    /// <summary>
    /// Devuelve el índice elegido según los pesos. valor va de 0 a 1 (0 = primera opción, 1 = última).
    /// Ignora los pesos 0 o negativos. Devuelve -1 si no queda ninguno.
    /// </summary>
    public static int ElegirIndice(float[] pesos, float valor)
    {
        if (pesos == null)
            return -1;

        float total = 0f;
        foreach (float peso in pesos)
        {
            if (peso > 0f)
                total += peso;
        }

        float objetivo = UnityEngine.Mathf.Clamp01(valor) * total;
        float acumulado = 0f;
        int ultimo = -1;

        for (int i = 0; i < pesos.Length; i++)
        {
            if (pesos[i] <= 0f)
                continue;

            acumulado += pesos[i];
            ultimo = i;

            if (objetivo < acumulado)
                return i;
        }

        return ultimo; // valor = 1 cae acá
    }
}
