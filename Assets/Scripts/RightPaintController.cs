using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightPaintController : MonoBehaviour {

    private LineRenderer currentStroke;
    private int numClicks = 0;
    private ArrayList lines = new ArrayList();
    private ArrayList archivedLines = new ArrayList();
    private List<Gradient> lineGradients = new List<Gradient>();
    private List<Material> primitiveMaterials = new List<Material>();
    private Gradient selectedLineGradient;
    private Material selectedPrimitiveMaterial;
    private List<Color> colors = new List<Color> {
         new Color(231f/255, 76f/255, 60f/255),
         new Color(230f/255, 126f/255, 34f/255),
         new Color(241f/255, 196f/255, 15f/255),
         new Color(46f/255, 204f/255, 113f/255),
         new Color(26f/255, 188f/255, 156f/255),
         new Color(52f/255, 152f/255, 219f/255),
         new Color(155f/255, 89f/255, 182f/255),
         new Color(236f/255, 240f/255, 241f/255),
         new Color(149f/255, 165f/255, 166f/255),
         new Color(52f/255, 73f/255, 94f/255)
    };
    private Material lineMaterial;
    private float strokeWidth = 0.05f;
    private float strokeHeight = 0.05f;
	// Use this for initialization
	void Start () {

        lineMaterial = new Material(Shader.Find("Sprites/Default"));
        foreach(Color color in colors)
        {
            float alpha = 1.0f;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.5f), new GradientAlphaKey(0.0f, 1.0f) }
                );
            lineGradients.Add(gradient);

            Material diffuse = new Material(Shader.Find("Transparent/Diffuse"));
            diffuse.color = color;
            primitiveMaterials.Add(diffuse);
        }
        selectedLineGradient = lineGradients[0];
        selectedPrimitiveMaterial = primitiveMaterials[0];
        
    }

    // Update is called once per frame
    void Update () {

        transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        bool down = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        bool stillDown = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        //Debug.Log(down);

        bool button2Down = OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch);
        bool button2StillDown = OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch);

        bool button1Down = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch);
        bool button1StillDown = OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch);

        if (down)
        {
            GameObject go = new GameObject();
            lines.Add(go);
            currentStroke = go.AddComponent<LineRenderer>();
            currentStroke.material = lineMaterial;
            currentStroke.colorGradient = selectedLineGradient;
            currentStroke.SetWidth(strokeWidth, strokeHeight);
    
            numClicks = 0;
            archivedLines.Clear();
        } else if (stillDown)
        {
            currentStroke.SetVertexCount(numClicks + 1);
            currentStroke.SetPosition(numClicks, transform.localPosition);
            numClicks++;
        }

        if(button2Down)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Material diffuse = new Material(Shader.Find("Transparent/Diffuse"));
            go.GetComponent<MeshRenderer>().material = selectedPrimitiveMaterial;
            go.transform.position = transform.localPosition;
            go.transform.rotation = transform.localRotation;
            go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            lines.Add(go);
            archivedLines.Clear();
            Debug.Log("In button 2 down");
        }

        if (button1Down)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.GetComponent<MeshRenderer>().material = selectedPrimitiveMaterial;
            go.transform.position = transform.localPosition;
            go.transform.rotation = transform.localRotation;
            go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            lines.Add(go);
            archivedLines.Clear();
            Debug.Log("Button 1 down");
        }

    }

    public Transform explosionPrefab;

    void ChangeStrokeWidth(float w, float h)
    {
        strokeHeight = h;
        strokeWidth = w;
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts) {

            Debug.Log("right paint all contacts" + contact.otherCollider.gameObject.name);

            //Debug.Log("Collision object outside  " + contact.otherCollider.gameObject.name);
            if (contact.otherCollider.gameObject.name == "ClearButton")
            {
                Debug.Log("Clear Everything");
                foreach (GameObject gobj in lines)
                {
                    Destroy(gobj);
                }
            }

            else if (contact.otherCollider.gameObject.name == "UndoButton")
            {
                if (lines.Count > 0)
                {
                    Debug.Log("Undo last move");
                    GameObject lastStroke = (GameObject)lines[lines.Count - 1];
                    lastStroke.SetActive(false);
                    lines.Remove(lastStroke);
                    Debug.Log("Undo move:: " + lastStroke);
                    archivedLines.Add(lastStroke);
                }
            }

            else if (contact.otherCollider.gameObject.name == "RedoButton")
            {
                Debug.Log("Redo last move");
                if (archivedLines.Count > 0)
                {
                    GameObject lastStroke = (GameObject)archivedLines[0];
                    lastStroke.SetActive(true);
                    archivedLines.Remove(lastStroke);
                    lines.Add(lastStroke);
                    Debug.Log("Instantiating stroke: " + lastStroke);
                }
            }

            else if (contact.otherCollider.gameObject.name == "BrushButton1")
            {
                Debug.Log("Brush Button 1");
                ChangeStrokeWidth(0.01f, 0.01f);
            }

            else if (contact.otherCollider.gameObject.name == "BrushButton2")
            {
                Debug.Log("Brush Button 2");
                ChangeStrokeWidth(0.05f, 0.05f);
            }

            else if (contact.otherCollider.gameObject.name == "BrushButton3")
            {
                Debug.Log("Brush Button 3");
                ChangeStrokeWidth(0.19f, 0.19f);
            }

            else if (contact.otherCollider.gameObject.name.StartsWith("Color"))
            {
               
                try
                {
                    int m = Int32.Parse(contact.otherCollider.gameObject.name.Substring(5));
                    Debug.Log("Color " + m);
                    selectedLineGradient = lineGradients[m];
                    selectedPrimitiveMaterial = primitiveMaterials[m];
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
                
                
            }

        }

    }
}
