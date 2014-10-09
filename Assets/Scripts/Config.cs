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


	public Config(){
		grandConfig = JsonMapper.ToObject(Loader.GetText("Config/GroundConfig"));
		characterConfig = JsonMapper.ToObject(Loader.GetText("Config/Character"));
	}


	public JsonData GetGroundConfig(int id){
		return grandConfig[id.ToString()];
	}

	public Attribute GetCharacterConfig(int id , int level = 1){

		Attribute attribute = new Attribute();

		JsonData elem = characterConfig[id.ToString()];

		attribute.level = level;
		attribute.description = elem["description"].ToString();
		attribute.addhp = (int)elem["addhp"];
		attribute.name = elem["name"].ToString();
		attribute.opentype = (int)elem["opentype"];
		attribute.attribute = (int)elem["attribute"];
		attribute.maxHp = (int)elem["hp"];
		attribute.hp = attribute.maxHp;
		attribute.addatk = (int)elem["addatk"];
		attribute.atk = (int)elem["atk"];
		attribute.need = (int)elem["need"];
		attribute.range = (int)elem["range"];
		attribute.crit = (int)elem["crit"];
		attribute.mod = (int)elem["mod"];
		attribute.type = (int)elem["type"];
		attribute.opennum = (int)elem["opennum"];

		return attribute;
	}
}
