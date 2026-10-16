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
    public void Desvio_cambia_con_el_tiempo()
    {
        Assert.AreNotEqual(Oscilacion.Desvio(7.1f, 0f), Oscilacion.Desvio(7.1f, 1.3f));
    }
}
