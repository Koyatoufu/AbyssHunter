using UnityEngine;

public class CInputCtrl : MonoBehaviour
{
    private InputData m_inputData = new InputData();
    public InputData InputData { get { return m_inputData; } }

    CInputCtrl()
    {
        m_inputData.btnInputs = new bool[(int)InputIndex.MAX];
    }

    public void InputProcess()
    {
        m_inputData.vertical = Input.GetAxis("Vertical");
        m_inputData.horizontal = Input.GetAxis("Horizontal");

        m_inputData.mouseAxisX = Input.GetAxis("Mouse X");
        m_inputData.mouseAxisY = Input.GetAxis("Mouse Y");
        m_inputData.rightAxisX = Input.GetAxis("RightAxisX");
        m_inputData.rightAxisY = Input.GetAxis("RightAxisY");

        m_inputData.dPadX = Input.GetAxisRaw("DPadX");
        m_inputData.dPadY = Input.GetAxisRaw("DPadY");

        // Next Button Process
        m_inputData.btnInputs[(int)InputIndex.A] = Input.GetButtonDown("A");
        m_inputData.btnInputs[(int)InputIndex.B] = Input.GetButtonDown("B");
        m_inputData.btnInputs[(int)InputIndex.X] = Input.GetButtonDown("X");
        m_inputData.btnInputs[(int)InputIndex.Y] = Input.GetButtonDown("Y");

        m_inputData.btnInputs[(int)InputIndex.RB] = Input.GetButton("RB");
        m_inputData.btnInputs[(int)InputIndex.LB] = Input.GetButtonDown("LB");

        m_inputData.btnInputs[(int)InputIndex.RT] = Input.GetButtonUp("RT");
        if (Input.GetAxis("RT") != 0f)
            m_inputData.btnInputs[(int)InputIndex.RT] = true;
        m_inputData.btnInputs[(int)InputIndex.LT] = Input.GetButtonDown("LT");
        if (Input.GetAxis("LT") != 0f)
            m_inputData.btnInputs[(int)InputIndex.LT] = true;

        m_inputData.btnInputs[(int)InputIndex.AXIS_L] = Input.GetButtonDown("LeftAxisBtn");
        m_inputData.btnInputs[(int)InputIndex.AXIS_R] = Input.GetButtonDown("RightAxisBtn");

    }
}

public struct InputData
{
    public float vertical;
    public float horizontal;

    public float mouseAxisX;
    public float mouseAxisY;

    public float rightAxisX;
    public float rightAxisY;

    public float dPadX;
    public float dPadY;

    public bool [] btnInputs;
}

public enum InputIndex
{
    //attack,itemuse, confirm
    X, Y, A, B,
    //parry,dudge,run
    LB, RB, LT, RT,
    // JUMP, LOCKON
    AXIS_L, AXIS_R,
    MAX,
    NONE
}