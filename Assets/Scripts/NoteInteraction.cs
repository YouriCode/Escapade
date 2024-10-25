using UnityEngine;

public class NoteInteraction : MonoBehaviour
{
    public Canvas noteCanvas;             // Canvas contenant l'image de la note
    public float interactionRange = 5f;   // Distance de portée de la note
    private Transform player;             // Référence au joueur
    private bool isInRange = false;       // Indicateur si le joueur est à portée
    private bool isNoteVisible = false;   // Indicateur si le canvas de la note est affiché

    private void Start()
    {
        // Assigner la référence au joueur via son tag
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Masquer le canvas au démarrage
        if (noteCanvas != null)
        {
            noteCanvas.enabled = false;
        }
        else
        {
            Debug.LogWarning("Le Canvas de la note n'est pas assigné !");
        }
    }

    private void Update()
    {
        if (player == null) return;

        // Vérifier la distance entre le joueur et la note
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isInRange = distanceToPlayer <= interactionRange;

        if (isInRange && Input.GetKeyDown(KeyCode.F))
        {
            // Alterner la visibilité du canvas de la note
            ToggleNoteDisplay();
        }
        else if (!isInRange && isNoteVisible)
        {
            // Masquer le canvas si le joueur s'éloigne
            HideNote();
        }
    }

    // Fonction pour afficher/masquer le canvas de la note
    private void ToggleNoteDisplay()
    {
        isNoteVisible = !isNoteVisible;
        noteCanvas.enabled = isNoteVisible;
    }

    // Fonction pour masquer le canvas de la note si le joueur s'éloigne
    private void HideNote()
    {
        isNoteVisible = false;
        noteCanvas.enabled = false;
    }
}
