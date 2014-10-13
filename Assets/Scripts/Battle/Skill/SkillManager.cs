using UnityEngine;
using System.Collections;

public class SkillManager {

	public const int TYPE_NORMAL_ATT = 0;

	public const int TYPE_POINT_FLYOBJECT_ATT = 1;
	
	public const int TYPE_DIRECTIONI_FLYOBJECT_ATT = 2;
	
	public const int TYPE_RANGE_ATT = 3;
	
	public const int TYPE_FIREWALL_ATT = 4;
	
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
			s = new PointFlyAttackSkill(attack , skillConfig);
			break;
		case TYPE_DIRECTIONI_FLYOBJECT_ATT:
			s = new DirectionFlyAttackSkill(attack , skillConfig);
			break;
		case TYPE_RANGE_ATT:
			s = new RangeAttackSkill(attack , skillConfig);
			break;
		case TYPE_FIREWALL_ATT:
			s = new FireWallAttackSkill(attack , skillConfig);
			break;
		}

		s.Start();
		runningSkill.Add(s);
	}

}
