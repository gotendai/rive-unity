using System;
using System.Collections.Generic;
using Rive.Utils;
using UnityEngine;

namespace Rive.Components.Utilities
{
    /// <summary>
    /// Manages multiple state machines for a Rive artboard.
    /// </summary>
    public class StateMachineController : IDisposable
    {
        private Dictionary<string, StateMachine> m_stateMachines = new Dictionary<string, StateMachine>();
        private List<ReportedEvent> m_reportedEvents = new List<ReportedEvent>();
        private Artboard m_artboard;
        
        public Artboard Artboard => m_artboard;
        
        /// <summary>
        /// The primary state machine that's used for default behaviors when specific state machine isn't specified.
        /// </summary>
        public StateMachine PrimaryStateMachine { get; private set; }

        /// <summary>
        /// Event triggered when a Rive event is reported from any state machine.
        /// </summary>
        public event Action<ReportedEvent, string> OnRiveEventReported;

        /// <summary>
        /// Initialize the controller with an artboard and load state machines.
        /// </summary>
        public void Initialize(Artboard artboard, string primaryStateMachineName = null)
        {
            if (artboard == null)
            {
                DebugLogger.Instance.LogError("Cannot initialize StateMachineController with null artboard");
                return;
            }
            
            m_artboard = artboard;
            
            // Clear existing state machines if any
            Clear();
            
            // If a primary state machine is specified, load it first
            if (!string.IsNullOrEmpty(primaryStateMachineName))
            {
                PrimaryStateMachine = LoadStateMachine(primaryStateMachineName);
            }
        }
        
        /// <summary>
        /// Load all available state machines from the artboard.
        /// </summary>
        public void LoadAllStateMachines()
        {
            if (m_artboard == null) return;
            
            for (uint i = 0; i < m_artboard.StateMachineCount; i++)
            {
                string name = m_artboard.StateMachineName(i);
                if (!m_stateMachines.ContainsKey(name))
                {
                    LoadStateMachine(name);
                }
            }
        }
        
        /// <summary>
        /// Load a state machine by name.
        /// </summary>
        public StateMachine LoadStateMachine(string name)
        {
            if (m_artboard == null)
            {
                DebugLogger.Instance.LogError("Cannot load state machine: artboard is null");
                return null;
            }
            
            if (string.IsNullOrEmpty(name))
            {
                DebugLogger.Instance.LogError("Cannot load state machine: name is null or empty");
                return null;
            }
            
            // If we already have this state machine, return it
            if (m_stateMachines.TryGetValue(name, out StateMachine existingStateMachine))
            {
                return existingStateMachine;
            }
            
            // Create the state machine
            StateMachine stateMachine = m_artboard.StateMachine(name);
            if (stateMachine == null)
            {
                DebugLogger.Instance.LogError($"Failed to load state machine: {name}");
                return null;
            }
            
            // Add to our dictionary
            m_stateMachines.Add(name, stateMachine);
            
            // If this is the first state machine, make it the primary
            if (PrimaryStateMachine == null)
            {
                PrimaryStateMachine = stateMachine;
            }
            
            return stateMachine;
        }
        
        /// <summary>
        /// Get a state machine by name.
        /// </summary>
        public StateMachine GetStateMachine(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return PrimaryStateMachine;
            }
            
            if (m_stateMachines.TryGetValue(name, out StateMachine stateMachine))
            {
                return stateMachine;
            }
            
            return LoadStateMachine(name);
        }
        
        /// <summary>
        /// Check if a state machine with the given name exists.
        /// </summary>
        public bool HasStateMachine(string name)
        {
            return m_stateMachines.ContainsKey(name);
        }
        
        /// <summary>
        /// Get a list of all loaded state machine names.
        /// </summary>
        public List<string> GetStateMachineNames()
        {
            return new List<string>(m_stateMachines.Keys);
        }
        
        /// <summary>
        /// Advance all state machines by the specified time.
        /// </summary>
        public void Tick(float deltaTime, RiveWidget.EventPoolingMode poolingMode)
        {
            if (m_artboard == null) return;
            
            m_reportedEvents.Clear();
            
            foreach (var kvp in m_stateMachines)
            {
                string stateMachineName = kvp.Key;
                StateMachine stateMachine = kvp.Value;
                
                // Process events for this state machine
                m_reportedEvents.Clear();
                stateMachine.ReportedEvents(m_reportedEvents);
                
                for (int i = 0; i < m_reportedEvents.Count; i++)
                {
                    var evt = m_reportedEvents[i];
                    OnRiveEventReported?.Invoke(evt, stateMachineName);
                    
                    // If pooling is enabled, auto-dispose the event
                    if (poolingMode == RiveWidget.EventPoolingMode.Enabled)
                    {
                        evt.Dispose();
                    }
                }
                
                // Advance the state machine
                stateMachine.Advance(deltaTime);
            }
        }
        
        /// <summary>
        /// Set a boolean input on a state machine.
        /// </summary>
        public bool SetBooleanInput(string stateMachineName, string inputName, bool value)
        {
            StateMachine stateMachine = GetStateMachine(stateMachineName);
            if (stateMachine == null) return false;
            
            SMIBool input = stateMachine.GetBool(inputName);
            if (input == null) return false;
            
            input.Value = value;
            return true;
        }
        
        /// <summary>
        /// Set a number input on a state machine.
        /// </summary>
        public bool SetNumberInput(string stateMachineName, string inputName, float value)
        {
            StateMachine stateMachine = GetStateMachine(stateMachineName);
            if (stateMachine == null) return false;
            
            SMINumber input = stateMachine.GetNumber(inputName);
            if (input == null) return false;
            
            input.Value = value;
            return true;
        }
        
        /// <summary>
        /// Fire a trigger input on a state machine.
        /// </summary>
        public bool FireInput(string stateMachineName, string inputName)
        {
            StateMachine stateMachine = GetStateMachine(stateMachineName);
            if (stateMachine == null) return false;
            
            SMITrigger input = stateMachine.GetTrigger(inputName);
            if (input == null) return false;
            
            input.Fire();
            return true;
        }
        
        /// <summary>
        /// Process pointer input for the primary state machine.
        /// </summary>
        public HitResult PointerDown(Vector2 position)
        {
            if (PrimaryStateMachine == null) return HitResult.None;
            return PrimaryStateMachine.PointerDown(position);
        }
        
        /// <summary>
        /// Process pointer input for the primary state machine.
        /// </summary>
        public HitResult PointerUp(Vector2 position)
        {
            if (PrimaryStateMachine == null) return HitResult.None;
            return PrimaryStateMachine.PointerUp(position);
        }
        
        /// <summary>
        /// Process pointer input for the primary state machine.
        /// </summary>
        public HitResult PointerMove(Vector2 position)
        {
            if (PrimaryStateMachine == null) return HitResult.None;
            return PrimaryStateMachine.PointerMove(position);
        }
        
        /// <summary>
        /// Perform hit testing using the primary state machine.
        /// </summary>
        public bool HitTest(Vector2 position)
        {
            if (PrimaryStateMachine == null) return false;
            return PrimaryStateMachine.HitTest(position);
        }

        /// <summary>
        /// Process pointer input for a specific state machine.
        /// </summary>
        public HitResult PointerDown(string stateMachineName, Vector2 position)
        {
            StateMachine stateMachine = GetStateMachine(stateMachineName);
            if (stateMachine == null) return HitResult.None;
            return stateMachine.PointerDown(position);
        }
        
        /// <summary>
        /// Process pointer input for a specific state machine.
        /// </summary>
        public HitResult PointerUp(string stateMachineName, Vector2 position)
        {
            StateMachine stateMachine = GetStateMachine(stateMachineName);
            if (stateMachine == null) return HitResult.None;
            return stateMachine.PointerUp(position);
        }
        
        /// <summary>
        /// Process pointer input for a specific state machine.
        /// </summary>
        public HitResult PointerMove(string stateMachineName, Vector2 position)
        {
            StateMachine stateMachine = GetStateMachine(stateMachineName);
            if (stateMachine == null) return HitResult.None;
            return stateMachine.PointerMove(position);
        }
        
        /// <summary>
        /// Clear all state machines.
        /// </summary>
        public void Clear()
        {
            m_stateMachines.Clear();
            PrimaryStateMachine = null;
        }
        
        /// <summary>
        /// Dispose of all state machines.
        /// </summary>
        public void Dispose()
        {
            Clear();
            m_artboard = null;
        }
    }
} 