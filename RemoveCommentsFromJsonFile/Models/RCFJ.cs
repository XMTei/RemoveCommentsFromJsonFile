using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace RemoveCommentsFromJsonFile.Models
{
	//************************************************************************
	//Name: 	DataType
	//Author: 	Zheng XM (2017/12/10)
	//Modify: 	
	//Return:  	
	//Description: data types
	//
	public enum DataType
	{
		Non = 0,
		PngBase64,
		JpgBase64,
		GifBase64,
		TiffBase64,
		SvgBase64,
		Text,
		Json,
	}
	
	//************************************************************************
		//Name: 	GetUIStringsParam
		//Author: 	Zheng XM (2017/12/10)
		//Modify: 	
		//Return:  	
		//Description: use this param to get UI strings
		//
	public class GetUIStringsParam
	{
		//language
		public string Language { get; set; } = string.Empty;
	}
	//************************************************************************
	//Name: 	ResponseParam
	//Author: 	Zheng XM (2017/12/11)
	//Modify: 	
	//Return:  	
	//Description: data type to return client
	//
	public class ResponseParam
	{
		#region inner class
		#endregion inner class

		#region Constructor
		//public ResponseParam()
		//{
		//}
		//public ResponseParam(DataType eType,string strData, int nCallFiFoGUID)
		//{
		//	Type = eType;
		//	Data = string.IsNullOrEmpty(strData) ? string.Empty:strData ;
		//	CallFiFoGUID = nCallFiFoGUID;
		//}
		#endregion Constructor


		//call sequenc ID, copy from XXXXXXXXParam, when async
		public int CallFiFoGUID { get; set; } = -1;
		//data type for this.Data
		[JsonConverter(typeof(StringEnumConverter))]
		public DataType Type { get; set; } = DataType.Non;
		//data
		public string Data { get; set; } = string.Empty;
	}
	//************************************************************************
	//Name: 	HtmlLang
	//Author: 	Zheng XM (2018/2/7)
	//Modify:
	//Return:
	//Description:Html的语言和国家
	//			关于html语言的定义参照（https://www.w3schools.com/tags/ref_language_codes.asp），
	//			关于htmlCountryCode见(https://www.w3schools.com/tags/ref_language_codes.asp).
	//
	public class HtmlLang
	{
		public HtmlLang(string strHtmlLang)
		{
			string[] astrLangAndCountry = strHtmlLang.Split('-');
			if (astrLangAndCountry.Length > 0)
			{
				if (astrLangAndCountry.Length > 2)
				{//处理啊比如说 zh-Hans等
					this.LangFirst = astrLangAndCountry[0];
					this.LangSecond = astrLangAndCountry[1];
					this.Country = astrLangAndCountry[2];
				}
				else
				{
					this.LangFirst = astrLangAndCountry[0];
					if (astrLangAndCountry.Length > 1)
					{
						this.Country = astrLangAndCountry[1];
					}
				}
			}
		}

		#region Properties
		//与UIString中最相近的Lang
		public string UIStringLang
		{
			get
			{
				string strRcd = "en-US";
				if (this.LangFirst.Equals("zh", StringComparison.OrdinalIgnoreCase))
				{//中文的时候,目前只有中文简体，等有了zh-Hant后再添加代码
				 //if (string.IsNullOrEmpty(this.Country))
				 //{//没有country的时候
					strRcd = "zh-Hans";//认为是简体中文
									   //}
									   //else
									   //{
									   //	if(this.Country.Equals("", StringComparison.OrdinalIgnoreCase))
									   //	{

					//	}
					//}
				}
				else if (this.LangFirst.Equals("ja", StringComparison.OrdinalIgnoreCase))
				{
					strRcd = "ja-JP";
				}
				else if (this.LangFirst.Equals("en", StringComparison.OrdinalIgnoreCase))
				{
					strRcd = "en-US";//目前没有对应其他国家
				}
				return strRcd;
			}
		}

		//语言+国家
		public string LangAndCounty
		{
			get
			{
				string strRcd = string.Empty;
				if (string.IsNullOrEmpty(this.Country))
				{
					strRcd = this.Lang;
				}
				else
				{
					strRcd = $"{this.Lang}-{this.Country}";
				}
				return strRcd;
			}
		}
		//语言(例如：en-Hans等)。参照（https://www.w3schools.com/tags/ref_language_codes.asp）
		public string Lang
		{
			get
			{
				string strRcd = string.Empty;
				if (string.IsNullOrEmpty(this.LangSecond))
				{
					strRcd = this.LangFirst;
				}
				else
				{
					strRcd = $"{this.LangFirst}-{this.LangSecond}";
				}
				return strRcd;
			}
		}
		//语言（例如：en等）
		public string LangFirst
		{
			get;
			set;
		} = "en";
		//sub语言(如果有)。例如：zh-Hans中的Hans
		public string LangSecond
		{
			get;
			set;
		} = string.Empty;
		//国家或地区(如果有)。例如：zh-Hans-CN中的CN,见(https://www.w3schools.com/tags/ref_language_codes.asp)
		public string Country
		{
			get;
			set;
		} = string.Empty;
		#endregion Properties
	}
}
