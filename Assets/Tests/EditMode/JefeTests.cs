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
// Desc: Pruebas del jefe: vida y estados de sus partes, power-ups de las pinzas, muerte, rayo, abanico de las pinzas y esbirros.

using System.Collections;
using System.Linq;
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
    public void Pinza_dispara_su_abanico_de_a_una_solo_mientras_esta_viva()
    {
        var balaPrefab = new GameObject("bala", typeof(BalaEnemiga));
        var disparo = pinza.gameObject.AddComponent<DisparoPinza>();
        typeof(DisparoPinza).GetField("balaPrefab", privado).SetValue(disparo, balaPrefab);
        typeof(DisparoPinza).GetField("puntaDisparo", privado).SetValue(disparo, pinza.transform);
        typeof(DisparoPinza).GetMethod("Awake", privado).Invoke(disparo, null);
        var bola = new GameObject("bola");
        bola.transform.SetParent(pinza.transform);
        bola.SetActive(false);
        typeof(DisparoPinza).GetField("bola", privado).SetValue(disparo, bola);
        int Balas() => Object.FindObjectsByType<BalaEnemiga>(FindObjectsSortMode.None).Length;
        int antes = Balas();

        // En EditMode las corrutinas no avanzan: se recorre a mano, sin las esperas.
        var abanico = disparo.Disparar();
        abanico.MoveNext();
        Assert.IsTrue(bola.activeSelf, "primero avisa con la bola");
        Assert.AreEqual(antes, Balas());
        abanico.MoveNext();
        Assert.AreEqual(antes + 1, Balas(), "salen de a una");
        while (abanico.MoveNext()) { }
        Assert.AreEqual(antes + 5, Balas());
        Assert.IsFalse(bola.activeSelf);

        pinza.TakeDamage(99f);
        abanico = disparo.Disparar();
        while (abanico.MoveNext()) { }
        Assert.AreEqual(antes + 5, Balas(), "la pinza destruida no dispara");

        foreach (var bala in Object.FindObjectsByType<BalaEnemiga>(FindObjectsSortMode.None))
            Object.DestroyImmediate(bala.gameObject);
    }

    [Test]
    public void Pinza_suelta_dos_power_ups_al_destruirse()
    {
        var powerUp = new GameObject("powerUpPrueba");
        typeof(ParteBoss).GetField("powerUps", privado).SetValue(pinza, new[] { new OpcionPowerUp(powerUp, 1f) });
        int Sueltos() => Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Count(t => t.name == "powerUpPrueba(Clone)");

        pinza.TakeDamage(1f);
        Assert.AreEqual(0, Sueltos(), "viva no suelta nada");

        pinza.TakeDamage(99f);
        Assert.AreEqual(2, Sueltos());

        foreach (var t in Object.FindObjectsByType<Transform>(FindObjectsSortMode.None))
            if (t != null && t.name.StartsWith("powerUpPrueba"))
                Object.DestroyImmediate(t.gameObject);
    }

    Esbirro CrearEsbirro(string nombre, Vector3 posicion)
    {
        var go = new GameObject(nombre, typeof(Animator), typeof(EnemigoAlien), typeof(CircleCollider2D));
        go.transform.SetParent(jefeGo.transform);
        go.GetComponent<EnemigoAlien>().ResetEnemy();
        var esbirro = go.AddComponent<Esbirro>();
        typeof(Esbirro).GetMethod("Awake", privado).Invoke(esbirro, null);
        esbirro.Iniciar(posicion, 1f, null);
        typeof(Esbirro).GetField("isSaliendo", privado).SetValue(esbirro, false); // Ya salió del boss.
        esbirro.transform.position = posicion;
        return esbirro;
    }

    [Test]
    public void Esbirros_se_empujan_poco_al_chocar_y_no_mientras_salen()
    {
        var izquierda = CrearEsbirro("izq", new Vector3(-0.1f, 0f, 0f));
        var derecha = CrearEsbirro("der", new Vector3(0.1f, 0f, 0f));
        var tocar = typeof(Esbirro).GetMethod("OnTriggerEnter2D", privado);
        Vector2 Empuje(Esbirro e) => (Vector2)typeof(Esbirro).GetField("velocidadEmpuje", privado).GetValue(e);

        tocar.Invoke(izquierda, new object[] { derecha.GetComponent<Collider2D>() });
        Assert.AreEqual(Vector2.left * 1.5f, Empuje(izquierda), "se aleja del otro, mucho menos que del jugador (7)");

        typeof(Esbirro).GetField("isSaliendo", privado).SetValue(derecha, true);
        typeof(Esbirro).GetField("finRetroceso", privado).SetValue(izquierda, float.MinValue);
        typeof(Esbirro).GetField("velocidadEmpuje", privado).SetValue(izquierda, Vector2.zero);
        tocar.Invoke(izquierda, new object[] { derecha.GetComponent<Collider2D>() });
        Assert.AreEqual(Vector2.zero, Empuje(izquierda), "con uno que todavía sale no se empujan");
    }

    [Test]
    public void Esbirro_se_muere_solo_sin_soltar_power_up_al_pasar_su_tiempo()
    {
        var esbirroGo = new GameObject("esbirroViejo", typeof(Animator), typeof(EnemigoAlien));
        esbirroGo.transform.SetParent(jefeGo.transform);
        var alien = esbirroGo.GetComponent<EnemigoAlien>();
        alien.ResetEnemy();
        var esbirro = esbirroGo.AddComponent<Esbirro>();
        typeof(Esbirro).GetMethod("Awake", privado).Invoke(esbirro, null);
        esbirro.Iniciar(Vector3.zero, 1f, null);

        typeof(Esbirro).GetMethod("Update", privado).Invoke(esbirro, null);
        Assert.Greater(alien.GetSalud(), 0f, "recién salido sigue vivo");

        typeof(Esbirro).GetField("finVida", privado).SetValue(esbirro, float.MinValue);
        typeof(Esbirro).GetMethod("Update", privado).Invoke(esbirro, null);
        Assert.AreEqual(0f, alien.GetSalud());
        Assert.IsTrue((bool)typeof(EnemigoAlien).GetField("isSinPowerUp", privado).GetValue(alien));
    }

    [Test]
    public void Esbirro_rebota_alejandose_del_jugador_o_para_arriba_si_estan_encimados()
    {
        Assert.AreEqual(Vector2.left, Esbirro.DireccionRetroceso(new Vector2(-2f, 0f), Vector2.zero));
        Assert.AreEqual(Vector2.up, Esbirro.DireccionRetroceso(new Vector2(1f, 1f), new Vector2(1f, 1f)));
    }

    [Test]
    public void Esbirro_golpea_una_vez_y_no_vuelve_a_golpear_mientras_rebota()
    {
        var jugador = CrearJugador();
        jugador.transform.position = Vector3.zero;
        var esbirroGo = new GameObject("esbirro", typeof(Animator), typeof(EnemigoAlien));
        esbirroGo.transform.SetParent(jefeGo.transform);
        esbirroGo.transform.position = new Vector3(0f, 1f, 0f);
        esbirroGo.GetComponent<EnemigoAlien>().ResetEnemy();
        var esbirro = esbirroGo.AddComponent<Esbirro>();
        typeof(Esbirro).GetMethod("Awake", privado).Invoke(esbirro, null);
        var tocar = typeof(Esbirro).GetMethod("OnTriggerEnter2D", privado);
        var colliderJugador = jugador.GetComponent<Collider2D>();

        tocar.Invoke(esbirro, new object[] { colliderJugador });
        Assert.AreEqual(4, jugador.VidaActual);
        Assert.AreEqual(Vector2.up * 7f, (Vector2)typeof(Esbirro).GetField("velocidadEmpuje", privado).GetValue(esbirro));

        typeof(VidaJugador).GetField("finInvulnerable", privado).SetValue(jugador, float.MinValue);
        tocar.Invoke(esbirro, new object[] { colliderJugador });
        Assert.AreEqual(4, jugador.VidaActual, "mientras rebota no golpea");
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
        var go = new GameObject("jugador", typeof(BoxCollider2D), typeof(SpriteRenderer));
        go.tag = "Player";
        go.SetActive(false);
        go.transform.SetParent(jefeGo.transform);
        var vida = go.AddComponent<VidaJugador>();
        var barra = go.AddComponent<BarraVida>();
        typeof(BarraVida).GetField("fichas", privado).SetValue(barra, new Image[0]);
        typeof(VidaJugador).GetField("barraVida", privado).SetValue(vida, barra);
        typeof(VidaJugador).GetField("materialDestello", privado).SetValue(vida, new Material(Shader.Find("GUI/Text Shader")));
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
        typeof(VidaJugador).GetField("finInvulnerable", privado).SetValue(jugador, float.MinValue); // y el titileo del golpe anterior
        Llamar("Update");
        Assert.AreEqual(3, jugador.VidaActual);

        Llamar("OnTriggerExit2D", colliderJugador);
        Llamar("Update");
        Assert.AreEqual(3, jugador.VidaActual);
    }
}
