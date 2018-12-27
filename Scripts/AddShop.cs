using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddShop : MonoBehaviour
{
    bool inHandShop, inHandRemove;
    GameObject shop, remove;

    [SerializeField] MovingCamera movingCamera;
    [SerializeField] GameObject templateShop;
    [SerializeField] GameObject delete;
    [SerializeField] InputField weight;
    public readonly HashSet<GameObject> shops = new HashSet<GameObject>();

    public void GetInHand()
    {
        if (inHandRemove) return;
        inHandShop = true;
        shop = Instantiate(templateShop);
        ShopChangePos();
        shop.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = weight.text;
    }

    public void RemoveShop()
    {
        if (inHandShop) return;
        Destroy(remove);
        inHandRemove = true;
        remove = Instantiate(delete);
        RemoveChangePos();
    }

    public void RemoveAllShop()
    {
        foreach (GameObject o in shops)
            Destroy(o);
        shops.Clear();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (inHandShop) ShopChangePos();
        if (inHandRemove) RemoveChangePos();
    }

    void ShopChangePos()
    {
        shop.transform.position = movingCamera.Camera1.transform.position +
                                  new Vector3(Input.mousePosition.x / movingCamera.Camera1.pixelWidth *
                                              movingCamera.MaxXCamera * 2 - movingCamera.MaxXCamera,
                                              Input.mousePosition.y / movingCamera.Camera1.pixelHeight *
                                              movingCamera.MaxYCamera * 2 - movingCamera.MaxYCamera, 10);
    }

    void RemoveChangePos()
    {
        remove.transform.position = movingCamera.Camera1.transform.position +
                                    new Vector3(Input.mousePosition.x / movingCamera.Camera1.pixelWidth *
                                                movingCamera.MaxXCamera * 2 - movingCamera.MaxXCamera,
                                                Input.mousePosition.y / movingCamera.Camera1.pixelHeight *
                                                movingCamera.MaxYCamera * 2 - movingCamera.MaxYCamera, 10);
    }

    public void UpdateInput()
    {
        weight.text = new string(weight.text.Replace(',', '.').Where(x => char.IsDigit(x) || x == '.').ToArray()) +
                      ".0";
        if (weight.text.Count(x => x == '.') > 1)
        {
            int firstPoint = weight.text.IndexOf('.') + 1;
            weight.text = weight.text.Substring(0, firstPoint) +
                          weight.text.Substring(firstPoint).Replace(".", "");
        }
        float iWeight = Math.Max(0.3f, Math.Min(float.Parse(weight.text.Replace('.', ',')), 3f));
        weight.text = MovingCamera.Round(iWeight, 2).ToString().Replace(',', '.');
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (inHandShop)
            {
                inHandShop = false;
                shops.Add(shop);
            }
            if (inHandRemove)
            {
                inHandRemove = false;
                RaycastHit2D raycastHit2D = Physics2D.Raycast(remove.transform.position, Vector3.forward);
                if ((raycastHit2D.collider?.gameObject.name ?? "") == "Shop(Clone)")
                {
                    shops.Remove(raycastHit2D.collider.gameObject);
                    Destroy(raycastHit2D.collider.gameObject);
                }
                Destroy(remove);
            }
        }
    }
}