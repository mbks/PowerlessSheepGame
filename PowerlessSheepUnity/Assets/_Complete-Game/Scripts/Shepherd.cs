using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
    public class Shepherd : MovingObject
    {

        private float timePassed = 0;
        private bool moveInstead = false;
        public AudioClip laughSound;

        public GameObject laser;

        private int hp = 10;

        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {
            timePassed += Time.deltaTime;
            if(timePassed > 1.5f) {

                if (moveInstead) {
                    int xDir, yDir = 0;
                    Transform target = GameManager.instance.player.transform;

                    yDir = target.position.y > transform.position.y ? 1 : -1;

                    xDir = target.position.x > transform.position.x ? 1 : -1;

                    AttemptMove<Wall>(xDir, yDir);
                    SoundManager.instance.PlaySingle (laughSound);
                    moveInstead = false;
                } else {
                    for (int i = 0; i < 4; i++) {
                        GameObject instanceLaser = Instantiate (laser, new Vector3 (0f, 0f, 0f), Quaternion.identity);
                        instanceLaser.GetComponent<Laser>().setOwner(this.gameObject);
                        instanceLaser.GetComponent<Laser>().AttemptMove<Enemy>(i, 4);
                        Destroy(instanceLaser, 0.4f);
                    }


                    moveInstead = true;
                }
                timePassed = 0;
            }

        }

        protected override void OnCantMove<T>(T component) {
        }

        public void Hit() {
            hp -= 1;
            print(hp);
            if (hp <= 0) {
                Die();
            }
        }

        private void Die() {
            GameManager.instance.Win();
            Destroy(this.gameObject);
        }
    }
}
