using UnityEngine;

public class JoystickReader
{
    public bool IsPressed => Power > 0f;
    public float Power => joystick.GetDistance();

    UltimateJoystick joystick;

    public JoystickReader(string joystickName) => joystick = UltimateJoystick.GetUltimateJoystick(joystickName);

    public Vector2 Direction2D(float rotationOffset = 0f) => Rotate(
        new Vector2(joystick.HorizontalAxis, joystick.VerticalAxis),
        rotationOffset);

    public Vector3 Direction3D(float rotationOffset = 0f) => Rotate(
        new Vector3(joystick.HorizontalAxis, 0, joystick.VerticalAxis),
        rotationOffset);

    Vector2 Rotate(Vector2 vector, float degrees) =>
        Quaternion.Euler(0, 0, degrees) * vector;

    Vector3 Rotate(Vector3 vector, float degrees) =>
        Quaternion.Euler(0, degrees, 0) * vector;
}