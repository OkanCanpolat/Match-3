using System;
using UnityEngine;

public class BreakableTile : BackgroundTile, DestroyReactiveTile
{
    public int hitPoint;
    [SerializeField] private BreakableTileSpriteDTO levelOfSprites;
    [SerializeField] private EventBusType eventType;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public bool ReactToDestroy()
    {
        hitPoint--;
        return ControlHitPoint();
    }

    private bool ControlHitPoint()
    {
        if(hitPoint <= 0)
        {
            Destroy(gameObject);
            EventBus.Publish(eventType);
            return true;
        }
        ChangeSprite();
        return false;
    }

    private void ChangeSprite()
    {
        spriteRenderer.sprite = levelOfSprites.TileSprites[hitPoint - 1];
    }
}
