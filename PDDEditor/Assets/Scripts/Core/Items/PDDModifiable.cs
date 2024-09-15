using System.Collections;
using UnityEngine;

public class PDDModifiable : MonoBehaviour
{
    public string Name;
    public string AssetPath;

    public PDDBaseSettings PDDBaseSettings;

    public ToggleSetting[] ToggleSettings;
    public ColorSetting[] ColorSettings;
    public TransformGroupSetting[] PDDTransformInfo;
    public EmmiterSetting[] EmmiterSettings;
    public TextureSetting[] TextureSettings;
    public TextSetting[] TextSettings;

    public virtual void Set()
    {
        for (int i = 0; i < ToggleSettings.Length; i++)
        {
            ToggleSettings[i].ID = i;
            ToggleSettings[i].Set();
        }

        for (int i = 0; i < ColorSettings.Length; i++)
        {
            ColorSettings[i].ID = i;
            ColorSettings[i].Set();
        }

        for (int i = 0; i < PDDTransformInfo.Length; i++)
        {
            PDDTransformInfo[i].ID = i;
            PDDTransformInfo[i].Set();
        }

        for (int i = 0; i < EmmiterSettings.Length; i++)
        {
            EmmiterSettings[i].ID = i;
            EmmiterSettings[i].Set();
        }

        for (int i = 0; i < TextureSettings.Length; i++)
        {
            TextureSettings[i].ID = i;
            TextureSettings[i].Set();
        }

        for (int i = 0; i < TextSettings.Length; i++)
        {
            TextSettings[i].ID = i;
            TextSettings[i].Set();
        }
    }

    public virtual void ApplyBaseSettings()
    {
        try
        {
            if (PDDBaseSettings.LinkedSpline != "")
            {
                if (PDDBaseSettings.Movable)
                {
                    PDDSpline spline = (PDDSpline)Context.Instance.LevelSystem.GetUtilByID(PDDBaseSettings.LinkedSpline);

                    (Vector3 pos, Vector3 dir) = spline.EvaluateSpline(PDDBaseSettings.SplinePosition);
                    this.transform.parent.position = pos;
                    this.transform.parent.LookAt(pos + dir);
                }
            }
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public virtual void SetRuntimeChanges()
    {
        Debug.Log("Runtime");
        try
        {
            if (PDDBaseSettings.LinkedSpline != "")
            {
                if (PDDBaseSettings.Movable)
                {
                    PDDSpline spline = (PDDSpline)Context.Instance.LevelSystem.GetUtilByID(PDDBaseSettings.LinkedSpline);

                    (Vector3 pos, Vector3 dir) = spline.EvaluateSpline(PDDBaseSettings.SplinePosition);
                    this.transform.parent.position = pos;
                    this.transform.parent.LookAt(pos + dir);

                    PDDBaseSettings.SplinePosition = Mathf.Clamp(PDDBaseSettings.SplinePosition += PDDBaseSettings.MoveSpeed * 0.01f, 0f, 0.975f);

                    if (PDDBaseSettings.SplinePosition > 0.95f || PDDBaseSettings.SplinePosition < 0.05f)
                    {
                        if (PDDBaseSettings.MoveLoop)
                        {
                            if (PDDBaseSettings.SplinePosition > 0.5f)
                            {
                                PDDBaseSettings.SplinePosition = 0.075f;
                            }
                            else
                            {
                                PDDBaseSettings.SplinePosition = 0.925f;
                            }
                        }
                    }

                }
            }
        }
        catch (System.Exception)
        {

            throw;
        }
    }
}