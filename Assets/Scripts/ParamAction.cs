using MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// パラメータの操作を行ったときの動作記述
/// </summary>
public class ParamAction : MonoBehaviour {

    private ClientMaster _cm;

    private void Awake() {
        _cm = gameObject.GetComponent<ClientMaster>();
    }

    public void OnButtonClicked(Button button) {
        var dic = new Dictionary<string, string>();

        dic["Time"] = UnixTime();
        dic["Signal"] = "Action";
        dic["Component"] = "Button";
        dic["Action"] = "OnClicked";
        dic["Name"] = button.name;
        dic["Id"] = button.gameObject.GetComponent<IdTag>().ID.ToString();

        _cm.SendSignal(Json.Serialize(dic));
    }

    public void OnInputFieldValueChanged(InputField inputField) {
        var dic = new Dictionary<string, string>();

        dic["Time"] = UnixTime();
        dic["Signal"] = "Action";
        dic["Component"] = "InputField";
        dic["Action"] = "OnValueChanged";
        dic["Name"] = inputField.name;
        dic["Value"] = inputField.text;
        dic["Id"] = inputField.gameObject.GetComponent<IdTag>().ID.ToString();

        _cm.SendSignal(Json.Serialize(dic));
    }

    public void OnInputFieldEndEdit(InputField inputField) {
        var dic = new Dictionary<string, string>();

        dic["Time"] = UnixTime();
        dic["Signal"] = "Action";
        dic["Component"] = "InputField";
        dic["Action"] = "OnEndEdit";
        dic["Name"] = inputField.name;
        dic["Value"] = inputField.text;
        dic["Id"] = inputField.gameObject.GetComponent<IdTag>().ID.ToString();

        _cm.SendSignal(Json.Serialize(dic));
    }

    public void OnSliderValueChanged(Slider slider) {
        var dic = new Dictionary<string, string>();

        dic["Time"] = UnixTime();
        dic["Signal"] = "Action";
        dic["Component"] = "Slider";
        dic["Action"] = "OnValueChanged";
        dic["Name"] = slider.name;
        dic["Value"] = slider.value.ToString();
        dic["Id"] = slider.gameObject.GetComponent<IdTag>().ID.ToString();

        _cm.SendSignal(Json.Serialize(dic));
    }

    public void OnToggleValueChanged(Toggle toggle) {
        var dic = new Dictionary<string, string>();

        dic["Time"] = UnixTime();
        dic["Signal"] = "Action";
        dic["Component"] = "Toggle";
        dic["Action"] = "OnValueChanged";
        dic["Name"] = toggle.name;
        dic["Value"] = toggle.isOn.ToString();
        dic["Id"] = toggle.gameObject.GetComponent<IdTag>().ID.ToString();

        _cm.SendSignal(Json.Serialize(dic));
    }

    public void OnDropdownValueChanged(Dropdown dropdown) {
        var dic = new Dictionary<string, string>();

        dic["Time"] = UnixTime();
        dic["Signal"] = "Action";
        dic["Component"] = "Dropdown";
        dic["Action"] = "OnValueChanged";
        dic["Name"] = dropdown.name;
        dic["Value"] = dropdown.value.ToString();
        dic["Id"] = dropdown.gameObject.GetComponent<IdTag>().ID.ToString();

        _cm.SendSignal(Json.Serialize(dic));
    }

    private string UnixTime() {
        return (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString();
    }
}
