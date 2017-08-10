using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class AnimationMovement : MonoBehaviour {

    // Use this for initialization
    private List<Vector2> _headshot = new List<Vector2>()
    {
        new Vector2(0,0),
        new Vector2(0,0),
        new Vector2(-0.04f,0), // 2
        new Vector2(-0.07f,0), // 3
        new Vector2(-0.07f,0),
        new Vector2(-0.07f,0),
        new Vector2(-0.05f,0), // 6
        new Vector2(-0.04f,0),
        new Vector2(-0.021f,0),
        new Vector2(-0.021f,0),
        new Vector2(0,0.025f),
        new Vector2(0,0.025f),
        new Vector2(0.011f,0.035f),
        new Vector2(0.011f,0.035f),
        new Vector2(0.011f,0.035f),
        new Vector2(0.021f,0.067f),
        new Vector2(0.058f,0.087f),
        new Vector2(0.075f,0.1f),       //17

        new Vector2(0.09f, 0.11f), // 18
        new Vector2(0.12f, 0.157f),
        new Vector2(0.134f, 0.174f),
        new Vector2(0.169f, 0.2f),
        new Vector2(0.2f, 0.24f),  // 22
        new Vector2(0.213f, 0.256f),
        new Vector2(0.225f, 0.291f),
        new Vector2(0.26f, 0.329f),
        new Vector2(0.272f, 0.364f),

        new Vector2(0.3f,0.4f),
        new Vector2(0.3f,0.41f),
        new Vector2(0.318f,0.428f),
        new Vector2(0.333f,0.453f),
        new Vector2(0.364f,0.471f),
        new Vector2(0.364f,0.5f),
        new Vector2(0.379f,0.544f),
        new Vector2(0.459f,0.572f),
        new Vector2(0.471f,0.606f),
        new Vector2(0.483f,0.637f),

        new Vector2(0.49f,0.637f),
        new Vector2(0.51f,0.671f),
        new Vector2(0.53f,0.714f),
        new Vector2(0.55f,0.748f),
        new Vector2(0.57f,0.748f),
        new Vector2(0.58f,0.798f),

        new Vector2(0.60f,0.831f),
        new Vector2(0.6171f,0.868f),
        new Vector2(0.6371f,0.911f),

    };
    private SpriteRenderer _spriteRenderer;
    private bool _init = false;
    private Vector3 _initPosition;
    private float _eulerZ;
    private float rotationSpeed = 70;
    private bool _hitGround = false;
    void Start () {

        var parent = transform.root;
        var spriteTransform = parent.Find("ParentSprite");
        _spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
        //GetComponent<Animation>().Play("deathArm");
    }

    // Update is called once per frame new Vector3(_headshot[index].x, _headshot[index].y, 0) + _initPosition;
    void Update ()
    {
        if (_spriteRenderer.sprite == null)
            return;

        string astr = _spriteRenderer.sprite.name;
        var s = astr.Substring(0, astr.Length -2);

        if (s == "HEADSHOT2" || s == "HEADSHOT2_")
        {
            // LERPA AXEL POSITION
            var tempEuler = transform.localEulerAngles;

            
            Vector3 transformRight = transform.right;

            var gunPosition = transform.position + (transformRight * 0.65f);
            while(gunPosition.y < (transform.root.position.y+0.1f))
            {
                _hitGround = true;

                tempEuler.z += Time.deltaTime * 1;
                transform.localEulerAngles = tempEuler;
                transformRight = transform.right;
                gunPosition = transform.position + (transformRight * 0.65f);
            }

            if (!(tempEuler.z < 276 && tempEuler.z > 90) && !_hitGround)
            {
                tempEuler.z -= Time.deltaTime * rotationSpeed;
            }
            transform.localEulerAngles = tempEuler;

            if (!_init)
            {
                var pos = transform.parent.localPosition;
                transform.parent = transform.root;
                _initPosition = pos;
                _init = true;
                _eulerZ = transform.localEulerAngles.z;
                GetComponent<Animator>().SetTrigger("headshot");
            }
            int index = 0;
            if (astr.Length == 11)
            {
                index = Int32.Parse(astr[astr.Length - 1].ToString()) + 1;
                transform.localPosition = new Vector3(0, -_headshot[index].y, -_headshot[index].x) + _initPosition;
            }
            if (astr.Length == 12)
            {
                var first = Int32.Parse(astr[astr.Length - 1].ToString());
                var second = Int32.Parse(astr[astr.Length - 2].ToString()) * 10;

                index = first + second + 1;
                if(index < 45)
                    transform.localPosition = new Vector3(0, -_headshot[index].y, -_headshot[index].x) + _initPosition;
            }
            //if(index)
        }
    }
}
