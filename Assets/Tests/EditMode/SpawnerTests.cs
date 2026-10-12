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
// Desc: Pruebas del Spawner (capacidad y liberación de posiciones) y de la probabilidad de PowerUp.

using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SpawnerTests
{
    const BindingFlags privado = BindingFlags.NonPublic | BindingFlags.Instance;

    GameObject spawnerGo;
    Transform[] puntos;
    EnemigoAlien prefab;
    Spawner spawner;

    [SetUp]
    public void SetUp()
    {
        LogAssert.ignoreFailingMessages = true; // CrossFade sin Animator Controller solo avisa

        prefab = new GameObject("prefab", typeof(Animator), typeof(EnemigoAlien)).GetComponent<EnemigoAlien>();

        // Spawn y posición final en el mismo lugar: el alien ya "llegó" y no hace falta esperar frames.
        puntos = new Transform[3];
        for (int i = 0; i < puntos.Length; i++)
            puntos[i] = new GameObject("punto" + i).transform;

        spawnerGo = new GameObject("spawner");
        spawnerGo.SetActive(false);
        spawner = spawnerGo.AddComponent<Spawner>();

        Asignar("enemyPrefab", prefab);
        Asignar("puntosSpawn", puntos);
        Asignar("posicionesEnemigos", puntos);
        Asignar("capacidadMaxima", 2);
        typeof(Spawner).GetMethod("Awake", privado).Invoke(spawner, null);
    }

    [TearDown]
    public void TearDown()
    {
        foreach (var a in Object.FindObjectsByType<EnemigoAlien>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            Object.DestroyImmediate(a.gameObject);
        foreach (var p in Object.FindObjectsByType<PowerUp>(FindObjectsSortMode.None))
            Object.DestroyImmediate(p.gameObject);
        foreach (var punto in puntos)
            Object.DestroyImmediate(punto.gameObject);
        Object.DestroyImmediate(spawnerGo);
    }

    void Asignar(string campo, object valor) =>
        typeof(Spawner).GetField(campo, privado).SetValue(spawner, valor);

    [Test]
    public void Spawner_no_pasa_de_la_capacidad_maxima()
    {
        Assert.IsTrue(spawner.SpawnEnemy());
        Assert.IsTrue(spawner.SpawnEnemy());
        Assert.IsFalse(spawner.SpawnEnemy());
    }

    [Test]
    public void Spawner_libera_el_lugar_cuando_un_alien_muere()
    {
        spawner.SpawnEnemy();
        spawner.SpawnEnemy();

        var vivo = Object.FindObjectsByType<EnemigoAlien>(FindObjectsSortMode.None)
            .First(a => a != prefab && a.gameObject.activeSelf);
        typeof(EnemigoAlien).GetMethod("Morir", privado).Invoke(vivo, null);

        Assert.IsTrue(spawner.SpawnEnemy());
    }

    [Test]
    public void PowerUp_probabilidad_cero_no_aparece_y_uno_siempre_aparece()
    {
        var powerUpPrefab = new GameObject("powerup", typeof(PowerUp));

        PowerUp.SpawnPowerUp(powerUpPrefab, Vector3.zero, 0f);
        Assert.AreEqual(1, Object.FindObjectsByType<PowerUp>(FindObjectsSortMode.None).Length); // solo el prefab

        PowerUp.SpawnPowerUp(powerUpPrefab, Vector3.zero, 1f);
        Assert.AreEqual(2, Object.FindObjectsByType<PowerUp>(FindObjectsSortMode.None).Length);
    }
}
