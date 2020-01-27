namespace Mazes
{
	public static class SnakeMazeTask
	{
        /*
        public static void MoveRight(Robot robot, int stepCount)
        {
            for(int i=0; i<stepCount; i++)
                robot.MoveTo(Direction.Right);
        }
                
        public static void MoveLeft(Robot robot, int stepCount)
        {
            for(int i=0; i<stepCount; i++)
                robot.MoveTo(Direction.Left);
        }

        public static void MoveDown(Robot robot, int stepCount)
        {
            for(int i=0; i<stepCount; i++)
                robot.MoveTo(Direction.Down);
        }

        public static void MoveDownLeft(Robot robot, int stepDown, int stepLeft)
        {
            MoveDown(robot, stepDown);
            MoveLeft(robot, stepLeft);
        }

        public static void MoveDownRight(Robot robot, int stepDown, int stepRight)
        {
            MoveDown(robot, stepDown);
            MoveRight(robot, stepRight);
        }
        */
		public static void MoveOut(Robot robot, int width, int height)
		{
        /*
            MoveRight(robot, width-3);
            int i=0;
            while (i<height-3) {
                MoveDownLeft(robot, 2, width-3);
                i+=2;
                if (i < height-3) {
                    MoveDownRight(robot, 2, width-3);
                    i+=2;
                }
            }
            */
            
            robot.MoveRight(robot, width-3);
            int i=0;
            while (i<height-3) {
                robot.MoveDownLeft(robot, 2, width-3);
                i+=2;
                if (i < height-3) {
                    robot.MoveDownRight(robot, 2, width-3);
                    i+=2;
                }
            }
            
		}
	}
}