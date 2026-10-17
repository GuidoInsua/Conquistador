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
// Desc: Pruebas del límite que evita que el mismo efecto de sonido se apile.

using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class EfectosSonidoTests
{
    GameObject go;

    [TearDown]
    public void TearDown() => Object.DestroyImmediate(go);

    [Test]
    public void El_mismo_clip_no_se_repite_antes_de_la_separacion_minima()
    {
        go = new GameObject("sonidos", typeof(AudioSource), typeof(EfectosSonido));
        var efectos = go.GetComponent<EfectosSonido>();
        var golpe = AudioClip.Create("golpe", 100, 1, 44100, false);
        var disparo = AudioClip.Create("disparo", 100, 1, 44100, false);
        var anotar = typeof(EfectosSonido).GetMethod("Anotar", BindingFlags.NonPublic | BindingFlags.Instance);
        bool Anotar(AudioClip clip, float ahora) => (bool)anotar.Invoke(efectos, new object[] { clip, ahora });

        // separacionMinima por defecto: 0.05
        Assert.IsTrue(Anotar(golpe, 10f));
        Assert.IsFalse(Anotar(golpe, 10.01f));
        Assert.IsTrue(Anotar(disparo, 10.01f), "Otro clip no espera al primero.");
        Assert.IsTrue(Anotar(golpe, 10.06f));
    }
}
