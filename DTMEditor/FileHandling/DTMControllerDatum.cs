﻿using System;

namespace DTMEditor.FileHandling
{
	/// <summary>
	/// Represents a single grouping of controller data.
	/// There will be one of these created per frame.
	/// </summary>
	public class DTMControllerDatum
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">Data bytes</param>
		/// <param name="frameNumber">Frame number the given data corresponds to.</param>
		public DTMControllerDatum(ulong data, long frameNumber)
		{
			Data = data;
			FrameNumber = frameNumber;
		}

		#endregion

		#region Enums

		#region GameCube

		[Flags]
		public enum GameCubeButton
		{
			Start     = (1 << 0),
			A         = (1 << 1),
			B         = (1 << 2),
			X         = (1 << 3),
			Y         = (1 << 4),
			Z         = (1 << 5),
			DPadUp    = (1 << 6),
			DPadDown  = (1 << 7),
			DPadLeft  = (1 << 8),
			DPadRight = (1 << 9)
		}

		public enum GameCubeTrigger
		{
			L,
			R
		}

		public enum GameCubeAxis
		{
			AnalogXAxis,
			AnalogYAxis,
			CStickXAxis,
			CStickYAxis
		}

		#endregion

		#endregion

		#region Properties

		/// <summary>Raw representation of the controller data.</summary>
		public ulong Data { get; protected set; }

		/// <summary>Frame number this control data corresponds to.</summary>
		public long FrameNumber { get; private set; }

		#endregion

		#region Getter Methods

		/// <summary>
		/// Whether or not the given button is pressed.
		/// </summary>
		/// <param name="button">The button to check the state of.</param>
		/// <returns>true if the button is pressed, false otherwise.</returns>
		public bool IsButtonPressed(GameCubeButton button)
		{
			return (Data & (uint)button) != 0;
		}

		/// <summary>
		/// Gets the encoded axis value
		/// </summary>
		/// <param name="axis">Axis to get the value of.</param>
		/// <returns>The encoded axis value.</returns>
		public int GetAxisValue(GameCubeAxis axis)
		{
			switch (axis)
			{
				case GameCubeAxis.AnalogXAxis:
					return (int) ((Data >> 32) & 0xFF);

				case GameCubeAxis.AnalogYAxis:
					return (int) ((Data >> 40) & 0xFF);

				case GameCubeAxis.CStickXAxis:
					return (int) ((Data >> 48) & 0xFF);

				case GameCubeAxis.CStickYAxis:
					return (int) ((Data >> 56) & 0xFF);
			}

			// Will literally never happen
			return 0;
		}

		/// <summary>
		/// Gets the encoded trigger value.
		/// </summary>
		/// <param name="trigger">The trigger to get the value of.</param>
		/// <returns>The encoded trigger value.</returns>
		public int GetTriggerValue(GameCubeTrigger trigger)
		{
			switch (trigger)
			{
				case GameCubeTrigger.L:
					return (int) ((Data >> 16) & 0xFF);

				case GameCubeTrigger.R:
					return (int)((Data >> 24) & 0xFF);
			}

			// Will literally never happen
			return 0;
		}

		#endregion

		#region Modification Methods

		/// <summary>
		/// Modifies the data to indicate whether or not a given button is pressed.
		/// </summary>
		/// <param name="button">The button to modify data of</param>
		/// <param name="pressed">Whether or not the given button should be considered pressed.</param>
		public void ModifyButton(GameCubeButton button, bool pressed)
		{
			if (pressed)
				Data |= (uint) button;
			else
				Data &= ~((uint) button);
		}

		public void ModifyTrigger(GameCubeTrigger trigger, bool pressed, int value)
		{
			// TODO: I think L and R might be consider pressed after a threshold,
			//       not simply if value is larger than zero.
			if (trigger == GameCubeTrigger.L)
			{
				if (pressed)
				{
					Data |= (1 << 10);
					Data |= (uint)(value << 16);
				}
				else
				{
					Data &= ~0xFF0400U;
				}
			}
			else
			{
				if (pressed)
				{
					Data |= (1 << 11);
					Data |= (uint)(value << 24);
				}
				else
				{
					Data &= ~0xFF000800U;
				}
			}
		}

		/// <summary>
		/// Modifies data relating to a certain axis.
		/// </summary>
		/// <param name="axis">The axis to modify the data of.</param>
		/// <param name="value">The value to use for the given axis.</param>
		public void ModifyAxis(GameCubeAxis axis, int value)
		{
			if (axis == GameCubeAxis.AnalogXAxis)
				Data |= ~(Data & 0x000000FF000000UL) | ((ulong)value << 32);
			else if (axis == GameCubeAxis.AnalogYAxis)
				Data |= ~(Data & 0x0000FF00000000UL) | ((ulong)value << 40);
			else if (axis == GameCubeAxis.CStickXAxis)
				Data |= ~(Data & 0x00FF0000000000UL) | ((ulong)value << 48);
			else if (axis == GameCubeAxis.CStickYAxis)
				Data |= ~(Data & 0xFF000000000000UL) | ((ulong)value << 56);
		}

		// Used by the list view to enumerate frame titles.
		public override string ToString()
		{
			return "Frame " + FrameNumber;
		}

		#endregion
	}
}
