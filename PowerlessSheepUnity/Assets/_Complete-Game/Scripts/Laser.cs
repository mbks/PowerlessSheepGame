using UnityEngine;
using System.Collections;

namespace Completed
{
	//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
	public class Laser : MovingObject
	{


		//Start overrides the virtual Start function of the base class.
		protected override void Start ()
		{
			gameObject.transform.position = GameManager.instance.player.gameObject.transform.position;
			//Call the start function of our base class MovingObject.
			base.Start ();
		}


		//Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
		//See comments in MovingObject for more on how base AttemptMove function works.
		public override void AttemptMove <T> (int dir, int ignore)
		{
		  gameObject.transform.position = GameManager.instance.player.gameObject.transform.position;
			//Call the AttemptMove function from MovingObject.
			int xDir = 0;
			int yDir = 0;
			if (dir == 0)
				xDir = 1;
			if (dir == 2)
				xDir = -1;
			if (dir == 3)
				yDir = 1;
			if (dir == 1)
				yDir = -1;
			base.AttemptMove <Enemy> (xDir, yDir);
		}

		//OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject
		//and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
		protected override void OnCantMove <T> (T component)
		{
			//Declare hitPlayer and set it to equal the encountered component.
			Enemy hitEnemy = component as Enemy;

			//Call the LoseFood function of hitPlayer passing it playerDamage, the amount of foodpoints to be subtracted.
		  hitEnemy.Kill ();

		}
	}
}
