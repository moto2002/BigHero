﻿using UnityEngine;
using System.Collections;

public class Charactor : MonoBehaviour {

	public const int TYPE_MONSTER = -1;

	public const int TYPE_FOLLOWER = 1;

	public const int TYPE_HERO = 1;

	public virtual Attribute GetAttribute(){return null;}

	public virtual MoveDirection GetDirection(){return MoveDirection.UP;}

	public virtual void ChangeHP(float hp , bool crit){}

	public virtual void StopMoving(){}
	
	public virtual void PlayMoving(){}

	public virtual void PlayAttack(){}

	public virtual void PlayAttacked(){}

	public virtual void PlayDead(){}

	public virtual void PlaySkillAttack(){}

	public virtual void SetPlayLock(bool b){}

	public virtual void SetSpec(bool b){}

	public virtual bool IsInAttIndex(){return false;}

	public virtual int GetType(){return TYPE_MONSTER;}

	public virtual Vector2 GetPoint(){return Vector2.zero;}

	public virtual bool IsActive(){return false;}

	public virtual void PlayEffect(int type){}
}
