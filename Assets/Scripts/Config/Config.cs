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

	private JsonData copyConfig;

	private JsonData itemCong;

	public Config(){
		grandConfig = JsonMapper.ToObject(Loader.GetText("Config/GroundConfig"));
		characterConfig = JsonMapper.ToObject(Loader.GetText("Config/Character"));
		skillConfig = JsonMapper.ToObject(Loader.GetText("Config/Skill"));
		copyConfig = JsonMapper.ToObject(Loader.GetText("Config/Copy"));
		itemCong = JsonMapper.ToObject(Loader.GetText("Config/Item"));
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
				config.demageratio = ((int)skillJsonData["demageratio"]) / 10000f;
				config.cd = (int)skillJsonData["cd"];
				break;
			}
		}

		return config;
	}


	public CopyConfig GetCopyConfig(int copyId){

		CopyConfig config = new CopyConfig();

		for(int i = 0 ; i < copyConfig.Count ; i++){
			
			JsonData copyJsonData = copyConfig[i] as JsonData;
			
			if((int)copyJsonData["copyid"] == copyId){
				config.id = (int)copyJsonData["copyid"];
				config.name = (string)copyJsonData["copyname"];
				config.exp = (int)copyJsonData["exp"];
				config.gold = (int)copyJsonData["gold"];
				config.box = (int)copyJsonData["box"];
				
				config.maps.Add((int)copyJsonData["map1"]);
				config.maps.Add((int)copyJsonData["map2"]);
				config.maps.Add((int)copyJsonData["map3"]);
				config.maps.Add((int)copyJsonData["map4"]);
				config.maps.Add((int)copyJsonData["map5"]);
				break;
			}
		}
		
		return config;
	}


	public CopyConfig GetNextCopyConfig(int copyId){

		CopyConfig config = new CopyConfig();

		if(copyId == 0){
			JsonData copyJsonData = copyConfig[0] as JsonData;
			config.id = (int)copyJsonData["copyid"];
			config.name = (string)copyJsonData["copyname"];
			config.exp = (int)copyJsonData["exp"];
			config.gold = (int)copyJsonData["gold"];
			config.box = (int)copyJsonData["box"];
			
			config.maps.Add((int)copyJsonData["map1"]);
			config.maps.Add((int)copyJsonData["map2"]);
			config.maps.Add((int)copyJsonData["map3"]);
			config.maps.Add((int)copyJsonData["map4"]);
			config.maps.Add((int)copyJsonData["map5"]);
		}else{

			for(int i = 0 ; i < copyConfig.Count ; i++){
				
				JsonData copyJsonData = copyConfig[i] as JsonData;
				
				if((int)copyJsonData["copyid"] == copyId){
					
					copyJsonData = copyConfig[++i] as JsonData;
					
					if(copyJsonData == null){
						return null;
					}
					
					config.id = (int)copyJsonData["copyid"];
					config.name = (string)copyJsonData["copyname"];
					config.exp = (int)copyJsonData["exp"];
					config.gold = (int)copyJsonData["gold"];
					config.box = (int)copyJsonData["box"];
					
					config.maps.Add((int)copyJsonData["map1"]);
					config.maps.Add((int)copyJsonData["map2"]);
					config.maps.Add((int)copyJsonData["map3"]);
					config.maps.Add((int)copyJsonData["map4"]);
					config.maps.Add((int)copyJsonData["map5"]);
					break;
				}
			}

		}
		

		
		return config;
	}


	public CopyConfig GetCopyConfigByIndex(int index){
		
		CopyConfig config = null;

		for(int i = 0 ; i < copyConfig.Count ; i++){

			if(index == i){
				JsonData copyJsonData = copyConfig[i] as JsonData;
				
				if(copyJsonData == null){
					return null;
				}

				config = new CopyConfig();

				config.id = (int)copyJsonData["copyid"];
				config.name = (string)copyJsonData["copyname"];
				config.exp = (int)copyJsonData["exp"];
				config.gold = (int)copyJsonData["gold"];
				config.box = (int)copyJsonData["box"];
				
				config.maps.Add((int)copyJsonData["map1"]);
				config.maps.Add((int)copyJsonData["map2"]);
				config.maps.Add((int)copyJsonData["map3"]);
				config.maps.Add((int)copyJsonData["map4"]);
				config.maps.Add((int)copyJsonData["map5"]);
				break;
			}
		}

		return config;

	}


	public ItemConfig GetItemConfig(int itemId){
		ItemConfig config = new ItemConfig();

		for(int i = 0 ; i < itemCong.Count ; i++){
			
			JsonData itemJsonData = itemCong[i] as JsonData;
			
			if((int)itemJsonData["id"] == itemId){

				config.id = (int)itemJsonData["id"];
				config.name = (string)itemJsonData["name"];
				config.type = (int)itemJsonData["type"];
				config.quality = (int)itemJsonData["quality"];
				config.atk = (int)itemJsonData["atk"];
				config.crt = (int)itemJsonData["crt"];
				config.addhp = (int)itemJsonData["addhp"];
				config.addpow = (int)itemJsonData["addpow"];
				config.shielddm = (int)itemJsonData["shielddm"];
				config.needlv = (int)itemJsonData["needlv"];
				config.attribute = (int)itemJsonData["attribute"];
				config.sell = (int)itemJsonData["sell"];
				config.buydiamond = (int)itemJsonData["buydiamond"];
				config.buygold = (int)itemJsonData["buygold"];
				config.res = (int)itemJsonData["res"];
				config.desc = (string)itemJsonData["des"];

				break;
			}
		}

		return config;
	}

}
