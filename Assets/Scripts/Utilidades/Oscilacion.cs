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
// Desc: Desvío suave al azar para que los enemigos no se queden quietos.

using UnityEngine;

public static class Oscilacion
{
    // Perlin da un desvío al azar pero suave. Se le resta el valor inicial para que arranque en 0 y no salte.
    // Cada semilla da un recorrido distinto; el resultado ronda entre -0.5 y 0.5 en cada eje.
    public static Vector2 Desvio(float semilla, float tiempo)
    {
        // PerlinNoise devuelve un número entre 0 y 1 que cambia suave a medida que avanza el tiempo.
        // Para x e y se lee en dos lugares distintos del ruido, así no se mueven igual.
        float x = Mathf.PerlinNoise(semilla, tiempo) - Mathf.PerlinNoise(semilla, 0f);
        float y = Mathf.PerlinNoise(tiempo, semilla) - Mathf.PerlinNoise(0f, semilla);
        return new Vector2(x, y);
    }
}
