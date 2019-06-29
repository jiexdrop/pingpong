using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Join : MonoBehaviour
{
    public GameObject joinPanel;
    public GameObject serverPanel;
    public GameObject clientPanel;

    public InputField ipInputField;

    public void ServerButton()
    {
        HideAll();
        serverPanel.SetActive(true);
    }

    public void HideAll()
    {
        joinPanel.SetActive(false);
        serverPanel.SetActive(false);
        clientPanel.SetActive(false);
    }

    public void ClientButton()
    {
        HideAll();
        clientPanel.SetActive(true);
        clientPanel.GetComponent<Client>().StartServerWithIp(ipInputField.text);
    }

    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }
}
