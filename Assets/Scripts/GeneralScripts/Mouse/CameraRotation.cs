using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private Transform targetObject; // Объект, вокруг которого будет вращаться камера
    [SerializeField] private float rotationSpeed = 2f; // Скорость вращения

    private Vector3 initialPosition; // Исходная позиция камеры
    private Quaternion initialRotation; // Исходное вращение камеры
    //private bool isRotating = false; // Флаг, указывающий, происходит ли вращение

    private void Start()
    {
        // Сохраняем начальную позицию и вращение камеры
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        LookAtTarget();
        // Проверка зажатия колёсика мыши (обычно это Input.GetMouseButton(2))
        if (Input.GetMouseButton(2))
        {
            // Получение значений осей мыши
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Вращение камеры вокруг целевого объекта
            transform.RotateAround(targetObject.position, Vector3.up, mouseX * rotationSpeed);
            //transform.Rotate(new Vector3(mouseY, 0, 0) * rotationSpeed, Space.Self); // Локальное вращение по вертикали
        }
        else
        {
            //isRotating = false;
        }

        //// Проверка двойного нажатия колёсика мыши
        //if (Input.GetMouseButton(2) && Input.GetKeyDown(KeyCode.Mouse2))
        //{
        //    ResetCameraPosition();
        //}
    }

    private void LookAtTarget()
    {
        transform.LookAt(targetObject.position);
    }

    private void ResetCameraPosition()
    {
        // Возврат камеры в исходную позицию и вращение
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        Debug.Log("Камера возвращена в исходную позицию");
    }
}
