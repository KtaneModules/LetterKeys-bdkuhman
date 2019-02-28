using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

public class LetteredKeys : MonoBehaviour {

    public KMSelectable[] buttons;

    int magicNum;
    public TextMesh textMesh;

    void Start()
    {
        Init();
    }

    void Init()
    {
        magicNum = Random.Range(0, 100);
        string[] temp2 = { "A", "B", "C", "D" };
        List<int> temp1 = new List<int>();
        while (temp1.Count != 4)
        {
            int i = Random.Range(0, 4);
            if (!temp1.Contains(i))
            {
                temp1.Add(i);
            }
        }

        TextMesh numberText = textMesh;
        numberText.text = magicNum.ToString();
        for (int i = 0; i < buttons.Length; i++)
        {
            TextMesh buttonText = buttons[i].GetComponentInChildren<TextMesh>();
            buttonText.text = temp2[temp1[i]];
            int j = i;
            buttons[i].OnInteract += delegate () { OnPress(temp2[temp1[j]], buttons[j]); return false; };
        }
    }

    void OnPress(string button, KMSelectable buttonObj)
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        List<string> responses = null;
        int batteryCount = 0;
        responses = GetComponent<KMBombInfo>().QueryWidgets(KMBombInfo.QUERYKEY_GET_BATTERIES, null);
        foreach (string response in responses)
        {
            Dictionary<string, int> responseDict = JsonConvert.DeserializeObject<Dictionary<string, int>>(response);
            batteryCount += responseDict["numbatteries"];
        }

        string serial = "";
        responses = GetComponent<KMBombInfo>().QueryWidgets(KMBombInfo.QUERYKEY_GET_SERIAL_NUMBER, null);
        foreach (string response in responses)
        {
            Dictionary<string, string> responseDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
            serial = responseDict["serial"];
        }
        //write(batteryCount.ToString());
        // write(serial);
       
        string s = getCorrectButton(batteryCount, serial);
        if (button.Equals(s))
        {
            GetComponent<KMBombModule>().HandlePass();
        }
        else
        {
            GetComponent<KMBombModule>().HandleStrike();
        }
        
    }

    private string getCorrectButton(int batteryCount, string serial)
    {
        if (magicNum == 69)
        {
            return "D";
        }
        else if (magicNum % 6 == 0)
        {
            return "A";
        }
        else if (magicNum % 3 == 0 && batteryCount >= 2)
        {
            return "B";
        }
        else if (serial.Contains("E") || serial.Contains("C") || serial.Contains("3"))
        {
            if (magicNum >= 22 && magicNum <= 79)
            {
                return "B";
            }
            else
            {
                return "C";
            }
        }
        else if (magicNum < 46)
        {
            return "D";
        }
        else
        {
            return "A";
        }
    }

    public string TwitchHelpMessage = "!{0} press b";
    public KMSelectable[] ProcessTwitchCommand(string command)
    {
        if (command.StartsWith("press ", System.StringComparison.InvariantCultureIgnoreCase))
        {
            Match match = Regex.Match(command, "[1-4a-d]", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return null;
            }

            int buttonID;
            if (int.TryParse(match.Value, out buttonID))
            {
                return new KMSelectable[] { buttons[buttonID - 1] };
            }

            foreach (KMSelectable button in buttons)
            {
                if (match.Value.Equals(button.GetComponentInChildren<TextMesh>().text, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return new KMSelectable[] { button };
                }
            }
        }
        return null;
    }

    /*
    private void write(string s)
    {
        using (StreamWriter writer = new StreamWriter("E:\\Documents\\test.txt", true))
        {
            writer.WriteLine(s);
        }
    }
    */
}
