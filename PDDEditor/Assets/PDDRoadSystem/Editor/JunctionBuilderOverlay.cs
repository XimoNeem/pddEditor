using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

[Icon("Assets/PDDRoadSystem/Editor/roadIcon.png")]
[Overlay(typeof(SceneView), "Intersection Builder", true)]
public class JunctionBuilderOverlay : Overlay
{

    Label SelectionInfoLabel;
    Button BuildJunctionButton;
    VisualElement SliderArea;

    public static JunctionBuilderOverlay instance;

    public Action OnChangeValueEvent { get; internal set; }

    public override VisualElement CreatePanelContent()
    {
        instance = this;

        SelectionInfoLabel = new Label();

        BuildJunctionButton = new Button(OnBuildJunction);
        BuildJunctionButton.text = "Build Intersection";
        BuildJunctionButton.SetEnabled(false);

        var root = new VisualElement() { name = "My Toolbar Root" };
        root.Add(SelectionInfoLabel);
        root.Add(BuildJunctionButton);

        SliderArea = new VisualElement();
        root.Add(SliderArea);


        SplineToolUtility.RegisterSelectionChangedEvent();
        SplineToolUtility.Changed += OnSelectionChanged;

        return root;

    }

    private void OnBuildJunction()
    {
        List<SelectedSplineElementInfo> selection = SplineToolUtility.GetSelection();

        Intersection intersection = new Intersection();
        foreach(SelectedSplineElementInfo item in selection)
        {
            //Get the spline container;
            SplineContainer container = (SplineContainer)item.target;
            Spline spline = container.Splines[item.targetIndex];
            intersection.AddJunction(item.targetIndex, item.knotIndex, spline);
        }


        Selection.activeObject.GetComponent<SplineRoad>().AddIntersection(intersection);
    }

    private void OnSelectionChanged()
    {
        BuildJunctionButton.SetEnabled(SplineToolUtility.GetSelection().Count > 1);

        BuildJunctionButton.visible = true;
        if (SplineToolUtility.GetSelection().Count>0)
        {
            UpdateSelectionInfo();
        }
        else
        {
            ClearSelectionInfo();
        }
    }

    private void ClearSelectionInfo()
    {
        SelectionInfoLabel.text = "";
    }

    private void UpdateSelectionInfo()
    {
        ClearSelectionInfo();

       List<SelectedSplineElementInfo> infos = SplineToolUtility.GetSelection();
        foreach(SelectedSplineElementInfo element in infos)
        {
            SelectionInfoLabel.text += $"Spline {element.targetIndex}, Knot {element.knotIndex} \n";
        }

    }

    public void ShowIntersection(Intersection intersection)
    {
        SelectionInfoLabel.text = "Selected Intersection";
        BuildJunctionButton.visible = false;

        SliderArea.Clear();

        for(int i=0; i<intersection.curves.Count; i++)
        {
            int value = i;
            Slider slider = new Slider($"Radius {i}", 0, 1, SliderDirection.Horizontal);
            slider.labelElement.style.minWidth = 60;
            slider.labelElement.style.maxWidth = 80;
            slider.style.minWidth = 150;
            slider.value = intersection.curves[i];
            slider.RegisterValueChangedCallback((x) =>
            {
                intersection.curves[value] = x.newValue;
                OnChangeValueEvent.Invoke();
            });
            SliderArea.Add(slider);
        }

    }
}