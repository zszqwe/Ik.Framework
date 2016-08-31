/*Trim String*/
String.prototype.trim = function(text) {
	if(typeof(text) != "string") text = /(^\s*)|(\s*$)/g;
	else text = eval("/(^(" + text + ")*)|((" + text + ")*$)/g");
	return this.replace(text, "");
};

/*HTML Encode*/
var htmlEncode = function(html, encodeBlank, encodeNewline) {
	if(!html) return html;
	html = html.replace(/&/g, "&amp;").replace(/\"/g, "&quot;")
		.replace(/'/g, "&apos;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
	if(encodeBlank) html = html.replace(/ /g, "&#160;");
	if(encodeNewline) html = html.replace(/\n/g, "<br/>");
	return html;
};

/*HTML Decode*/
var htmlDecode = function(html, decodeBlank, decodeNewline) {
	if(!html) return html;
	if(decodeNewline) html = html.replace(/<br\/>/g, "\n");
	if(decodeBlank) html = html.replace(/&#160;/g, " ");
	return html.replace(/&quot;/g, "\"").replace(/&apos;/g, "'")
		.replace(/&lt;/g, "<").replace(/&gt;/g, ">").replace(/&amp;/g, "&");
};

/*Init Wisco Date*/
var initWiscoDate = function(id) {
	var date = document.getElementById(id);
	if(!date) return;
	var input = date.children[0];
	input.readOnly = true;
	input.onclick = function() {
		WdatePicker();
	};	
	var link = date.children[1];
	link.onclick = function() {
		input.click();
	};
};

/*Init Wisco Time*/
var initWiscoTime = function(id) {
	var time = document.getElementById(id);
	if(!time) return;
	var input = time.children[0];
	input.readOnly = true;
	input.onclick = function() {
		WdatePicker({ dateFmt : "yyyy-MM-dd HH:mm:ss" });
	};
	var link = time.children[1];
	link.onclick = function() {
		input.click();
	};
};

/*Init Wisco More*/
var initWiscoMore = function(id) {
	var more = document.getElementById(id);
	if(!more) return;
	var input = more.children[0];
	input.readOnly = true;
};

/*Init Wisco File*/
var initWiscoFile = function(id) {
	var file = document.getElementById(id);
	if(!file) return;
	var e = file.children[1].children[0];
	var input = file.children[0];
	input.readOnly = true;
	e.onchange = function() {
		var array = e.value.split("\\");
		input.value = array[array.length - 1];
	};
};

/*Init Wisco Editor*/
var initWiscoEditor = function(id) {
	CKEDITOR.replace(id);
};

/*Init Wisco Album*/
var initWiscoAlbum = function(id) {
	var e = document.getElementById(id);
	var images = e ? e.children[1].children[0].children[0].children : [];
	if(images.length <= 4) return;
	var album = new WiscoAlbum(id);
	e.children[0].onmouseover = function() {
		album.startScrollLeft();
	};
	e.children[0].onmouseout = function() {
		album.stopScrollLeft();
	};
	e.children[2].onmouseover = function() {
		album.startScrollRight();
	};
	e.children[2].onmouseout = function() {
		album.stopScrollRight();
	};
	album.container = e.children[1];
	album.innerList1 = e.children[1].children[0].children[0];
    album.innerList2 = e.children[1].children[0].children[1];
    album.innerList2.innerHTML = album.innerList1.innerHTML;
    album.container.scrollLeft = album.correctValue;
    album.scrollInterval = 50;
    album.scrollLength = 10;
    album.scaleLength = 140;
};

/*Init Wisco Grid*/
var initWiscoGrid = function(id) {
	var grid = document.getElementById(id);
	if(!grid) return;
	var table = grid.children[0];
	var row = table.rows[0];
	var input = row.children[0].children[0];
	if(input && input.checked !== undefined) {
		var onclick = input.getAttribute("onclick");
		input.setAttribute("handle", onclick);
		input.onclick = function() {
			_wiscoGridHeadCheckboxClick(this);
			var handle = this.getAttribute("handle");
			if(typeof(handle) == "function") handle.call(this);
			else if(typeof(handle) == "string" && handle != "")
				new Function(handle).call(this);
		}
	}
	var length = table.rows.length;
	for(var i = 1; i < length; i++) {
		row = table.rows[i];
		row.onmouseover = function() {
			_wiscoGridRowOver(this);
		};
		row.onmouseout = function() {
			_wiscoGridRowOut(this);
		};
		input = row.children[0].children[0];
		if(input && input.checked !== undefined) {
			var onclick = input.getAttribute("onclick");
			input.setAttribute("handle", onclick);
			input.onclick = function() {
				_wiscoGridRowCheckboxClick(this);
				var handle = this.getAttribute("handle");
				if(typeof(handle) == "function") handle.call(this);
				else if(typeof(handle) == "string" && handle != "")
					new Function(handle).call(this);
			}
		}
	}
};
var _wiscoGridRowOver = function(e) {
	e.className = "highlight";
};
var _wiscoGridRowOut = function(e) {
	e.className = "normal";
};
var _wiscoGridHeadCheckboxClick = function(e) {
	var table = e.parentNode.parentNode.parentNode;
	var length = table.rows.length;
	for(var i = 1; i < length; i++) {
		var input = table.rows[i].children[0].children[0];
		if(input.checked != e.checked) input.click();
	}
};
var _wiscoGridRowCheckboxClick = function(e) {
	var table = e.parentNode.parentNode.parentNode;
	var length = table.rows.length;
	for(var i = 1; i < length; i++) {
		var row = table.rows[i];
		var input = row.children[0].children[0];
		if(input.checked) {
			row.className = "selected";
			row.onmouseover = null;
			row.onmouseout = null;
		}
		else {
			row.className = "normal";
			row.onmouseover = function() {
				_wiscoGridRowOver(this);
			};
			row.onmouseout = function() {
				_wiscoGridRowOut(this);
			};
		}
	}
};

/*Get Wisco Dialog*/
var getWiscoDialog = function() {
	return window.top.WiscoLayerManager.getDialog();
};
/*Show Wisco Dialog*/
var showWiscoDialog = function(src, width, height, title, argu, onload, onunload) {
	window.top.WiscoLayerManager.showDialog(src, width, height, title, argu, onload, onunload);
};
/*Hide Wisco Dialog*/
var hideWiscoDialog = function() {
	var dialog = getWiscoDialog();
	if(dialog) dialog.unload();
};

/*Init Wisco Map*/
var initWiscoMap = function(id, name, url, lonlat, cacheEnabled, oninit) {
	var map = new WiscoMap(id);
	map.name = name;
	map.url = url;
	if(cacheEnabled === false) 
		map.cacheEnabled = false;
	map.init(lonlat, oninit);
	return map;
};
var initWiscoBMap = function(id, position, oninit) {
	var map = new WiscoBMap(id);
	map.init(position, oninit);
	return map;
};
var initWiscoLMap = function(id, url, x, y, yaw, oninit) {
	var map = new WiscoLMap(id);
	map.url = url;
	map.init(x, y, yaw, oninit);
	return map;
};

/*Init Wisco View*/
var initWiscoView = function(id) {
	var view = document.getElementById(id);
	if(!view) view = document;
	var url = document.URL;
	url = url.replace(window.location.search, "");	// Replace Query String
	url = url.replace(window.location.hash, "");	// Replace Hash String
	var manager = window.top.WiscoViewManager;
	if(window.top.name == url)
		manager.restoreViewStates(url, document);
	else manager.clearViewStates(window.top.name);
	var onunload = window.onunload;
	window.onunload = function() {
		if(typeof(onunload) == "function") onunload.call(this);
		else if(typeof(onunload) == "string" && onunload != "")
			new Function(onunload).call(this);
		manager.saveViewStates(url, view);
		window.top.name = url;
	};
};
/*Uninit Wisco View*/
var uninitWiscoView = function() {
	var manager = window.top.WiscoViewManager;
	manager.clearViewStates(window.top.name);
};