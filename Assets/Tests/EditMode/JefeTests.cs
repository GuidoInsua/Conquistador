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
// Desc: Pruebas de la vida de las partes del boss y de cuándo muere.

using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class JefeTests
{
    const BindingFlags privado = BindingFlags.NonPublic | BindingFlags.Instance;

    GameObject jefeGo;
    JefeCangrejo jefe;
    ParteBoss cuerpo;
    ParteBoss pinza;
    bool murio;

    [SetUp]
    public void SetUp()
    {
        jefeGo = new GameObject("jefe");
        cuerpo = CrearParte("cuerpo", 10f);
        pinza = CrearParte("pinza", 3f);

        jefe = jefeGo.AddComponent<JefeCangrejo>();
        typeof(JefeCangrejo).GetField("cuerpo", privado).SetValue(jefe, cuerpo);
        typeof(JefeCangrejo).GetMethod("Awake", privado).Invoke(jefe, null);

        murio = false;
        System.Action alMorir = () => murio = true;
        typeof(JefeCangrejo).GetField("alMorir", privado).SetValue(jefe, alMorir);
    }

    [TearDown]
    public void TearDown() => Object.DestroyImmediate(jefeGo);

    ParteBoss CrearParte(string nombre, float vida)
    {
        var go = new GameObject(nombre, typeof(SpriteRenderer));
        go.transform.SetParent(jefeGo.transform);
        var parte = go.AddComponent<ParteBoss>();
        typeof(ParteBoss).GetField("vidaMaxima", privado).SetValue(parte, vida);
        typeof(ParteBoss).GetMethod("Awake", privado).Invoke(parte, null);
        return parte;
    }

    [Test]
    public void Parte_pierde_vida_y_se_desactiva_al_llegar_a_cero()
    {
        pinza.TakeDamage(2f);
        Assert.AreEqual(1f, pinza.GetSalud());
        Assert.IsTrue(pinza.gameObject.activeSelf);

        pinza.TakeDamage(5f);
        Assert.AreEqual(0f, pinza.GetSalud());
        Assert.IsFalse(pinza.EstaViva);
        Assert.IsFalse(pinza.gameObject.activeSelf);
    }

    [Test]
    public void Jefe_sigue_vivo_si_se_destruye_una_pinza()
    {
        pinza.TakeDamage(3f);

        Assert.IsFalse(pinza.gameObject.activeSelf);
        Assert.IsTrue(jefeGo.activeSelf);
        Assert.IsFalse(murio);
    }

    [Test]
    public void Jefe_muere_y_avisa_una_sola_vez_al_destruir_el_cuerpo()
    {
        int avisos = 0;
        System.Action alMorir = () => avisos++;
        typeof(JefeCangrejo).GetField("alMorir", privado).SetValue(jefe, alMorir);

        cuerpo.TakeDamage(9f);
        Assert.IsTrue(jefeGo.activeSelf);

        cuerpo.TakeDamage(5f);
        cuerpo.TakeDamage(5f);
        Assert.IsFalse(jefeGo.activeSelf);
        Assert.AreEqual(1, avisos);
    }
}
