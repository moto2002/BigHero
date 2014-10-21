using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;


public class Config {

	private static Config instance;

	public static Config GetInstance(){
		if(instance == null){
			instance = new Config();
		}

		return instance;
	}

	private JsonData grandConfig;

	private JsonData characterConfig;

	private JsonData skillConfig;

	public Config(){
		grandConfig = JsonMapper.ToObject(Loader.GetText("Config/GroundConfig"));
		characterConfig = JsonMapper.ToObject(Loader.GetText("Config/Character"));
		skillConfig = JsonMapper.ToObject(Loader.GetText("Config/Skill"));
	}


	public JsonData GetGroundConfig(int id){
		JsonData config;


		for(int i = 0 ; i < grandConfig.Count ; i++){
			config = grandConfig[i];

			if((int)config["id"] == id){
				return config;
			}
		}

		return null;
	}

	public Attribute GetCharacterConfig(int id , int level = 1){

		Attribute attribute = new Attribute();

		JsonData elem = characterConfig[id.ToString()];

		attribute.level = level;
		attribute.description = elem["description"].ToString();
		attribute.addhp = (int)elem["addhp"];
		attribute.name = elem["name"].ToString();
		attribute.opentype = (int)elem["opentype"];
		attribute.maxHp = (int)elem["hp"];
		attribute.hp = attribute.maxHp;
		attribute.addatk = (int)elem["addatk"];
		attribute.atk = (int)elem["atk"];
		attribute.need = (int)elem["need"];
		attribute.crit = (int)elem["crit"];
		attribute.mod = (int)elem["mod"];
		attribute.type = (int)elem["type"];
		attribute.opennum = (int)elem["opennum"];
		attribute.volume = (int)elem["volume"];
		attribute.nskill = (int)elem["nskill"];
		attribute.sskill = (int)elem["sskill"];
		attribute.equip = (int)elem["equip"];

		return attribute;
	}

	public SkillConfig GetSkillCOnfig(int id){

		SkillConfig config = new SkillConfig();


		for(int i = 0 ; i < skillConfig.Count ; i++){

			JsonData skillJsonData = skillConfig[i] as JsonData;

			if((int)skillJsonData["id"] == id){
				config.id = (int)skillJsonData["id"];
				config.attack_type = (int)skillJsonData["attack_type"];
				config.name = (string)skillJsonData["name"];
				config.target_num = (int)skillJsonData["target_num"];
				config.param1 = (int)skillJsonData["param1"];
				config.param2 = (int)skillJsonData["param2"];
				config.res = (int)skillJsonData["res"];
				config.target = (int)skillJsonData["target"];
				config.range  = (int)skillJsonData["range"];
				config.singTime  = (int)skillJsonData["singtime"];
				break;
			}
		}

		return config;
	}
}
