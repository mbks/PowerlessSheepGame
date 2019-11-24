using UnityEngine;
using System.Collections;

namespace Completed
{
	//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
	public class Box : MovingObject
	{
		public AudioClip moveSound1;

		//Start overrides the virtual Start function of the base class.
		protected override void Start ()
		{
			//Call the start function of our base class MovingObject.
			base.Start ();
		}


		//Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
		//See comments in MovingObject for more on how base AttemptMove function works.
		public override void AttemptMove <T> (int xDir, int yDir)
		{
			//Call the AttemptMove function from MovingObject.
			base.AttemptMove <T> (xDir, yDir);
		}

		protected override bool Move(int xDir, int yDir, out RaycastHit2D hit) {
			bool moved = base.Move(xDir, yDir, out hit);
			if (moved) {
				GameManager.instance.player.ForceMove<Wall>(xDir, yDir);
				SoundManager.instance.PlaySingle (moveSound1);
			}
			return moved;
		}


		//MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.


		//OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject
		//and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
		protected override void OnCantMove <T> (T component)
		{
		}
	}
}
