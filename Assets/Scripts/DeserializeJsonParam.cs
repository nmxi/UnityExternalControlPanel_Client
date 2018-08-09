using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class DeserializeJsonParam {
    public static Color JsonToColor(JsonNode json) {
        var r = float.Parse(json["R"].Get<string>());
        var g = float.Parse(json["G"].Get<string>());
        var b = float.Parse(json["B"].Get<string>());
        var a = float.Parse(json["A"].Get<string>());

        return new Color(r, g, b, a);
    }

    public static ColorBlock JsonToColorBlock(JsonNode json) {
        var normalColor = JsonToColor(json["NormalColor"]);
        var highlightedColor = JsonToColor(json["HighlightedColor"]);
        var pressedColor = JsonToColor(json["PressedColor"]);
        var disabledColor = JsonToColor(json["DisabledColor"]);

        ColorBlock cb = new ColorBlock();
        cb.normalColor = normalColor;
        cb.highlightedColor = highlightedColor;
        cb.pressedColor = pressedColor;
        cb.disabledColor = disabledColor;
        cb.colorMultiplier = 1;

        return cb;
    }

    public static Vector3 JsonToVector3(JsonNode json) {
        var x = float.Parse(json["X"].Get<string>());
        var y = float.Parse(json["Y"].Get<string>());
        var z = float.Parse(json["Z"].Get<string>());

        return new Vector3(x, y, z);
    }

    public static Vector2 JsonToVector2(JsonNode json) {
        var x = float.Parse(json["X"].Get<string>());
        var y = float.Parse(json["Y"].Get<string>());
        return new Vector2(x, y);
    }

    public static List<string> JsonToDropDownOption(JsonNode json) {
        var list = new List<string>();
        //Debug.Log(json.Count);

        for (int i = 0; i < json.Count; i++) {
            list.Add(json[i.ToString()].Get<string>().UnicodeToStrings());
        }

        return list;
    }

    public static TextAnchor JsonToTextAnchor(JsonNode json) {
        TextAnchor ta = TextAnchor.MiddleCenter;
        switch (json.Get<string>()) {
            case "LowerCenter":
                ta = TextAnchor.LowerCenter;
                break;
            case "LowerLeft":
                ta = TextAnchor.LowerLeft;
                break;
            case "LowerRight":
                ta = TextAnchor.LowerRight;
                break;
            case "MiddleCenter":
                ta = TextAnchor.MiddleCenter;
                break;
            case "MiddleLeft":
                ta = TextAnchor.MiddleLeft;
                break;
            case "MiddleRight":
                ta = TextAnchor.MiddleRight;
                break;
            case "UpperCenter":
                ta = TextAnchor.UpperCenter;
                break;
            case "UpperLeft":
                ta = TextAnchor.UpperLeft;
                break;
            case "UpperRight":
                ta = TextAnchor.UpperRight;
                break;
            default:
                Debug.LogError("Unknown Alignment");
                break;
        }

        return ta;
    }

    public static Sprite DeserializeBase64PngToSprite(string base64Str, Vector2 hw) {

        int width = (int)hw.x;
        int height = (int)hw.y;

        //Debug.Log("WH " + width + "," + height);

        byte[] data = System.Convert.FromBase64String(base64Str);
        var tex = new Texture2D(width, height);
        tex.LoadImage(data);

        Sprite s = Sprite.Create(tex, new Rect(0, 0, width, height), Vector2.zero);

        return s;
    }

    public static RectTransform JsonToRectTransform(JsonNode json) {
        var r = new RectTransform();

        r.rotation = Quaternion.Euler(DeserializeJsonParam.JsonToVector3(json["BackgroundRectTransform"]["Rotation"]));
        r.localPosition = DeserializeJsonParam.JsonToVector3(json["Position"]);
        r.localScale = DeserializeJsonParam.JsonToVector3(json["LocalScale"]);
        r.anchorMin = DeserializeJsonParam.JsonToVector2(json["AnchorMin"]);
        r.anchorMax = DeserializeJsonParam.JsonToVector2(json["AnchorMax"]);
        r.anchoredPosition = DeserializeJsonParam.JsonToVector2(json["AnchoredPosition"]);
        r.sizeDelta = DeserializeJsonParam.JsonToVector2(json["SizeDelta"]);
        r.pivot = DeserializeJsonParam.JsonToVector2(json["Pivot"]);

        return r;
    }
}
