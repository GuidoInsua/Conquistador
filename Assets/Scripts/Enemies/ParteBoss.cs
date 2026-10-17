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
// Desc: Una parte del boss (cuerpo o pinza) con vida propia que se va gastando con los golpes y cambia de animación según el daño.

using UnityEngine;

public class ParteBoss : MonoBehaviour, IDamageable
{
    [SerializeField] private float vidaMaxima = 40f;
    [SerializeField] private GameObject particulasGolpePrefab;
    [SerializeField] private AudioClip sonidoGolpe;

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
    // Pasó a otro estado de daño (el índice en estadosDano). No se llama al destruirse.
    public event System.Action<int> CambioEstado;

    public bool EstaViva => vidaActual > 0f;

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
        spriteRenderer.color = Time.time < finGolpe ? colorGolpe : Color.white;
    }

    public float GetSalud() => vidaActual;

    public void TakeDamage(float cantidad)
    {
        if (!EstaViva)
            return;

        vidaActual = Mathf.Max(0f, vidaActual - cantidad);
        finGolpe = Time.time + duracionGolpe;
        EfectosSonido.Reproducir(sonidoGolpe);

        if (particulasGolpePrefab != null)
            Instantiate(particulasGolpePrefab, transform.position, Quaternion.identity);

        if (EstaViva)
        {
            int estado = EstadoSegunVida(vidaActual / vidaMaxima, estadosDano.Length);
            if (estado != estadoActual)
            {
                estadoActual = estado;
                ReproducirEstado();
                CambioEstado?.Invoke(estado);
            }
            return;
        }

        if (string.IsNullOrEmpty(estadoDestruida))
        {
            gameObject.SetActive(false);
        }
        else
        {
            animator.Play(estadoDestruida);
            if (zonaGolpe != null)
                zonaGolpe.enabled = false; // Queda el sprite de destruida, pero las balas la atraviesan.
        }

        Destruida?.Invoke();
    }

    // La usa el boss mientras dispara el rayo.
    public void SetEnojada(bool enojada)
    {
        isEnojada = enojada;
        if (EstaViva)
            ReproducirEstado();
    }

    // Reparte la vida en tramos iguales, uno por cada estado de la parte viva.
    private static int EstadoSegunVida(float fraccionVida, int estadosVivos) =>
        Mathf.Clamp((int)((1f - fraccionVida) * estadosVivos), 0, Mathf.Max(0, estadosVivos - 1));

    private void ReproducirEstado()
    {
        if (estadosDano.Length == 0)
            return;

        // Sigue desde el mismo punto del ciclo para que el cambio no salte de frame.
        float tiempo = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
        animator.Play(estadosDano[estadoActual] + (isEnojada ? sufijoEnojado : ""), 0, tiempo);
    }
}
