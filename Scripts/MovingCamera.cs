using System;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public class MovingCamera : MonoBehaviour
{
    [SerializeField] Camera camera;

    public Camera Camera1
    {
        get => camera;
        private set => camera = value;
    }

    [SerializeField] GameObject sliceMap;

    float widthSliceMap, heightSliceMap;
    float marginLeft, marginRight, marginTop, marginBottom;

    public float MaxXCamera { get; private set; }
    public float MaxYCamera { get; private set; }

    void Start()
    {
        widthSliceMap = sliceMap.GetComponent<SpriteRenderer>().sprite.rect.width /
                        sliceMap.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        heightSliceMap = sliceMap.GetComponent<SpriteRenderer>().sprite.rect.height /
                         sliceMap.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;

        MaxYCamera = 5;
        MaxXCamera = Camera1.pixelWidth * MaxYCamera / Camera1.pixelHeight;
        Debug.Log($"w={MaxXCamera} h={MaxYCamera}");

        marginRight = ((int) Math.Ceiling(MaxXCamera / widthSliceMap) + 0.5f) * widthSliceMap;
        marginLeft = -marginRight;
        marginTop = ((int) Math.Ceiling(MaxYCamera / heightSliceMap) + 0.5f) * heightSliceMap;
        marginBottom = -marginTop;

        int maxI = (int) (marginRight / widthSliceMap);
        int maxJ = (int) (marginTop / heightSliceMap);
        for (int i = (int) (marginLeft / widthSliceMap); i <= maxI; i++)
            for (int j = (int) (marginBottom / widthSliceMap); j <= maxJ; j++)
            {
                GameObject copySliceMap = Instantiate(sliceMap);
                copySliceMap.transform.position = new Vector2(i * widthSliceMap, j * heightSliceMap);
                copySliceMap.transform.parent = transform;
                copySliceMap.name = $"S - {i * widthSliceMap}, {j * heightSliceMap}";
            }
    }

    Vector3 startedPosition;
    [SerializeField] float scaleMouseMove = 0.02f;

    void FixedUpdate()
    {
        if (Camera1.transform.position.x + MaxXCamera > marginRight)
            ChangeMarginX(true);
        else if (Camera1.transform.position.x - MaxXCamera < marginLeft)
            ChangeMarginX(false);
        else if (Camera1.transform.position.y + MaxYCamera > marginTop)
            ChangeMarginY(true);
        else if (Camera1.transform.position.y - MaxYCamera < marginBottom)
            ChangeMarginY(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2)) startedPosition = Input.mousePosition;
        else if (Input.GetKey(KeyCode.Mouse2))
        {
            Camera1.transform.position += (startedPosition - Input.mousePosition) * scaleMouseMove;
            startedPosition = Input.mousePosition;
        }
    }

    void ChangeMarginX(bool right)
    {
        int fact = right ? 1 : -1;
        float curX = Round((right ? marginLeft : marginRight) + widthSliceMap / 2 * fact, 2);
        float futX = Round((right ? marginRight : marginLeft) + widthSliceMap / 2 * fact, 2);
        marginRight += widthSliceMap * fact;
        marginLeft += widthSliceMap * fact;
        for (float i = marginBottom + heightSliceMap / 2; i < marginTop; i += heightSliceMap)
            try
            {
                i = Round(i, 2);
                GameObject findedObject = GameObject.Find($"S - {curX}, {i}");
                findedObject.transform.position = new Vector2(futX, i);
                findedObject.name = $"S - {futX}, {i}";
            }
            catch (Exception e)
            {
                throw new Exception("Не нашел - " + $"\"S - {curX}, {i}\"\n{e.Message}");
            }
    }

    void ChangeMarginY(bool top)
    {
        int fact = top ? 1 : -1;
        float curY = Round((top ? marginBottom : marginTop) + heightSliceMap / 2 * fact, 2);
        float futY = Round((top ? marginTop : marginBottom) + heightSliceMap / 2 * fact, 2);
        marginTop += heightSliceMap * fact;
        marginBottom += heightSliceMap * fact;
        for (float i = marginLeft + widthSliceMap / 2; i < marginRight; i += widthSliceMap)
            try
            {
                i = Round(i, 2);
                GameObject findedObject = GameObject.Find($"S - {i}, {curY}");
                findedObject.transform.position = new Vector2(i, futY);
                findedObject.name = $"S - {i}, {futY}";
            }
            catch (Exception e)
            {
                throw new Exception("Не нашел - " + $"\"S - {i}, {curY}\"\n{e.Message}");
            }
    }

    public static float Round(float value, int countAfterComma)
    {
        int z = Enumerable.Range(0, countAfterComma).Aggregate(1, (i, i1) => i * 10);
        return (float) (Math.Round(value * z) / z);
    }
}