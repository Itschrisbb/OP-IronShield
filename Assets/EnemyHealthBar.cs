using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;
    public Transform lookAtTarget;

    void Update()
    {
        if (lookAtTarget != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }

    public void SetMaxHealth(int max)
    {
        slider.maxValue = max;
        slider.value = max;
    }

    public void SetHealth(int current)
    {
        slider.value = current;
    }
}
