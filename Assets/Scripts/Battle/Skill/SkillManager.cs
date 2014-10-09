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

	public static void PlaySkill(Charactor attack , Charactor attacked , SkillConfig skillConfig){

		Skill s = null;
		switch(skillConfig.attack_type){
		case TYPE_NORMAL_ATT:
			s = new NormalAttackSkill(attack , attacked);
			break;
		case TYPE_POINT_FLYOBJECT_ATT:
			s = new PointFlyAttackSkill(attack , attacked , skillConfig.res);
			break;
		case TYPE_DIRECTIONI_FLYOBJECT_ATT:
			s = new DirectionFlyAttackSkill(attack , skillConfig);
			break;
		}

		s.Start();
		runningSkill.Add(s);
	}

}
