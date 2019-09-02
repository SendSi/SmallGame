using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETModel {
    public class FishMonoCtl : MonoBehaviour
    {

        private void OnTriggerEnter2D(Collider2D collision)
        {
                //Game.EventSystem.Run(EventIdType.FishColliderEvent, transform.GetComponentInChildren<Text>().text, collision);
        }
         
    }
}
