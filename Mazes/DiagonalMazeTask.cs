namespace Mazes
{
	public static class DiagonalMazeTask
	{
        public static void MoveRight(Robot robot, int stepCount)
        {
            for(int i=0; i<stepCount; i++)
                robot.MoveTo(Direction.Right);
        }

        public static void MoveDown(Robot robot, int stepCount)
        {
            for(int i=0; i<stepCount; i++)
                robot.MoveTo(Direction.Down);
        }

        public static void MoveDownRight(Robot robot, int stepDown, int stepRight)
        {
            MoveDown(robot, stepDown);
            MoveRight(robot, stepRight);
        }

        public static void MoveDownRightInLoop(Robot robot, int stepDown, int stepRight, int countLoop)
        {
            for (int i=0; i<countLoop; i++)
                MoveDownRight(robot, stepDown, stepRight);
        }

		public static void MoveOut(Robot robot, int width, int height)
		{
            if (width>height) {
                int stepToRight=(width-3)/(height-2);
                MoveRight(robot, stepToRight);
                MoveDownRightInLoop(robot, 1, stepToRight, height-3);
            } else {
                int stepToDown=(height-3)/(width-2);
                MoveDownRightInLoop(robot, stepToDown, 1, width-3);
                MoveDown(robot, stepToDown);
            }
		}
	}
}