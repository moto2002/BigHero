using UnityEngine;
using System.Collections;

public interface Skill {

	void Start ();

	void Update ();

	bool IsEnd();

	void SetSpec(bool b);

	void Clean();
}
