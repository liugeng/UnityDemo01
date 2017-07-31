using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	//摄像机聚焦的对象
	public GameObject _target;
	public GameObject target {
		get {
			return _target;
		}
		set {
			_target = value;
		}
	}

	public float _xSpeed = 4;
	public float _ySpeed = 8;
	public float _xMinLimit = 1;
	public float _xMaxLimit = 40;
	public float _scrollSpeed = 20;
	public float _zoomMin = 2;
	public float _zoomMax = 10;
	public float _yOffset = 0.6f;	//相对于目标位置在Y轴上做一些偏移，可以是屏幕中心对着角色的腰而不是脚

	private float _distance;
	private float _distanceLerp;
	private Vector3 _position;
	private bool _isActivated;
	private float _x;
	private float _y;
	private bool _setupCamera;

	//减速停止动画相关
	public bool _stopWithAnimation = true;
	private float _xdlt, _ydlt;
	private bool _isOnStop;
	private float _minStartSpeed = 0.2f;  //停止时上一帧的位移大于这个数就会执行减速过程
	private float _maxStartSpped = 3.0f;  //超出最大值就设置成这个值
	private float _stopElapsed;
	private float _stopDur = 0.3f; //从减速到停止的时间
	private float _xa, _y0a;
	private bool _moveOnX, _moveOnY;

	void Start() {
		Setup();
	}

	void LateUpdate() {
		if (_target == null) {
			return;
		}
		ScrollMouse();
		RotateCamera();
	}

	void Setup() {
		if (_target == null) {
			_target = GameObject.FindGameObjectWithTag("CameraTarget");
			if (_target == null) {
				return;
			}
		}

		Vector3 angle = transform.eulerAngles;
		_x = angle.x;
		_y = angle.y;

		_distance = (_zoomMin + _zoomMax) * 0.5f;
		_distanceLerp = _distance;
		ApplyTransform();
	}

	void ApplyTransform() {
		//rotation * pos: Quaternion * Vector3
		//将Z轴上的一段距离绕X、Y轴旋转后，得到相对于_target的坐标
		//最后加上_target的坐标得到实际的世界坐标
		Quaternion rotation = Quaternion.Euler(_x, _y, 0);
		Vector3 pos = new Vector3(0, _yOffset, -_distanceLerp);
		_position = rotation * pos + _target.transform.position;
		transform.rotation = rotation;
		transform.position = _position;
	}

	void ScrollMouse() {
		_distanceLerp = Mathf.Lerp(_distanceLerp, _distance, Time.deltaTime * 5);
		if (Input.GetAxis("Mouse ScrollWheel") != 0) {
			_distance = Vector3.Distance(transform.position, _target.transform.position);
			_distance = Mathf.Clamp(_distance - Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed, _zoomMin, _zoomMax);
		}
	}

	//鼠标右键拖拽
	void RotateCamera() {
		if (Input.GetMouseButtonDown(1)) { //鼠标右键
			_isActivated = true;
			_isOnStop = false;
		}
		else if (Input.GetMouseButtonUp(1)) {
			_isActivated = false;

			//判断是否需要停止动画
			if (_stopWithAnimation) {
				//限制停止时的最大速度
				_xdlt = Mathf.Clamp(_xdlt, -_maxStartSpped, _maxStartSpped);
				_ydlt = Mathf.Clamp(_ydlt, -_maxStartSpped, _maxStartSpped);

				float xdltAbs = Mathf.Abs(_xdlt);
				float ydltAbs = Mathf.Abs(_ydlt);

				_moveOnX = xdltAbs > _minStartSpeed;
				_moveOnY = ydltAbs > _minStartSpeed;

				if (_moveOnX || _moveOnY) {
					_isOnStop = true;
					_stopElapsed = 0f;

					//计算减速度
					if (_moveOnX) {
						_xa = xdltAbs / _stopDur * _xdlt / xdltAbs;
					}

					if (_moveOnY) {
						_y0a = ydltAbs / _stopDur * _ydlt / ydltAbs;
					}
				}
			}
		}

		if (_isActivated || _isOnStop) {
			if (_isOnStop) {
				_stopElapsed += Time.deltaTime;
				if (_stopElapsed >= _stopDur) {
					_isOnStop = false;
					return;
				}
				else {
					//timeRatio使减速先急后缓
					float timeRatio = Mathf.Sqrt(_stopElapsed / _stopDur);
					if (_moveOnX) {
						_x -= _xdlt - _xa * _stopElapsed * timeRatio;
					}

					if (_moveOnY) {
						_y += _ydlt - _y0a * _stopElapsed * timeRatio;
					}
				}
			}
			else {
				//鼠标在屏幕Y轴上的移动使摄像机绕X轴转动
				_xdlt = Input.GetAxis("Mouse Y") * _xSpeed;
				_ydlt = Input.GetAxis("Mouse X") * _ySpeed;
				_x -= _xdlt;
				_y += _ydlt;
			}

			_x %= 360;
			_x = Mathf.Clamp(_x, _xMinLimit, _xMaxLimit);
		}

		ApplyTransform();
	}
}
