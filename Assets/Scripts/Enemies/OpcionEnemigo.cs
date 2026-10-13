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
// Desc: Un tipo de enemigo posible en una oleada, con el peso (chance relativa) de que salga.

using UnityEngine;

[System.Serializable]
public class OpcionEnemigo
{
    [SerializeField] private EnemigoAlien prefab;
    [SerializeField] private float peso = 1f; // Chance relativa: 60 y 40 es 60% y 40%

    public EnemigoAlien Prefab => prefab;
    public float Peso => peso;

    public OpcionEnemigo(EnemigoAlien prefab, float peso)
    {
        this.prefab = prefab;
        this.peso = peso;
    }
}
