using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public interface IJoyStickHandler {
	void OnJoyStickDrag(Vector2 delta);
	void OnJoyStickStop();
}

public class JoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

	public static JoyStick instance;

	[SerializeField]
	private GameObject _thumb;
	[SerializeField]
	private float _radius;

	private Vector2 _startPosition;
	public IJoyStickHandler handler { get;set; }

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		_startPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnPointerDown(PointerEventData eventData) {
		if ((eventData.position - _startPosition).magnitude > _radius) {
			return;
		}
		transform.position = eventData.position;
	}

	public void OnPointerUp(PointerEventData eventData) {
		transform.position = _startPosition;
		_thumb.transform.localPosition = Vector2.zero;

		if (handler != null) {
			handler.OnJoyStickStop();
		}
	}

	public void OnDrag(PointerEventData eventData) {
		Vector2 delta = (Vector2)transform.position - eventData.position;
		if (delta.magnitude > _radius) {
			transform.position = eventData.position + delta * _radius / delta.magnitude;
		}
		_thumb.transform.position = eventData.position;

		if (handler != null) {
			handler.OnJoyStickDrag(delta * -1);
		}
	}
}
