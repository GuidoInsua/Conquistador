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
    public void Enemigo_con_vida_fraccionaria_sigue_vivo()
    {
        go = new GameObject("alien", typeof(Animator), typeof(EnemigoAlien));
        var alien = go.GetComponent<EnemigoAlien>();
        alien.ResetEnemy(); // vida 4

        alien.TakeDamage(3.5f);

        var animacion = typeof(EnemigoAlien).GetField("animacionActual", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.AreEqual(0.5f, alien.GetSalud());
        Assert.AreNotEqual(EnemigoAlien.animMuerte, animacion.GetValue(alien));
    }

    VidaJugador CrearJugador(out Image[] fichas)
    {
        // Inactivo para que Awake no corra hasta tener la barra asignada.
        go = new GameObject("jugador", typeof(SpriteRenderer));
        go.SetActive(false);
        var vida = go.AddComponent<VidaJugador>();
        var barra = go.AddComponent<BarraVida>();

        fichas = new Image[5];
        for (int i = 0; i < fichas.Length; i++)
        {
            var ficha = new GameObject("ficha" + i, typeof(Image));
            ficha.transform.SetParent(go.transform);
            fichas[i] = ficha.GetComponent<Image>();
        }

        var privado = BindingFlags.NonPublic | BindingFlags.Instance;
        typeof(BarraVida).GetField("fichas", privado).SetValue(barra, fichas);
        typeof(VidaJugador).GetField("barraVida", privado).SetValue(vida, barra);
        typeof(VidaJugador).GetField("materialDestello", privado).SetValue(vida, new Material(Shader.Find("GUI/Text Shader")));
        typeof(VidaJugador).GetMethod("Awake", privado).Invoke(vida, null);
        return vida;
    }

    void TerminarInvulnerabilidad(VidaJugador vida) =>
        typeof(VidaJugador).GetField("finInvulnerable", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(vida, float.MinValue);

    [Test]
    public void Jugador_vida_queda_entre_cero_y_maxima_y_la_barra_la_refleja()
    {
        var vida = CrearJugador(out var fichas);

        Assert.AreEqual(5, vida.VidaActual);
        Assert.AreEqual(5, fichas.Count(f => f.enabled));

        vida.actualizarVida(-3);
        Assert.AreEqual(2, vida.VidaActual);
        Assert.AreEqual(2, fichas.Count(f => f.enabled));

        vida.actualizarVida(100);
        Assert.AreEqual(5, vida.VidaActual);
        Assert.AreEqual(5, fichas.Count(f => f.enabled));
    }

    [Test]
    public void Jugador_no_recibe_dano_mientras_titila_pero_si_se_cura()
    {
        var vida = CrearJugador(out _);

        vida.actualizarVida(-1);
        Assert.IsTrue(vida.EstaInvulnerable);

        vida.actualizarVida(-1);
        Assert.AreEqual(4, vida.VidaActual, "el segundo golpe no cuenta");

        vida.actualizarVida(1);
        Assert.AreEqual(5, vida.VidaActual, "curarse sí funciona");

        TerminarInvulnerabilidad(vida);
        vida.actualizarVida(-1);
        Assert.AreEqual(4, vida.VidaActual);
    }
}
