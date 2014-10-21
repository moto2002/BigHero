using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class Battle : MonoBehaviour {

	public Ground ground;

	public GameObject follower_pre;

	public GameObject monster_pre;

	public GameObject hero_pre;

	public static int v;

	public static int h;

	public static Hero hero;

	public static ArrayList followers = new ArrayList();
	public static ArrayList monsters = new ArrayList();

	private static Dictionary<Charactor , ArrayList> objectPositionTable = new Dictionary<Charactor , ArrayList>();
	private static Dictionary<Vector2 , ArrayList> positionObjectTable = new Dictionary<Vector2 , ArrayList>();

	private static JsonData groundMapConfig;

	private static float timeLine = 0f;

	private ArrayList prepareMonsters;
	private ArrayList prepareFollowers;

	public static float countdownTime = 0;

	private int heroID = 1001;

	public delegate void OnGameWin();

	public delegate void onGameOver();


	void Start () {
	}



	void Update () {
		if(Constance.RUNNING == false){
			return;
		}

		if((int)timeLine < (int)(timeLine + Time.deltaTime)){
			CheckTimePoint();
		}

		timeLine += Time.deltaTime;

//		if(timeLine >= countdownTime){
//			//time's up
//			Constance.RUNNING = false;
//		}

		SkillManager.Update();
	}

	public void Build(){

		Constance.RUNNING = false;

		if(hero != null)Destroy(hero.gameObject);

		ground.Clear();

		for(int i = 0 ; i < followers.Count ; i++){
			Follower follower = (Follower)followers[i];
			Destroy(follower.gameObject);
		}
		followers.RemoveRange(0 , followers.Count);

		
		for(int i = 0 ; i < monsters.Count ; i++){
			Monster monster = (Monster)monsters[i];
			Destroy(monster.gameObject);
		}

		monsters.RemoveRange(0 , monsters.Count);

		objectPositionTable = new Dictionary<Charactor , ArrayList>();
		positionObjectTable = new Dictionary<Vector2 , ArrayList>();

		JsonData groundConfig = Config.GetInstance().GetGroundConfig(1002);
		groundMapConfig = groundConfig["map"];
		ground.drawGround(groundMapConfig);

		Battle.h = ground.h;
		Battle.v = ground.v;
		Battle.countdownTime = (int)groundConfig["limitTime"];
		
		AddHero(groundConfig);

		
		prepareMonsters = new ArrayList();
		prepareFollowers = new ArrayList();
		
		JsonData monsterConfigs = groundConfig["monster"];
		
		for(int i = 0 ; i < monsterConfigs.Count ; i++){
			JsonData monsterConfig = monsterConfigs[i];
			prepareMonsters.Add(monsterConfig);
		}
		
		JsonData followerConfigs = groundConfig["follower"];
		
		for(int i = 0 ; i < followerConfigs.Count ; i++){
			JsonData followerConfig = followerConfigs[i];
			prepareFollowers.Add(followerConfig);
		}

	}


	private void CheckTimePoint(){

		for(int i = 0 ; i < prepareFollowers.Count ; i++){
			JsonData followerConfig = (JsonData)prepareFollowers[i];
			
			if((int)followerConfig["startTime"] <= timeLine){
				this.AddFollower(followerConfig);
				prepareFollowers.Remove(followerConfig);
				i--;
			}

		}

		for(int i = 0 ; i < prepareMonsters.Count ; i++){
			JsonData monsterConfig = (JsonData)prepareMonsters[i];
			
			if((int)monsterConfig["startTime"] <= timeLine){
				this.AddMonster(monsterConfig);
				prepareMonsters.Remove(monsterConfig);
				i--;
			}
		}
	}


	public void playSkill(){
		if(hero == null){
			return;
		}

		hero.playSkill(1);
	}

	public static void UpdatePosition(Charactor charactor , Vector2 position , int volume = 1){
		
		//	private static Dictionary<GameObject , ArrayList> objectPositionTable = new Dictionary<GameObject , ArrayList>();
		//	private static Dictionary<Vector2 , ArrayList> positionObjectTable = new Dictionary<Vector2 , ArrayList>();

		RemovePosition(charactor);

		ArrayList positions = new ArrayList();
			
		for(int i = 0 ; i < volume ; i++){
			for(int j = 0 ; j < volume ; j++){
					
				Vector2 v = new Vector2(position.x + i , position.y + j);
					
				positions.Add(v);
			}
		}

		objectPositionTable.Add(charactor , positions);


		for(int k = 0 ; k < positions.Count ; k ++){
			Vector2 v = (Vector2)positions[k];

			ArrayList objs = null;

			positionObjectTable.TryGetValue(v , out objs);

			if(objs == null){
				objs = new ArrayList();
				positionObjectTable.Add(v , objs);
			}

			objs.Add(charactor);
		}

	}


	public static void RemovePosition(Charactor  charactor){

		ArrayList lastPositions = null;
		
		objectPositionTable.TryGetValue(charactor , out lastPositions);

		if(lastPositions != null){

			for(int a = 0 ; a < lastPositions.Count ; a++){
				Vector2 v = (Vector2)lastPositions[a];
				
				positionObjectTable[v].Remove(charactor);
			}
		}
		
		if(objectPositionTable.ContainsKey(charactor) == true)objectPositionTable.Remove(charactor);
	}


	public static ArrayList GetGameObjectsByPosition(Vector2 p){
		ArrayList list;
		positionObjectTable.TryGetValue(p , out list);

		if(list == null){
			list = new ArrayList();

			positionObjectTable[p] = list;
		}

		return list;
	}
	

	public static bool HasGameObject(Vector2 p){
		if(positionObjectTable.ContainsKey(p) == false){
			return false;
		}

		if(positionObjectTable[p].Count == 0){
			return false;
		}

		return true;
	}

	public static bool IsMoveable(Vector2 v){
		int x = (int)v.x;
		int y = (int)v.y;

		if(groundMapConfig == null){
			return false;
		}
		
		if(x < 0 || y < 0){
			return false;
		}
		
		
		if(y >= groundMapConfig.Count){
			return false;
		}
		
		if(x >= groundMapConfig[0].Count){
			return false;
		}
		
		if((int)groundMapConfig[y][x] > 0){
			return true;
		}
		
		return false;
	}


	public static ArrayList GetMonstersByPoint(Vector2 p){

		ArrayList list = new ArrayList();

		ArrayList objs;

		positionObjectTable.TryGetValue(p, out objs);

		if(objs == null){
			return list;
		}

		for(int i = 0 ; i < objs.Count ; i++){

			Charactor obj = (Charactor)objs[i];

			if(obj.tag == "Monster"){
				list.Add(obj);
			}
		}

		return list;
	}

	
	public static ArrayList GetFollowersByPoint(Vector2 p){
		
		ArrayList list = new ArrayList();
		for(int i = 0 ; i < followers.Count ; i++){
			if(p == ((Follower)followers[i]).GetPoint()){
				list.Add(followers[i]);
			}
		}
		
		return list;
	}



	public static Hero GetHeroByPoint(Vector2 p){
		
		if(hero.GetPoint() == p){
			return hero;
		}
		
		return null;
	}

	public static int Attack(Attribute active , Attribute unactive){

		//float att = (active.atk * 0.1f) + (active.atk * 0.1f) * (Random.Range(0,10) - 5)/100;
		//return (int)att;
		
		return 1;
	}

	public void AddHero(JsonData groundConfig){
		hero = ((GameObject)Instantiate(hero_pre)).GetComponent<Hero>();
		hero.transform.parent = this.transform;
		
		hero.direction = hero.currentDirection = (MoveDirection)System.Enum.Parse(typeof(MoveDirection) , groundConfig["start_direction"].ToString());
		hero.SetStartPosition((int)groundConfig["start_x"] , (int)groundConfig["start_y"]);


		Attribute attribute = Config.GetInstance().GetCharacterConfig(heroID);
		
		hero.attribute = attribute;
		//hero.SetChararerID(attribute.mod);
		hero.SetChararerID(1);
	}

	public void AddFollower(JsonData followerConfig){

		GameObject followerObject = (GameObject)Instantiate(follower_pre);

		Follower follower = followerObject.GetComponent<Follower>();

		follower.transform.parent = this.transform;

		follower.SetPosition (BattleUtils.GridToPosition((int)followerConfig["x"] , (int)followerConfig["y"]));
		follower.SetSkills(followerConfig["skills"]);
		follower.SetSkillPolicy((int)followerConfig["policy"]);
		follower.SetDirection(MoveDirection.DOWN);
		follower.StopAnimation();


		int charID = (int)followerConfig["id"];
		Attribute attribute = Config.GetInstance().GetCharacterConfig(charID);
		attribute.level = ((int)followerConfig["level"]);

		follower.attribute = attribute;
		follower.SetCharaterID(attribute.mod);

		followers.Add(follower);
	}


	public void AddMonster(JsonData monsterConfig){
		GameObject monsterObject = (GameObject)Instantiate(monster_pre);

		Monster monster = monsterObject.GetComponent<Monster>();

		monster.transform.parent = this.transform;
		monster.SetPosition (BattleUtils.GridToPosition((int)monsterConfig["x"] , (int)monsterConfig["y"]));
		monster.SetMovePath(monsterConfig["path"]);
		monster.SetSpeed(float.Parse(monsterConfig["speed"]+ ""));
		monster.SetSkills(monsterConfig["skills"]);
		monster.SetSkillPolicy((int)monsterConfig["policy"]);

		int charID = int.Parse(monsterConfig["id"] + "");
		Attribute attribute = Config.GetInstance().GetCharacterConfig(charID);

		monster.setCharaterID(attribute.mod);
		monster.attribute = attribute;

		monsters.Add(monster);
	}

	public static void RemoveMonster(Monster monster){

		RemovePosition(monster);

		monsters.Remove(monster);

		if(monsters.Count == 0){
			hero.StopMoving();
			EventDispather.DispatchEvent(new GameEvent(EventName.UI_SHOW_SINGLE ,"WinUI"));
		}
	}

	public static void RemoveFollower(Follower follower){
		
		RemovePosition(follower);
		followers.Remove(follower);
	}

	public static void HeroDead(Hero hero){
		hero.StopMoving();

		EventDispather.DispatchEvent(new GameEvent(EventName.UI_SHOW_SINGLE ,"GameOverUI"));
	}

	public void AnimationPlay(){
		Constance.RUNNING = true;
	}

	public void AnimationStop(){
		Constance.RUNNING = false;
	}

	public void X1(){
		Time.timeScale = 1;
	}

	public void X2(){
		Time.timeScale = 2;
	}


}

















