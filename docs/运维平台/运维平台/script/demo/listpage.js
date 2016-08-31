/*Fold Search Panel*/
var foldSearchPanel = function(e) {
	var panel = e.parentNode.parentNode.parentNode.rows[1];
	if(panel) panel.style.display = "none";
	e.className = "arrowleft";
	e.onclick = function() {
		unfoldSearchPanel(e);
	};
};

/*Unfold Search Panel*/
var unfoldSearchPanel = function(e) {
	var panel = e.parentNode.parentNode.parentNode.rows[1];
	if(panel) panel.style.display = "";
	e.className = "arrowdown";
	e.onclick = function() {
		foldSearchPanel(e);
	};
};