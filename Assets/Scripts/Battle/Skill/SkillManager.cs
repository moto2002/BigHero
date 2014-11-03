using UnityEngine;
using System.Collections;

public class SkillManager {
	
	public static ArrayList runningSkill = new ArrayList();

	public static void Clean(){
		for(int i = 0 ; i < runningSkill.Count ; i ++){
			Skill skill = (Skill)runningSkill[i];
			
			skill.Clean();
		}

		runningSkill = new ArrayList();
	}

	public static void Update(){

		for(int i = 0 ; i < runningSkill.Count ; i ++){
			Skill skill = (Skill)runningSkill[i];

			if(skill.IsEnd()){
				runningSkill.Remove(skill);
			}

			skill.Update();
		}
	}

	public static Skill PlaySkill(Charactor attack , SkillConfig skillConfig , Charactor attackedOne = null){

		Skill s = null;
		switch(skillConfig.id){
		case 1001:
			s = new NormalAttackSkill(attack , skillConfig , attackedOne);
			break;
		case 1002:
			s = new S1002(attack , skillConfig);
			break;
		case 1003:
			s = new S1003(attack , skillConfig);
			break;
		case 1004:
			s = new S1004(attack , skillConfig);
			break;
		case 1005:
			break;
		case 1006:
			s = new S1006(attack , skillConfig);
			break;
		case 1007:
			break;
		case 1008:
			break;
		case 1009:
			break;
		case 1010:
			break;
		case 2001:
			s = new S2001(attack , skillConfig);
			break;
		case 2002:
			break;
		case 2003:
			s = new S2003(attack , skillConfig);
			break;
		case 2004:
			s = new S2004(attack , skillConfig);
			break;
		case 2005:
			//s = new S2005(attack , skillConfig);
			break;
		case 2006:
			//s = new S2006(attack , skillConfig);
			break;
		case 2007:
			s = new S2007(attack , skillConfig);
			break;
		case 2008:
			s = new S2008(attack , skillConfig);
			break;
		case 2009:
			//s = new S2009(attack , skillConfig);
			break;
		case 2010:
			s = new S2010(attack , skillConfig);
			break;
		case 2011:
			s = new S2011(attack , skillConfig);
			break;
		}

		if(s != null){
			s.Start();
			runningSkill.Add(s);
		}

		return s;
	}

}
