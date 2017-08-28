﻿using UnityEngine;
using System.Linq;
using System.Collections;
using System.Runtime.InteropServices;

public class WebVRCamera : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void FinishLoading();

	Camera cameraMain, cameraL, cameraR;

    Quaternion cq;
    Quaternion lhq;
    Quaternion rhq;
    Vector3 cp;
    Vector3 lhp;
    Vector3 rhp;
	Matrix4x4 clp = new Matrix4x4();
	Matrix4x4 clv = new Matrix4x4();
	Matrix4x4 crp = new Matrix4x4();
	Matrix4x4 crv = new Matrix4x4();


    bool active = false;
    private Vector3 rotation;

    public GameObject leftHandObj;
    public GameObject rightHandObj;

	private Matrix4x4 numbersStringToMatrix(string numbersStr) {
		float[] array = numbersStr.Split(',').Select(float.Parse).ToArray();

		var mat = new Matrix4x4 ();
		mat.m00 = array[0];
		mat.m01 = array[1];
		mat.m02 = array[2];
		mat.m03 = array[3];
		mat.m10 = array[4];
		mat.m11 = array[5];
		mat.m12 = array[6];
		mat.m13 = array[7];
		mat.m20 = array[8];
		mat.m21 = array[9];
		mat.m22 = array[10];
		mat.m23 = array[11];
		mat.m30 = array[12];
		mat.m31 = array[13];
		mat.m32 = array[14];
		mat.m33 = array[15];
		return mat;
	}
	// view and projection matrix, sent via SendMessage from webvr.js
	public void HMDLeftProjection(string numbersStr) {
		clp = numbersStringToMatrix(numbersStr);
	}

	public void HMDRightProjection(string numbersStr) {
		crp = numbersStringToMatrix(numbersStr);
	}

	public void HMDLeftView(string numbersStr) {
		clv = numbersStringToMatrix(numbersStr);
	}

	public void HMDRightView(string numbersStr) {
		crv = numbersStringToMatrix(numbersStr);
	}

    //orientation of left hand, sent via SendMessage from webvr.js
    public void LHTiltW(float w) { lhq.w = w; }
    public void LHTiltX(float x) { lhq.x = x; }
    public void LHTiltY(float y) { lhq.y = y; }
    public void LHTiltZ(float z) { lhq.z = z; }

    //position of left hand, sent via SendMessage from webvr.js
    public void LHPosX(float x) { lhp.x = x; }
    public void LHPosY(float y) { lhp.y = y; }
    public void LHPosZ(float z) { lhp.z = z; }

    //orientation of right hand, sent via SendMessage from webvr.js
    public void RHTiltW(float w) { rhq.w = w; }
    public void RHTiltX(float x) { rhq.x = x; }
    public void RHTiltY(float y) { rhq.y = y; }
    public void RHTiltZ(float z) { rhq.z = z; }

    //position of right hand, sent via SendMessage from webvr.js
    public void RHPosX(float x) { rhp.x = x; }
    public void RHPosY(float y) { rhp.y = y; }
    public void RHPosZ(float z) { rhp.z = z; }

    public void Begin()
    {
		changeMode("vr");
        active = true;
    }

	void toggleMode() {
		active = active == true ? false : true;
		string mode = active == true ? "vr" : "normal";
		changeMode (mode);
	}

	void changeMode(string mode)
	{
		Debug.Log("Switching to " + mode);
		switch (mode)
		{
		case "normal":
			cameraMain.GetComponent<Camera>().enabled = true;
			cameraL.GetComponent<Camera>().enabled = false;
			cameraR.GetComponent<Camera>().enabled = false;
			break;
		case "vr":
			cameraMain.GetComponent<Camera>().enabled = false;
			cameraL.GetComponent<Camera>().enabled = true;
			cameraR.GetComponent<Camera>().enabled = true;
			break;
		}
	}

    void Start()
    {
		cameraMain = GameObject.Find("CameraMain").GetComponent<Camera>();
		cameraL = GameObject.Find("CameraL").GetComponent<Camera>();
		cameraR = GameObject.Find("CameraR").GetComponent<Camera>();

		changeMode("normal");

       	FinishLoading();
    }

    void Update()
    {
		if (Input.GetKeyDown("space")) {
			toggleMode ();
		}


        if (active == true)
        {
            leftHandObj.transform.rotation = lhq;
            leftHandObj.transform.position = lhp;

            rightHandObj.transform.rotation = rhq;
            rightHandObj.transform.position = rhp;

			cameraL.worldToCameraMatrix = clv;
			cameraL.projectionMatrix = clp;

			cameraR.worldToCameraMatrix = crv;
			cameraR.projectionMatrix = crp;
        }
    }
}