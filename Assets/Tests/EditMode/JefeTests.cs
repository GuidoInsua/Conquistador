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
// Desc: Pruebas de la vida y el daño visible de las partes del boss, de cuándo muere y del daño del rayo.

using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

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
        var go = new GameObject(nombre, typeof(SpriteRenderer), typeof(Animator), typeof(BoxCollider2D));
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
    public void Parte_con_estados_de_dano_los_recorre_y_queda_a_la_vista_al_destruirse()
    {
        var parteGo = new GameObject("pinzaConDano", typeof(SpriteRenderer), typeof(Animator), typeof(BoxCollider2D));
        parteGo.transform.SetParent(jefeGo.transform);
        var parte = parteGo.AddComponent<ParteBoss>();
        typeof(ParteBoss).GetField("vidaMaxima", privado).SetValue(parte, 9f);
        typeof(ParteBoss).GetField("estadosDano", privado).SetValue(parte, new[] { "Idle", "Dano1", "Dano2" });
        typeof(ParteBoss).GetField("estadoDestruida", privado).SetValue(parte, "Destruida");
        typeof(ParteBoss).GetMethod("Awake", privado).Invoke(parte, null);
        int Estado() => (int)typeof(ParteBoss).GetField("estadoActual", privado).GetValue(parte);

        parte.TakeDamage(2f);
        Assert.AreEqual(0, Estado(), "con 7 de 9 sigue sana");
        parte.TakeDamage(2f);
        Assert.AreEqual(1, Estado(), "con 5 de 9 pasa al primer daño");
        parte.TakeDamage(4f);
        Assert.AreEqual(2, Estado(), "con 1 de 9 pasa al segundo daño");

        parte.TakeDamage(1f);
        Assert.IsFalse(parte.EstaViva);
        Assert.IsTrue(parteGo.activeSelf, "queda el sprite de destruida");
        Assert.IsFalse(parteGo.GetComponent<Collider2D>().enabled, "las balas ya no le pegan");
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
    public void Jefe_explota_y_avisa_una_sola_vez_al_destruir_el_cuerpo()
    {
        int avisos = 0;
        System.Action alMorir = () => avisos++;
        typeof(JefeCangrejo).GetField("alMorir", privado).SetValue(jefe, alMorir);

        cuerpo.TakeDamage(9f);
        cuerpo.TakeDamage(5f);
        cuerpo.TakeDamage(5f);
        Assert.IsTrue(jefeGo.activeSelf, "primero se hincha y explota");
        Assert.AreEqual(0, avisos);
        Assert.IsFalse(pinza.GetComponent<Collider2D>().enabled, "ya no recibe golpes");

        // En EditMode las corrutinas no avanzan: se recorre a mano, sin las esperas.
        var explotar = (IEnumerator)typeof(JefeCangrejo).GetMethod("Explotar", privado).Invoke(jefe, null);
        while (explotar.MoveNext()) { }

        Assert.IsFalse(jefeGo.activeSelf);
        Assert.AreEqual(1, avisos);
    }

    VidaJugador CrearJugador()
    {
        // Inactivo para que Awake no corra hasta tener la barra asignada.
        var go = new GameObject("jugador", typeof(BoxCollider2D));
        go.tag = "Player";
        go.SetActive(false);
        go.transform.SetParent(jefeGo.transform);
        var vida = go.AddComponent<VidaJugador>();
        var barra = go.AddComponent<BarraVida>();
        typeof(BarraVida).GetField("fichas", privado).SetValue(barra, new Image[0]);
        typeof(VidaJugador).GetField("barraVida", privado).SetValue(vida, barra);
        typeof(VidaJugador).GetMethod("Awake", privado).Invoke(vida, null);
        return vida;
    }

    [Test]
    public void Rayo_saca_una_ficha_por_intervalo_solo_con_el_jugador_dentro()
    {
        var jugador = CrearJugador();
        var rayoGo = new GameObject("rayo", typeof(Animator), typeof(BoxCollider2D));
        rayoGo.transform.SetParent(jefeGo.transform);
        var rayo = rayoGo.AddComponent<RayoBoss>();
        typeof(RayoBoss).GetMethod("Awake", privado).Invoke(rayo, null);
        void Llamar(string metodo, params object[] args) => typeof(RayoBoss).GetMethod(metodo, privado).Invoke(rayo, args);
        var colliderJugador = jugador.GetComponent<Collider2D>();

        Llamar("Update");
        Assert.AreEqual(5, jugador.VidaActual, "fuera del rayo no hay daño");

        Llamar("OnTriggerEnter2D", colliderJugador);
        Llamar("Update");
        Llamar("Update");
        Assert.AreEqual(4, jugador.VidaActual, "una ficha por intervalo, no una por frame");

        typeof(RayoBoss).GetField("proximoDano", privado).SetValue(rayo, float.MinValue); // como si ya hubiera pasado el intervalo
        Llamar("Update");
        Assert.AreEqual(3, jugador.VidaActual);

        Llamar("OnTriggerExit2D", colliderJugador);
        Llamar("Update");
        Assert.AreEqual(3, jugador.VidaActual);
    }
}
