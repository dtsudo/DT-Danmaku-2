
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class DanmakuMath
	{
		// 0 degrees is going straight up
		// 90 degrees is going right
		// 180 degrees is straight down
		// 270 degrees is going left
		//
		// If (currentX, currentY) == (desiredX, desiredY), this function returns 0
		//
		// Note function returns value as degrees * 128
		//
		// This function guarantees that the returned value will be within [0, 360 * 128)
		public static int GetMovementDirectionInDegreesScaled(int currentX, int currentY, int desiredX, int desiredY)
		{
			int deltaX = desiredX - currentX;
			int deltaY = desiredY - currentY;

			if (deltaX == 0 && deltaY == 0)
				return 0;

			int angleInDegreesScaled = DTMath.ArcTangentScaled(x: deltaX, y: deltaY);
			int angle2 = -angleInDegreesScaled + 90 * 128;
			return NormalizeAngleInDegreesScaled(angle2);
		}
		
		public class Offset
		{
			public int DeltaXInMillipixels;
			public int DeltaYInMillipixels;
		}

		public static Offset GetOffset(
			int speedInMillipixelsPerMillisecond,
			int movementDirectionInDegreesScaled,
			int elapsedMillisecondsPerIteration)
		{
			if (speedInMillipixelsPerMillisecond == 0)
				return new Offset { DeltaXInMillipixels = 0, DeltaYInMillipixels = 0 };

			movementDirectionInDegreesScaled = NormalizeAngleInDegreesScaled(movementDirectionInDegreesScaled);

			int numberOfMillipixels = speedInMillipixelsPerMillisecond * elapsedMillisecondsPerIteration;

			return new Offset
			{
				DeltaXInMillipixels = (numberOfMillipixels * DTMath.SineScaled(degreesScaled: movementDirectionInDegreesScaled)) >> 10,
				DeltaYInMillipixels = (numberOfMillipixels * DTMath.CosineScaled(degreesScaled: movementDirectionInDegreesScaled)) >> 10
			};
		}
		
		// Returns a number in [0 * 128, 360 * 128)
		public static int NormalizeAngleInDegreesScaled(int angleInDegreesScaled)
		{
			if (angleInDegreesScaled >= 0 && angleInDegreesScaled < 360 * 128)
				return angleInDegreesScaled;

			if (angleInDegreesScaled < 0)
			{
				int x = Math.Abs(angleInDegreesScaled / (360 * 128));
				angleInDegreesScaled = angleInDegreesScaled + 360 * 128 * x;
			}
			else if (angleInDegreesScaled > 0)
			{
				int x = angleInDegreesScaled / (360 * 128);
				angleInDegreesScaled = angleInDegreesScaled - 360 * 128 * x;
			}

			while (angleInDegreesScaled < 0)
				angleInDegreesScaled = angleInDegreesScaled + 360 * 128;

			while (angleInDegreesScaled >= 360 * 128)
				angleInDegreesScaled = angleInDegreesScaled - 360 * 128;

			return angleInDegreesScaled;
		}
	}
}
