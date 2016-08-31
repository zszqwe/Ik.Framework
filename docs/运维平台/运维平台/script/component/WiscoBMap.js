/**************************
 * Wisco Map (Baidu)
 *
 * @author Raiden
**************************/

/*Include BaiduMap Script*/
document.write("<script language='javascript' src='http://api.map.baidu.com/api?v=2.0&ak=19ff48ea6b533ee6f06c431c2e3a7bdd'></script>");
document.write("<script language='javascript' src='http://api.map.baidu.com/library/RectangleZoom/1.2/src/RectangleZoom_min.js'></script>");

/*Wisco Map (Baidu)*/
function WiscoBMap(id) {
	
	/*This (Private)*/
	var _map = this;
	
	/*BaiduMap Element*/
	var _baiduMap = null;
	var _zoomBox = null;
	function MousePositionControl() {
		this.defaultAnchor = BMAP_ANCHOR_TOP_RIGHT;
		this.defaultOffset = new BMap.Size(1, 0);
	};
	MousePositionControl.prototype = new BMap.Control();
	MousePositionControl.prototype.initialize = function(_baiduMap) {
		var span = document.createElement("span");
		span.id = _map.id + ":MousePositionControl";
		span.className = "unselectable";
		span.onselectstart = function() { return false; };
		_baiduMap.addEventListener("mousemove", function(e) {
			span.innerHTML = e.point.lng + ", " + e.point.lat;
		});
		_baiduMap.getContainer().appendChild(span);
		return span;
	};
	
	/*Attributes*/
	this.id = id;
	
	/*Get BaiduMap Element*/
	this.getBaiduMap = function() {
		return _baiduMap;
	};
	
	/*Init*/
	this.init = function(position, oninit) {
		_baiduMap = new BMap.Map(_map.id);
		_zoomBox = new BMapLib.RectangleZoom(_baiduMap);
		// Init Map Control
		_baiduMap.enableScrollWheelZoom();
		_baiduMap.enableKeyboard();
		_baiduMap.setDefaultCursor("default");
		_baiduMap.setDraggingCursor("default");
		_baiduMap.addControl(new BMap.NavigationControl());
		_baiduMap.addControl(new BMap.ScaleControl());  
		_baiduMap.addControl(new BMap.CopyrightControl());
		_baiduMap.addControl(new BMap.OverviewMapControl({ isOpen : true }));
		_baiduMap.addControl(new MousePositionControl());
		// Respond Oninit Event
		_baiduMap.addEventListener("tilesloaded", function() {
			if(typeof(oninit) == "function") oninit(_map);
		});
		// Set Map Center
		if(position) _baiduMap.centerAndZoom(position, 15);
		else {
			// Chongqing
			position = new BMap.Point(106.530635, 29.544606);
			_baiduMap.centerAndZoom(position, 15);
		}
	};
	
	/*Refresh*/
	this.refresh = function(position, onrefresh) {
		// Set Map Center
		if(position) _baiduMap.setCenter(position);
		// Respond Onrefresh Event
		if(typeof(onrefresh) == "function") onrefresh(_map);
	};
	
	/*Get Center*/
	this.getCenter = function() {
		return _baiduMap.getCenter();
	};
	
	/*Set Center*/
	this.setCenter = function(position) {
		_baiduMap.setCenter(position);
	};
	
	/*Get Zoom*/
	this.getZoom = function() {
		return _baiduMap.getZoom();
	};
	
	/*Zoom In*/
	this.zoomIn = function() {
		_baiduMap.zoomIn();
	};
	
	/*Zoom Out*/
	this.zoomOut = function() {
		_baiduMap.zoomOut();
	};
	
	/*Zoom To*/
	this.zoomTo = function(zoom) {
		_baiduMap.setZoom(zoom);
	};
	
	/*Start Zoom Box*/
	this.startZoomBox = function(out) {
		_zoomBox._opts.zoomType = out === true ? 1 : 0;
		_zoomBox.open();
	};
	
	/*Stop Zoom Box*/
	this.stopZoomBox = function() {
		_zoomBox.close();
	};
	
	/*Get Mouse Position*/
	this.getMousePosition = function() {
		var point = new BMap.Point(0, 0);
		var control = document.getElementById(_map.id + ":MousePositionControl");
		if(control) {
			var html = control.innerHTML;
			point.lng = html.split(",")[0].trim();
			point.lat = html.split(",")[1].trim();
		}
		return point;
	};
	
	/*Set Mouse Cursor*/
	this.setMouseCursor = function(cursor) {
		_baiduMap.setDefaultCursor(cursor);
		_baiduMap.setDraggingCursor(cursor);
	};
	
	/*Add Marker*/
	this.addMarker = function(point, imgUrl, width, height, onclick) {
		var size = new BMap.Size(width, height);
		var anchor = new BMap.Size(size.width / 2, size.height);
		var icon = new BMap.Icon(imgUrl, size, { anchor : anchor });
		var marker = new BMap.Marker(point, { icon : icon });
		if(typeof(onclick) == "function")
			marker.addEventListener("click", onclick);
		_baiduMap.addOverlay(marker);
	};
	
	/*Remove Markers*/
	this.removeMarkers = function(point) {
		var overlays = _baiduMap.getOverlays();
		var length = overlays.length;
		for(var i = length - 1; i >=0; i--) {
			var overlay = overlays[i];
			if(overlay.getPosition) {
				var position = overlay.getPosition();
				if(position && position.equals(point))
					_baiduMap.removeOverlay(overlay);
			}
		}
	};
	
	/*Clear Markers*/
	this.clearMarkers = function() {
		_baiduMap.clearOverlays();
	};
	
	/*Add Popup*/
	this.addPopup = function(point, html, width, height) {
		var options = {
			enableCloseOnClick : false,
			enableMessage : false
		};
		if(width) options.width = width;
		if(height) options.height = height;
		var infoWindow = new BMap.InfoWindow(html, options);
		_baiduMap.openInfoWindow(infoWindow, point);
	};
	
	/*Clear Popups*/
	this.clearPopups = function() {
		_baiduMap.closeInfoWindow();
	};
	
	/*Add Event*/
	this.addEvent = function(name, event) {
		if(typeof(event) != "function") return;
		_baiduMap.addEventListener(name, event);
	};
	
	/*Remove Event*/
	this.removeEvent = function(name, event) {
		if(typeof(event) != "function") return;
		_baiduMap.removeEventListener(name, event);
	};
};