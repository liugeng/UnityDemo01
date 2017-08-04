using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMenu : MonoBehaviour {

	public GameObject _toggleBtn;
	public GameObject[] _buttons;
	public float _shrinkedSize;
	public float _expandedSize;
	public float _animationDuration;

	enum State { None, Shrinking, Shrinked, Expanding, Expanded }
	private State _state = State.None;
	private RectTransform _rt;
	private Vector2 _sizeDelta;
	private float _curDuration;
	private float _elapsed;
	private float _origionY;
	private float _distance;


	void Start () {
		_rt = transform as RectTransform;
		_sizeDelta = _rt.sizeDelta;
		Shrink(false);
	}
	
	void Update () {
		if (_state == State.Shrinking) {
			_elapsed += Time.deltaTime;
			if (_elapsed >= _curDuration) {
				_state = State.Shrinked;
				_sizeDelta.y = _shrinkedSize;
				_rt.sizeDelta = _sizeDelta;

			} else {
				_sizeDelta.y = _origionY - _distance * _elapsed / _curDuration;
				_rt.sizeDelta = _sizeDelta;
			}
		} else if (_state == State.Expanding) {
			_elapsed += Time.deltaTime;
			if (_elapsed >= _curDuration) {
				_state = State.Expanded;
				_sizeDelta.y = _expandedSize;
				_rt.sizeDelta = _sizeDelta;

			} else {
				_sizeDelta.y = _origionY + _distance * _elapsed / _curDuration;
				_rt.sizeDelta = _sizeDelta;
			}
		}
	}

	public void OnToggleClick() {
		if (_state == State.Shrinking || _state == State.Shrinked) {
			Expand(true);
		} else if (_state == State.Expanding || _state == State.Expanded) {
			Shrink(true);
		}
	}

	public void Shrink(bool withAnimation = true) {
		if (_state == State.Shrinking || _state == State.Shrinked) {
			return;
		}

		if ((int)_shrinkedSize == 0 || ((int)_expandedSize == 0)) {
			return;
		}

		if (withAnimation) {
			_state = State.Shrinking;
			_elapsed = 0f;
			_origionY = _rt.sizeDelta.y;
			_distance = _rt.sizeDelta.y - _shrinkedSize;
			_curDuration = _animationDuration * _distance / (_expandedSize - _shrinkedSize);

		} else {
			_state = State.Shrinked;
			_sizeDelta.y = _shrinkedSize;
			_rt.sizeDelta = _sizeDelta;
		}
	}

	public void Expand(bool withAnimation = true) {
		if (_state == State.Expanding || _state == State.Expanded) {
			return;
		}

		if ((int)_shrinkedSize == 0 || ((int)_expandedSize == 0)) {
			return;
		}

		if (withAnimation) {
			_state = State.Expanding;
			_elapsed = 0f;
			_origionY = _rt.sizeDelta.y;
			_distance = _expandedSize - _rt.sizeDelta.y;
			_curDuration = _animationDuration * _distance / (_expandedSize - _shrinkedSize);

		} else {
			_state = State.Expanded;
			_sizeDelta.y = _expandedSize;
			_rt.sizeDelta = _sizeDelta;
			
		}
	}
}
