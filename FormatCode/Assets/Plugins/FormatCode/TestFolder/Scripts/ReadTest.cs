using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReadTest : MonoBehaviour
{
	private bool readFinish = false;
	void Start()
	{
		CSVHelper.Instance().ReadCSVFile("csv_test", (table) => {
			readFinish = true;
			// 可以遍历整张表
			foreach (CSVLine line in table)
			{
				foreach (KeyValuePair<string,string> item in line)
				{
					Debug.Log(string.Format("item key = {0} item value = {1}", item.Key, item.Value));
				}
			}
			//可以拿到表中任意一项数据
			Debug.Log(table["10011"]["id"]);
		});
	}

	void Update()
	{
		if (readFinish)
		{
			// 可以类似访问数据库一样访问配置表中的数据
			CSVLine line = CSVHelper.Instance().SelectFrom("csv_test").WhereIDEquals(10011);
			Debug.Log(line["name"]);
			readFinish = false;
		}
	}
}
