using UnityEngine;
using UnityEngine.UI;

public class RequestMainUIController : MonoBehaviour
{
    public Image Bar;

    public void UpdateBar(float timeLeft)
    {
        Bar.fillAmount = timeLeft;
    }
}
