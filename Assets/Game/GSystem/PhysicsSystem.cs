using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.GEntity;
using Game.Component;

namespace Game.Systems
{
    public class PhysicsSystem : ISystem
    {
        //public float walkSpeed = 1.0f;
		//
        //public float runSpeed = 1.0f;
		//
        //// If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
        //public bool limitDiagonalSpeed = true;
		//
        //// If checked, the run key toggles between running and walking. Otherwise player runs if the key is held down and walks otherwise
        //// There must be a button set up in the Input Manager called "Run"
        //public bool toggleRun = false;
		//
        //public float jumpSpeed = 8.0f;
        //public float gravity = 20.0f;
		//
        //// Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
        //public float fallingDamageThreshold = 10.0f;
		//
        //// If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
        //public bool slideWhenOverSlopeLimit = false;
		//
        //// If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
        //public bool slideOnTaggedObjects = false;
		//
        //public float slideSpeed = 12.0f;
		//
        //// If checked, then the player can change direction while in the air
        //public bool airControl = false;
		//
        // Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
        public float antiBumpFactor = .75f;
		//
        //// Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
        //public int antiBunnyHopFactor = 1;


        private float _speed = 1.0f;
        private float acceleration = 30;
        private float _gravity = 20;

        private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Movement, ActionQueue>();

        public void Update(GameManager game)
        {
            var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
            foreach (int e in entities)
            {
                Movement movement = game.Entities.GetComponentOf<Movement>(e);
                CharacterController charcontroller = game.Entities.GetEntity(e).gameObject.GetComponent<CharacterController>();
                GameObject go = game.Entities.GetEntity(e).gameObject;

                if (movement.Grounded)
                {
                    // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
                    if (movement.Falling)
                    {
                        movement.Falling = false;
                    }

                    movement.Movedirection = new Vector3(movement.Input.x, -antiBumpFactor * _speed, movement.Input.y);
                    movement.Input = Vector2.zero;

                    if (movement.jumpForce != 0)
                    {
                        movement.Movedirection.y = movement.jumpForce;
                        movement.jumpForce = 0;
                    }
                }
                else
                {

                    if (!movement.Falling)
                    {
                        movement.Falling = true;
                    }
                    movement.Movedirection = new Vector3(movement.Input.x, movement.Movedirection.y, movement.Input.y);
                }
                movement.Movedirection.y -= _gravity * Time.deltaTime;
                Vector3 newNove = new Vector3(movement.Movedirection.x, movement.Movedirection.y * Time.deltaTime, movement.Movedirection.z);
                movement.Grounded = (charcontroller.Move(newNove) & CollisionFlags.Below) != 0;
                movement.Input = Vector2.zero;
            }  
        }

        public void Initiate(GameManager game)
        {
            var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
            foreach (int e in entities)
            {
                GameObjects gameObjects = game.Entities.GetComponentOf<GameObjects>(e);
                gameObjects.Head.transform.eulerAngles = new Vector3(0, 90, 0);
            }
        }
        public void SendMessage(GameManager game, int reciever, Message message)
        {

        }
    }
}