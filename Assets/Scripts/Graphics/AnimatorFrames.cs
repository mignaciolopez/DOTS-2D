using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorFrames : MonoBehaviour
{
    enum States
    {
        Idle = 0,
        Animating,
        Waiting
    }

    States state;

    SpriteRenderer spriteRenderer;
    Sprite spriteBeforeAnimation;

    public System.UInt32 animationID = 25;
    public bool loop = true;
    
    int frameIndex = 0;

    [SerializeField]
    float timeInfluence = 100f;


    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteBeforeAnimation = spriteRenderer.sprite;
        state = States.Idle;
    }

    private void Update()
    {
        switch(state)
        {
            case States.Idle:
                if(loop)
                    Animate();
                break;
            case States.Animating:
                StartCoroutine(WaitForNextFrameAndPlay());
                break;
            default:
                break;
        }            
    }

    public void Animate()
    {
        frameIndex = 0;
        //spriteBeforeAnimation = spriteRenderer.sprite;
        Play();
    }

    void Play()
    {
        if (!GameGraphics.instance.spriteFrames.ContainsKey(animationID))
        {
            state = States.Idle;
            frameIndex = 0;
            Debug.LogWarning("Animation ID Not Found.");
            return;
        }

        state = States.Animating;

        spriteRenderer.sprite = GameGraphics.instance.spriteFrames[animationID][frameIndex];

        frameIndex++;

        if (frameIndex >= GameGraphics.instance.spriteFrames[animationID].Count)
        {
            frameIndex = 0;
            if (!loop)
            {
                state = States.Idle;
                spriteRenderer.sprite = spriteBeforeAnimation;
            }
        }
    }

    IEnumerator WaitForNextFrameAndPlay()
    {
        state = States.Waiting;
        yield return new WaitForSeconds(GameGraphics.instance.indexes[animationID].speed / timeInfluence * Time.deltaTime);
        Play();
    }
}
