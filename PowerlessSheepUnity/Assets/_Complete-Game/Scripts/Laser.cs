using UnityEngine;
using System.Collections;

namespace Completed
{
	//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
	public class Laser : MovingObject
	{


		protected void Awake() {
			gameObject.transform.position = GameManager.instance.player.gameObject.transform.position;
			base.Start();
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
				xDir = 3;
			if (dir == 2)
				xDir = -3;
			if (dir == 3)
				yDir = 3;
			if (dir == 1)
				yDir = -3;


			if (dir % 2 == 1)
				transform.Rotate(Vector3.forward * -90);
			//Store start position to move from, based on objects current transform position.
			Vector2 start = transform.position;

			// Calculate end position based on the direction parameters passed in when calling Move.
			Vector2 end = start + new Vector2 (xDir, yDir);

			StartCoroutine (SmoothMovement (end));

			// Check hit

			//Disable the boxCollider so that linecast doesn't hit this object's own collider.
			gameObject.GetComponent<BoxCollider2D>().enabled = false;
			GameManager.instance.player.gameObject.GetComponent<BoxCollider2D>().enabled = false;

			//Cast a line from start point to end point checking collision on blockingLayer.
			RaycastHit2D hit = Physics2D.Linecast (start, end, blockingLayer);

			//Re-enable boxCollider after linecast
			gameObject.GetComponent<BoxCollider2D>().enabled = true;
			GameManager.instance.player.gameObject.GetComponent<BoxCollider2D>().enabled = true;
			if (hit.transform != null) {
				Enemy hitEnemy = hit.transform.GetComponent<Enemy>();
				if (hitEnemy != null)
					hitEnemy.Kill();
				Ice hitIce = hit.transform.GetComponent<Ice>();
				if (hitIce != null)
					hitIce.DamageWall(3);
			}
			
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
