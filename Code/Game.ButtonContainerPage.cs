using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace GodTools.Game
{
    internal class ButtonContainerPage : MonoBehaviour
    {
        public void add_button(GameObject button_obj)
        {
            button_obj.transform.SetParent(this.transform);
            button_obj.transform.Find("Button").GetComponent<RectTransform>().sizeDelta = new Vector2(48, 48);
        }
    }
}
