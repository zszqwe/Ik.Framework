/*Init Flow Element*/
var elementType = {
	normal : "normal",
	inhand : "inhand",
	finished : "finished"
};
var initFlowElement = function() {
	var type = arguments[0];
	var length = arguments.length;
	for(var i = 1; i < length; i++) {
		var id = "element" + arguments[i];
		var element = document.getElementById(id);
		if(element) element.className = type;
	}
};

/*Init Flow Text*/
var initFlowText = function() {
	var visible = arguments[0];
	var length = arguments.length;
	for(var i = 1; i < length; i++) {
		var id = "text" + arguments[i];
		var element = document.getElementById(id);
		if(element) {
			if(visible) element.style.visibility = "visible";
			else element.style.visibility = "hidden";
		}
	}
};