using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
   [SerializeField] private new Transform camera;
   
   [SerializeField] private float sensitivity;
   [SerializeField] private float interactDistance = 2;
   [SerializeField] private LayerMask interactMask;
   private Vector2 rotation;

   private void Start()
   {
      sensitivity *= PlayerPrefs.GetFloat("Sensitivity");
   }

   private void Update()
   {
      if(GameManager.singleton.pause)
         return;
      
      Look();  
      CheckBack();
      RaycastHit hit;
      if (Input.GetButtonDown("Use") &&
          Physics.Raycast(camera.position, camera.forward, out hit, interactDistance, interactMask))
      {
         Debug.Log("Tried to use: " + hit.transform.gameObject.name);

         if (hit.transform.GetComponentInParent<Task>() != null)
         {
            hit.transform.GetComponentInParent<Task>().Interact(GetComponent<PlayerTasks>(), GetComponent<PlayerImpostor>());
         }
         else if (!GetComponent<PlayerManager>().isDead && hit.transform.GetComponentInParent<DeadBody>() != null)
         {
            GameManager.singleton.Meeting(GetComponent<PlayerManager>());
         }
      }
   }

   private void Look()
   {
      rotation.x -= Input.GetAxisRaw("Mouse Y") * sensitivity;
      rotation.y += Input.GetAxisRaw("Mouse X") * sensitivity;

      rotation.x = Mathf.Clamp(rotation.x, -90, 90);
      
      camera.localRotation = Quaternion.Euler(new Vector3(rotation.x, 0, 0));
      transform.localRotation = Quaternion.Euler(new Vector3(0, rotation.y, 0));
   }

   private void CheckBack()
   {
      if (Input.GetButton("Look"))
      {
         camera.localRotation = Quaternion.Euler(new Vector3(rotation.x, 180, 0));
      }
   }
}
