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
}
