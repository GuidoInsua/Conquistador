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

public static class SorteoPonderado
{
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
