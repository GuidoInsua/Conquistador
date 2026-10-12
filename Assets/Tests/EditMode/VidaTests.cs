// ##########################################################################
// ## o ########## o ##                                                     #
// ### \ ######## / ###                                                     #
// #### \ ###### / ####       CONQUISTADOR                                  #
// ##### :-'""'-: #####       ------------                                  #
// ## .-'  ____  `-. ##       Autor:        Guido Insua                     #
// # ( (  (_()_)  ) ) #       Materia:      Programación 1                  #
// ## `-.   ^^   .-' ##       Cuatrimestre: Segundo cuatrimestre 2026       #
// ##### `._==_.' #####                                                     #
// ###### __)(___ #####                                                     #
// ##########################################################################

using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

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
    public void Jugador_vida_queda_entre_cero_y_maxima()
    {
        // Inactivo para que Awake no corra hasta tener el texto asignado.
        go = new GameObject("jugador");
        go.SetActive(false);
        var vida = go.AddComponent<VidaJugador>();
        var texto = go.AddComponent<TextMeshProUGUI>();
        typeof(VidaJugador).GetField("textoVida", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(vida, texto);
        typeof(VidaJugador).GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(vida, null);

        vida.actualizarVida(-3);
        Assert.AreEqual("HP 7", texto.text);

        vida.actualizarVida(100);
        Assert.AreEqual("HP 10", texto.text);
    }
}
