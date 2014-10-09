using UnityEngine;
using System.Collections;

public class SkillManager {

	public const int TYPE_NORMAL_ATT = 0;

	public const int TYPE_POINT_FLYOBJECT_ATT = 1;

	public const int TYPE_DIRECTIONI_FLYOBJECT_ATT = 2;

	
	public static ArrayList runningSkill = new ArrayList();

	public static void Update(){

		for(int i = 0 ; i < runningSkill.Count ; i ++){
			Skill skill = (Skill)runningSkill[i];

			if(skill.IsEnd()){
				runningSkill.Remove(skill);
			}

			skill.Update();
		}
	}

	public static void PlaySkill(Charactor attack , Charactor attacked , int skillType , int effectId = 0){

		Skill s = null;
		switch(skillType){
		case TYPE_NORMAL_ATT:
			s = new NormalAttackSkill(attack , attacked);
			break;
		case TYPE_POINT_FLYOBJECT_ATT:
			s = new PointFlyAttackSkill(attack , attacked , effectId);
			break;
		}

		s.Start();
		runningSkill.Add(s);
	}

}
