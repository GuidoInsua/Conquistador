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
// Desc: Una parte del boss (cuerpo o pinza) con vida propia que se va gastando con los golpes.

using UnityEngine;

public class ParteBoss : MonoBehaviour, IDamageable
{
    [SerializeField] private float vidaMaxima = 40f;
    [SerializeField] private GameObject particulasGolpePrefab;

    [Header("Desgaste")]
    [Tooltip("Color que toma la parte cuando le queda poca vida. Con vida llena se ve normal.")]
    [SerializeField] private Color colorGastado = new Color(1f, 0.3f, 0.3f);
    [Tooltip("Color durante el parpadeo de cada golpe.")]
    [SerializeField] private Color colorGolpe = new Color(1f, 1f, 1f, 0.4f);
    [SerializeField] private float duracionGolpe = 0.08f;

    private float vidaActual;
    private float finGolpe;
    private SpriteRenderer spriteRenderer;

    public event System.Action Destruida;

    public bool EstaViva => vidaActual > 0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null || vidaMaxima <= 0f)
        {
            Debug.LogError("ParteBoss: falta el SpriteRenderer o la vida máxima es 0.");
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

        if (!EstaViva)
        {
            gameObject.SetActive(false);
            Destruida?.Invoke();
        }
    }
}
