using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WayPointEditor : EditorWindow
{
    [MenuItem("Tools/Waypoint Editor")]
    public static void OpenWindow() => GetWindow<WayPointEditor>("Waypoint Editor");



    
    Waypoints currentWPSystem;
    List<Waypoints> WPSystemList;
    bool hideWP = false;

    Vector2 scrollPos;

    private void OnEnable()
    {
        SceneView.duringSceneGui += DuringSceneGUI;
       
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGUI;
    }
   

    private void OnGUI()
    {
        
            hideWP = GUILayout.Toggle(hideWP, "Hide Waypoints on screen");

        
        if (GUILayout.Button("Add Waypoint System"))
        {
            AddWaypointSystem();
            
        }
        if (GUILayout.Button("Clear All Waypoints"))
        {
            currentWPSystem.WPList.Clear();
        }
        currentWPSystem = EditorGUILayout.ObjectField(currentWPSystem, typeof(Waypoints), true) as Waypoints;
        
        if (currentWPSystem != null)
        {
            currentWPSystem.cycleWP = GUILayout.Toggle(currentWPSystem.cycleWP, "Cycle all WayPoints");
            if (GUILayout.Button("Add Waypoint"))
            {
                CreateWayPoint(new Vector3(0,0,0));

            }
            scrollPos =EditorGUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < currentWPSystem.WPList.Count; i++)
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    currentWPSystem.WPList[i] = EditorGUILayout.Vector3Field("Waypoint: " + i.ToString(), currentWPSystem.WPList[i]);
                    GUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Move up"))
                    {
                        if (i > 0)
                        {
                            SwapElements(i, i - 1);
                        }
                    }
                    if (GUILayout.Button("Move Down"))
                    {
                        if (i < currentWPSystem.WPList.Count -1)
                        {
                            SwapElements(i, i +1);
                        }
                    }
                    GUILayout.EndHorizontal();

                    if (GUILayout.Button("Remove Waypoint"))
                    {
                        DeleteWaypoint(i);
                    }

                }
                    
            }
            EditorGUILayout.EndScrollView();
        }
       

    }

    void DuringSceneGUI(SceneView sceneView)
    {
        if (!hideWP)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.modifiers == EventModifiers.Shift)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    CreateWayPoint(hit.point);


                }
                else
                {
                    CreateWayPoint(ray.origin + (ray.direction * 30));

                }

            }

            if (Event.current.modifiers == EventModifiers.Control && Event.current.type == EventType.MouseDown)
            {
                for (int i = 0; i < currentWPSystem.WPList.Count; i++)
                {
                    if (currentWPSystem.cycleWP)
                    {
                        if (i == currentWPSystem.WPList.Count - 1)
                        {
                            if (HandleUtility.DistanceToLine(currentWPSystem.WPList[i], currentWPSystem.WPList[0]) < 3)
                            {
                                CreateWayPoint((currentWPSystem.WPList[i] + currentWPSystem.WPList[0]) / 2);

                            }
                        }
                        else if (currentWPSystem.WPList.Count > 1)
                        {
                            if (HandleUtility.DistanceToLine(currentWPSystem.WPList[i], currentWPSystem.WPList[i + 1]) < 3)
                            {
                                InsertElement(i + 1, (currentWPSystem.WPList[i] + currentWPSystem.WPList[i + 1]) / 2);

                            }
                        }

                    }
                    else
                    {
                        if (i < currentWPSystem.WPList.Count - 1)
                        {
                            if (HandleUtility.DistanceToLine(currentWPSystem.WPList[i], currentWPSystem.WPList[i + 1]) < 3)
                            {
                                InsertElement(i + 1, (currentWPSystem.WPList[i] + currentWPSystem.WPList[i + 1]) / 2);

                            }
                        }
                    }

                }

            }

            if (currentWPSystem != null)
            {
                if (currentWPSystem.WPList.Count > 0)
                {
                    for (int i = 0; i < currentWPSystem.WPList.Count; i++)
                    {
                        DrawLines(i);

                        Undo.RecordObject(currentWPSystem, "hello");
                        Handles.SphereHandleCap(-1, currentWPSystem.WPList[i], Quaternion.identity, 0.25f, EventType.Repaint);
                        Handles.Label(currentWPSystem.WPList[i], "Waypoint: " + i.ToString());
                        currentWPSystem.WPList[i] = Handles.PositionHandle(currentWPSystem.WPList[i], Quaternion.identity);
                        EditorUtility.SetDirty(currentWPSystem);

                        Repaint();

                    }
                }

            }
        }
     
    }
 
    void DrawLines(int index)
    {
        if (currentWPSystem.cycleWP)
        {
            if (index < currentWPSystem.WPList.Count - 1)
            {
                Handles.DrawLine(currentWPSystem.WPList[index], currentWPSystem.WPList[index + 1]);
            }
            else
            {
                Handles.DrawLine(currentWPSystem.WPList[index], currentWPSystem.WPList[0]);
            }
        }
        else
        {
            if (index < currentWPSystem.WPList.Count - 1)
            {
                Handles.DrawLine(currentWPSystem.WPList[index], currentWPSystem.WPList[index + 1]);
            }
        }
    }
    void AddWaypointSystem()
    {
        
        Waypoints newWP = ScriptableObject.CreateInstance<Waypoints>();
        AssetDatabase.CreateAsset(newWP, "Assets/WayPoints" + WPSystemList.Count.ToString() + ".asset");
        WPSystemList.Add(newWP);
        EditorUtility.SetDirty(currentWPSystem);
    }

   
    void DeleteWaypoint(int index)
    {
        Undo.RegisterCompleteObjectUndo(currentWPSystem, "Removed WP");
        currentWPSystem.WPList.RemoveAt(index);
        Repaint();
        EditorUtility.SetDirty(currentWPSystem);
    }
    void SwapElements(int i1, int i2)
    {
        Vector3 temp = currentWPSystem.WPList[i1];
        currentWPSystem.WPList[i1] = currentWPSystem.WPList[i2];
        currentWPSystem.WPList[i2] = temp;
        EditorUtility.SetDirty(currentWPSystem);
    }
    void InsertElement(int index, Vector3 vector)
    {
        currentWPSystem.WPList.Insert(index, vector);
        EditorUtility.SetDirty(currentWPSystem);
    }
    void CreateWayPoint(Vector3 vector)
    {
        currentWPSystem.WPList.Add(vector);
        Repaint();
        EditorUtility.SetDirty(currentWPSystem);
    }
}
