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
// Desc: Prueba que Potenciar ensancha la bala y le sube el daño.

using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class BalaTests
{
    GameObject go;

    [TearDown]
    public void TearDown() => Object.DestroyImmediate(go);

    [Test]
    public void Potenciar_multiplica_el_ancho_y_el_dano()
    {
        go = new GameObject("bala", typeof(Bala));
        var bala = go.GetComponent<Bala>();
        var campoDano = typeof(Bala).GetField("dano", BindingFlags.NonPublic | BindingFlags.Instance);
        float danoInicial = (float)campoDano.GetValue(bala);

        bala.Potenciar(1.5f, 2f);

        Assert.AreEqual(danoInicial * 1.5f, (float)campoDano.GetValue(bala));
        Assert.AreEqual(2f, go.transform.localScale.x);
        Assert.AreEqual(1f, go.transform.localScale.y);
    }
}
