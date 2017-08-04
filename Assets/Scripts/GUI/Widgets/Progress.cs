using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LGUI {

	public class Progress : UIBehaviour {

		[SerializeField]
		private float _minValue = 0;
		public float minValue { get { return GetFloat(ref _minValue); } set { SetFloat(ref _minValue, Mathf.Min(maxValue, value)); } }

		[SerializeField]
		private float _maxValue = 1;
		public float maxValue { get { return GetFloat(ref _maxValue); } set { SetFloat(ref _maxValue, Mathf.Max(minValue, value)); } }

		[SerializeField]
		private float _value = 0;
		private float _oldValue;
		public float value { get { return GetFloat(ref _value); } set { _oldValue = normalized; SetFloat(ref _value, Mathf.Clamp(value, minValue, maxValue)); } }

		[SerializeField]
		private bool _wholeNumber = false;
		public bool wholeNumber { get { return _wholeNumber; } set { if (_wholeNumber != value) { _wholeNumber = value; UpdateVisuals(); } } }

		public float normalized {
			get {
				if (Mathf.Approximately(minValue, maxValue)) {
					return 0;
				}
				return Mathf.InverseLerp(minValue, maxValue, value);
			}
			set {
				this.value = Mathf.Lerp(minValue, maxValue, value);
			}
		}

		[SerializeField]
		private RectTransform _fillRect;
		public RectTransform fillRect {
			get {
				return _fillRect;
			}
			set {
				if ((_fillRect == null && value == null) || (_fillRect != null && _fillRect.Equals(value))) {
					return;
				}
				_fillRect = value;
				UpdateVisuals();
			}
		}

		[SerializeField]
		private Image _fillImage;
		public Image fillImage { get; set; }

		[SerializeField]
		private bool _zeroAnimate;
		public bool zeroAnimate { get; set; }

		[SerializeField]
		private bool _valueAnimate;
		public bool valueAnimate { get; set; }

		// value changed animation
		private bool _isRunningValueAnim;
		private float _valueAnimDur = 0.3f;
		private float _valueAnimElapsed;
		private float _startAnchorMaxX;
		private float _deltaAnchorMaxX;
		private Vector2 _tempAnchorMax;

		private bool _isRunningZeroAnim;
		private float _zeroAnimDur = 0.7f;
		private float _zeroAnimElapsed;
		private float _startAlpha;
		private float _deltaAlpha;
		private Color _tempColor;


		protected override void Awake() {
			float savedValue = value;
			_value = minValue - 1;
			SetValue(savedValue, false);
		}

		public void SetValue(float val, bool animate = true) {
			if (animate) {
				value = val;
			} else {
				bool savedValueAnimate = _valueAnimate;
				bool savedZeroAnimate = _zeroAnimate;

				_valueAnimate = false;
				_zeroAnimate = false;

				_isRunningValueAnim = false;
				_isRunningZeroAnim = false;

				value = val;
			
				_valueAnimate = savedValueAnimate;
				_zeroAnimate = savedZeroAnimate;
			}
		}

		float GetFloat(ref float value) {
			if (wholeNumber) {
				return Mathf.Round(value);
			}
			return value;
		}

		void SetFloat(ref float curValue, float newValue) {
			if (curValue == newValue) {
				return;
			}

			if (wholeNumber) {
				curValue = Mathf.Round(newValue);
			} else {
				curValue = newValue;
			}

			_value = Mathf.Clamp(_value, minValue, maxValue);

			UpdateVisuals();
		}

		void UpdateVisuals() {
			if (_fillRect == null) {
				return;
			}

			float normalizedValue = normalized;

			if (_valueAnimate && _valueAnimDur > 0 && !Mathf.Approximately(_oldValue, normalizedValue)) {
				_isRunningValueAnim = true;
				_valueAnimElapsed = 0;
				_tempAnchorMax = fillRect.anchorMax;
				_startAnchorMaxX = _tempAnchorMax.x;
				_deltaAnchorMaxX = normalizedValue - _startAnchorMaxX;

				if (_oldValue == 0 && _zeroAnimate && _zeroAnimDur > 0 && _fillImage) {
					_isRunningZeroAnim = true;
					_zeroAnimElapsed = 0;
					_tempColor = _fillImage.color;
					_startAlpha = _tempColor.a;
					_deltaAlpha = 1 - _startAlpha;
				}

			} else {
				Vector2 anchorMax = Vector2.one;
				anchorMax.x = normalizedValue;
				fillRect.anchorMax = anchorMax;

				if (_fillImage) {
					float a = value == 0 ? 0 : 1;
					if (_fillImage.color.a != a) {
						_tempColor = _fillImage.color;
						_tempColor.a = a;
						_fillImage.color = _tempColor;
					}
				}
			}
		}

		void Update() {
			if (_valueAnimate) {
				if (_isRunningValueAnim) {
					_valueAnimElapsed += Time.deltaTime;
					if (_valueAnimElapsed >= _valueAnimDur) {
						_tempAnchorMax.x = _startAnchorMaxX + _deltaAnchorMaxX;
						_isRunningValueAnim = false;

						if (value == 0 && _zeroAnimate && _zeroAnimDur > 0 && _fillImage) {
							_isRunningZeroAnim = true;
							_zeroAnimElapsed = 0;
							_tempColor = _fillImage.color;
							_startAlpha = _tempColor.a;
							_deltaAlpha = -_startAlpha;
						}

					} else {
						float ratio = 1 - Mathf.Pow(_valueAnimElapsed / _valueAnimDur - 1, 2);
						_tempAnchorMax.x = _startAnchorMaxX + _deltaAnchorMaxX * ratio;//_elapsed / _valueAnimDur;
					}

					fillRect.anchorMax = _tempAnchorMax;
				}

				if (_isRunningZeroAnim) {
					_zeroAnimElapsed += Time.deltaTime;
					if (_zeroAnimElapsed >= _zeroAnimDur) {
						_tempColor.a = _startAlpha + _deltaAlpha;
						_isRunningZeroAnim = false;

					} else {
						float ratio = 1 - Mathf.Pow(_zeroAnimElapsed / _zeroAnimDur -1, 2);
						_tempColor.a = _startAlpha + _deltaAlpha * ratio;
					}

					_fillImage.color = _tempColor;
				}
			}
		}

#if UNITY_EDITOR
		protected override void OnValidate() {
			base.OnValidate();

			if (wholeNumber) {
				_minValue = Mathf.Round(_minValue);
				_maxValue = Mathf.Round(_maxValue);
				_value = Mathf.Round(_value);
			}

			_value = Mathf.Clamp(_value, _minValue, _maxValue);

			if (IsActive()) {
				if (Application.isPlaying) {
					UpdateVisuals();
					_oldValue = normalized;
				} else {
					float savedValue = value;
					_value = minValue - 1;
					SetValue(savedValue, false);
				}
			}
		}
#endif // if UNITY_EDITOR
	}

}