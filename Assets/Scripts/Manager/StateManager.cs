using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


//状态之间的关系类型
enum StateRelation {
	StopCurrent,	//停止当前状态，开启新状态
	KeepCurrent,	//保持当前状态，忽略新状态
	Coexist			//当前状态和新状态共存
}

public interface IStateManager {
	void OnStartState(int stateType);
	void OnInterruptState(int stateType);
	void OnStateEnd(int stateType);
}

public class StateManager {

	private int[,] _relations;		//NxN数组
	private int _stateCount;
	private List<int> _curStates = new List<int>();
	private IStateManager _handler;

	public StateManager(IStateManager handler, string csvfile) {
		_handler = handler;
		loadCSV(Application.dataPath + "/Resources/" + csvfile);
	}

	private void loadCSV(string filepath) {
		string[] str = File.ReadAllLines(filepath);
		int col = str[0].Split(',').Length-2;
		_relations = new int[str.Length-1, col];

		for (int i = 1; i < str.Length; i++) {
			string[] row = str[i].Split(',');
			for (int j = 2; j < row.Length; j++) {
				_relations[i-1, j-2] = int.Parse(row[j]);
			}
		}

		_stateCount = col;
	}

	public bool CanSwitch(object stateObj) {
		if (_curStates.Count == 0) {
			return true;
		}

		int state = (int)stateObj;
		if (state >= _stateCount) {
			Debug.LogError("State Type Error: " + state);
			return false;
		}

		StateRelation relation = 0;
		foreach (int s in _curStates) {
			relation = (StateRelation)_relations[s, state];
			if (relation == StateRelation.KeepCurrent) {
				return false;
			}
		}

		return true;
	}

	public bool Switch(object stateObj) {
		int state = (int)stateObj;

		if (!CanSwitch(state)) {
			return false;
		}

		StateRelation relation = 0;
		if (_curStates.Count > 0) {
			for (int i = _curStates.Count - 1; i >= 0; i--) {
				relation = (StateRelation)_relations[_curStates[i], state];
				if (relation == StateRelation.StopCurrent) {
					if (_handler != null) {
						_handler.OnInterruptState(_curStates[i]);
					}
					_curStates.RemoveAt(i);
				}
			}
		}

		_curStates.Add(state);

		if (_handler != null) {
			_handler.OnStartState(state);
		}

		return true;
	}

	public bool IsOn(object stateObj) {
		return _curStates.Contains((int)stateObj);
	}

	public void Interrupt(object stateObj) {
		int state = (int)stateObj;
		if (!_curStates.Contains(state)) {
			return;
		}

		_curStates.Remove(state);

		if (_handler != null) {
			_handler.OnInterruptState(state);
		}
	}

	public void End(object stateObj) {
		int state = (int)stateObj;
		if (!_curStates.Contains(state)) {
			return;
		}

		_curStates.Remove(state);

		if (_handler != null) {
			_handler.OnStateEnd(state);
		}
	}

}
