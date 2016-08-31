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

/*Add Map Marker*/
var addMapMarker = function() {
	var imgUrl = "../image/demo/bmappage/marker.png";
	var handle = function() {
		var point = map.getMousePosition();
		var html = "Position: " + point.lng + ", " + point.lat;
		var onclick = function() {
			map.addPopup(point, html);
		};
		var geocoder = new BMap.Geocoder(); 
		geocoder.getLocation(point, function(result) {
			var address = result.addressComponents;
			html += "<br/><br/>" + address.province + " | " + address.city 
				+ address.district + address.street + address.streetNumber
				+ "<br/><br/><a href='javascript:;' onclick='removeMapMarkers(" 
				+ point.lng + ", " + point.lat + ");'>Remove</a>";
			map.addMarker(point, imgUrl, 20, 34, onclick);
			map.addPopup(point, html);
			map.removeEvent("click", handle);
			map.setMouseCursor("default");
		});
	};
	map.addEvent("click", handle);
	map.setMouseCursor("crosshair");
};

/*Remove Map Markers*/
var removeMapMarkers = function(lng, lat) {
	var point = new BMap.Point(lng, lat);
	map.removeMarkers(point);
};

/*Clear Map Markers*/
var clearMapMarkers = function() {
	map.clearMarkers();
	map.clearPopups();
};