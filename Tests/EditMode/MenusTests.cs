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
// Desc: Pruebas del volumen guardado, de la música que lo sigue y de la pausa con Escape.

using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class MenusTests
{
    const BindingFlags privado = BindingFlags.NonPublic | BindingFlags.Instance;

    GameObject raiz;
    float musicaAntes;
    float efectosAntes;

    [SetUp]
    public void SetUp()
    {
        raiz = new GameObject("raiz");
        musicaAntes = VolumenAudio.Musica;
        efectosAntes = VolumenAudio.Efectos;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(raiz);
        VolumenAudio.Musica = musicaAntes;
        VolumenAudio.Efectos = efectosAntes;
        Time.timeScale = 1f;
    }

    [Test]
    public void Volumen_queda_entre_cero_y_uno()
    {
        VolumenAudio.Musica = 2f;
        VolumenAudio.Efectos = -1f;

        Assert.AreEqual(1f, VolumenAudio.Musica);
        Assert.AreEqual(0f, VolumenAudio.Efectos);
    }

    [Test]
    public void Musica_escala_su_volumen_propio_al_cambiar_la_opcion()
    {
        var go = new GameObject("musica", typeof(AudioSource));
        go.transform.SetParent(raiz.transform);
        go.GetComponent<AudioSource>().volume = 0.4f;
        var musica = go.AddComponent<VolumenMusica>();
        typeof(VolumenMusica).GetMethod("Awake", privado).Invoke(musica, null);
        typeof(VolumenMusica).GetMethod("OnEnable", privado).Invoke(musica, null);

        VolumenAudio.Musica = 0.5f;
        Assert.AreEqual(0.2f, go.GetComponent<AudioSource>().volume, 0.0001f);

        typeof(VolumenMusica).GetMethod("OnDisable", privado).Invoke(musica, null);
    }

    [Test]
    public void Escape_pausa_y_continua_pero_no_con_una_pantalla_final()
    {
        GameObject Hijo(string nombre, bool activo)
        {
            var hijo = new GameObject(nombre);
            hijo.transform.SetParent(raiz.transform);
            hijo.SetActive(activo);
            return hijo;
        }
        var canvas = Hijo("pausa", false);
        var victoria = Hijo("victoria", false);
        var pausa = raiz.AddComponent<MenuPausa>();
        typeof(MenuPausa).GetField("canvasPausa", privado).SetValue(pausa, canvas);
        typeof(MenuPausa).GetField("botones", privado).SetValue(pausa, Hijo("botones", false));
        typeof(MenuPausa).GetField("panelOpciones", privado).SetValue(pausa, Hijo("opciones", true));
        typeof(MenuPausa).GetField("pantallasFinales", privado).SetValue(pausa, new[] { victoria });
        var alternar = typeof(MenuPausa).GetMethod("AlternarPausa", privado);

        alternar.Invoke(pausa, null);
        Assert.IsTrue(canvas.activeSelf);
        Assert.AreEqual(0f, Time.timeScale);

        alternar.Invoke(pausa, null);
        Assert.IsFalse(canvas.activeSelf);
        Assert.AreEqual(1f, Time.timeScale);

        victoria.SetActive(true);
        alternar.Invoke(pausa, null);
        Assert.IsFalse(canvas.activeSelf, "con la victoria a la vista no pausa");
        Assert.AreEqual(1f, Time.timeScale);
    }
}
