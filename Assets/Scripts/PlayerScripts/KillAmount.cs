using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAmount : MonoBehaviour
{
    public static KillAmount instance;

	public int amount;

	void Awake()
	{
		if (instance)
		{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		instance = this;
	}
}
