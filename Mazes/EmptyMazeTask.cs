namespace Mazes
{
	public static class EmptyMazeTask
	{
		public static void MoveOut(Robot robot, int width, int height)
		{
             for (int i=0; i<width-3; i++)
                 robot.MoveTo(Direction.Right);
             for (int j=0; j<height-3; j++)
                 robot.MoveTo(Direction.Down);

              /*
             robot.MoveRight(robot, width-3);
             robot.MoveDown(robot, height-3);
             */
		}
	}
}