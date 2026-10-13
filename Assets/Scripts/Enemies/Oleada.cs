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
// Desc: Una oleada del Spawner: cuántos enemigos salen y qué tipos (con su peso) se mezclan.

using UnityEngine;

[System.Serializable]
public class Oleada
{
    [SerializeField] private int cantidad = 10;
    [SerializeField] private OpcionEnemigo[] enemigos;

    public int Cantidad => cantidad;
    public OpcionEnemigo[] Enemigos => enemigos;

    public Oleada(int cantidad, OpcionEnemigo[] enemigos)
    {
        this.cantidad = cantidad;
        this.enemigos = enemigos;
    }

    /// <summary>
    /// Elige el prefab de un enemigo según el peso de cada opción. valor va de 0 a 1.
    /// Devuelve null si ninguna opción es válida.
    /// </summary>
    public EnemigoAlien ElegirPrefab(float valor)
    {
        if (enemigos == null)
            return null;

        float[] pesos = new float[enemigos.Length];
        for (int i = 0; i < enemigos.Length; i++)
        {
            bool valida = enemigos[i] != null && enemigos[i].Prefab != null;
            pesos[i] = valida ? enemigos[i].Peso : 0f;
        }

        int indice = SorteoPonderado.ElegirIndice(pesos, valor);
        return indice < 0 ? null : enemigos[indice].Prefab;
    }
}
