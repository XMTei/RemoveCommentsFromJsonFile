using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;//for get server path
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RemoveCommentsFromJsonFile.Models;
using System.Text.RegularExpressions;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemoveCommentsFromJsonFile.Controllers
{
	//************************************************************************
	//Name: 	RCFJController
	//Author: 	Zheng XM (2018/2/19)
	//Modify:
	//Return:
	//Description: recieve data from client, and then return data to the client
	//				All the data types are define in Models/RCFJ.cs
	//			1.support web commands GET,POST,PUT,DELETE, but PUT and DELETE are not uead frequncly.
	//
	[Route("api/[controller]")]
	[Produces("application/json")]
	public class RCFJController : Controller
	{
		#region Vars
		private IHostingEnvironment m_oEnveroment;//for get server path
		#endregion Vars
		#region Constructors
		public RCFJController(IHostingEnvironment oEnveroment) //for get server path
		{
			this.m_oEnveroment = oEnveroment;
		}
		#endregion Constructors

		//for testing
		[HttpGet]
		public IEnumerable<string> Get()
		{
			return new string[] { "you", "got your server" };
		}

		//for testing	 
		[HttpGet("{id}")]
		public string Get(int id)
		{
			return $"you got your data by using an id({id})";
		}

		//************************************************************************
		//Name: 	GetUIStrings
		//Author: 	Zheng XM (201712/10)
		//Modify:
		//Modify:
		//Return:  	async Task<ActionResult>
		//Description: get UI strings for specified UI language
		//			1.follow those styeps in Client to call this WEB API
		//				var settings = {
		//					"async": false,
		//					"crossDomain": true,
		//					"url": '/api/RCFJ/GetUIStrings/',
		//					"method": "POST",
		//					"data": jsondata,
		//					"success": function(data, status, xhr)
		//					{
		//						//got your data
		//					},
		//					"error": function(jqXHR, textStatus, errorThrown)
		//					{
		//						//error
		//					},
		//				};
		//				$.ajax(settings);
		//
		[HttpPost("GetUIStrings")]
		public async Task<ActionResult> GetUIStrings([FromBody]GetUIStringsParam value)
		{
			ActionResult oRcd = new EmptyResult();
			if ((ModelState.IsValid) && (value != null))
			{//got right param from client
				oRcd = await Task.Run(() =>
				{//use async to make return data
					ActionResult oJson = new EmptyResult();
					HtmlLang oHtmlLang = new HtmlLang(value.Language);
					//string strFontFile = System.IO.Path.Combine(this.m_oEnveroment.WebRootPath, string.Format("App_Data\\{0}.json",value.Language));
					string strFontFile = System.IO.Path.Combine(this.m_oEnveroment.WebRootPath, @"App_Data/UIStrings.json");
					try
					{
						using (StreamReader file = System.IO.File.OpenText(strFontFile))
						{
							//string strData = string.Empty;
							JsonSerializer oSerializer = new JsonSerializer();
							//Dictionary<string,string> oUIDic = (Dictionary<string, string>)oSerializer.Deserialize(file, typeof(Dictionary<string, string>));
							//这样读取可以过滤掉文件中的comment部分
							dynamic oUIString = oSerializer.Deserialize(file, typeof(object));
							//需要从oUIDic中找到对应value.Language的语言,首先找是否存在完全一致的语言
							int nIdex = 0;
							for (int i = 0; i < oUIString.Count; i++)
							{
								string strTemp = oUIString[i].Lang.ToString();
								if (strTemp.Equals(oHtmlLang.UIStringLang))
								{
									nIdex = i;
									break;
								}
							}
							//string strTemp = JsonConvert.SerializeObject(oUIStringTemp);//再次转为strng
							//dynamic oUIString = JArray.Parse(strTemp);
							//oUIString是个数组，每个要素是一个语言所属的字符串。需要通过
							//if (oUIString.Lang == "en")
							//{
							//	strData = JsonConvert.SerializeObject(oUIStringTemp);
							//}
							string strData = JsonConvert.SerializeObject(oUIString[nIdex].Strings);
							ResponseParam oResponseParam = new ResponseParam
							{//返回所有可选择的字体
								Type = DataType.Json,
								Data = strData,
								CallFiFoGUID = -1
							};
							oJson = Json(oResponseParam);
						}
					}
					catch (Exception e)
					{//一般是没有找到ColorGroups.json文件等问题

					}
					return oJson;
				});
			}
			return oRcd;
		}

		//************************************************************************
		//Name: 	UploadJsonFile
		//Author: 	Zheng XM (2018/2/19)
		//Modify:
		//Modify:
		//Return:  	async Task<ActionResult>
		//Description: recieve upload JSON file and return a non-comment JSON file
		//			1.follow those steps in Client to call this WEB API
		//				var settings = {
		//					"async": true,
		//					"crossDomain": true,
		//					"url": '/api/RCFJ/Upload/',
		//					"method": "POST",
		//					"contentType": false,
		//					"processData": false,
		//					"data": jsondata,
		//					"success": function(data, status, xhr)
		//					{
		//						//got your data
		//					},
		//					"error": function(jqXHR, textStatus, errorThrown)
		//					{
		//						//error
		//					},
		//				};
		//				$.ajax(settings);
		//
		[HttpPost("UploadJsonFile")]
		public async Task<ActionResult> UploadJsonFile(ICollection<IFormFile> value)//want to get the data form ajax, but does not work
		{
			ActionResult oRcd = await Task.Run(() =>
			{//use async to make return data
				ActionResult oJson = new EmptyResult();
				//file directory to save all the uploaded file
				try
				{
					string strMessage = string.Empty;
					var oFiles = Request.Form.Files;//got uploaded files. 
					if ((oFiles.Count > 0) && (oFiles[0] != null))
					{
						var oFile = oFiles[0];
						string strUnuploaedFile = oFile.Name;
						string strExt = Path.GetExtension(oFile.Name);
						if ((!string.IsNullOrEmpty(strExt))&&
							(strExt.Equals(".json", StringComparison.OrdinalIgnoreCase)))
						{
							using (MemoryStream oMemoryStream = new MemoryStream())
							{
								oFile.CopyTo(oMemoryStream);
								oMemoryStream.Flush();
								string strJsonWithCommnent = string.Empty;
								using (StreamReader osReader = new StreamReader(oMemoryStream))
								{
									osReader.BaseStream.Seek(0, SeekOrigin.Begin);
									//for (; ; )
									//{
									//	string strLine = osReader.ReadLine();
									//	if (strLine == null)
									//	{//end of file
									//		break;
									//	}
									//	else
									//	{
									//		strLine = strLine.Trim();
									//		if ((strLine != string.Empty) && !Regex.IsMatch(strLine, @"^\s*;(.*)$"))
									//		{
									//			if (Regex.IsMatch(strLine, @"^(.*);(.*)$"))
									//				sb.AppendLine(line.Substring(0, line.IndexOf(';')).Trim());
									//			else
									//				sb.AppendLine(line);
									//		}
									//		strJsonWithCommnent += strTemp + System.Environment.NewLine;
									//	}
									//}
									strJsonWithCommnent = osReader.ReadToEnd();
									var re = @"(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|//.*|/\*(?s:.*?)\*/";
									ResponseParam oResponseParam = new ResponseParam
									{//返回所有可选择的字体
										Type = DataType.Json,
										Data = Regex.Replace(strJsonWithCommnent, re, "$1"),
										CallFiFoGUID = -1
									};
									oJson = Json(oResponseParam);
								}
							}
						}
						else
						{
							strMessage = @"The file type is not correct.";
						}
					}
					if (!string.IsNullOrEmpty(strMessage))
					{//if error occured during upload
						strMessage.Substring(0, strMessage.Length - 1);
						oJson = Json(strMessage);
					}
				}
				catch (Exception e)
				{//file operation error...
					oJson = Json(e.Message);
				}
				return oJson;
			});
			return oRcd;
		}
	}
}
