using UnityEngine;
using System.Collections;
using System.Xml.Serialization;  
using System.IO;  

public class UserInfoMgr  {

	private int _heroId;
	public int heroId{
		get{
			return _heroId;
		}
		set{
			_heroId = value;
			PlayerPrefs.SetInt("heroId" , _heroId);
		}
	}

	private int _heroLevel;
	public int heroLevel{
		set{
			_heroLevel = value;
			PlayerPrefs.SetInt("heroLevel" , heroLevel);
		}
		get{
			return _heroLevel;
		}
	}
	
	private int _gold;
	public int gold{
		set{
			_gold = value;
			PlayerPrefs.SetInt("gold" , _gold);
		}
		get{
			return _gold;
		}
	}
	
	private int _exp;
	public int exp{
		set{
			_exp = value;
			PlayerPrefs.SetInt("exp" , _exp);
		}
		get{
			return _exp;
		}
	}
	
	private int _money;
	public int money{
		set{
			_money = value;
			PlayerPrefs.SetInt("money" , _money);
		}
		get{
			return _money;
		}
	}


	private int _finishCopyId;
	public int finishCopyId{
		set{
			_finishCopyId = value;
			PlayerPrefs.SetInt("finishCopyId" , _finishCopyId);
		}
		get{
			return _finishCopyId;
		}
	}

	private static UserInfoMgr instance;

	public static UserInfoMgr GetInstance(){
		if(instance == null){
			instance = new UserInfoMgr();
		}

		return instance;
	}
	 

	private UserInfoMgr(){
		_heroId = PlayerPrefs.GetInt("heroId");
		_money = PlayerPrefs.GetInt("money");
		_exp = PlayerPrefs.GetInt("exp");
		_gold = PlayerPrefs.GetInt("gold");
		_heroId = PlayerPrefs.GetInt("heroId");
	}


	public void Save(){
		PlayerPrefs.Save();
	}


	
	public void SetCopyInfo(CopyInfo info){

		XmlSerializer serializer = new XmlSerializer( typeof(CopyInfo) );
		StringWriter sw = new StringWriter();

		serializer.Serialize(sw, info);  

		PlayerPrefs.SetString( "copy_" + info.id , sw.ToString() );
	}


	public CopyInfo GetCopyInfo(int copyId){

		XmlSerializer serializer = new XmlSerializer( typeof( CopyInfo ) );  

		string copyInfoString = PlayerPrefs.GetString( "copy_" + copyId) ;

		if(copyInfoString == null || copyInfoString == ""){
			return null;
		}

		StringReader sr = new StringReader( copyInfoString );  
		CopyInfo info = (CopyInfo)serializer.Deserialize( sr );
		
		return info;
	}
	

}












