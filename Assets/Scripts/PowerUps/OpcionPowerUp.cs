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
// Desc: Un power-up posible al morir un alien, con el peso (chance relativa) de que sea el elegido.

using UnityEngine;

[System.Serializable]
public class OpcionPowerUp
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float peso = 1f; // Chance relativa: 50 y 50 es mitad y mitad, 1 y 3 es 25% y 75%

    public GameObject Prefab
    {
        get { return prefab; }
    }

    public float Peso
    {
        get { return peso; }
    }

    public OpcionPowerUp(GameObject prefab, float peso)
    {
        this.prefab = prefab;
        this.peso = peso;
    }
}
