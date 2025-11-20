using Unity.VisualScripting;
using UnityEngine;

public class Calculator : MonoBehaviour
{
    [Header("Numbers to calculate")]
    [SerializeField] private float number1 = 10f;
    [SerializeField] private float number2 = 5f;


    void Start()
    {
        Debug.Log("First Result");
        Add();
        Subtract();
        Multiply();
        Divide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private float Add()
    {
        float result = number1 + number2;
        return result;
    }

    private float Subtract()
    {
        float result = number1 - number2;
        return result;
    }

    private float Multiply()
    {
        float result = number1 * number2;
        return result;
    }

    private float Divide()
    {
        float result = number1 / number2;
        return result;
    }

    private string GetPressedOperation()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            return "Add";
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            return "Subtract";
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)){
            return "Multiply";
        }
        if(Input.GetKeyDown(KeyCode.Alpha4)){
            return "Divide";
        }
        return null;
    }



    private void OutputResult(string result){
        switch(result){
            case "Add":
                Debug.Log("Add: " + Add());
                break;
            case "Subtract":
                Debug.Log("Subtract: " + Subtract());
                break;
            case "Multiply":
                Debug.Log("Multiply: " + Multiply());
                break;
            case "Divide":
                Debug.Log("Divide: " + Divide());
                break;
            default:
                Debug.Log("Invalid operation");
                break;
        }   

    }




}
