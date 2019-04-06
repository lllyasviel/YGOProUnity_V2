using UnityEngine;
using System.Collections;
using System.IO;

public class barPngLoader : MonoBehaviour {
    public UITexture under;
    public UIPanel healthBarCC;
    public UIPanel timeBarCC;
    public UIPanel faceCC;  
    public UITexture api_healthBar;
    public UITexture api_timeBar;
    public UITexture api_face;
    public UILabel api_name;
    public UILabel api_timeHint;
    public UILabel api_healthHint;
    // Use this for initialization
    void Start () {
        under.mainTexture = GameTextureManager.bar;
        api_healthBar.mainTexture = GameTextureManager.lp;
        api_timeBar.mainTexture = GameTextureManager.time;
        try
        {
            string[] allLines = (File.ReadAllText("texture/duel/healthBar/config.txt").Replace("\r", "").Replace(" ", "").Split("\n"));
            foreach (var item in allLines)
            {
                string[] mats = item.Split("=");
                if (mats.Length == 2)
                {
                    Color c;
                    switch (mats[0])
                    {
                        case "totalSize.width":
                            under.width = int.Parse(mats[1]);
                            under.transform.localPosition = new Vector3(-float.Parse(mats[1]) / 2f, under.transform.localPosition.y, 0);
                            break;
                        case "flatType":
                            if (int.Parse(mats[1]) == 1)
                            {
                                healthBarCC.clipSoftness = new Vector2(0, 0);
                                timeBarCC.clipSoftness = new Vector2(0, 0);
                            }
                            else
                            {
                                healthBarCC.clipSoftness = new Vector2(4, 4);
                                timeBarCC.clipSoftness = new Vector2(4, 4);
                            }
                            break;
                        case "totalSize.height":
                            under.height = int.Parse(mats[1]);
                            under.transform.localPosition = new Vector3(under.transform.localPosition.x, -float.Parse(mats[1]) / 2f, 0);
                            break;
                        case "playerNameLable.x":
                            api_name.transform.localPosition = new Vector3(-float.Parse(mats[1]), api_name.transform.localPosition.y, 0);
                            break;
                        case "playerNameLable.y":
                            api_name.transform.localPosition = new Vector3(api_name.transform.localPosition.x, -float.Parse(mats[1]), 0);
                            break;
                        case "playerNameLable.width":
                            api_name.width = int.Parse(mats[1]);
                            break;
                        case "playerNameLable.height":
                            api_name.height = int.Parse(mats[1]);
                            break;
                        case "playerNameLable.color":
                            ColorUtility.TryParseHtmlString(mats[1],out c);
                            api_name.color = c;
                            api_name.gradientTop = api_name.color;
                            break;
                        case "playerNameLable.alignment":
                            if (mats[1] == "0")
                            {
                                api_name.alignment = NGUIText.Alignment.Center;
                            }
                            if (mats[1] == "1")
                            {
                                api_name.alignment = NGUIText.Alignment.Left;
                            }
                            if (mats[1] == "2")
                            {
                                api_name.alignment = NGUIText.Alignment.Right;
                            }
                            break;
                        case "playerNameLable.effect":
                            if (mats[1] == "0")
                            {
                                api_name.effectStyle = UILabel.Effect.None;
                            }
                            if (mats[1] == "1")
                            {
                                api_name.effectStyle = UILabel.Effect.Shadow;
                            }
                            if (mats[1] == "2")
                            {
                                api_name.effectStyle = UILabel.Effect.Outline;
                            }
                            break;
                        case "healthLable.x":
                            api_healthHint.transform.localPosition = new Vector3(-float.Parse(mats[1]), api_healthHint.transform.localPosition.y, 0);
                            break;
                        case "healthLable.y":
                            api_healthHint.transform.localPosition = new Vector3(api_healthHint.transform.localPosition.x, -float.Parse(mats[1]), 0);
                            break;
                        case "healthLable.width":
                            api_healthHint.width = int.Parse(mats[1]);
                            break;
                        case "healthLable.height":
                            api_healthHint.height = int.Parse(mats[1]);
                            break;
                        case "healthLable.color":
                            ColorUtility.TryParseHtmlString(mats[1], out c);
                            api_healthHint.color = c;
                            api_healthHint.gradientTop = api_name.color;
                            break;
                        case "healthLable.alignment":
                            if (mats[1] == "0")
                            {
                                api_healthHint.alignment = NGUIText.Alignment.Center;
                            }
                            if (mats[1] == "1")
                            {
                                api_healthHint.alignment = NGUIText.Alignment.Left;
                            }
                            if (mats[1] == "2")
                            {
                                api_healthHint.alignment = NGUIText.Alignment.Right;
                            }
                            break;
                        case "healthLable.effect":
                            if (mats[1] == "0")
                            {
                                api_healthHint.effectStyle = UILabel.Effect.None;
                            }
                            if (mats[1] == "1")
                            {
                                api_healthHint.effectStyle = UILabel.Effect.Shadow;
                            }
                            if (mats[1] == "2")
                            {
                                api_healthHint.effectStyle = UILabel.Effect.Outline;
                            }
                            break;
                        case "timeLable.x":
                            api_timeHint.transform.localPosition = new Vector3(-float.Parse(mats[1]), api_timeHint.transform.localPosition.y, 0);
                            break;
                        case "timeLable.y":
                            api_timeHint.transform.localPosition = new Vector3(api_timeHint.transform.localPosition.x, -float.Parse(mats[1]), 0);
                            break;
                        case "timeLable.width":
                            api_timeHint.width = int.Parse(mats[1]);
                            break;
                        case "timeLable.height":
                            api_timeHint.height = int.Parse(mats[1]);
                            break;
                        case "timeLable.color":
                            ColorUtility.TryParseHtmlString(mats[1], out c);
                            api_timeHint.color = c;
                            api_timeHint.gradientTop = api_name.color;
                            break;
                        case "timeLable.alignment":
                            if (mats[1] == "0")
                            {
                                api_timeHint.alignment = NGUIText.Alignment.Center;
                            }
                            if (mats[1] == "1")
                            {
                                api_timeHint.alignment = NGUIText.Alignment.Left;
                            }
                            if (mats[1] == "2")
                            {
                                api_timeHint.alignment = NGUIText.Alignment.Right;
                            }
                            break;
                        case "timeLable.effect":
                            if (mats[1] == "0")
                            {
                                api_timeHint.effectStyle = UILabel.Effect.None;
                            }
                            if (mats[1] == "1")
                            {
                                api_timeHint.effectStyle = UILabel.Effect.Shadow;
                            }
                            if (mats[1] == "2")
                            {
                                api_timeHint.effectStyle = UILabel.Effect.Outline;
                            }
                            break;
                        case "health.x":
                            healthBarCC.transform.localPosition = new Vector3(-float.Parse(mats[1]), healthBarCC.transform.localPosition.y, 0);
                            api_healthBar.transform.localPosition = Vector3.zero;
                            break;
                        case "health.y":
                            healthBarCC.transform.localPosition = new Vector3(healthBarCC.transform.localPosition.x, -float.Parse(mats[1]), 0);
                            api_healthBar.transform.localPosition = Vector3.zero;
                            break;
                        case "health.width":
                            healthBarCC.baseClipRegion = new Vector4(healthBarCC.baseClipRegion.x, healthBarCC.baseClipRegion.y, float.Parse(mats[1]), healthBarCC.baseClipRegion.w);
                            api_healthBar.width = int.Parse(mats[1]);
                            break;
                        case "health.height":
                            healthBarCC.baseClipRegion = new Vector4(healthBarCC.baseClipRegion.x, healthBarCC.baseClipRegion.y, healthBarCC.baseClipRegion.z, float.Parse(mats[1]));
                            api_healthBar.height = int.Parse(mats[1]);
                            break;
                        case "time.x":
                            timeBarCC.transform.localPosition = new Vector3(-float.Parse(mats[1]), timeBarCC.transform.localPosition.y, 0);
                            api_timeBar.transform.localPosition = Vector3.zero;
                            break;
                        case "time.y":
                            timeBarCC.transform.localPosition = new Vector3(timeBarCC.transform.localPosition.x, -float.Parse(mats[1]), 0);
                            api_timeBar.transform.localPosition = Vector3.zero;
                            break;
                        case "time.width":
                            timeBarCC.baseClipRegion = new Vector4(timeBarCC.baseClipRegion.x, timeBarCC.baseClipRegion.y, float.Parse(mats[1]), timeBarCC.baseClipRegion.w);
                            api_timeBar.width = int.Parse(mats[1]);
                            break;
                        case "time.height":
                            timeBarCC.baseClipRegion = new Vector4(timeBarCC.baseClipRegion.x, timeBarCC.baseClipRegion.y, timeBarCC.baseClipRegion.z, float.Parse(mats[1]));
                            api_timeBar.height = int.Parse(mats[1]);
                            break;
                        case "face.x":
                            faceCC.transform.localPosition = new Vector3(-float.Parse(mats[1]), faceCC.transform.localPosition.y, 0);
                            api_timeBar.transform.localPosition = Vector3.zero;
                            break;
                        case "face.y":
                            faceCC.transform.localPosition = new Vector3(faceCC.transform.localPosition.x, -float.Parse(mats[1]), 0);
                            api_timeBar.transform.localPosition = Vector3.zero;
                            break;
                        case "face.size":
                            faceCC.baseClipRegion = new Vector4(faceCC.baseClipRegion.x, faceCC.baseClipRegion.y, float.Parse(mats[1]), float.Parse(mats[1]));
                            api_face.width = int.Parse(mats[1]);
                            api_face.height = int.Parse(mats[1]);
                            break;
                        case "face.type":
                            if (mats[1] == "0")
                            {
                                faceCC.clipping = UIDrawCall.Clipping.TextureMask;
                            }
                            if (mats[1] == "1")
                            {
                                faceCC.clipping = UIDrawCall.Clipping.SoftClip;
                            }
                            break;
                    }
                }
            }
        }
        catch (System.Exception e)  
        {
            Debug.LogError(e);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
