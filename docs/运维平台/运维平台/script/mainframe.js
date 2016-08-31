/*Fold Left Panel*/
var foldLeftPanel = function(e) {
	var panel = document.getElementById("leftpanel");
	if(panel) panel.style.display = "none";
	e.className = "arrowright";
	e.onclick = function() {
		unfoldLeftPanel(e);
	};
};

/*Unfold Left Panel*/
var unfoldLeftPanel = function(e) {
	var panel = document.getElementById("leftpanel");
	if(panel) panel.style.display = "";
	e.className = "arrowleft";
	e.onclick = function() {
		foldLeftPanel(e);
	};
};

/*Fold Guide Panel*/
var foldGuidePanel = function(e) {
	var panel = e.parentNode.parentNode.children[1];
	if(panel) panel.style.display = "none";
	e.className = "arrowright";
	e.onclick = function() {
		unfoldGuidePanel(e);
	};
};

/*Unfold Guide Panel*/
var unfoldGuidePanel = function(e) {
	var panel = e.parentNode.parentNode.children[1];
	if(panel) panel.style.display = "";
	e.className = "arrowdown";
	e.onclick = function() {
		foldGuidePanel(e);
	};
};

/*Menu Item Click*/
var menuItemClick = function(e, url) {
	var menu = e.parentNode;
	var length = menu.children.length;
	for(var i = 0; i < length; i++) {
		menu.children[i].className = "";
	}
	e.className = "current";
	_setPathPanel(e.children[1].innerHTML, 1);
	_setPathPanel("&nbsp;", 3);
	_setLeftPanel(e.children[3].innerHTML);
	_setRightPanel(url);
};

/*Guide Item Click*/
var guideItemClick = function(e, url) {
	var panel = document.getElementById("leftpanel");
	var length1 = panel.children.length;
	for(var i = 0; i < length1; i++) {
		var guide = panel.children[i].children[1];
		var length2 = guide.children.length;
		for(var j = 0; j < length2; j++) {
			guide.children[j].className = "";
		}
	}
	e.className = "current";
	var title = e.parentNode.parentNode.children[0];
	var path = title.innerText ? title.innerText : title.textContent;
	path = "&nbsp;>>&nbsp;" + path + "&nbsp;>>&nbsp;" + e.innerHTML;
	_setPathPanel(path, 3);
	_setRightPanel(url);
};

/*Set Path Panel*/
var _setPathPanel = function(path, level) {
	var panel = document.getElementById("headpanel");
	if(level == 1) panel.children[3].children[0].innerHTML = path;
	else if(level == 3) panel.children[3].children[1].innerHTML = path;
};

/*Set Left Panel*/
var _setLeftPanel = function(html) {
	var panel = document.getElementById("leftpanel");
	panel.innerHTML = htmlDecode(html);
};

/*Set Right Panel*/
var _setRightPanel = function(url) {
	var panel = document.getElementById("rightpanel");
	panel.children[0].src = url;
};