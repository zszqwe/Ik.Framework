/*Load XML*/
var loadXML = function(url) {
	var xObject = null;
	if (window.ActiveXObject) {
		xObject = new ActiveXObject("Microsoft.XMLDOM");
		xObject.async = false;
		xObject.load(url);
	}
	else if(document.implementation && 
		document.implementation.createDocument) {
		var xRequest = new window.XMLHttpRequest();
		xRequest.open("GET", url, false);
		xRequest.send(null);
		xObject = xRequest.responseXML;
	}
	if(xObject) {
		var xElement = xObject.documentElement;
		var nodeName = xElement.nodeName;
		if(nodeName == "application") {
			var xApplication = new XApplication(null);
			xApplication.parseXML(xElement);
			if(xApplication.id === null) return;
			// Init ZTree
			var setting = { callback : { onClick : viewXML } };
			var node = xApplication.toZTreeNode();
			$.fn.zTree.init($("#wiscotree1"), setting, [node]);
		}
	}
};

/*View XML*/
var viewXML = function() {
	var tree = $.fn.zTree.getZTreeObj("wiscotree1");
	var node = tree.getSelectedNodes()[0];
	var html = node.element.toHTML();
	var panel = document.getElementById("panel");
	if(panel) panel.children[0].innerHTML = html;
};

/*XApplication*/
function XApplication(id) {
	
	/*This (Private)*/
	var _xApplication = this;
	
	/*Attributes*/
	this.id = id;
	this.code = null;
	this.name = null;
	this.entrance = null;
	this.domain = null;
	this.url = null;
	this.enabled = false;
	this.description = null;
	this.parentId = null;
	this.xForms = [];
	
	/*Parse XML*/
	this.parseXML = function(xElement) {
		if(!xElement) return;
		var attribute = xElement.getAttribute("id");
		if(attribute) _xApplication.id = attribute;
		attribute = xElement.getAttribute("code");
		if(attribute) _xApplication.code = attribute;
		attribute = xElement.getAttribute("name");
		if(attribute) _xApplication.name = attribute;
		attribute = xElement.getAttribute("entrance");
		if(attribute) _xApplication.entrance = attribute;
		attribute = xElement.getAttribute("domain");
		if(attribute) _xApplication.domain = attribute;
		attribute = xElement.getAttribute("url");
		if(attribute) _xApplication.url = attribute;
		attribute = xElement.getAttribute("enabled");
		_xApplication.enabled = attribute === "true";
		attribute = xElement.getAttribute("description");
		if(attribute) _xApplication.description = attribute;
		attribute = xElement.getAttribute("parentId");
		if(attribute) _xApplication.parentId = attribute;
		var childXElements = xElement.childNodes;
		if(!childXElements) return;
		var length = childXElements.length;
		for(var i = 0; i < length; i++) {
			var childXElement = childXElements.item(i);
			var nodeName = childXElement.nodeName;
			if(nodeName == "form") {
				var xForm = new XForm(null);
				xForm.parseXML(childXElement);
				if(xForm.id === null) continue;
				_xApplication.xForms.push(xForm);
			}
		}
	};	
	
	/*To ZTree Node*/
	this.toZTreeNode = function() {
		var node = {};
		node.name = _xApplication.name;
		node.open = true;
		node.children = [];
		var length = _xApplication.xForms.length;
		for(var i = 0; i < length; i++) {
			var xForm = _xApplication.xForms[i];
			var childNode = xForm.toZTreeNode();
			node.children.push(childNode);
		}
		node.element = _xApplication;
		return node;
	};
	
	/*To HTML*/
	this.toHTML = function() {
		return "Id:&nbsp;" + _xApplication.id + "<br/>"
			+ "Code:&nbsp;" + _xApplication.code + "<br/>"
			+ "Name:&nbsp;" + _xApplication.name + "<br/>"
			+ "Entrance:&nbsp;" + _xApplication.entrance + "<br/>"
			+ "Domain:&nbsp;" + _xApplication.domain + "<br/>"
			+ "Url:&nbsp;" + _xApplication.url + "<br/>"
			+ "Enabled:&nbsp;" + _xApplication.enabled + "<br/>"
			+ "Description:&nbsp;" + _xApplication.description + "<br/>"
			+ "ParentId:&nbsp;" + _xApplication.parentId;
	};
};

/*XForm*/
function XForm(id) {

	/*This (Private)*/
	var _xForm = this;
	
	/*Attributes*/
	this.id = id;
	this.code = null;
	this.name = null;
	this.entrance = null;
	this.domain = null;
	this.url = null;
	this.enabled = false;
	this.description = null;
	this.parentId = null;
	this.applicationId = null;
	this.childXForms = [];
	this.xFunctions = [];
	
	/*Parse XML*/
	this.parseXML = function(xElement) {
		if(!xElement) return;
		var attribute = xElement.getAttribute("id");
		if(attribute) _xForm.id = attribute;
		attribute = xElement.getAttribute("code");
		if(attribute) _xForm.code = attribute;
		attribute = xElement.getAttribute("name");
		if(attribute) _xForm.name = attribute;
		attribute = xElement.getAttribute("entrance");
		if(attribute) _xForm.entrance = attribute;
		attribute = xElement.getAttribute("domain");
		if(attribute) _xForm.domain = attribute;
		attribute = xElement.getAttribute("url");
		if(attribute) _xForm.url = attribute;
		attribute = xElement.getAttribute("enabled");
		_xForm.enabled = attribute === "true";
		attribute = xElement.getAttribute("description");
		if(attribute) _xForm.description = attribute;
		attribute = xElement.getAttribute("parentId");
		if(attribute) _xForm.parentId = attribute;
		attribute = xElement.getAttribute("applicationId");
		if(attribute) _xForm.applicationId = attribute;
		var childXElements = xElement.childNodes;
		if(!childXElements) return;
		var length = childXElements.length;
		for(var i = 0; i < length; i++) {
			var childXElement = childXElements.item(i);
			var nodeName = childXElement.nodeName;
			if(nodeName == "form") {
				var xForm = new XForm(null);
				xForm.parseXML(childXElement);
				if(xForm.id === null) continue;
				_xForm.childXForms.push(xForm);
			}
			else if(nodeName == "function") {
				var xFunction = new XFunction(null);
				xFunction.parseXML(childXElement);
				if(xFunction.id === null) continue;
				_xForm.xFunctions.push(xFunction);
			}
		}
	};
	
	/*To ZTree Node*/
	this.toZTreeNode = function() {
		var node = {};
		node.name = _xForm.name;
		node.children = [];
		var length = _xForm.childXForms.length;
		for(var i = 0; i < length; i++) {
			var childXForm = _xForm.childXForms[i];
			var childNode = childXForm.toZTreeNode();
			node.children.push(childNode);
		}
		length = _xForm.xFunctions.length;
		for(var i = 0; i < length; i++) {
			var xFunction = _xForm.xFunctions[i];
			var childNode = xFunction.toZTreeNode();
			node.children.push(childNode);
		}
		node.element = _xForm;
		return node;
	};
	
	/*To HTML*/
	this.toHTML = function() {
		return "Id:&nbsp;" + _xForm.id + "<br/>"
			+ "Code:&nbsp;" + _xForm.code + "<br/>"
			+ "Name:&nbsp;" + _xForm.name + "<br/>"
			+ "Entrance:&nbsp;" + _xForm.entrance + "<br/>"
			+ "Domain:&nbsp;" + _xForm.domain + "<br/>"
			+ "Url:&nbsp;" + _xForm.url + "<br/>"
			+ "Enabled:&nbsp;" + _xForm.enabled + "<br/>"
			+ "Description:&nbsp;" + _xForm.description + "<br/>"
			+ "ParentId:&nbsp;" + _xForm.parentId + "<br/>"
			+ "ApplicationId:&nbsp;" + _xForm.applicationId;
	};
};

/*XFunction*/
function XFunction(id) {
	
	/*This (Private)*/
	var _xFunction = this;
	
	/*Attributes*/
	this.id = id;
	this.code = null;
	this.name = null;
	this.description = null;
	this.formId = null;
	
	/*Parse XML*/
	this.parseXML = function(xElement) {
		if(!xElement) return;
		var attribute = xElement.getAttribute("id");
		if(attribute) _xFunction.id = attribute;
		attribute = xElement.getAttribute("code");
		if(attribute) _xFunction.code = attribute;
		attribute = xElement.getAttribute("name");
		if(attribute) _xFunction.name = attribute;
		attribute = xElement.getAttribute("description");
		if(attribute) _xFunction.description = attribute;
		attribute = xElement.getAttribute("formId");
		if(attribute) _xFunction.formId = attribute;
	};
	
	/*To ZTree Node*/
	this.toZTreeNode = function() {
		var node = {};
		node.name = _xFunction.name;
		node.element = _xFunction;
		return node;
	};
	
	/*To HTML*/
	this.toHTML = function() {
		return "Id:&nbsp;" + _xFunction.id + "<br/>"
			+ "Code:&nbsp;" + _xFunction.code + "<br/>"
			+ "Name:&nbsp;" + _xFunction.name + "<br/>"
			+ "Description:&nbsp;" + _xFunction.description + "<br/>"
			+ "FormId:&nbsp;" + _xFunction.formId;
	};
};