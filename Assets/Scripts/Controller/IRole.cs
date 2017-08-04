using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoleKind {
	None, Hero, Player, Monster, Npc
}

public abstract class IRole : MonoBehaviour {

	private HighlightableObject _highlight;

	private RoleKind _kind;
	public RoleKind kind {
		get {
			return _kind;
		}
		protected set {
			_kind = value;

			if (value == RoleKind.Hero) {
				_highlight.constantColor = Color.green;
			} else if (value == RoleKind.Player) {
				_highlight.constantColor = Color.yellow;
			} else if (value == RoleKind.Monster) {
				_highlight.constantColor = Color.red;
			} else if (value == RoleKind.Npc) {
				_highlight.constantColor = Color.blue;
			}
		}
	}

	private static int _autoId = 0;
	
	public int id;
	public const int invalidId = -1;

	protected CharacterController _controller;
	protected Animator _animator;



	protected void Awake() {
		id = _autoId++;

		gameObject.layer = GameLayer.Role;

		_controller = GetComponent<CharacterController>();
		_animator = GetComponent<Animator>();
		_highlight = gameObject.AddComponent<HighlightableObject>();

		if (RoleManager.instance != null) {
			RoleManager.instance.Push(this);
		}
	}

	protected void Update() {
		if (kind != RoleKind.Hero) {
			ShowOutline(id == HeroFight.instance.targetId);
		}
	}

	public void ShowOutline(bool show) {
		_highlight.constantly = show;
	}
}
