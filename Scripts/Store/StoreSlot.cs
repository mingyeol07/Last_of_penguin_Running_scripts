using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lop.Game
{
    public class StoreSlot : MonoBehaviour
    {
        //[SerializeField] private RawImage img_item;
        //[SerializeField] private TMP_Text txt_price;
        //[SerializeField] private Image img_gem;

        [SerializeField] private Button btn_explain;
        [SerializeField] private Button btn_explainBack;
        [SerializeField] private GameObject pnl_explain;
        [SerializeField] private Button btn_purchase;


        private void Awake()
        {
            btn_explain.onClick.AddListener(() => { pnl_explain.SetActive(true); });
            btn_explainBack.onClick.AddListener(() => { pnl_explain.SetActive(false); });
            btn_purchase.onClick.AddListener(() => { Purchase(); });
        }

        //public void Init(RenderTexture itemRender, int price, Sprite gemSprite)
        //{
        //    img_item.texture = itemRender;
        //    txt_price.text = price.ToString();
        //    img_gem.sprite = gemSprite;
        //}

        private void Purchase()
        {
            Destroy(gameObject);
            Destroy(pnl_explain);
            // 구매에 대한 정보를 서버에 전송
        }
    }
}