using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RemoveCommentsFromJsonFile
{
	#region JsonElement的扩充
	//************************************************************************
	//Name: 	Extensions
	//Author: 	Zheng XM (2019/11/08)
	//Modify: 	
	//Return:  	
	//Description: 各种class的扩充
	//			
	static class Extensions
	{
		//************************************************************************
		//Name: 	ToDynamic
		//Author: 	Zheng XM (2019/1/24)
		//Modify: 	
		//Return:  	JsonElement的dynamic数据
		//Description: 将指定path利用指定函变形，变形后的Path全部为Line，即没有曲线了
		//			
		public static dynamic ToDynamic(this JsonElement oElement)
		{
			dynamic oRcd = null;
			switch (oElement.ValueKind)
			{
				case JsonValueKind.Array:
					var oList = new List<ExpandoObject>();
					foreach (JsonElement oArrayElement in oElement.EnumerateArray())
					{
						oList.Add(oArrayElement.ToDynamic());
					}
					oRcd = (dynamic)oList;
					break;
				case JsonValueKind.False:
				case JsonValueKind.True:
					oRcd = (dynamic)oElement.GetBoolean();
					break;
				case JsonValueKind.String:
					oRcd = (dynamic)oElement.GetString();
					break;
				case JsonValueKind.Number:
					oRcd = (dynamic)oElement.GetSingle();
					break;
				case JsonValueKind.Object:
					oRcd = new ExpandoObject();
					foreach (JsonProperty oProperty in oElement.EnumerateObject())
					{
						var expandoDict = oRcd as IDictionary<string, object>;
						expandoDict.Add(oProperty.Name, oProperty.Value.ToDynamic());
					}
					break;
				case JsonValueKind.Undefined:
				default:
					break;

			}
			return oRcd;
		}

		//public static string GetJsonElement(this JsonElement oValue, int nIndex, string strName)
		//{
		//	JsonElement strRcd = string.Empty;

		//	Type oType = oValue.GetType();
		//	if (oType.IsArray ||    //object is []
		//		((oValue is IList) && (oType.IsGenericType)))    //object is IList
		//	{
		//	}

		//	return strRcd;
		//}

		//public static string ToJson(this object oValue)
		//{
		//	string strRcd = string.Empty;

		//	Type oType = oValue.GetType();
		//	if (oType.IsArray ||    //object is []
		//		((oValue is IList) && (oType.IsGenericType)))    //object is IList
		//	{
		//	}

		//	return strRcd;
		//}
	}
	#endregion
}
