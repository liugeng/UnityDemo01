using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroFight : MonoBehaviour {

	public static HeroFight instance;
	private StateManager _sm;

	//与目标交互的距离
	private float _interactRange = 1.5f;
	public float traceDistance {
		get {
			//与目标站位的距离，如果使用技能，就返回技能的攻击距离
			return _interactRange;
		}
	}

	//自动选择目标的范围
	private int _autoSelectRange = 10;

	//目标
	public int targetId;

	//是否是手动选择的
	private bool _selectByHand;
	

	// Use this for initialization
	void Start () {
		instance = this;
		_sm = HeroController.instance._sm;
	}
	
	// Update is called once per frame
	void Update () {
		if (NeedSwitchTarget()) {
			_selectByHand = false;

			IRole target = RoleManager.instance.GetTargetByDistance(_autoSelectRange);
			if (target) {
				targetId = target.id;
			} else {
				targetId = IRole.invalidId;
			}
		}
	}

	bool NeedSwitchTarget() {
		if (targetId != IRole.invalidId) {

			//目标已经消失
			IRole target = RoleManager.instance.Find(targetId);
			if (target == null) {
				return true;
			}

			//正在攻击
			if (_sm.IsOn(HeroState.Attack)) {
				return false;
			}

			//手动选择的并且在追踪范围以内
			if (_selectByHand) {
				float distance = HeroController.instance.GetDistance(target);
				if (distance <= 100f) {
					return false;
				}
			}
		}

		return true;
	}

	public void SelecteTarget(IRole r) {
		if (targetId == r.id) {
			return;
		}

		if (_sm.CanSwitch(HeroState.SelTarget)) {
			_sm.Switch(HeroState.SelTarget);

			targetId = r.id;
			_selectByHand = true;

			_sm.End(HeroState.SelTarget);
		}
	}

	public void Attack() {
		IRole target = GetTraceTarget();
		if (target == null) {
			return;
		}
		
		float distance = HeroController.instance.GetDistance(target);
		if (distance > traceDistance) {
			HeroMove.instance.TraceMove();
			return;
		}

		if (_sm.CanSwitch(HeroState.Attack)) {
			_sm.Switch(HeroState.Attack);
			Quaternion rotation = Quaternion.LookRotation(target.transform.position - transform.position);
			rotation.x = 0;
			rotation.z = 0;
			transform.rotation = rotation;
			Invoke("AttackOnceEnd", 1f);
		}
	}

	public void AttackOnceEnd() {
		_sm.End(HeroState.Attack);
	}

	public IRole GetTraceTarget() {
		if (targetId == IRole.invalidId) {
			return null;
		}

		return RoleManager.instance.Find(targetId);
	}

	//IEnumerator WaitForAttackAnimationEnd() {
	//	yield return new WaitForSeconds(1f);
	//	_curState = HeroState.Idle;
	//	ResetAnimation();
	//}
}
