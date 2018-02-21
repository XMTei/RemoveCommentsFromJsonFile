//Remove comments from Json file you uploaded
//globla var
var uri = '/api/RCFJ';//web api URL
var oUIStringDic = null;//UI strings for current UI language
var biOS = false;
//var bCompositioning = false;	//is using IME
var strCurrentLanguage = 'en';//记录当前语言，初始状态为en

//************************************************************************
//Name: 	$(document).ready
//Author: 	TEI (2018/2/19)
//Modify: 	
//Return:  	
//Description: initailization
//
$(document).ready(function () {
	ShowHideOnProcessing(true);//show on processing
	Init();
	GetUIStrings();//get all the strings for UI in current UI language

	ResetAllUIString();//set UI string
	//handle IME composition
	//$('#LetteringText').on('compositionstart', function (ev) {
	//	bCompositioning = true;
	//});
	//$('#LetteringText').on('compositionend', function (ev) {
	//	bCompositioning = false;
	//});

	//stop show onprocessing
	ShowHideOnProcessing(false);
	//hide upload progress bar
	$("#UploadProgressbar").hide();
	//lesten upload file
	$('#UploadFiles').change(function (e) {
		UploadFiles(e);
	});
});

//************************************************************************
//Name: 	GetUIStrings
//Author: 	TEI (2018/2/19)
//Modify:
//Return:  	
//Description: get all the strings for UI in current UI language
//
function GetUIStrings() {
	$('#ShowErrorMessage').hide();//首先关闭所显示的Error
	var jsondata = getGetUIStringsParamJsonData();
	var settings = {
		"async": false,
		"crossDomain": true,
		"url": uri + "/GetUIStrings",//gifdataController.GetUIStrings() will be called
		"method": "POST",
		"headers": {
			"content-type": "application/json",//tell server that it will get json data
			"cache-control": "no-cache"
		},
		"data": jsondata,//use gifdataController.GetUIStrings([FromBody]GetUIStringsParam value) in gifdataController.cs
		"success": function (data, status, xhr) {//get contents from ResponseParam
			var contentType = xhr.getResponseHeader("content-type") || "";
			if (contentType.indexOf("json") >= 0) {//we need JSON data
				if ((data.type) && (data.type.indexOf('Json') >= 0) && //is Json data
					(data.data) && (data.data.length > 0)) {//has real data
					oUIStringDic = null;
					try {
						var oJsonObj = JSON.parse(data.data);
						if (oJsonObj) {
							oUIStringDic = oJsonObj;
						}
					} catch (e) {
						ShowErrorMessage('Data Error!', 'Bad UI strings JSON Data:' + e);
					}
				}
				else {
					ShowErrorMessage('Data Error!', 'Unsupported data type:' + data.type);
				}
			}
		},
		"error": function (jqXHR, textStatus, errorThrown) {//for http error
			//显示错误信息
			ShowErrorMessage('Server Error!', textStatus);
		},
	};

	$.ajax(settings);
}

//************************************************************************
//Name: 	UploadFiles
//Author: 	TEI (2018/2/19)
//Modify:
//Return:  	
//Description: upload selected files
//
function UploadFiles(e) {
	$('#ShowErrorMessage').hide();//首先关闭所显示的Error
	var files = e.currentTarget.files;
	var data = new FormData();
	for (var i = 0; i < files.length; i++) {
		data.append(files[i].name, files[i]);
	}
	var settings = {
		//async: false,
		//crossDomain: true,
		url: uri + "/Upload",//gifdataController.Upload() will be called
		method: "POST",
		headers: {//Use header to set the content type
			'content-type': "multipart/form-data"//tell server that it will get form data
			//"cache-control": "no-cache"
		},
		//contentType: false,//use headers.content-type to set the content type
		processData: false,
		cache: false,
		dataType: 'json',
		data: data,
		xhr: function () {
			var oUploadProgress = $("#UploadProgress");
			oUploadProgress.css("width", "0%");
			oUploadProgress.html("0%");
			var xhr = $.ajaxSettings.xhr();
			//var xhr = new window.XMLHttpRequest();//this is a another way to get xhr
			xhr.upload.addEventListener("progress", function (evt) {
				if (evt.lengthComputable) {
					var progress = Math.round((evt.loaded / evt.total) * 100);
					oUploadProgress.css("width", progress + "%");
					oUploadProgress.html(progress + "%");
				}
			}, false);
			return xhr;
		},
		success: function (data, status, xhr) {//get contents from ResponseParam
			//hide upload progress
			$("#UploadProgressbar").hide();
			//check completed message 
			//var contentType = xhr.getResponseHeader("content-type") || "";
			//if (contentType.indexOf("json") >= 0) {//we need JSON data
			//	if ((data) && (data != '')) {
			//		var strMsg = data;
			//		if (oUIStringDic) {
			//			strMsg = oUIStringDic.Constant.UnuploadedFiles + data;
			//		}
			//		alert(strMsg);
			//	}
			//}
		},
		error: function (jqXHR, textStatus, errorThrown) {
			//hide upload progress
			$("#UploadProgressbar").hide();
			//show error
			ShowErrorMessage('Server Error!', textStatus);
		},
	};

	//Show upload progress
	$("#UploadProgressbar").show();
	$.ajax(settings);
}

//************************************************************************
//Name: 	getGetUIStringsParamJsonData
//Author: 	TEI (2017/12/11)
//Modify:
//Return:  	JSON data(see GetUIStringsParam in gifdata.cs)
//Description: return a json data that is for get UI strings from server
//
function getGetUIStringsParamJsonData() {
	var strRcd = '';
	var oJsonData = {
		Language: strCurrentLanguage,
	};
	strRcd = JSON.stringify(oJsonData);
	return strRcd;
}

//************************************************************************
//Name: 	ResetUIString
//Author: 	TEI (2018/2/20)
//Modify:
//Return:  	
//Description: set UI strings. call this after GetUIStrings()
//
function ResetAllUIString() {
	if (oUIStringDic) {
		//find all the elements to be translateed（the element to be translated has a class .needTranslation
		$('.needTranslation').each(function (index) {//for each element to be translated
			ResetUIStringOf(this, '');
		});
	}
}

//************************************************************************
//Name: 	ResetAllUIString
//Author: 	Zheng (2018/2/20)
//Modify:
//Return:  	
//Description: set an element string to be translated 
//
function ResetUIStringOf(oElement,//an element
	strIdStartFrom) {//a special id, in most case this should be empty string, if it is not empty,use this as id
	if (oElement) {
		var strId = strIdStartFrom;//优先使用特殊ID
		if (strId == '') {//没有指定特殊ID的时候
			strId = oElement.id;//没有特殊ID的话，使用element的ID
		}
		else if (oElement.id.indexOf(strId) != 0) {
			return;//不是以此起始的element就不做修改
		}
		var oTemp = null;
		if (oUIStringDic) {
			oTemp = oUIStringDic[strId];//从字符串字典中取出对应当前element的object（包含所有的相关的字符串,详情见UIStrings.json中的说明）
		}
		if (oTemp) {
			var oJElement = $(oElement);
			$.each(oTemp, function (key, value) {//处理里面所有需要置换的字符串
				if (key == 'html') {//如果是html的话
					oJElement.html(value);//将其添加给此element的下属,既 <oElement> value </oElement>
				}
				else if (key == 'options') {//用于设置select下属的option
					//获取旗下的所有option
					oJElement.children("option").each(function (index) {
						var oJOption = $(this);
						var strHtml = value[oJOption.val()];//获取
						if (strHtml) {
							oJOption.html(strHtml);
						}
					});
				}
				else {
					oJElement.attr(key, value);//修改此element的attribute
				}
			});
		}
	}
}

//************************************************************************
//Name: 	Init
//Author: 	TEI (2018/2/19)
//Modify: 	
//Return:  	
//Description: initialize Elements...
//
function Init() {
	biOS = !!navigator.platform && /iPad|iPhone|iPod/.test(navigator.platform);
	bMSIE = !!navigator.userAgent.match(/Trident/g) || !!navigator.userAgent.match(/MSIE/g);

	//记录当前界面语言
	if (navigator.language) {
		strCurrentLanguage = navigator.language;
	}
	else if (navigator.browserLanguage) {
		strCurrentLanguage = navigator.browserLanguage;
	}
}

//************************************************************************
//Name: 	ShowErrorMessage
//Author: 	TEI (2018/2/19)
//Modify:
//Return:  	
//Description: show message (error, warning...)
//
function ShowErrorMessage(title, msg) {//标题，和具体内容；其一为空的时候表示要关闭此信息框
	var errorMessageBox = $('#ShowErrorMessage');
	if (errorMessageBox) {
		if ((title == "") && (msg == "")) {
			//没有给出显示内容时，认为是关闭显示
			//errorMessageBox.style.display = "none";
			errorMessageBox.hide();
		} else {
			//errorMessageBox.empty();//清空当前显示内容
			//从后面去掉两个(default.html中必须有两个element，<h3></h3>和<p></p>)
			var messageHeader = errorMessageBox.children('h3')
			if (messageHeader) {
				messageHeader.html(title);
			}
			var messageParagraph = errorMessageBox.children('p')
			if (messageParagraph) {
				messageParagraph.html(msg);
			}
			//errorMessageBox.html('<h3>' + title + '</h3>' + '<p>' + msg + '</p>'); //添加好显示内容
			//errorMessageBox.style.display = "block";//显示
			errorMessageBox.show();//显示
		}
	}
}

//************************************************************************
//Name: 	ShowHideOnProcessing
//Author: 	TEI (2018/2/19)
//Modify: 	
//Return:  	
//Description: show / hide an onprocessing animation
//
function ShowHideOnProcessing(bShow) {	//true:show
	if (bShow) {
		$("#OnProcessing").show();
	} else {
		$("#OnProcessing").hide();
	}
}

//************************************************************************
//Name: 	SaveAs
//Author: 	Zheng XM (2017/5/3)
//Modify: 	
//Return:  	
//Description: 保存数据，目前测试了
//				IE				OK
//				Edge				OK
//				Chrome(Win)		OK
//				Chrome(MacOS)	OK
//				Chrome(iOS)		NG,去掉保存按钮
//				Safari(MacOS)	OK？？
//				Safari(iOS)		NG,去掉保存按钮
//
function SaveAs(oBtn) {//strType:'plain/text','plain/text','image/png'...
	if (oBtn) {
		var oImageSrc = $('#Imagedata').attr('src');//获取Preview中的数据，此数据必须是 "data:image/png;base64," + base64data 或"data:image/svg+xml;base64," + base64data
		if (oImageSrc) {
			var nComa = oImageSrc.indexOf(',');
			if (nComa > 0) {
				var oData = atob(oImageSrc.slice(nComa + 1));//由Base64转为原文
				var strFileName = 'ITLLettering';
				//var strType = 'plain/text';	//Safari不工作
				var strType = 'attachment/text;charset=utf-8';//Safari不工作
				if (oImageSrc.indexOf('png') > 0) {
					strType = 'image/png';//目前保存不了PNG，有待处理
					strFileName += '.png'
				} else {
					strFileName += '.svg'
				}
				if (oData) {
					var aData = [oData];
					aData.push()
					properties = { type: strType }; // Specify the file's mime-type.
					file = new Blob(aData, properties);
					//以下是调用FileSaver.js
					saveAs(file, strFileName);

					//以下是自己做的，其能力如下
					//				IE				OK
					//				Edge				OK
					//				Chrome(Win)		OK
					//				Chrome(MacOS)	OK
					//				Chrome(iOS)		NG,本来就不能保存，所以此时要把保存按钮去掉
					//				Safari(MacOS)	NG
					//				Safari(iOS)		NG,本来就不能保存，所以此时要把保存按钮去掉
					//if (navigator.appVersion.toString().indexOf('.NET') > 0) {//IE的时候，需要特殊对待了
					//	window.navigator.msSaveBlob(file, strFileName);
					//} else {
					//	//创建一个<a>
					//	var downloadLink = document.createElement("a");
					//	downloadLink.download = strFileName;//指定下载的文件名
					//	downloadLink.innerHTML = "Download File";//内部为DownloadFile
					//	if (window.webkitURL != null) {
					//		// Chrome allows the link to be clicked
					//		// without actually adding it to the DOM.
					//		downloadLink.href = window.webkitURL.createObjectURL(file);
					//	}
					//	else {//此部分未经测试，不知是否工作
					//		// Firefox requires the link to be added to the DOM
					//		// before it can be clicked.
					//		downloadLink.href = window.URL.createObjectURL(file);
					//		downloadLink.onclick = function () {
					//			document.body.removeChild(event.target);
					//		};
					//		downloadLink.style.display = "none";
					//		document.body.appendChild(downloadLink);
					//	}

					//	downloadLink.click();//执行下载
					//}
				}
			}
		}
	}
}
