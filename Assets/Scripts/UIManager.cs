using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour {

    [SerializeField] private GameObject _canvasObj;

    [SerializeField] private GameObject _cameraPrefabObj;
    [SerializeField] private GameObject _textPrefabObj;
    [SerializeField] private GameObject _imagePrefabObj;
    [SerializeField] private GameObject _buttonPrefabObj;
    [SerializeField] private GameObject _togglePrefabObj;
    [SerializeField] private GameObject _sliderPrefabObj;
    [SerializeField] private GameObject _dropDownPrefabObj;
    [SerializeField] private GameObject _inputFieldPrefabObj;

    private string _jsonStr;   //最後に受け取ったJson

    public string JsonStr {
        set {
            _jsonStr = value;
        }
    }

    /// <summary>
    /// 受け取った_jsonStrの解析とそれに伴ったアクションを実行
    /// </summary>
    public void SignalBehaviour() {
        Debug.Log(_jsonStr);
        var json = JsonNode.Parse(_jsonStr);
        var signal = json["Signal"].Get<string>();

        switch (signal) {
            case "UIInit":
                UIInit(json);
                break;
            case "Null":
                break;
            case "Quit":
                Debug.Log("Quit Signal");
                gameObject.GetComponent<ClientMaster>()._isEnableQuit = true;
                Application.Quit();
                break;
            default:
                Debug.LogError("Unknown Signal");
                break;
        }
    }

    /// <summary>
    /// JsonからUIを再現
    /// </summary>
    private void UIInit(JsonNode json) {
        Screen.SetResolution(int.Parse(json["Width"].Get<string>()), int.Parse(json["Height"].Get<string>()), false);

        foreach (var uiItem in json["UiItems"]) {
            //Debug.Log(uiItem["ObjectName"].Get<string>());
            GameObject g;
            //Debug.Log(ObjDetermination(uiItem));
            switch (ObjDetermination(uiItem)) {
                case "Camera":
                    g = Instantiate(_cameraPrefabObj, _canvasObj.transform);
                    break;
                case "Text":
                    g = Instantiate(_textPrefabObj, _canvasObj.transform);
                    break;
                case "Image":
                    g = Instantiate(_imagePrefabObj, _canvasObj.transform);
                    break;
                case "Button":
                    g = Instantiate(_buttonPrefabObj, _canvasObj.transform);
                    break;
                case "Toggle":
                    g = Instantiate(_togglePrefabObj, _canvasObj.transform);
                    break;
                case "Slider":
                    g = Instantiate(_sliderPrefabObj, _canvasObj.transform);
                    break;
                case "Dropdown":
                    g = Instantiate(_dropDownPrefabObj, _canvasObj.transform);
                    break;
                case "InputField":
                    g = Instantiate(_inputFieldPrefabObj, _canvasObj.transform);
                    break;
                default:
                    Debug.LogError("Unknown Obj Type");
                    g = new GameObject();
                    g.name = "EmptyObject";
                    break;
            }
            g.name = uiItem["ObjectName"].Get<string>();
            g.GetComponent<IdTag>().ID = int.Parse(uiItem["ID"].Get<string>());

            foreach (var component in uiItem["Components"]) {
                //Debug.Log(component["Name"].Get<string>());

                switch (component["Name"].Get<string>()) {
                    case "Camera":
                        var camera = g.GetComponent<Camera>();
                        InitRectTransform(g.GetComponent<RectTransform>(), uiItem);
                        var pos = g.GetComponent<RectTransform>().position;
                        g.GetComponent<RectTransform>().position = new Vector3(pos.x, pos.y, pos.z - 10f);

                        switch (component["ClearFlags"].Get<string>()) {
                            case "Skybox":
                                camera.clearFlags = CameraClearFlags.Skybox;
                                break;
                            case "SolidColor":
                                camera.clearFlags = CameraClearFlags.SolidColor;
                                break;
                            case "DepthOnly":
                                camera.clearFlags = CameraClearFlags.Depth;
                                break;
                            case "DontClear":
                                camera.clearFlags = CameraClearFlags.Nothing;
                                break;
                            default:
                                Debug.LogError("Unknown CameraClearFlags");
                                break;
                        }

                        if (component["Projection"].Get<string>() == "True") {
                            camera.orthographic = true;
                        } else {
                            camera.orthographic = false;
                        }

                        camera.backgroundColor = DeserializeJsonParam.JsonToColor(component["BackGround"]);
                        camera.orthographicSize = float.Parse(component["Size"].Get<string>());
                        camera.farClipPlane = 30f;

                        _canvasObj.GetComponent<Canvas>().worldCamera = camera;

                        break;
                    case "Text":
                        var text = g.GetComponent<Text>();
                        InitRectTransform(g.GetComponent<RectTransform>(), uiItem);

                        text.text = component["Text"].Get<string>().UnicodeToStrings();
                        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                        text.fontSize = int.Parse(component["FontSize"].Get<string>());
                        text.alignment = DeserializeJsonParam.JsonToTextAnchor(component["Alignment"]);
                        text.color = DeserializeJsonParam.JsonToColor(component["Color"]);

                        break;
                    case "Image":
                        var image = g.GetComponent<Image>();
                        InitRectTransform(g.GetComponent<RectTransform>(), uiItem);

                        if (component["PngBase64Data"].Get<string>() != "Null") {
                            var pngBase64Data = component["PngBase64Data"].Get<string>();
                            var pngWH = DeserializeJsonParam.JsonToVector2(component["PngWH"]);
                            image.sprite = DeserializeJsonParam.DeserializeBase64PngToSprite(pngBase64Data, pngWH);
                        }

                        image.color = DeserializeJsonParam.JsonToColor(component["Color"]);

                        break;
                    case "Button":
                        var button = g.GetComponent<Button>();
                        InitRectTransform(g.GetComponent<RectTransform>(), uiItem);

                        if (component["Interactable"].Get<string>() == "True") {
                            button.interactable = true;
                        } else {
                            button.interactable = false;
                        }

                        button.colors = DeserializeJsonParam.JsonToColorBlock(component);

                        foreach (Transform child in g.transform.transform) {
                            if (child.gameObject.GetComponent<Text>()) {
                                child.gameObject.GetComponent<Text>().text = component["ButtonText"].Get<string>().UnicodeToStrings();
                            }
                        }

                        button.onClick.AddListener(() => gameObject.GetComponent<ParamAction>().OnButtonClicked(button));

                        break;
                    case "Toggle":
                        var toggle = g.GetComponent<Toggle>();
                        InitRectTransform(g.GetComponent<RectTransform>(), uiItem);

                        if (component["Interactable"].Get<string>() == "True") {
                            toggle.interactable = true;
                        } else {
                            toggle.interactable = false;
                        }

                        toggle.colors = DeserializeJsonParam.JsonToColorBlock(component);

                        foreach (Transform child in g.transform.transform) {
                            if (child.gameObject.GetComponent<Text>()) {
                                child.gameObject.GetComponent<Text>().text = component["ToggleLabel"].Get<string>();
                            }
                        }

                        toggle.onValueChanged.AddListener((x) => gameObject.GetComponent<ParamAction>().OnToggleValueChanged(toggle));

                        break;
                    case "Slider":
                        var slider = g.GetComponent<Slider>();
                        InitRectTransform(g.GetComponent<RectTransform>(), uiItem);

                        if (component["Interactable"].Get<string>() == "True") {
                            slider.interactable = true;
                        }else{
                            slider.interactable = false;
                        }

                        slider.colors = DeserializeJsonParam.JsonToColorBlock(component);

                        switch (component["Direction"].Get<string>()) {
                            case "ButtomToTop":
                                slider.direction = Slider.Direction.BottomToTop;
                                break;
                            case "LeftToRight":
                                slider.direction = Slider.Direction.LeftToRight;
                                break;
                            case "RightToLeft":
                                slider.direction = Slider.Direction.RightToLeft;
                                break;
                            case "TopToBottom":
                                slider.direction = Slider.Direction.TopToBottom;
                                break;
                            default:
                                Debug.LogError("Unknown Slider Direction");
                                break;
                        }

                        slider.minValue = float.Parse(component["MinValue"].Get<string>());
                        slider.maxValue = float.Parse(component["MaxValue"].Get<string>());
                        slider.value = float.Parse(component["Value"].Get<string>());

                        foreach (var child in g.GetComponentsInChildren<Transform>()) {
                            var r = child.gameObject.GetComponent<RectTransform>();
                            r.rotation = Quaternion.Euler(DeserializeJsonParam.JsonToVector3(component[child.gameObject.name + "RectTransform"]["Rotation"]));
                            r.localPosition = DeserializeJsonParam.JsonToVector3(component[child.gameObject.name + "RectTransform"]["Position"]);
                            r.localScale = DeserializeJsonParam.JsonToVector3(component[child.gameObject.name + "RectTransform"]["LocalScale"]);
                            r.anchorMin = DeserializeJsonParam.JsonToVector2(component[child.gameObject.name + "RectTransform"]["AnchorMin"]);
                            r.anchorMax = DeserializeJsonParam.JsonToVector2(component[child.gameObject.name + "RectTransform"]["AnchorMax"]);
                            r.anchoredPosition = DeserializeJsonParam.JsonToVector2(component[child.gameObject.name + "RectTransform"]["AnchoredPosition"]);
                            r.sizeDelta = DeserializeJsonParam.JsonToVector2(component[child.gameObject.name + "RectTransform"]["SizeDelta"]);
                            r.pivot = DeserializeJsonParam.JsonToVector2(component[child.gameObject.name + "RectTransform"]["Pivot"]);
                        }

                        slider.onValueChanged.AddListener((x) => gameObject.GetComponent<ParamAction>().OnSliderValueChanged(slider));

                        break;
                    case "Dropdown":
                        var dropdown = g.GetComponent<Dropdown>();
                        InitRectTransform(g.GetComponent<RectTransform>(), uiItem);
                        dropdown.ClearOptions();
                        dropdown.AddOptions(DeserializeJsonParam.JsonToDropDownOption(component["Options"]));
                        dropdown.value = int.Parse(component["Value"].Get<string>());

                        dropdown.onValueChanged.AddListener((x) => gameObject.GetComponent<ParamAction>().OnDropdownValueChanged(dropdown));

                        break;
                    case "InputField":
                        var inputfield = g.GetComponent<InputField>();
                        InitRectTransform(g.GetComponent<RectTransform>(), uiItem);

                        foreach (Transform child in g.transform.transform) {
                            if (child.gameObject.name == "Placeholder") {
                                child.gameObject.GetComponent<Text>().text = component["PlaceholderText"].Get<string>().UnicodeToStrings();
                            }
                        }

                        inputfield.onValueChanged.AddListener((x) => gameObject.GetComponent<ParamAction>().OnInputFieldValueChanged(inputfield));
                        inputfield.onEndEdit.AddListener((x) => gameObject.GetComponent<ParamAction>().OnInputFieldEndEdit(inputfield));

                        break;
                    default:
                        Debug.LogError("Unknown Component");
                        break;
                }
            }
        }
    }

    /// <summary>
    /// なんのために存在するオブジェクトか判別する
    /// </summary>
    /// <param name="uiItem"></param>
    /// <returns></returns>
    private string ObjDetermination(JsonNode uiItem) {
        string res = "";
        foreach (var component in uiItem["Components"]) {
            var name = component["Name"].Get<string>();
            if (name == "Camera") {
                return name;
            }

            if (name == "Button" || name == "Dropdown" || name == "InputField" || name == "Slider" || name == "Toggle") {
                return name;
            }

            if (name == "Image" || name == "Text") {
                res = name;
            }
        }
        return res;
    }

    private void InitRectTransform(RectTransform t, JsonNode uiItem) {
        t.position = DeserializeJsonParam.JsonToVector2(uiItem["Position"]);
        var pos = t.position;
        var json = JsonNode.Parse(_jsonStr);
        var canvasPos = DeserializeJsonParam.JsonToVector2(json["CanvasPos"]);
        t.position = new Vector3(pos.x - canvasPos.x, pos.y - canvasPos.y, pos.z);

        t.sizeDelta = DeserializeJsonParam.JsonToVector2(uiItem["SizeDelta"]);
        t.anchorMin = DeserializeJsonParam.JsonToVector2(uiItem["AnchorMin"]);
        t.anchorMax = DeserializeJsonParam.JsonToVector2(uiItem["AnchorMax"]);
        t.pivot = DeserializeJsonParam.JsonToVector2(uiItem["Pivot"]);
        t.rotation = Quaternion.Euler(DeserializeJsonParam.JsonToVector3(uiItem["Rotation"]));
        t.localScale = DeserializeJsonParam.JsonToVector3(uiItem["Scale"]);
    }
}
