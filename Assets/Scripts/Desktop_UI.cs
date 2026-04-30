using UnityEngine;

public class Desktop_UI : MonoBehaviour
{
    [SerializeField] public Canvas canvas;
    public bool abierto;

    private void Start()
    {
        canvas.enabled = false;
    }
    public void AbrirDesktop()
    {
        if (abierto == false)
        {
            canvas.enabled = true;
            abierto = true;
        } else
        {
            canvas.enabled = false;
            abierto = false;
        }
    }

    
    
}
