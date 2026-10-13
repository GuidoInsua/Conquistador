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
// Desc: Pruebas de la vida del enemigo y del jugador.

using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class VidaTests
{
    GameObject go;

    [SetUp]
    public void SetUp() => LogAssert.ignoreFailingMessages = true; // CrossFade sin Animator Controller solo avisa

    [TearDown]
    public void TearDown() => Object.DestroyImmediate(go);

    [Test]
    public void Enemigo_muere_a_cero_y_no_baja_de_cero()
    {
        go = new GameObject("alien", typeof(Animator), typeof(EnemigoAlien));
        var alien = go.GetComponent<EnemigoAlien>();
        alien.ResetEnemy();

        alien.TakeDamage(10f);
        alien.TakeDamage(10f);

        Assert.AreEqual(0f, alien.GetSalud());
    }

    [Test]
    public void Jugador_vida_queda_entre_cero_y_maxima_y_la_barra_la_refleja()
    {
        // Inactivo para que Awake no corra hasta tener la barra asignada.
        go = new GameObject("jugador");
        go.SetActive(false);
        var vida = go.AddComponent<VidaJugador>();
        var barra = go.AddComponent<BarraVida>();

        var fichas = new Image[5];
        for (int i = 0; i < fichas.Length; i++)
        {
            var ficha = new GameObject("ficha" + i, typeof(Image));
            ficha.transform.SetParent(go.transform);
            fichas[i] = ficha.GetComponent<Image>();
        }

        var privado = BindingFlags.NonPublic | BindingFlags.Instance;
        typeof(BarraVida).GetField("fichas", privado).SetValue(barra, fichas);
        typeof(VidaJugador).GetField("barraVida", privado).SetValue(vida, barra);
        typeof(VidaJugador).GetMethod("Awake", privado).Invoke(vida, null);

        Assert.AreEqual(5, vida.VidaActual);
        Assert.AreEqual(5, fichas.Count(f => f.enabled));

        vida.actualizarVida(-3);
        Assert.AreEqual(2, vida.VidaActual);
        Assert.AreEqual(2, fichas.Count(f => f.enabled));

        vida.actualizarVida(100);
        Assert.AreEqual(5, vida.VidaActual);
        Assert.AreEqual(5, fichas.Count(f => f.enabled));
    }
}
