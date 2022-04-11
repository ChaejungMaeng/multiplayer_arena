using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Spec.Specialisation.SpecMulti
{
    public class Healthbar : MonoBehaviour
    {
        [SerializeField]
        public Slider slider;

        public void SetHealth(float health)
        {
            slider.value = health;
        }
        public void SetMaxHealth(float health)
        {
            slider.maxValue = health;
            slider.value = health;
        }
    }
}