using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;

namespace Rive
{
    /// <summary>
    /// Represents a Rive StateMachine from an Artboard. A StateMachine contains Inputs.
    /// </summary>
    public class StateMachine
    {
        private IntPtr m_nativeStateMachine;

        internal IntPtr nativeStateMachine => m_nativeStateMachine;

        internal StateMachine(IntPtr nativeStateMachine)
        {
            m_nativeStateMachine = nativeStateMachine;
        }

        ~StateMachine()
        {
            unrefStateMachine(m_nativeStateMachine);
        }

        public bool advance(float seconds)
        {
            return advanceStateMachine(m_nativeStateMachine, seconds);
        }

        /// The number of Inputs stored in the StateMachine.
        public uint inputCount()
        {
            return getSMIInputCountStateMachine(m_nativeStateMachine);
        }

        /// The SMIInput at the given index.
        public SMIInput input(uint index)
        {
            IntPtr ptr = getSMIInputFromIndexStateMachine(m_nativeStateMachine, index);
            return ptr == IntPtr.Zero ? null : new SMIInput(ptr, this);
        }

        private SMIInput convertInput(SMIInput input)
        {
            if (input.isBoolean)
            {
                return new SMIBool(input.nativeSMI, this);
            }
            else if (input.isTrigger)
            {
                return new SMITrigger(input.nativeSMI, this);
            }
            else if (input.isNumber)
            {
                return new SMINumber(input.nativeSMI, this);
            }
            else
            {
                return null;
            }
        }

        /// A list of all the SMIInputs stored in the StateMachine.
        public List<SMIInput> inputs()
        {
            var list = new List<SMIInput>();
            for (uint i = 0; i < inputCount(); i++)
            {
                var inputAtIndex = input(i);
                if (inputAtIndex == null) continue;

                var converted = convertInput(inputAtIndex);
                if (converted != null)
                {
                    list.Add(converted);
                }
            }

            return list;
        }

        /// Get a SMIBool by name.
        public SMIBool getBool(string name)
        {
            IntPtr ptr = getSMIBoolStateMachine(m_nativeStateMachine, name);
            if (ptr != IntPtr.Zero) return new SMIBool(ptr, this);
            Debug.Log($"No SMIBool found with name: {name}.");
            return null;

        }

        /// Get a SMITrigger by name.
        public SMITrigger getTrigger(string name)
        {
            IntPtr ptr = getSMITriggerStateMachine(m_nativeStateMachine, name);
            if (ptr != IntPtr.Zero) return new SMITrigger(ptr, this);
            Debug.Log($"No SMITrigger found with name: {name}.");
            return null;

        }

        /// Get a SMINumber by name.
        public SMINumber getNumber(string name)
        {
            IntPtr ptr = getSMINumberStateMachine(m_nativeStateMachine, name);
            if (ptr != IntPtr.Zero) return new SMINumber(ptr, this);
            Debug.Log($"No SMINumber found with name: {name}.");
            return null;

        }

        public void pointerMove(Vector2 position)
        {
            pointerMoveStateMachine(m_nativeStateMachine, position.x, position.y);
        }

        public void pointerDown(Vector2 position)
        {
            pointerDownStateMachine(m_nativeStateMachine, position.x, position.y);
        }

        public void pointerUp(Vector2 position)
        {
            pointerUpStateMachine(m_nativeStateMachine, position.x, position.y);
        }

        public List<ReportedEvent> reportedEvents()
        {
            uint count = getReportedEventCount(m_nativeStateMachine);
            var list = new List<ReportedEvent>();
            for (uint i = 0; i < count; i++)
            {
                list.Add(new ReportedEvent(getReportedEventAt(m_nativeStateMachine, i)));
            }
            return list;
        }

        #region Native Methods
        [DllImport(NativeLibrary.name)]
        internal static extern IntPtr unrefStateMachine(IntPtr stateMachine);

        [DllImport(NativeLibrary.name)]
        internal static extern bool advanceStateMachine(IntPtr stateMachine, float seconds);

        [DllImport(NativeLibrary.name)]
        internal static extern uint getSMIInputCountStateMachine(IntPtr stateMachine);

        [DllImport(NativeLibrary.name)]
        internal static extern IntPtr getSMIInputFromIndexStateMachine(IntPtr stateMachine, uint index);

        [DllImport(NativeLibrary.name)]
        internal static extern IntPtr getSMIBoolStateMachine(IntPtr stateMachine, string name);

        [DllImport(NativeLibrary.name)]
        internal static extern IntPtr getSMITriggerStateMachine(IntPtr stateMachine, string name);

        [DllImport(NativeLibrary.name)]
        internal static extern IntPtr getSMINumberStateMachine(IntPtr stateMachine, string name);

        [DllImport(NativeLibrary.name)]
        internal static extern void pointerMoveStateMachine(IntPtr stateMachine, float x, float y);

        [DllImport(NativeLibrary.name)]
        internal static extern void pointerDownStateMachine(IntPtr stateMachine, float x, float y);

        [DllImport(NativeLibrary.name)]
        internal static extern void pointerUpStateMachine(IntPtr stateMachine, float x, float y);

        [DllImport(NativeLibrary.name)]
        internal static extern uint getReportedEventCount(IntPtr stateMachine);

        [DllImport(NativeLibrary.name)]
        internal static extern ReportedEventData getReportedEventAt(IntPtr stateMachine, uint index);
        #endregion
    }
}
