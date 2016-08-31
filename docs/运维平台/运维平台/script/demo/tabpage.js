/*Tab Item Click*/
var tabItemClick = function(e, url) {
	var length = e.parentNode.children.length;
	for(var i = 0; i < length; i++) {
		var item = e.parentNode.children[i];
		var disabled = item.getAttribute("disabled");
		if(!item.disabled && disabled != "disabled") 
			item.className = "wiscotab";
	}
	e.className = "wiscotab_current";
	var index = e.getAttribute("index");
	var div = document.createElement("div");
	div.innerHTML = htmlDecode(e.children[3].innerHTML);
	var child = div.children[0];
	if(url) {
		var src = child.getAttribute("src");
		if(!src || src == "") child.setAttribute("src", url);
	}
	var panel = e.parentNode.parentNode.parentNode.rows[1].cells[0];
	if(panel) _setTabPanel(panel, index, child);
};

/*Set Tab Panel*/
var _setTabPanel = function(panel, index, e) {
	var length = panel.children.length;
	var existent = false;
	for(var i = 0; i < length; i++) {
		var j = panel.children[i].getAttribute("index");
		if(j != index) panel.children[i].style.display = "none";
		else {
			panel.children[i].style.display = "";
			existent = true;
		}
	}
	if(!existent) panel.appendChild(e);
};