using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // static instance 
    public static GameController gameController;
    // components
    public UIController uiController;
    public LevelController levelController;
    public ColourPalette colourPalette;
    public SoundManager soundManager;
    public CheckpointManager checkpointManager;
    public GameObject Player;
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        if (gameController != null && gameController != this)
        {
            Destroy(this);
        }
        else
        {
            gameController = this;
        }

        //DontDestroyOnLoad(this);

        uiController = GetComponent<UIController>();
        levelController = GetComponent<LevelController>();
        colourPalette = GetComponent<ColourPalette>();
        soundManager = GetComponent<SoundManager>();
        checkpointManager = GetComponent<CheckpointManager>();
        checkpointManager.save = new SaveState(Player.transform);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
    }

    public void UpdateCamera()
    {
        if (Player != null)
        {
            // update camera position
            Vector3 targetPosition = Player.transform.position;
            if (Vector3.Distance(Player.transform.position, cam.transform.position)>2)
            {
                Vector3 pos = Vector3.MoveTowards(cam.transform.position, targetPosition, 6 * Time.deltaTime);
                pos.y = cam.transform.position.y;
                cam.transform.position = pos;

            }

            // update camera rotation
            //cam.transform.LookAt(currentPlayer.transform);
            Quaternion targetRotation = Quaternion.LookRotation(Player.transform.position - cam.transform.position);
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRotation, 1 * Time.deltaTime);
        } 
    }
}
