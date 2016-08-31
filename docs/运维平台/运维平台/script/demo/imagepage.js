/*Init Current Image*/
var initCurrentImage = function() {
	var argu = getWiscoDialog().argu;
	var image = document.getElementById("image");
	if(image) image.src = argu[0];
};

/*View Large Image*/
var viewLargeImage = function() {
	var image = document.getElementById("image");
	if(image) window.open(image.src);
};

/*View Other Image*/
var viewOtherImage = function(step) {
	var argu = getWiscoDialog().argu;
	var image = document.getElementById("image");
	if(image) {
		var currentIndex = 0;
		var currentUrl = image.src;
		var length = argu[1].length;
		for(var i = 0; i < length; i++) {
			if(currentUrl == argu[1][i].src) {
				currentIndex = i;
				break;
			}
		}
		currentIndex = currentIndex + step;
		currentIndex = currentIndex % length;
		if(currentIndex < 0) currentIndex += length;
		image.src = argu[1][currentIndex].src;
		resetImage();	// Reset Image
	}
};

/*Rotate Image*/
var rotateImage = function(clockwise) {
	var image = document.getElementById("image");
	if(image) {
		var currentAngle = image.getAttribute("angle");
		if(!currentAngle) currentAngle = 0;
		if(clockwise === false) 
			currentAngle = currentAngle - 1;
		else currentAngle = currentAngle - 3;
		if(currentAngle < 0) currentAngle += 4;
		image.setAttribute("angle", currentAngle);
		var oWidth = image.getAttribute("oWidth");
		var oHeight = image.getAttribute("oHeight");
		if(!oWidth) {
			oWidth = image.scrollWidth;
			image.setAttribute("oWidth", oWidth);
		}
		if(!oHeight) {
			oHeight = image.scrollHeight;
			image.setAttribute("oHeight", oHeight);
		}
		image.className = "rotate" + currentAngle;
		if(currentAngle == 1 || currentAngle == 3) {
			image.setAttribute("width", oWidth * oWidth / oHeight);
			image.setAttribute("height", oWidth);
		}
		else {
			image.setAttribute("width", oWidth);
			image.setAttribute("height", oHeight);
		}
	}
};

/*Zoom Image*/
var zoomImage = function(out) {
	var image = document.getElementById("image");
	if(image) {
		var currentScale = image.getAttribute("scale");
		if(!currentScale) currentScale = 1;
		currentScale = currentScale * 1;
		if(out === true) 
			currentScale = currentScale - 0.1;
		else currentScale = currentScale + 0.1;
		if(currentScale > 2) currentScale = 2;
		if(currentScale < 0.5) currentScale = 0.5;
		image.setAttribute("scale", currentScale);
		var oWidth = image.getAttribute("oWidth");
		var oHeight = image.getAttribute("oHeight");
		if(!oWidth) {
			oWidth = image.scrollWidth;
			image.setAttribute("oWidth", oWidth);
		}
		if(!oHeight) {
			oHeight = image.scrollHeight;
			image.setAttribute("oHeight", oHeight);
		}
		image.setAttribute("width", oWidth * currentScale);
		image.setAttribute("height", oHeight * currentScale);
	}
};

/*Reset Image*/
var resetImage = function() {
	var image = document.getElementById("image");
	if(image) {
		image.className = "rotate0";
		image.removeAttribute("angle");
		image.removeAttribute("scale");
		image.removeAttribute("oWidth");
		image.removeAttribute("oHeight");
		image.removeAttribute("width");
		image.removeAttribute("height");
	}
};