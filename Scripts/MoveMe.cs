using System;
using System.Collections.Generic;
using BackpackTask;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveMe : MonoBehaviour
{
    [SerializeField] MovingCamera movingCamera;
    [SerializeField] Slider factSpeed;
    bool isMoving;
    Vector2 beginPos, endPos;
    float beginTime, endTime;

    Queue<Item> queue;
    Item curTarget;

    float GetAngleMouse()
    {
        var mouseRegCenter = new Vector2(
            Input.mousePosition.x - movingCamera.Camera1.pixelWidth / 2 +
            movingCamera.Camera1.transform.position.x / (movingCamera.MaxXCamera * 2) * movingCamera.Camera1.pixelWidth,
            Input.mousePosition.y - movingCamera.Camera1.pixelHeight / 2 +
            movingCamera.Camera1.transform.position.y / (movingCamera.MaxYCamera * 2) *
            movingCamera.Camera1.pixelHeight);
        float hyp = (float) Math.Sqrt(mouseRegCenter.y * mouseRegCenter.y + mouseRegCenter.x * mouseRegCenter.x);
        if (mouseRegCenter.x > 0)
            return (float) (Math.Asin(mouseRegCenter.y / hyp) / Math.PI * 180);
        return 180 - (float) (Math.Asin(mouseRegCenter.y / hyp) / Math.PI * 180);
    }

    float GetAngleRanDirection()
    {
        Vector2 mouseRegCenter = endPos - beginPos;
        float hyp = (float) Math.Sqrt(mouseRegCenter.y * mouseRegCenter.y + mouseRegCenter.x * mouseRegCenter.x);
        if (mouseRegCenter.x > 0)
            return (float) (Math.Asin(mouseRegCenter.y / hyp) / Math.PI * 180);
        return 180 - (float) (Math.Asin(mouseRegCenter.y / hyp) / Math.PI * 180);
    }

    public void Move(Queue<Item> queue)
    {
        transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "5";
        this.queue = queue;
        curTarget = this.queue.Dequeue();
        beginPos = transform.position;
        endPos = curTarget.shop.transform.position;
        beginTime = Time.time;
        endTime = Distance(beginPos, endPos) / (factSpeed.value * 10) + beginTime;
        isMoving = true;
        Debug.Log($"{beginTime} ---> {endTime}");
        Debug.Log($"({endPos.x}, {endPos.y})");
    }

    void Update()
    {
        if (!isMoving)
            transform.localEulerAngles = Vector3.forward * (GetAngleMouse() - 90);
        else
        {
            if (Time.time > endTime)
                if (queue.Count > 0)
                {
                    if (curTarget.orderValue != 0)
                        transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                            MovingCamera.Round(float.Parse(transform.GetChild(0).GetChild(0)
                                                               .GetComponent<TextMeshProUGUI>()
                                                               .text
                                                               .Replace('.', ',')) -
                                               MovingCamera.Round(
                                                   float.Parse(curTarget.shop.transform.GetChild(0).GetChild(0)
                                                                   .GetComponent<TextMeshProUGUI>().text
                                                                   .Replace('.', ',')),
                                                   2), 2)
                                .ToString().Replace(',', '.');
                    else
                        transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "5";
                    transform.position = endPos;
                    beginTime = Time.time;
                    beginPos = curTarget.shop.transform.position;
                    curTarget = queue.Dequeue();
                    endPos = curTarget.shop.transform.position;
                    endTime = Distance(beginPos, endPos) / (factSpeed.value * 10) + beginTime;
                }
                else
                {
                    transform.position = endPos;
                    isMoving = false;
                    return;
                }
            transform.localEulerAngles = Vector3.forward * (GetAngleRanDirection() - 90);
            transform.position = GetPosWithInterpolation();
        }
    }

    Vector2 GetPosWithInterpolation() =>
        new Vector2(beginPos.x + (Time.time - beginTime) / (endTime - beginTime) * (endPos.x - beginPos.x),
                    beginPos.y + (Time.time - beginTime) / (endTime - beginTime) * (endPos.y - beginPos.y));

    static float Distance(Vector3 pos1, Vector3 pos2) =>
        (float) Math.Sqrt((pos1.x - pos2.x) * (pos1.x - pos2.x) +
                          (pos1.y - pos2.y) * (pos1.y - pos2.y));
}