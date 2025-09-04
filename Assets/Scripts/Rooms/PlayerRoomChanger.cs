using UnityEngine;

namespace Rooms
{
    public class PlayerRoomChanger : MonoBehaviour
    {
        [SerializeField] private Transform room;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.transform.SetParent(room,true);
            }
        }

        public void SetRoomTransform(Transform roomContainer)
        {
            room = roomContainer;
        }
    }
}
