using UnityEngine;

public class Object_ItemPickup : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;
    [Space]
    [SerializeField] private ItemDataSO itemData;
    [SerializeField] private Vector2 dropForce = new Vector2(2,10);


    private void OnValidate()
    {
        if (itemData == null)
            return;

        sr = GetComponent<SpriteRenderer>();
        SetupVisuals();
    }

    public void SetupItem(ItemDataSO itemData)
    {

        this.itemData = itemData;
        SetupVisuals();
        transform.localScale = itemData.imgScale; // ép cứng scale sau khi setup visual
        float xDropForce = Random.Range(-dropForce.x, dropForce.x);
        rb.linearVelocity = new Vector2(xDropForce, dropForce.y);
        col.isTrigger = false;
    }
    private void SetupVisuals()
    {

        sr.sprite = itemData.itemIcon;                  //khuyến khích k nên dùng nếu muốn có thể thử
        gameObject.name = "Object_ItemPickup - " + itemData.itemName;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && col.isTrigger == false)
        {
            col.isTrigger = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        InventoryPlayer inventory = collision.GetComponent<InventoryPlayer>();
        if (inventory == null)
            return;
        InventoryItem itemToAdd = new InventoryItem(itemData);

        if (inventory.CanAddItem(itemToAdd))
        {
            inventory.AddItem(itemToAdd);
            Destroy(gameObject);
        }
    }

}
