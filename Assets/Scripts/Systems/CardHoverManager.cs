using System.Collections;
using System.Collections.Generic;
using Systems;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardHoverManager : MonoBehaviour
{
    public static CardHoverManager Instance { get; protected set; }

    [SerializeField]
    private TextMeshProUGUI _CursorQuantityText;

    public Card Hovered { get; protected set; }

    private static int _CursorQuantity = 0;
    public static int CursorQuantity { get => _CursorQuantity; 
        protected set {
            if (value == _CursorQuantity) return;
            _CursorQuantity = value;
            Instance._CursorQuantityText.text = value == 0 ? "" : $"x{value}";
        } 
    }

    private void Awake()
    {
        if (Instance != null) throw new System.Exception("Instance of CardHoverManager already exists.");
        Instance = this;
        _CursorQuantityText.text = "";
    }

    private void Start()
    {
        ImprovedPhysicsRaycaster.HitChanged += UpdatePointerCard;
        GameManager.Controls.Game.ChangeQuantity.performed += ChangeQuantity;
    }

    private void Update()
    {
        _CursorQuantityText.transform.position = Pointer.current.position.ReadValue();
    }

    private void ChangeQuantity(InputAction.CallbackContext context)
    {
        if (Hovered == null)
        {
            return;
        }
        CursorQuantity = Mathf.Clamp(CursorQuantity + (context.ReadValue<float>() > 0 ? 1 : -1), 0, Hovered.Quantity - 1);
    }

    private void UpdatePointerCard(Collider hit)
    {
        if (hit != null && hit.transform.parent.TryGetComponent(out Card card))
        {
            Hovered = card;
        }
        else
        {
            Hovered = null;
        }
        CursorQuantity = 0;
    }
}
