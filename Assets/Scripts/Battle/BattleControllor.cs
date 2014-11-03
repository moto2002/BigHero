using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class BattleControllor {

	public static float countdownTime = 0;

	public static int groundIndex = 0;
	public static int copyId = 1001;
	public static CopyConfig copyConfig;
	public static int heroID = 1001;
	public static int heroLevel = 1;


	public static Battle battle;

	public static Hero hero;
	
	public static int v;
	public static int h;
	
	public static ArrayList followers = new ArrayList();
	public static ArrayList monsters = new ArrayList();
	public static ArrayList items = new ArrayList();
	
	public static Dictionary<Charactor , ArrayList> objectPositionTable = new Dictionary<Charactor , ArrayList>();
	public static Dictionary<Vector2 , ArrayList> positionObjectTable = new Dictionary<Vector2 , ArrayList>();
	
	public static JsonData groundMapConfig;
	
	public static ArrayList prepareMonsters;
	public static ArrayList prepareFollowers;

	public static GameObject follower_pre = (GameObject)Resources.Load("Prefabs/Follower");
	
	public static GameObject monster_pre = (GameObject)Resources.Load("Prefabs/Monster");
	
	public static GameObject hero_pre = (GameObject)Resources.Load("Prefabs/Hero");
	
	public static GameObject item_pre = (GameObject)Resources.Load("Prefabs/Item");

	
	public static float timeLine = 0f;

	public static BattleState state = BattleState.STATE_LOADING;
	
	public static void Update () {

		switch(state){
		case BattleState.STATE_LOADING:
			state = BattleState.STATE_BUILDING;
			break;
			
		case BattleState.STATE_BUILDING:
			battle.SetExitPoint(false);
			battle.SetEnterPoint(true);
			Clear();
			Build();
			break;

		case BattleState.STATE_START_ANIMATION:
			int groundId = (int)copyConfig.maps[groundIndex];
			JsonData groundConfig = Config.GetInstance().GetGroundConfig(groundId);

			if(hero == null){
				AddHero(groundConfig);
			}else{
				hero.SetStartPosition((int)groundConfig["start_x"] , (int)groundConfig["start_y"]);
			}

			hero.currentDirection = MoveDirection.UP;

			Follower f = hero.follower;

			while(f != null){
				f.ResetPostion();
				f.SetDirection(hero.currentDirection);
				f.SetPosition(hero.transform.localPosition);
				f.gameObject.SetActive(true);

				f.SetNextPosition(hero.transform.localPosition);

				f = f.follower;
			}

			hero.audio.clip = null;
			hero.gameObject.SetActive(true);
			hero.StopAnimation();
			hero.StopMoving();
			
			hero.Invoke("PlayMoving" , 2);
			hero.Invoke("PlayAnimation" , 2);
			battle.Invoke("HideEnterPoint" , 1);

			state = BattleState.STATE_BATTLING;
			break;
		case BattleState.STATE_BATTLING:
			if((int)timeLine < (int)(timeLine + Time.deltaTime)){
				CheckTimePoint();
			}
			
			timeLine += Time.deltaTime;
			SkillManager.Update();
			break;
		case BattleState.STATE_WIN:
			groundIndex++;
			
			if(groundIndex >= copyConfig.maps.Count){
				hero.StopMoving();
				EventDispather.DispatchEvent(new GameEvent(EventName.UI_SHOW_SINGLE ,"WinUI"));
			}else{
				state = BattleState.STATE_TO_NEXT;
				battle.SetExitPoint(true);
			}

			break;
		case BattleState.STATE_END_ANIMATION:
			state = BattleState.STATE_BUILDING;
			break;
		case BattleState.STATE_TO_NEXT:
			HeroAutoMove();
			break;
		case BattleState.STATE_LOSE:
			break;
		}

	}


	public static void Clear(){
		
		timeLine = 0;
		SkillManager.Clean();

		for(int i = 0 ; i < followers.Count ; i++){
			Follower follower = (Follower)followers[i];
			if(follower != null && follower.prev == null){
				MonoBehaviour.Destroy(follower.gameObject);
			}
		}
		followers = new ArrayList();
		
		
		for(int i = 0 ; i < monsters.Count ; i++){
			Monster monster = (Monster)monsters[i];
			if(monster != null){
				MonoBehaviour.Destroy(monster.gameObject);
			}
		}
		monsters = new ArrayList();
	}

	public static void Build(){
		Constance.RUNNING = true;
		state = BattleState.STATE_START_ANIMATION;
		//battle.ground.Clear();

		objectPositionTable = new Dictionary<Charactor , ArrayList>();
		positionObjectTable = new Dictionary<Vector2 , ArrayList>();


		//battle.ground.Clear();

		copyConfig = Config.GetInstance().GetCopyConfig(copyId);

		int groundId = (int)copyConfig.maps[groundIndex];
		JsonData groundConfig = Config.GetInstance().GetGroundConfig(groundId);

		groundMapConfig = groundConfig["map"];
		battle.ground.drawGround(groundMapConfig);
		
		h = battle.ground.h;
		v = battle.ground.v;

		countdownTime = (int)groundConfig["limitTime"];

		
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

	public static void UpdatePosition(Charactor charactor , Vector2 position , int volume = 1){
		
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


	public static ArrayList GetPath(Vector2 startPoint , Vector2 endPoint){

		ArrayList list = new ArrayList();

		if(startPoint.x != endPoint.x && startPoint.y != endPoint.y){
			Vector2 midPoint = new Vector2(endPoint.x , startPoint.y);

			list.Add(midPoint);
		}

		list.Add(endPoint);

		return list;
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
		
		ArrayList list = null;
		for(int i = 0 ; i < followers.Count ; i++){
			if(p == ((Follower)followers[i]).GetPoint()){

				if(list == null)list = new ArrayList();
				list.Add(followers[i]);
			}
		}
		
		return list;
	}


	public static ArrayList GetItemByPoint(Vector2 p){

		ArrayList list = new ArrayList();

		for(int i = 0 ; i < items.Count ; i++){
			if(p == ((Item)items[i]).GetPoint()){
				if(list == null)list = new ArrayList();

				list.Add((Item)items[i]);
			}
		}

		return list;
	}


	public static void RemoveItem(Item item){
		items.Remove(item);

		MonoBehaviour.Destroy(item.gameObject);
	}

	
	
	public static Hero GetHeroByPoint(Vector2 p){
		
		if(hero.GetPoint() == p){
			return hero;
		}
		
		return null;
	}
	
	
	public static bool Crit(int crit){
		
		if(crit > Random.Range(0 , 10000)){
			return true;
		}
		
		return false;
	}
	
	public static int Attack(Attribute active , Attribute unactive , float a = 1, int b = 0 , bool crit = false){
		
		float att = (a * active.atk  + b) + (a * active.atk  + b) * (Random.Range(0,10) - 5)/100;
		
		
		if(crit == true){
			att = att * 1.5f;
		}
		
		return (int)att;
	}



	
	public static void AddHero(JsonData groundConfig){
		hero = ((GameObject)MonoBehaviour.Instantiate(hero_pre)).GetComponent<Hero>();
		hero.transform.parent = battle.transform;
		
		hero.direction = hero.currentDirection = (MoveDirection)System.Enum.Parse(typeof(MoveDirection) , groundConfig["start_direction"].ToString());
		hero.SetStartPosition((int)groundConfig["start_x"] , (int)groundConfig["start_y"]);
		
		Attribute attribute = Config.GetInstance().GetCharacterConfig(heroID , heroLevel);
		
		hero.attribute = attribute;
		//hero.SetChararerID(attribute.mod);
		hero.SetChararerID(3002);
	}
	
	public static void AddFollower(JsonData followerConfig){
		
		GameObject followerObject = (GameObject)MonoBehaviour.Instantiate(follower_pre);
		
		Follower follower = followerObject.GetComponent<Follower>();
		
		follower.transform.parent = battle.transform;
		
		follower.SetPosition (BattleUtils.GridToPosition((int)followerConfig["x"] , (int)followerConfig["y"]));
		follower.SetSkills(followerConfig["skills"]);
		follower.SetSkillPolicy((int)followerConfig["policy"]);
		follower.SetDirection(MoveDirection.DOWN);
		follower.StopAnimation();
		
		
		int charID = (int)followerConfig["id"];
		Attribute attribute = Config.GetInstance().GetCharacterConfig(charID , (int)followerConfig["level"]);
		
		follower.attribute = attribute;
		follower.SetCharaterID(attribute.mod);
		
		followers.Add(follower);
	}
	
	
	public static void AddMonster(JsonData monsterConfig){
		GameObject monsterObject = (GameObject)MonoBehaviour.Instantiate(monster_pre);
		
		Monster monster = monsterObject.GetComponent<Monster>();
		
		monster.transform.parent = battle.transform;
		monster.SetPosition (BattleUtils.GridToPosition((int)monsterConfig["x"] , (int)monsterConfig["y"]));
		monster.SetMovePath(monsterConfig["path"]);
		monster.SetSpeed(float.Parse(monsterConfig["speed"]+ ""));
		monster.SetSkills(monsterConfig["skills"]);
		monster.SetSkillPolicy((int)monsterConfig["policy"]);
		monster.dropOdds = (int)monsterConfig["dropOdds"];
		monster.dropItemId = (int)monsterConfig["drop"];
		
		int charID = int.Parse(monsterConfig["id"] + "");
		Attribute attribute = Config.GetInstance().GetCharacterConfig(charID , (int)monsterConfig["level"]);

		monster.setCharaterID(attribute.mod);
		monster.attribute = attribute;
		
		monsters.Add(monster);
	}


	public static void AddItem(int itemId , Vector2 position){

		GameObject itemObject = (GameObject)MonoBehaviour.Instantiate(item_pre);

		ItemConfig itemConfig = Config.GetInstance().GetItemConfig(itemId);


		Item item = itemObject.GetComponent<Item>();
		item.itemConfig = itemConfig;

		item.transform.localPosition = position;
		item.SetItemId(itemId);

		items.Add(item);
	}
	
	public static void RemoveMonster(Monster monster){
		
		RemovePosition(monster);
		monsters.Remove(monster);

		if(monsters.Count == 0 && prepareMonsters.Count == 0){
			state = BattleState.STATE_WIN;
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

	private static void CheckTimePoint(){
		
		for(int i = 0 ; i < prepareFollowers.Count ; i++){
			JsonData followerConfig = (JsonData)prepareFollowers[i];
			
			if((int)followerConfig["startTime"] <= timeLine){
				AddFollower(followerConfig);
				prepareFollowers.Remove(followerConfig);
				i--;
			}
			
		}
		
		for(int i = 0 ; i < prepareMonsters.Count ; i++){
			JsonData monsterConfig = (JsonData)prepareMonsters[i];
			
			if((int)monsterConfig["startTime"] <= timeLine){
				AddMonster(monsterConfig);
				prepareMonsters.Remove(monsterConfig);
				i--;
			}
		}
	}



	private static Transform _heroTransform;

	private static Vector2 autoMoveEndPoint = new Vector2(4 * Constance.GRID_GAP , -2 * Constance.GRID_GAP);

	private static void HeroAutoMove(){

		float moveDistance = Time.deltaTime * 0.8f;

		if(_heroTransform == null)_heroTransform = hero.transform;

		if(autoMoveEndPoint.x != _heroTransform.localPosition.x || autoMoveEndPoint.y != _heroTransform.localPosition.y){

			if(autoMoveEndPoint.x != _heroTransform.localPosition.x){
				
				if(autoMoveEndPoint.x > _heroTransform.localPosition.x){
					hero.direction = MoveDirection.RIGHT;
				}else{
					hero.direction = MoveDirection.LEFT;
				}
				
				if(Mathf.Abs(autoMoveEndPoint.x - _heroTransform.localPosition.x) < moveDistance){
					moveDistance = Mathf.Abs(autoMoveEndPoint.x - _heroTransform.localPosition.x);
				}
			}else if(autoMoveEndPoint.y != _heroTransform.localPosition.y){
				if(autoMoveEndPoint.y > _heroTransform.localPosition.y){
					hero.direction = MoveDirection.UP;
				}else{
					hero.direction = MoveDirection.DOWN;
				}
				
				if(Mathf.Abs(autoMoveEndPoint.y - _heroTransform.localPosition.y) < moveDistance){
					moveDistance = Mathf.Abs(autoMoveEndPoint.y - _heroTransform.localPosition.y);
				}
			}
			
			if(hero.currentDirection != hero.direction){
				hero.currentDirection = hero.direction;
				if(hero.follower != null)hero.follower.SetNextPosition(_heroTransform.localPosition);
			}
			
			hero.Move(moveDistance);
			if(hero.follower != null)hero.follower.Move(moveDistance);

		}else if(hero.gameObject.activeSelf == true){
			hero.gameObject.SetActive(false);
			battle.PlayExitEffect();

			if(hero.follower != null){
				hero.follower.SetNextPosition(_heroTransform.localPosition);
			}else{
				BattleControllor.state = BattleState.STATE_END_ANIMATION;
			}
				
		}else{

			Follower f = hero.follower;

			while(f != null){
				if(f.gameObject.activeSelf == true){
					f.Move(moveDistance);
					
					if(f.HasNextPoint() == false){
						f.gameObject.SetActive(false);
						battle.PlayExitEffect();
					}
				}

				if(f.follower == null && f.gameObject.activeSelf == false){
					BattleControllor.state = BattleState.STATE_END_ANIMATION;
					return;
				}

				f = f.follower;
			}

		}
	}
}
