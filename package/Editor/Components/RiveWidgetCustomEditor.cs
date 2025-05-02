using System.Collections.Generic;
using Rive.Components;
using UnityEditor;
using UnityEngine;

namespace Rive.EditorTools
{
    [CustomEditor(typeof(RiveWidget))]
    internal class RiveWidgetCustomEditor : Editor
    {
        private RiveWidget m_widget;
        private bool m_showStateMachines = false;
        
        private void OnEnable()
        {
            m_widget = (RiveWidget)target;
        }
        
        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();
            
            if (!Application.isPlaying || m_widget == null || m_widget.Artboard == null)
            {
                return;
            }
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Runtime State Machine Controls", EditorStyles.boldLabel);
            
            m_showStateMachines = EditorGUILayout.Foldout(m_showStateMachines, "Available State Machines");
            
            if (m_showStateMachines)
            {
                EditorGUI.indentLevel++;
                
                // Button to load all state machines
                if (GUILayout.Button("Load All State Machines"))
                {
                    m_widget.LoadAllStateMachines();
                }
                
                // Get currently loaded state machines
                List<string> stateMachineNames = m_widget.GetStateMachineNames();
                
                if (stateMachineNames.Count == 0)
                {
                    EditorGUILayout.HelpBox("No state machines loaded. Use 'Load All State Machines' button to load all available state machines.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.LabelField("Loaded State Machines:");
                    
                    foreach (string name in stateMachineNames)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(name);
                        
                        // If this is the primary state machine, indicate it
                        if (m_widget.StateMachine != null && m_widget.StateMachine.Name == name)
                        {
                            EditorGUILayout.LabelField("(Primary)", EditorStyles.miniLabel);
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            
            // Repaint the inspector if playing to keep the state machine list updated
            if (Application.isPlaying)
            {
                Repaint();
            }
        }
    }
} 