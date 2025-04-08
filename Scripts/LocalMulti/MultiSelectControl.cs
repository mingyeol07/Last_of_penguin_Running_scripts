using Lop.Game;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MultiSelectControl : MonoBehaviour
{
    private MultiSelectManager multiSelectManager;
    private PlayerInput playerInput;
    private PenguinBase penguin;
    private int user = -1;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        SceneManager.sceneLoaded += LoadedsceneEvent;

        playerInput = GetComponentInParent<PlayerInput>();
        multiSelectManager = FindAnyObjectByType<MultiSelectManager>();

        user = playerInput.user.index;
    }
    private void LoadedsceneEvent(Scene scene, LoadSceneMode mode) {

        if(scene.name == SceneNameString._06_LocalMulti2 || scene.name == SceneNameString._06_LocalMulti4) {
            var device = playerInput.devices[0];
            penguin = GameManagerParent.Instance.GetMyPenguin(user + 1).GetComponent<PenguinBase>();

            //penguin.GetComponent<PlayerInput>().SwitchCurrentControlScheme(device);

            Destroy(penguin.GetComponent<PlayerInput>());
            playerInput.SwitchCurrentActionMap("InGame");
        }
        else if(scene.name == SceneNameString._99_Loading) {

        }
        else {
            SceneManager.sceneLoaded -= LoadedsceneEvent;
            Destroy(gameObject);
        }
    }

    #region UI Control
    public void OnMoveRight() {
        if (multiSelectManager == null) return;
        multiSelectManager.MoveRight(user);
    }
    public void OnMoveLeft() {
        if (multiSelectManager == null) return;
        multiSelectManager.MoveLeft(user);
    }
    public void OnSelect() {
        if (multiSelectManager == null) return;
        multiSelectManager.SelectPenguin(user);
    }
    public void OnBack() {
        if (multiSelectManager == null) return;
        multiSelectManager.BackHome();
    }
    #endregion
    public void OnJump(InputValue value) {
        if (penguin == null) return;
        penguin.OnJump(value);
    }
    public void OnSlide(InputValue value) {
        if (penguin == null) return;
        penguin.OnSlide(value);
    }
    public void OnAttack(InputValue value) {
        if (penguin == null) return;
        penguin.OnAttack(value);
    }
}
