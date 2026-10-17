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
// Desc: Pruebas del desvío suave que usan los enemigos para oscilar.

using NUnit.Framework;
using UnityEngine;

public class OscilacionTests
{
    [Test]
    public void Desvio_arranca_en_cero_y_no_se_aleja_mas_de_uno()
    {
        Assert.AreEqual(Vector2.zero, Oscilacion.Desvio(42.3f, 0f));

        for (float t = 0f; t < 50f; t += 0.37f)
        {
            Vector2 desvio = Oscilacion.Desvio(42.3f, t);
            Assert.LessOrEqual(Mathf.Abs(desvio.x), 1f);
            Assert.LessOrEqual(Mathf.Abs(desvio.y), 1f);
        }
    }

    [Test]
    public void Onda_del_camino_arranca_y_termina_en_cero_y_se_abre_en_el_medio()
    {
        Assert.AreEqual(0f, EnemigoAlien.OndaCamino(0f, 3, 0.6f), 0.0001f);
        Assert.AreEqual(0f, EnemigoAlien.OndaCamino(1f, 3, 0.6f), 0.0001f);
        Assert.Greater(Mathf.Abs(EnemigoAlien.OndaCamino(5f / 12f, 3, 0.6f)), 0.5f, "en una cresta cerca del medio se corre casi toda la amplitud");
    }

    [Test]
    public void Camara_no_cambia_en_16_9_y_sube_en_pantallas_angostas()
    {
        Assert.AreEqual(6.3f, CamaraAjustada.Altura(6.3f, 22.4f, 16f / 9f), 0.0001f, "16:9 queda igual");
        Assert.AreEqual(6.3f, CamaraAjustada.Altura(6.3f, 22.4f, 21f / 9f), 0.0001f, "más ancho queda igual");
        Assert.AreEqual(8.4f, CamaraAjustada.Altura(6.3f, 22.4f, 4f / 3f), 0.0001f, "4:3 sube para que entren los 22.4 de ancho");
    }

    [Test]
    public void Punto_de_entrada_solo_se_corre_si_quedaria_a_la_vista()
    {
        Assert.AreEqual(14.5f, Spawner.FueraDeVista(14.5f, 6.3f * 16f / 9f), "en 16:9 queda donde está");
        Assert.AreEqual(-14.9f, Spawner.FueraDeVista(-14.9f, 6.3f * 16f / 9f));
        Assert.AreEqual(6.3f * 21f / 9f + 1f, Spawner.FueraDeVista(14.5f, 6.3f * 21f / 9f), 0.0001f, "en 21:9 se corre afuera");
        Assert.AreEqual(-(6.3f * 21f / 9f + 1f), Spawner.FueraDeVista(-14.9f, 6.3f * 21f / 9f), 0.0001f);
    }

    [Test]
    public void Desvio_cambia_con_el_tiempo()
    {
        Assert.AreNotEqual(Oscilacion.Desvio(7.1f, 0f), Oscilacion.Desvio(7.1f, 1.3f));
    }
}
