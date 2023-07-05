using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryOuts : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject MyCandleGO;
    GameObject CenterGameObject; 
    [SerializeField]GameObject[] OtherCandles; 

    Vector3 directionMyCandleCenter;
    Vector3 directionOtherCandleCenter;
    Vector3 perpendicular;
    float[] anglesNotOrdered;
    float[] anglesOrdered;
    int[] idsOrdered;

    Material ogMat;

    Material instMat;

    [SerializeField]Sprite sp;

    void Start(){
        ogMat = this.GetComponent<Renderer>().material;
        instMat = new Material(ogMat);

        instMat.mainTexture = sp.texture;

        this.GetComponent<Renderer>().material = instMat;
            
        
    }

    // void GetOrderOfCandleHolders()
    // {   
    //     MyCandleGO = this.gameObject;
    //     CenterGameObject = GameObject.Find("Center");


    //     directionMyCandleCenter = CenterGameObject.transform.position - MyCandleGO.transform.position;
    //     directionMyCandleCenter.y = 0;

    //     int i = 0;
    //     anglesNotOrdered = new float[4]; // CHANGE
    //     foreach (var item in OtherCandles)
    //     {
    //         directionOtherCandleCenter = CenterGameObject.transform.position - item.transform.position;
    //         directionOtherCandleCenter.y = 0;

    //         anglesNotOrdered[i] = SignedAngleBetween(directionMyCandleCenter, directionOtherCandleCenter, Vector3.up);
            
    //         print("Nombre: " + item.name + " Angulo: " + anglesNotOrdered[i]);
    //         i++;

    //     }

    //     anglesOrdered = (float[])anglesNotOrdered.Clone();
    //     System.Array.Sort(anglesOrdered);

    //     int k = 0;
    //     idsOrdered = new int[4]; // CHANGE
    //     foreach (float or in anglesOrdered)
    //     {
    //         for (int j = 0; j < anglesNotOrdered.Length; j++)
    //         {
    //             if (or == anglesNotOrdered[j]){
    //                 idsOrdered[k] = j;
    //                 k++;
    //             }
    //         }
    //     }
        
    // }

    // float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n){
        
    //     float angle = Vector3.Angle(a,b);
    //     float sign = Mathf.Sign(Vector3.Dot(n,Vector3.Cross(a,b)));

    //     float signed_angle = angle * sign;

    //     float angle360 = signed_angle;
    //     if (signed_angle < 0){
    //         angle360 = 360 + signed_angle;
    //     }

    //     return angle360;
    // }
}
