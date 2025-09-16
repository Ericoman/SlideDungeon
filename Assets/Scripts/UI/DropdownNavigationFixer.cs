using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DropdownNavigationFixer : MonoBehaviour
    {
        [SerializeField] 
        private Scrollbar scrollbar;
        private void Start()
        {
            Toggle toggle = GetComponent<Toggle>();
            Navigation navigation = toggle.navigation;
            navigation.selectOnRight = scrollbar.gameObject.GetComponent<Selectable>();
            navigation.selectOnLeft = scrollbar.gameObject.GetComponent<Selectable>();
            toggle.navigation = navigation;
        }
    }
}
