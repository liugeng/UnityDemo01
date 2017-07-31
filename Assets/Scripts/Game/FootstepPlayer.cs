using UnityEngine;
using System.Collections;

public class FootstepPlayer : MonoBehaviour
{
	[SerializeField]
	private AudioClipCollection clips;
	[SerializeField]
	private AudioSource sourceSettings;
	private Animator animator;

	private AudioSource left;
	private AudioSource right;

    //[SerializeField]
    //private GameObject flower;

	private void Start()
	{
		animator = GetComponent<Animator>();

		Transform foot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
		left = Instantiate<AudioSource>(sourceSettings);
		Attatch(left.transform, foot);

		foot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
		right = Instantiate<AudioSource>(sourceSettings);
		Attatch(right.transform, foot);
	}
     
	static private void Attatch(Transform child, Transform parent)
	{
		child.parent = parent;
		child.localPosition = Vector3.zero;
		child.localRotation = Quaternion.identity;
	}

	public void FootL(int dummy)
	{
		left.PlayOneShot(clips.GetRandom(), GetRandomVolumeScale());
        
        // [2017-7-12 lg] 脚印
        //GameObject go = Instantiate(flower);
        //Transform foot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        //go.transform.position = foot.transform.position;
        //go.transform.rotation = foot.rotation;
        //go.transform.localScale = Vector3.one * 0.1f;
	}

	public void FootR(int dummy)
	{
		right.PlayOneShot(clips.GetRandom(), GetRandomVolumeScale());

		// [2017-7-12 lg] 脚印
		//GameObject go = Instantiate(flower);
		//Transform foot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
		//go.transform.position = foot.transform.position;
		//go.transform.localScale = Vector3.one * 0.1f;
	}

	static private float GetRandomVolumeScale()
	{
		const float minRandom = 0.7f;
		return Random.Range(minRandom, 1f);
	}
}
