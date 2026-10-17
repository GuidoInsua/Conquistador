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
// Desc: Una parte del boss (cuerpo o pinza) con vida propia: cambia de animación según el daño (y su versión enojada) y al romperse suena, puede soltar power-ups y avisa al boss.

using UnityEngine;
using UnityEngine.Serialization;

public class ParteBoss : MonoBehaviour, IDamageable
{
    [SerializeField] private float vidaMaxima = 40f;
    [SerializeField] private GameObject particulasGolpePrefab;
    [Tooltip("Suena una vez, al romperse.")]
    [FormerlySerializedAs("sonidoGolpe")]
    [SerializeField] private AudioClip sonidoMuerte;

    [Header("Al destruirse")]
    [Tooltip("Suelta esta cantidad de power-ups elegidos al azar de la lista. Lista vacía: no suelta nada.")]
    [SerializeField] private OpcionPowerUp[] powerUps = new OpcionPowerUp[0];
    [SerializeField] private int cantidadPowerUps = 2;
    [Tooltip("Distancia entre power-up y power-up, para que no caigan encimados.")]
    [SerializeField] private float separacionPowerUps = 0.8f;

    [Header("Animación")]
    [Tooltip("Estados del Animator de menos a más daño mientras la parte está viva. La vida se reparte en tramos iguales entre ellos.")]
    [SerializeField] private string[] estadosDano = new string[0];
    [Tooltip("Estado al quedar en 0. Vacío: la parte desaparece.")]
    [SerializeField] private string estadoDestruida;
    [Tooltip("Color durante el parpadeo de cada golpe.")]
    [SerializeField] private Color colorGolpe = new Color(1f, 1f, 1f, 0.4f);
    [SerializeField] private float duracionGolpe = 0.08f;

    private float vidaActual;
    private float finGolpe;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Collider2D zonaGolpe;
    private int estadoActual;
    private bool isEnojada;

    // Cada estado de daño tiene su versión enojada con este sufijo (Dano1 -> Dano1Enojado).
    private const string sufijoEnojado = "Enojado";

    public event System.Action Destruida;

    public bool EstaViva
    {
        get { return vidaActual > 0f; }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        zonaGolpe = GetComponent<Collider2D>();

        bool usaAnimator = estadosDano.Length > 0 || !string.IsNullOrEmpty(estadoDestruida);
        if (spriteRenderer == null || vidaMaxima <= 0f || (usaAnimator && animator == null))
        {
            Debug.LogError("ParteBoss: falta el SpriteRenderer, la vida máxima es 0 o los estados de animación no tienen Animator.");
            enabled = false;
            return;
        }

        vidaActual = vidaMaxima;
    }

    private void Update()
    {
        if (Time.time < finGolpe)
        {
            spriteRenderer.color = colorGolpe;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }

    public float GetSalud()
    {
        return vidaActual;
    }

    public void TakeDamage(float cantidad)
    {
        if (!EstaViva)
        {
            return;
        }

        vidaActual = Mathf.Max(0f, vidaActual - cantidad);
        finGolpe = Time.time + duracionGolpe;

        if (particulasGolpePrefab != null)
        {
            Instantiate(particulasGolpePrefab, transform.position, Quaternion.identity);
        }

        if (EstaViva)
        {
            int estado = EstadoSegunVida(vidaActual / vidaMaxima, estadosDano.Length);
            if (estado != estadoActual)
            {
                estadoActual = estado;
                ReproducirEstado();
            }
            return;
        }

        EfectosSonido.Reproducir(sonidoMuerte);

        if (string.IsNullOrEmpty(estadoDestruida))
        {
            gameObject.SetActive(false);
        }
        else
        {
            animator.Play(estadoDestruida);
            // Queda el sprite de destruida, pero las balas la atraviesan.
            if (zonaGolpe != null)
            {
                zonaGolpe.enabled = false;
            }
        }

        if (powerUps.Length > 0)
        {
            for (int i = 0; i < cantidadPowerUps; i++)
            {
                // Los reparte a lo ancho, centrados en la parte. Con 2: uno a la izquierda y otro a la derecha.
                float x = (i - (cantidadPowerUps - 1) / 2f) * separacionPowerUps;
                PowerUp.SpawnPowerUp(powerUps, transform.position + Vector3.right * x, 1f);
            }
        }

        // Avisa a quien esté escuchando (el boss), si hay alguien.
        if (Destruida != null)
        {
            Destruida();
        }
    }

    // La usa el boss mientras dispara el rayo.
    public void SetEnojada(bool enojada)
    {
        isEnojada = enojada;
        if (EstaViva)
        {
            ReproducirEstado();
        }
    }

    // Reparte la vida en tramos iguales, uno por cada estado de la parte viva.
    // Ejemplo: con 3 estados, de 100 % a 67 % de vida es el 0, de 67 % a 33 % el 1, y de ahí para abajo el 2.
    private static int EstadoSegunVida(float fraccionVida, int estadosVivos)
    {
        float fraccionPerdida = 1f - fraccionVida;
        int estado = (int)(fraccionPerdida * estadosVivos);

        // Nunca menos que el primero ni más que el último.
        int ultimoEstado = Mathf.Max(0, estadosVivos - 1);
        return Mathf.Clamp(estado, 0, ultimoEstado);
    }

    private void ReproducirEstado()
    {
        if (estadosDano.Length == 0)
        {
            return;
        }

        // Sigue desde el mismo punto del ciclo para que el cambio no salte de frame.
        // normalizedTime cuenta las vueltas del loop (2.3 = tercera vuelta, al 30 %); "% 1" deja solo el 0.3.
        float tiempo = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;

        string nombreEstado = estadosDano[estadoActual];
        if (isEnojada)
        {
            nombreEstado = nombreEstado + sufijoEnojado;
        }

        animator.Play(nombreEstado, 0, tiempo);
    }
}
