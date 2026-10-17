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
// Desc: Pruebas del Spawner (capacidad, oleadas, puntos de entrada, victoria y paso al jefe), del sorteo ponderado y de los power-ups.

using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SpawnerTests
{
    const BindingFlags privado = BindingFlags.NonPublic | BindingFlags.Instance;

    GameObject spawnerGo;
    GameObject canvasVictoria;
    Transform spawns;
    Transform posiciones;
    EnemigoAlien alienA;
    EnemigoAlien alienB;
    Spawner spawner;

    [SetUp]
    public void SetUp()
    {
        LogAssert.ignoreFailingMessages = true; // CrossFade sin Animator Controller solo avisa

        alienA = CrearAlien("alienA");
        alienB = CrearAlien("alienB");
        canvasVictoria = new GameObject("victoria");
        canvasVictoria.SetActive(false);
        spawns = new GameObject("spawns").transform;
        posiciones = new GameObject("posiciones").transform;

        spawnerGo = new GameObject("spawner");
        spawnerGo.SetActive(false);
        spawner = spawnerGo.AddComponent<Spawner>();
    }

    [TearDown]
    public void TearDown()
    {
        foreach (var a in Object.FindObjectsByType<EnemigoAlien>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            Object.DestroyImmediate(a.gameObject);
        foreach (var p in Object.FindObjectsByType<PowerUp>(FindObjectsSortMode.None))
            Object.DestroyImmediate(p.gameObject);
        Object.DestroyImmediate(spawns.gameObject);
        Object.DestroyImmediate(posiciones.gameObject);
        Object.DestroyImmediate(spawnerGo);
        Object.DestroyImmediate(canvasVictoria);
    }

    static EnemigoAlien CrearAlien(string nombre) =>
        new GameObject(nombre, typeof(Animator), typeof(EnemigoAlien)).GetComponent<EnemigoAlien>();

    static OpcionEnemigo[] Solo(EnemigoAlien alien) => new[] { new OpcionEnemigo(alien, 1f) };

    static void AgregarHijo(Transform padre, Vector3 posicion)
    {
        Transform hijo = new GameObject("hijo").transform;
        hijo.SetParent(padre, false);
        hijo.localPosition = posicion;
    }

    // Spawn y posiciones en el origen: el alien ya "llegó" y no hace falta esperar frames.
    void Iniciar(Oleada[] oleadas, int cantidadPosiciones)
    {
        AgregarHijo(spawns, Vector3.zero);
        for (int i = 0; i < cantidadPosiciones; i++)
            AgregarHijo(posiciones, Vector3.zero);

        Asignar("oleadas", oleadas);
        Asignar("contenedorSpawns", spawns);
        Asignar("contenedorPosiciones", posiciones);
        Asignar("canvasVictoria", canvasVictoria);
        typeof(Spawner).GetMethod("Awake", privado).Invoke(spawner, null);
    }

    void Asignar(string campo, object valor) =>
        typeof(Spawner).GetField(campo, privado).SetValue(spawner, valor);

    // Los aliens vivos son los clones: los dos prefabs de prueba no cuentan.
    EnemigoAlien[] Vivos() =>
        Object.FindObjectsByType<EnemigoAlien>(FindObjectsSortMode.None)
            .Where(a => a != alienA && a != alienB && a.gameObject.activeSelf).ToArray();

    void Matar(EnemigoAlien alien) =>
        typeof(EnemigoAlien).GetMethod("Morir", privado).Invoke(alien, null);

    [Test]
    public void Spawner_entra_por_uno_de_los_dos_puntos_de_spawn_mas_cercanos()
    {
        AgregarHijo(spawns, new Vector3(-10f, 0f, 0f));
        AgregarHijo(spawns, new Vector3(10f, 0f, 0f));
        AgregarHijo(spawns, new Vector3(12f, 0f, 0f));
        AgregarHijo(posiciones, new Vector3(9f, 0f, 0f));
        Asignar("velocidadMovimiento", 0f); // que se quede donde aparece
        Iniciar(new[] { new Oleada(50, Solo(alienA)) }, 0); // Iniciar suma otro spawn en el origen

        for (int i = 0; i < 30; i++)
        {
            Assert.IsTrue(spawner.SpawnEnemy());
            EnemigoAlien alien = Vivos().Single();
            float x = alien.transform.position.x;
            Assert.IsTrue(x == 10f || x == 12f, "entró por x = " + x);
            Matar(alien);
        }
    }

    [Test]
    public void Spawner_no_pasa_de_la_cantidad_de_posiciones()
    {
        Iniciar(new[] { new Oleada(10, Solo(alienA)) }, 2);

        Assert.IsTrue(spawner.SpawnEnemy());
        Assert.IsTrue(spawner.SpawnEnemy());
        Assert.IsFalse(spawner.SpawnEnemy());
    }

    [Test]
    public void Spawner_libera_el_lugar_cuando_un_alien_muere()
    {
        Iniciar(new[] { new Oleada(10, Solo(alienA)) }, 2);
        spawner.SpawnEnemy();
        spawner.SpawnEnemy();

        Matar(Vivos().First());

        Assert.IsTrue(spawner.SpawnEnemy());
    }

    [Test]
    public void Spawner_nunca_spawnea_mas_que_el_total_de_las_oleadas()
    {
        Iniciar(new[] { new Oleada(2, Solo(alienA)), new Oleada(2, Solo(alienB)) }, 10);

        for (int i = 0; i < 4; i++)
        {
            Assert.IsTrue(spawner.SpawnEnemy(), "spawn " + i);
            Matar(Vivos().First()); // libera lugar para que el límite sea el total y no el espacio
        }

        Assert.IsFalse(spawner.SpawnEnemy());
        Assert.AreEqual(4, spawner.TotalSpawneados);
    }

    [Test]
    public void Spawner_espera_que_muera_toda_la_oleada_para_pasar_a_la_siguiente()
    {
        Iniciar(new[] { new Oleada(2, Solo(alienA)), new Oleada(2, Solo(alienB)) }, 10);

        Assert.IsTrue(spawner.SpawnEnemy());
        Assert.IsTrue(spawner.SpawnEnemy());
        Assert.IsFalse(spawner.SpawnEnemy(), "la oleada 2 no sale con aliens de la 1 vivos");

        var vivos = Vivos();
        Matar(vivos[0]);
        Assert.IsFalse(spawner.SpawnEnemy(), "todavía queda uno de la oleada 1");

        Matar(vivos[1]);
        Assert.IsTrue(spawner.SpawnEnemy());
        Assert.AreEqual("alienB(Clone)", Vivos().Single().name);
    }

    [Test]
    public void Spawner_muestra_la_victoria_al_matar_a_todos_y_no_antes()
    {
        Iniciar(new[] { new Oleada(2, Solo(alienA)), new Oleada(1, Solo(alienB)) }, 10);

        spawner.SpawnEnemy();
        spawner.SpawnEnemy();
        var vivos = Vivos();
        Matar(vivos[0]);
        Matar(vivos[1]);
        Assert.IsFalse(canvasVictoria.activeSelf);

        spawner.SpawnEnemy();
        Matar(Vivos().Single());
        Assert.IsTrue(canvasVictoria.activeSelf);
        Assert.AreEqual(3, spawner.EnemigosEliminados);
    }

    [Test]
    public void Spawner_con_boss_mal_configurado_muestra_la_victoria_igual()
    {
        var faseBoss = new GameObject("faseBoss").AddComponent<FaseBoss>();
        faseBoss.enabled = false; // Lo que hace su Awake si le falta el prefab del jefe.
        Asignar("faseBoss", faseBoss);
        Iniciar(new[] { new Oleada(1, Solo(alienA)) }, 1);

        spawner.SpawnEnemy();
        Matar(Vivos().Single());

        Assert.IsTrue(canvasVictoria.activeSelf);

        Object.DestroyImmediate(faseBoss.gameObject);
    }

    [Test]
    public void Spawner_con_boss_inicia_la_fase_en_vez_de_la_victoria()
    {
        var camara = new GameObject("camara", typeof(Camera)).GetComponent<Camera>();
        var faseBoss = new GameObject("faseBoss").AddComponent<FaseBoss>();
        typeof(FaseBoss).GetField("camara", privado).SetValue(faseBoss, camara);
        Asignar("faseBoss", faseBoss);
        Iniciar(new[] { new Oleada(1, Solo(alienA)) }, 1);

        spawner.SpawnEnemy();
        Matar(Vivos().Single());

        Assert.IsFalse(canvasVictoria.activeSelf);
        Assert.IsTrue((bool)typeof(FaseBoss).GetField("isIniciada", privado).GetValue(faseBoss));

        Object.DestroyImmediate(faseBoss.gameObject);
        Object.DestroyImmediate(camara.gameObject);
    }

    [Test]
    public void Oleada_elige_el_tipo_segun_el_peso()
    {
        var oleada = new Oleada(10, new[] { new OpcionEnemigo(alienA, 60f), new OpcionEnemigo(alienB, 40f) });

        Assert.AreSame(alienA, oleada.ElegirPrefab(0f));
        Assert.AreSame(alienA, oleada.ElegirPrefab(0.59f));
        Assert.AreSame(alienB, oleada.ElegirPrefab(0.6f));
        Assert.AreSame(alienB, oleada.ElegirPrefab(1f));
        Assert.IsNull(new Oleada(10, new OpcionEnemigo[0]).ElegirPrefab(0.5f));
    }

    [Test]
    public void Sorteo_ignora_pesos_cero_y_devuelve_menos_uno_si_no_hay_ninguno()
    {
        Assert.AreEqual(2, SorteoPonderado.ElegirIndice(new[] { 0f, 0f, 5f }, 0f));
        Assert.AreEqual(-1, SorteoPonderado.ElegirIndice(new[] { 0f, -1f }, 0.5f));
        Assert.AreEqual(-1, SorteoPonderado.ElegirIndice(null, 0.5f));
    }

    [Test]
    public void PowerUp_probabilidad_cero_no_aparece_y_uno_siempre_aparece()
    {
        var opciones = new[] { new OpcionPowerUp(new GameObject("powerup", typeof(PowerUpVida)), 1f) };

        PowerUp.SpawnPowerUp(opciones, Vector3.zero, 0f);
        Assert.AreEqual(1, Object.FindObjectsByType<PowerUp>(FindObjectsSortMode.None).Length); // solo el prefab

        PowerUp.SpawnPowerUp(opciones, Vector3.zero, 1f);
        Assert.AreEqual(2, Object.FindObjectsByType<PowerUp>(FindObjectsSortMode.None).Length);
    }

    [Test]
    public void PowerUp_elige_segun_el_peso_e_ignora_opciones_invalidas()
    {
        var velocidad = new GameObject("velocidad", typeof(PowerUpVelocidad));
        var vida = new GameObject("vida", typeof(PowerUpVida));
        var opciones = new[]
        {
            new OpcionPowerUp(velocidad, 50f),
            new OpcionPowerUp(null, 50f), // sin prefab: se ignora
            new OpcionPowerUp(vida, 50f),
        };

        Assert.AreSame(velocidad, PowerUp.ElegirPrefab(opciones, 0f));
        Assert.AreSame(velocidad, PowerUp.ElegirPrefab(opciones, 0.49f));
        Assert.AreSame(vida, PowerUp.ElegirPrefab(opciones, 0.5f));
        Assert.AreSame(vida, PowerUp.ElegirPrefab(opciones, 1f));
        Assert.IsNull(PowerUp.ElegirPrefab(new OpcionPowerUp[0], 0.5f));
        Assert.IsNull(PowerUp.ElegirPrefab(null, 0.5f));
    }
}
