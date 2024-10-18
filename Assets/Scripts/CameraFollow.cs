using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;    // Le personnage à suivre
    public Vector3 offset;      // Offset de la caméra par rapport au personnage
    public float followSpeed = 5f;  // Vitesse de suivi

    void LateUpdate()
    {
        // Calcul de la position de la caméra en fonction de la position du personnage et de l'offset
        Vector3 targetPosition = target.position + offset;

        // Interpolation douce pour un mouvement fluide de la caméra
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
