using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class SpellHudManager : MonoBehaviour
{
    public enum SpellType { Thunder, Iceberg, Wind }
    
    private List<SpellType> spells = new List<SpellType>(); // Dynamic spell list
    private int currentSpellIndex = 0; // 
    
    [SerializeField] private ThunderSpellCast thunderSpellCast;
    [SerializeField] private IcebergSpellCast icebergSpellCast;
    [SerializeField] private WindSpellCast windSpellCast;
    
    [Header("HUD Elements")]
    [SerializeField] private Image currentSpellImage; // Reference to the UI Image component that shows the current spell
    [SerializeField] private Sprite thunderSpellSprite; // Sprite for Thunder spell
    [SerializeField] private Sprite icebergSpellSprite; // Sprite for Iceberg spell
    [SerializeField] private Sprite windSpellSprite; // Sprite for Wind spell
    
    private PlayerInput playerInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize spell list (add more spells here in the future)
        spells.Add(SpellType.Thunder);
        spells.Add(SpellType.Iceberg);
        spells.Add(SpellType.Wind);

        // Find spell cast components
        thunderSpellCast = GetComponent<ThunderSpellCast>();
        icebergSpellCast = GetComponent<IcebergSpellCast>();
        windSpellCast = GetComponent<WindSpellCast>();

        if (thunderSpellCast == null || icebergSpellCast == null || windSpellCast == null)
        {
            Debug.LogError("Spell scripts not attached to the same GameObject as the HUD Manager.");
        }

        // Get the PlayerInput component
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component missing!");
            return;
        }

        // Register input actions
        var castAction = playerInput.actions["Cast"];
        castAction.performed += OnCastSpell;

        var navigateAction = playerInput.actions["NavigateSpells"]; // Handles arrow key navigation
        navigateAction.performed += OnNavigateSpell;
        
        UpdateHUD();
    }

    private void OnDestroy()
    {
        if (playerInput != null)
        {
            var castAction = playerInput.actions["Cast"];
            castAction.started -= OnCastSpell;

            var navigateAction = playerInput.actions["NavigateSpells"];
            navigateAction.started -= OnNavigateSpell;
        }
    }

    // Method called when the 'Cast' input action is performed
    public void OnCastSpell(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) // Only trigger on button press (not hold)
        {
            switch (spells[currentSpellIndex])
            {
                case SpellType.Thunder:
                    thunderSpellCast?.OnCast(); // Call ThunderSpellCast method
                    break;

                case SpellType.Iceberg:
                    icebergSpellCast?.OnCastIce(); // Call IcebergSpellCast method
                    break;
                
                case SpellType.Wind:
                    windSpellCast?.CastWindSpell();
                    break;
            }
        }
    }
    
    public void OnNavigateSpell(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>(); // Get input as a Vector2 for left/right navigation

        if (context.phase == InputActionPhase.Started)
        {
            if (input.x > 0) // Right arrow: Move forward in the spell list
            {
                currentSpellIndex = (currentSpellIndex + 1) % spells.Count; // Wraps around to the start of the list
            }
            else if (input.x < 0) // Left arrow: Move backward in the spell list
            {
                currentSpellIndex = (currentSpellIndex - 1 + spells.Count) % spells.Count; // Wraps around to the end of the list
            }
        }

        // Debug or call HUD refresh logic here
        Debug.Log($"Selected spell: {spells[currentSpellIndex]}");
        UpdateHUD();
    }
    
    private void UpdateHUD()
    {
        
        // Ensure the HUD updates based on the currently selected spell
        switch (spells[currentSpellIndex])
        {
            case SpellType.Thunder:
                currentSpellImage.sprite = thunderSpellSprite; // Show Thunder spell image
                break;
            case SpellType.Iceberg:
                currentSpellImage.sprite = icebergSpellSprite; // Show Iceberg spell image
                break;
            case SpellType.Wind:
                currentSpellImage.sprite = windSpellSprite; // Show Wind spell image
                break;
        }
        Debug.Log($"HUD updated to show selected spell: {spells[currentSpellIndex]}");
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
