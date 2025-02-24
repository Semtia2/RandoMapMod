﻿using System.Collections.Generic;
using System.Diagnostics;
using MapChanger.MonoBehaviours;
using RandoMapMod.Modes;
using RandoMapMod.Pins;
using RandoMapMod.Transition;
using RandoMapMod.UI;

namespace RandoMapMod.Rooms
{
    internal class TransitionRoomSelector : RoomSelector
    {
        internal static TransitionRoomSelector Instance;

        internal override void Initialize(IEnumerable<MapObject> rooms)
        {
            Instance = this;

            base.Initialize(rooms);
        }

        public override void OnMainUpdate(bool active)
        {
            base.OnMainUpdate(active);

            attackHoldTimer.Reset();
        }

        private readonly Stopwatch attackHoldTimer = new();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by Unity")]
        private void Update()
        {
            if (InputHandler.Instance.inputActions.menuSubmit.WasPressed
                && SelectedObjectKey is not NONE_SELECTED)
            {
                attackHoldTimer.Reset();
                RouteTracker.GetRoute(SelectedObjectKey);
            }

            if (InputHandler.Instance.inputActions.attack.WasPressed)
            {
                attackHoldTimer.Restart();
            }

            if (InputHandler.Instance.inputActions.attack.WasReleased)
            {
                attackHoldTimer.Reset();
            }

            // Disable this benchwarp if the pin selector has already selected a bench
            if (attackHoldTimer.ElapsedMilliseconds >= 500)
            {
                attackHoldTimer.Reset();

                if (!RmmPinSelector.Instance.BenchSelected())
                {
                    RouteTracker.TryBenchwarp();
                }
            }
        }

        protected private override bool ActiveByCurrentMode()
        {
            return Conditions.TransitionRandoModeEnabled();
        }

        protected private override bool ActiveByToggle()
        {
            return RandoMapMod.GS.RoomSelectionOn;
        }

        protected override void OnSelectionChanged()
        {
            SelectionPanels.UpdateRoomPanel();
        }

        internal string GetText()
        {
            string instructions = RouteTracker.GetInstructionText();
            string transitions = SelectedObjectKey.GetUncheckedVisited();

            if (transitions is "") return instructions;

            return $"{instructions}\n\n{transitions}";
        }
    }
}
