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
    [Tooltip("Estados del Animator de menos a más daño. El último es el de destruida y la parte queda a la vista. Vacío: la parte desaparece al destruirse.")]
    [SerializeField] private string[] estadosDano = new string[0];

    [Header("Desgaste")]
    [Tooltip("Color que toma la parte cuando le queda poca vida. Con vida llena se ve normal.")]
    [SerializeField] private Color colorGastado = new Color(1f, 0.3f, 0.3f);
    [Tooltip("Color durante el parpadeo de cada golpe.")]
    [SerializeField] private Color colorGolpe = new Color(1f, 1f, 1f, 0.4f);
    [SerializeField] private float duracionGolpe = 0.08f;

    private float vidaActual;
    private float finGolpe;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Collider2D zonaGolpe;
    private int estadoActual;

    public event System.Action Destruida;

    public bool EstaViva => vidaActual > 0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        zonaGolpe = GetComponent<Collider2D>();

        // Con un solo estado no queda ninguno para la parte viva.
        if (spriteRenderer == null || vidaMaxima <= 0f || estadosDano.Length == 1 || (estadosDano.Length > 1 && animator == null))
        {
            Debug.LogError("ParteBoss: falta el SpriteRenderer, la vida máxima es 0, o los estados de daño necesitan un Animator y al menos dos estados.");
            enabled = false;
            return;
        }

        vidaActual = vidaMaxima;
    }

    private void Update()
    {
        Color gastado = Color.Lerp(Color.white, colorGastado, 1f - vidaActual / vidaMaxima);
        spriteRenderer.color = Time.time < finGolpe ? colorGolpe : gastado;
    }

    public float GetSalud() => vidaActual;

    public void TakeDamage(float cantidad)
    {
        if (!EstaViva)
            return;

        vidaActual = Mathf.Max(0f, vidaActual - cantidad);
        finGolpe = Time.time + duracionGolpe;

        if (particulasGolpePrefab != null)
            Instantiate(particulasGolpePrefab, transform.position, Quaternion.identity);

        if (estadosDano.Length > 0)
            CambiarEstado(EstaViva ? EstadoSegunVida(vidaActual / vidaMaxima, estadosDano.Length - 1) : estadosDano.Length - 1);

        if (!EstaViva)
        {
            if (estadosDano.Length == 0)
                gameObject.SetActive(false);
            else if (zonaGolpe != null)
                zonaGolpe.enabled = false; // Queda el sprite de destruida, pero las balas la atraviesan.

            Destruida?.Invoke();
        }
    }

    // Reparte la vida en tramos iguales, uno por cada estado de la parte viva.
    private static int EstadoSegunVida(float fraccionVida, int estadosVivos) =>
        Mathf.Min(estadosVivos - 1, (int)((1f - fraccionVida) * estadosVivos));

    private void CambiarEstado(int estado)
    {
        if (estado == estadoActual)
            return;

        estadoActual = estado;
        animator.Play(estadosDano[estado]);
    }
}
