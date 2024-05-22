using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Splines;

namespace UnityEditor.Splines
{
    public struct SelectedSplineElementInfo
    {
        public Object target;
        public int targetIndex;
        public int knotIndex;

        public SelectedSplineElementInfo(Object Object, int Index, int knot)
        {
            target = Object;
            targetIndex = Index;
            knotIndex = knot;
        }
    }

    public static class SplineToolUtility
    {
        public static event System.Action Changed;

        public static List<SelectedSplineElementInfo> GetSelection()
        {
            List<SelectedSplineElementInfo> infos = new List<SelectedSplineElementInfo>();
            List<SelectableSplineElement> elements = SplineSelection.selection;
            foreach(SelectableSplineElement element in elements)
            {
                infos.Add(new SelectedSplineElementInfo(element.target, element.targetIndex, element.knotIndex));
            }

            return infos;
        }

        public static void RegisterSelectionChangedEvent()
        {
            SplineSelection.changed += OnSelectionChangedEvent;
        }

        public static void DeregisterSelectionChangedEvent()
        {
            SplineSelection.changed -= OnSelectionChangedEvent;

        }

        private static void OnSelectionChangedEvent()
        {
            Changed.Invoke();
        }
    }
}
