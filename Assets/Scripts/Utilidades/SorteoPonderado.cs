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

using UnityEngine;

public static class SorteoPonderado
{
    /// <summary>
    /// Devuelve el índice elegido según los pesos. valor va de 0 a 1 (0 = primera opción, 1 = última).
    /// Ignora los pesos 0 o negativos. Devuelve -1 si no queda ninguno.
    /// </summary>
    public static int ElegirIndice(float[] pesos, float valor)
    {
        if (pesos == null)
        {
            return -1;
        }

        // Suma de todos los pesos válidos.
        float total = 0f;
        foreach (float peso in pesos)
        {
            if (peso > 0f)
            {
                total += peso;
            }
        }

        // Punto elegido dentro de esa suma. Ejemplo: con pesos 60 y 40, un valor de 0.7 da 70,
        // que cae después de los primeros 60, o sea en la segunda opción.
        float objetivo = Mathf.Clamp01(valor) * total;
        float acumulado = 0f;
        int ultimo = -1;

        for (int i = 0; i < pesos.Length; i++)
        {
            if (pesos[i] <= 0f)
            {
                continue;
            }

            acumulado += pesos[i];
            ultimo = i;

            if (objetivo < acumulado)
            {
                return i;
            }
        }

        // Con valor = 1 el objetivo es igual al total y nunca es menor: queda el último válido.
        return ultimo;
    }
}
