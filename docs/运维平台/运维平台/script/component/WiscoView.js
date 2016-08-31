/**************************
 * Wisco View
 *
 * @author Raiden
**************************/

/*Wisco State Types*/
var WiscoStateTypes = {
	Input : "a77c35836ecd4c6fabaf55da3868bc22",
	Select : "352ca88a1ea1419583abfbc6f6d848f6"
};

/*Wisco State*/
function WiscoState(id) {

	/*This (Private)*/
	var _state = this;
	
	/*Attributes*/
	this.id = id;
	this.type = null;
	this.value = null;
};

/*Wisco View*/
function WiscoView(id) {
	
	/*This (Private)*/
	var _view = this;
	
	/*Attributes*/
	this.id = id;
	/*View States*/
	this.states = [];
};

/*Wisco View Manager*/
var WiscoViewManager = {

	/*Views (Private)*/
	_views : {},
	
	/*To Json Select (Private)*/
	_toJsonSelect : function(select) {
		var jsonSelect = "[ " ;
		var length = select.children.length;
		for(var i = 0; i < length; i++) {
			var option = select.children[i];
			jsonSelect += "{ value : '" + option.value + 
				"', html : '" + option.innerHTML + "' }";
			if(i != length - 1) jsonSelect += ", ";
		}
		jsonSelect += " ]";
		return jsonSelect;
	},
	
	/*To Html Select (Private)*/
	_toHtmlSelect : function(select, jsonSelect, document) {
		var length1 = select.children.length;
		for(var i = length1 - 1; i >= 0; i--) {
			select.removeChild(select.children[i]);
		}
		var options = eval(jsonSelect);
		var length2 = options.length;
		for(var j = 0; j < length2; j++) {
			var option = document.createElement("option");
			option.value = options[j].value;
			option.innerHTML = options[j].html;
			select.appendChild(option);
		}
	},
	
	/*Save View States*/
	saveViewStates : function(url, element) {
		if(!element) return;
		var view = new WiscoView(url);
		// Save Input States
		var inputs = element.getElementsByTagName("input");
		var length1 = inputs.length;
		for(var i = 0; i < length1; i++) {
			var input = inputs[i];
			if(!input.id) continue;
			var state = new WiscoState(input.id);
			state.type = WiscoStateTypes.Input;
			if(input.type == "radio" || input.type == "checkbox")
				state.value = input.checked;	// Radio & Checkbox
			else state.value = input.value;
			view.states.push(state);
		}
		// Save Select States
		var selects = element.getElementsByTagName("select");
		var length2 = selects.length;
		for(var j = 0; j < length2; j++) {
			var select = selects[j];
			if(!select.id) continue;
			var state = new WiscoState(select.id);
			state.type = WiscoStateTypes.Select;
			state.value = [select.value, WiscoViewManager._toJsonSelect(select)];
			view.states.push(state);
		}
		// Put View
		WiscoViewManager._views[url] = view;
	},
	
	/*Restore View States*/
	restoreViewStates : function(url, document) {
		// Get View
		var view = WiscoViewManager._views[url];
		if(!view) return;
		var length = view.states.length;
		for(var i = 0; i < length; i++) {
			var state = view.states[i];
			var element = document.getElementById(state.id);
			if(!element) continue;
			// Restore Input States
			if(state.type == WiscoStateTypes.Input) {
				if(element.type == "radio" || element.type == "checkbox")
					element.checked = state.value !== false;	// Radio & Checkbox
				else element.value = state.value;
			}
			// Restore Select States
			else if(state.type == WiscoStateTypes.Select) {
				WiscoViewManager._toHtmlSelect(element, state.value[1], document);
				element.value = state.value[0];
			}
		}
	},
	
	/*Clear View States*/
	clearViewStates : function(url) {
		// Get View
		var view = WiscoViewManager._views[url];
		if(view) view.states = new Array();
	}
};