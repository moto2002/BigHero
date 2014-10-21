using UnityEngine;
using System.Collections;

public class SkillManager {

	//点对点攻击
	public const int TYPE_NORMAL_ATT = 1;

	//直线范围攻击
	public const int TYPE_LINE_ATT = 2;

	//以自身为中心的圆形范围攻击
	public const int TYPE_RANGE_ATT = 3;

	//除以自身为中心的圆形以外的范围攻击
	public const int TYPE_SUBTRACT_RANGE_ATT = 4;

	//前方扇形攻击
	public const int TYPE_SECTOR_ATT = 5;

	//从天而降的随机的攻击
	public const int TYPE_RANDOM_ATT = 6;

	//随机地面持续10秒的debuff攻击（火墙）
	public const int TYPE_FIREWALL_ATT = 8;

	//点对点的远程攻击
	public const int TYPE_POINT_FLYOBJECT_ATT = 9;


	public const int TYPE_DIRECTIONI_FLYOBJECT_ATT = 10;
	
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

	public static void PlaySkill(Charactor attack , SkillConfig skillConfig , Charactor attackedOne = null){

		Skill s = null;
		switch(skillConfig.attack_type){
		case TYPE_NORMAL_ATT:
			s = new NormalAttackSkill(attack , skillConfig , attackedOne);
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
		case TYPE_SUBTRACT_RANGE_ATT:
			s = new RangeAttackSkill(attack , skillConfig);
			break;
		case TYPE_FIREWALL_ATT:
			s = new FireWallAttackSkill(attack , skillConfig);
			break;
		case TYPE_RANDOM_ATT:
			s = new RandomAttackSkill(attack , skillConfig);
			break;
		case TYPE_LINE_ATT:
			s = new LineAttackSkill(attack , skillConfig);
			break;
		case TYPE_SECTOR_ATT:
			s = new SectorRangeAttackSkill(attack , skillConfig);
			break;
		}

		if(s == null){
			return;
		}
		
		s.Start();
		runningSkill.Add(s);
	}

}
