using UnityEngine;

public class MoveObjectAlongX : MonoBehaviour
{
    public float speed = 5f; // Скорость движения по оси X

    void Update()
    {
        // Перемещаем объект по оси X с заданной скоростью
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
}
