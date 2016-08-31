/**************************
 * Wisco Layer
 *
 * @author Raiden
**************************/

/*WiscoMask*/
function WiscoMask(id) {
	
	/*This (Private)*/
	var _mask = this;
	
	/*HTML Element (Private)*/
	var _element1 = null;
	var _element2 = null;
	
	/*Attributes*/
	this.id = id;
	this.zIndex = 0;
	
	/*Build*/
	this.build = function() {
		_element1 = document.createElement("div");
		_element1.id = _mask.id + ":div";
		_element1.className = "wiscomask";
		_element2 = document.createElement("iframe");
		_element2.id = _mask.id + ":iframe";
		_element2.className = "wiscomask";
		_element2.frameBorder = "no";
		_element2.setAttribute("scrolling", "no");
	};
	
	/*Load*/
	this.load = function() {
		_element1.style.zIndex = _mask.zIndex;
		_element2.style.zIndex = _mask.zIndex - 1;
		document.body.appendChild(_element1);
		document.body.appendChild(_element2);
	};
	
	/*Unload*/
	this.unload = function() {
		document.body.removeChild(_element1);
		document.body.removeChild(_element2);
	};
};

/*Wisco Dialog*/
function WiscoDialog(id) {
	
	/*This (Private)*/
	var _dialog = this;
	
	/*HTML Element (Private)*/
	var _element = null;
	
	/*Attributes*/
	this.id = id;
	this.src = null;
	this.width = 0;
	this.height = 0;
	var _top = 0;
	var _left = 0;
	this.zIndex = 0;
	this.title = null;
	this.argu = null;
	this.onload = null;
	this.onunload = null;
	// Dialog Mask
	this.mask = null;
	// Load Status
	this.loaded = false;
	
	/*Build*/
	this.build = function() {
		_element = document.createElement("div");
		_element.id = _dialog.id;
		_element.className = "wiscodialog";
		var div1 = document.createElement("div");
		div1.className = "title";
		var span = document.createElement("span");
		div1.appendChild(span);
		// Build Close Button
		var link1 = document.createElement("a");
		link1.href = "javascript:;";
		link1.className = "close";
		link1.onclick = function() { 
			window.setTimeout(_dialog.unload, 100); 
		};
		div1.appendChild(link1);
		// Build Maximize Button
		var link2 = document.createElement("a");
		link2.href = "javascript:;";
		link2.className = "maximize";
		link2.onclick = _dialog.maximize;
		div1.appendChild(link2);
		// Build Restore Button
		var link3 = document.createElement("a");
		link3.href = "javascript:;";
		link3.className = "restore";
		link3.onclick = _dialog.restore;
		div1.appendChild(link3);
		// Build Minimize Button
		var link4 = document.createElement("a");
		link4.href = "javascript:;";
		link4.className = "minimize";
		link4.onclick = _dialog.minimize;
		div1.appendChild(link4);
		_element.appendChild(div1);
		var div2 = document.createElement("div");
		div2.className = "info";
		var iframe = document.createElement("iframe");
		iframe.frameBorder = "no";
		iframe.setAttribute("scrolling", "auto");
		div2.appendChild(iframe);
		_element.appendChild(div2);
		// Build Dialog Mask
		_dialog.mask = new WiscoMask();
		_dialog.mask.build();
	};
	
	/*Load*/
	this.load = function() {
		// Set Position & Size
		_top = (document.body.clientHeight - _dialog.height - 30) / 2;
		_left = (document.body.clientWidth - _dialog.width - 10) / 2;
		_element.style.top = _top;
		_element.style.left = _left;
		_element.style.zIndex = _dialog.zIndex;
		var div1 = _element.children[0];
		div1.style.width = _dialog.width + 10;
		var span = div1.children[0];
		span.innerHTML = _dialog.title;
		var div2 = _element.children[1];
		div2.style.display = "block";
		var iframe = div2.children[0];
		iframe.style.width = _dialog.width;
		iframe.style.height = _dialog.height;
		iframe.src = _dialog.src;
		// Set Title Event
		_setTitleEvent(true);
		// Set Button Invisible
		_setButtonInvisible(3);
		// Load Dialog Mask
		_dialog.mask.zIndex = _dialog.zIndex - 1;
		_dialog.mask.load();
		// Load Dialog
		document.body.appendChild(_element);
		_element.children[0].children[1].focus();
		// Set Load Status
		_dialog.loaded = true;
		// Respond Onload Event
		if(typeof(_dialog.onload) == "function") {
			try { _dialog.onload(_dialog.argu); } catch(e) {}
		}
	};
	
	/*Unload*/
	this.unload = function() {
		// Respond Onunload Event
		if(typeof(_dialog.onunload) == "function") {
			try { if(_dialog.onunload(_dialog.argu) === false) return; } catch(e) {}
		}
		// Unload Dialog
		_element.children[1].children[0].src = "";
		document.body.removeChild(_element);
		// Unload Dialog Mask
		_dialog.mask.unload();
		// Set Load Status
		_dialog.loaded = false;
		// Remove Index
		WiscoLayerManager._indexes.pop();
	};
	
	/*Set Button Invisible*/
	var _setButtonInvisible = function() {
		var buttons = _element.children[0].children;
		var length1 = buttons.length;
		for(var i = 1; i < length1; i++) {
			buttons[i].style.display = "inline-block";
		}
		var length2 = arguments.length;
		for(var j = 0; j < length2; j++) {
			buttons[arguments[j]].style.display = "none";
		}
	};
	
	/*Minimize*/
	this.minimize = function() {
		// Set Position & Size
		_element.style.top = document.body.clientHeight - 28;
		_element.style.left = 0;
		var div1 = _element.children[0];
		div1.style.width = 200;
		var div2 = _element.children[1];
		div2.style.display = "none";
		// Set Title Event
		_setTitleEvent(false);
		// Set Button Invisible
		_setButtonInvisible(4);
	};
	
	/*Maximize*/
	this.maximize = function() {
		// Set Position & Size
		_element.style.top = 0;
		_element.style.left = 0;
		var div1 = _element.children[0];
		div1.style.width = document.body.clientWidth - 2;
		var div2 = _element.children[1];
		div2.style.display = "block";
		var iframe = div2.children[0];
		iframe.style.width = document.body.clientWidth - 14;
		iframe.style.height = document.body.clientHeight - 34;
		// Set Title Event
		_setTitleEvent(false);
		// Set Button Invisible
		_setButtonInvisible(2);
	};
	
	/*Restore*/
	this.restore = function() {
		// Set Position & Size
		_element.style.top = _top;
		_element.style.left = _left;
		var div1 = _element.children[0];
		div1.style.width = _dialog.width + 10;
		var div2 = _element.children[1];
		div2.style.display = "block";
		var iframe = div2.children[0];
		iframe.style.width = _dialog.width;
		iframe.style.height = _dialog.height;
		// Set Title Event
		_setTitleEvent(true);
		// Set Button Invisible
		_setButtonInvisible(3);
	};
	
	/*Set Title Event*/
	var _setTitleEvent = function(on) {
		var div1 = _element.children[0];
		if(on) {
			div1.ondblclick = _dialog.maximize;
			div1.onmousedown = _startMove;
			div1.onmouseup = _finishMove;
		}
		else {
			div1.ondblclick = _dialog.restore;
			div1.onmousedown = null;
			div1.onmouseup = null;
		}
	};
	
	/*Move Range*/
	var _moveRange = { top : 0, bottom : 0, left : 0, right : 0 };
	
	/*Start Move*/
	var _startMove = function(event) {
		event = event ? event : window.event;
		var div1 = _element.children[0];
		// Set Move Range
		_moveRange.top = event.clientY - _element.offsetTop;
		_moveRange.bottom = _moveRange.top 
			+ document.body.clientHeight - div1.clientHeight;
		_moveRange.left = event.clientX - _element.offsetLeft;
		_moveRange.right = _moveRange.left 
			+ document.body.clientWidth - _element.clientWidth;
		// Set Body Unselectable
		if(!div1.setCapture) return;
		document.body.className = "unselectable";
		var onselectstart = function() { return false; };
		document.body.onselectstart = onselectstart;
		div1.onmousemove = _move;
		div1.setCapture();
	};
	
	/*Finish Move*/
	var _finishMove = function() {
		var div1 = _element.children[0];
		if(!div1.releaseCapture) return;
		// Set Body Selectable
		document.body.className = "";
		document.body.onselectstart = null;
		div1.onmousemove = null;
		div1.releaseCapture();
	};
	
	/*Move*/
	var _move = function(event) {
		event = event ? event : window.event;
		var position = {
			x : event.clientX,
			y : event.clientY
		};
		if(position.y < _moveRange.top) 
			position.y = _moveRange.top;
		if(position.y > _moveRange.bottom) 
			position.y = _moveRange.bottom;
		_top = position.y - _moveRange.top;
		_left = position.x - _moveRange.left;
		_element.style.top = _top;
		_element.style.left = _left;
	};
};

/*Wisco Layer Manager*/
var WiscoLayerManager = {

	/*Dialogs (Private)*/
	_dialogs : [],
	
	/*Indexes (Private)*/
	_indexes : [],
	
	/*Get Free Dialog (Private)*/
	_getFreeDialog : function() {
		var dialog = null;
		var length = WiscoLayerManager._dialogs.length;
		for(var i = 0 ; i < length; i++) {
			dialog = WiscoLayerManager._dialogs[i];
			if(!dialog.loaded) return dialog;
		}
		dialog = new WiscoDialog("wiscodialog" + length);
		WiscoLayerManager._dialogs[length] = dialog;
		dialog.build();
		return dialog;
	},
	
	/*Get Next ZIndex (Private)*/
	_getNextZIndex : function() {
		var zIndex = 10000;
		var length = WiscoLayerManager._dialogs.length;
		for(var i = 0 ; i < length; i++) {
			var dialog = WiscoLayerManager._dialogs[i];
			if(dialog.loaded && zIndex < dialog.zIndex) 
				zIndex = dialog.zIndex;
		}
		return zIndex + 5;
	},
	
	/*Get Dialog*/
	getDialog : function() {
		// Get Index
		var length = WiscoLayerManager._indexes.length;
		var index = WiscoLayerManager._indexes[length - 1];
		return WiscoLayerManager._dialogs[index];
	},
	
	/*Show Dialog*/
	showDialog : function(src, width, height, title, argu, onload, onunload) {
		var dialog = WiscoLayerManager._getFreeDialog();
		// Add Index
		var index = dialog.id.replace("wiscodialog", "");
		WiscoLayerManager._indexes.push(index);
		dialog.src = src;
		dialog.width = width;
		dialog.height = height;
		dialog.zIndex = WiscoLayerManager._getNextZIndex();
		if(!title) dialog.title = "";
		else dialog.title = title;
		dialog.argu = argu;
		dialog.onload = onload;
		dialog.onunload = onunload;
		dialog.load();
	}
};