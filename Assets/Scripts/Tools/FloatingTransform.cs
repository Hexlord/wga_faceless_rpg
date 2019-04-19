using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   aknorre     Created
 * 16.03.2019   bkrylov     Allocated to Component Menu
 * 
 */
[AddComponentMenu("ProjectFaceless/Tools/FloatingTransform")]
public class FloatingTransform : MonoBehaviour
{
    
    [Header("Float Settings")]

    [Tooltip("Vertical speed baseline")]
    public float verticalSpeed = 1.0f;
    [Tooltip("Vertical position sinusoid")]
    public float verticalSin = 0.0f;
    [Tooltip("Vertical position sinusoid multiplier")]
    public float verticalSinFactor = 0.0f;
    [Tooltip("Vertical acceleration")]
    public float verticalAccel = 0.1f;

    [Tooltip("Horizontal speed baseline")]
    public float horizontalSpeed = 0.0f;
    [Tooltip("Horizontal position sinusoid")]
    public float horizontalSin = 2.5f;
    [Tooltip("Horizontal position sinusoid multiplier")]
    public float horizontalSinFactor = 0.5f;
    [Tooltip("Horizontal acceleration")]
    public float horizontalAccel = 0.0f;

    [Tooltip("Lifespan period")]
    public float lifespan = 1.0f;
    [Tooltip("Lifespan alpha fade out")]
    public bool lifespanFade = true;

    private float speedX = 0.0f;
    private float speedY = 0.0f;

    private float x = 0.0f;
    private float y = 0.0f;

    private Vector3 startPosition = Vector3.zero;

    private float timer = 0.0f;

    protected void Start()
    {
        startPosition = transform.position;
    }

    protected virtual void SetAlpha(float alpha)
    {
        // To be overridden
    }

    // Update is called once per frame
    protected void Update()
    {
        timer += Time.deltaTime;

        if (timer >= lifespan)
        {
            Destroy(gameObject);
            return;
        }

        x += Mathf.Sin(timer * horizontalSin) * horizontalSinFactor * Time.deltaTime;

        speedX += horizontalAccel * Time.deltaTime;
        x += (horizontalSpeed + speedX) * Time.deltaTime;

        y += Mathf.Sin(timer * verticalSin) * verticalSinFactor * Time.deltaTime;

        speedY += verticalAccel * Time.deltaTime;
        y += (verticalSpeed + speedY) * Time.deltaTime;

        transform.position = startPosition + transform.right * x + transform.up * y;

        if(lifespanFade)
        {
            SetAlpha((lifespan - timer) / lifespan);
        }
    }
}
