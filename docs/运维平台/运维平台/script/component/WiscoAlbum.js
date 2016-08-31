/**************************
 * Wisco Album
 *
 * @author Raiden
**************************/
function WiscoAlbum(id) {
	
	/*This (Private)*/
	var _album = this;
	
	/*Attributes*/
	this.container = null;
	this.innerList1 = null;
	this.innerList2 = null;
	this.scrollInterval = 0;
	this.scrollLength = 0;
	this.scaleLength = 0;
	this.correctValue = 0;
	this.offsetValue = 0;
	this.allowScroll = false;
	this.scrollTimeObj = null;
	this.autoScrollObj = null;
	
	/*Start Scroll Left*/
	this.startScrollLeft = function() {
		if (_album.scrollTimeObj) 
			window.clearInterval(_album.scrollTimeObj);
		if (_album.allowScroll) return;
		if (_album.autoScrollObj) 
			window.clearInterval(_album.autoScrollObj);
		_album.allowScroll = true;
		_album._doScrollLeft();
		_album.scrollTimeObj = window.setInterval(_album._doScrollLeft, _album.scrollInterval);
	};
	
	/*Stop Scroll Left*/
	this.stopScrollLeft = function() {
		if (_album.scrollTimeObj) 
			window.clearInterval(_album.scrollTimeObj);
		if (_album.container.scrollLeft % _album.scaleLength - _album.correctValue != 0) {
			_album.offsetValue = _album.scaleLength - _album.container.scrollLeft % _album.scaleLength + _album.correctValue;
			_album._closeScroll();
		}
		else {
			_album.allowScroll = false;
		}
	};
	
	/*Do Scroll Left (Private)*/
	this._doScrollLeft = function() {
		if (_album.container.scrollLeft >= _album.innerList1.scrollWidth)
			_album.container.scrollLeft -= _album.innerList1.scrollWidth;
		_album.container.scrollLeft += _album.scrollLength ;
	};
	
	/*Start Scroll Right*/
	this.startScrollRight = function() {
		if (_album.allowScroll) return;
		if (_album.autoScrollObj) 
			window.clearInterval(_album.autoScrollObj);
		_album.allowScroll = true;
		_album.scrollTimeObj = window.setInterval(_album._doScrollRight, this.scrollInterval);
	};
	
	/*Stop Scroll Right*/
	this.stopScrollRight = function() {
		if (_album.scrollTimeObj) 
			window.clearInterval(_album.scrollTimeObj);
		if (_album.container.scrollLeft % _album.scaleLength - _album.correctValue != 0) {
			_album.offsetValue = _album.correctValue - (_album.container.scrollLeft % _album.scaleLength);
			_album._closeScroll();
		}
		else {
			_album.allowScroll = false;
		}
	};
	
	/*Do Scroll Right (Private)*/
	this._doScrollRight = function() { 
		if (_album.container.scrollLeft <= 0)
			_album.container.scrollLeft += _album.innerList1.offsetWidth;
		_album.container.scrollLeft -= _album.scrollLength ;
	};
	
	/*Close Scroll (Private)*/
	this._closeScroll = function() {
		var value = 0;
		if (_album.offsetValue == 0) {
			_album.allowScroll = false;
			return;
		}
		if (_album.offsetValue < 0)	{
			if (_album.offsetValue < -_album.scrollLength) {
				_album.offsetValue += _album.scrollLength;
				value = _album.scrollLength;
			}
			else {
				value = -_album.offsetValue;
				_album.offsetValue = 0;
			}
			_album.container.scrollLeft -= value;
			window.setTimeout(_album._closeScroll, _album.scrollInterval);
		}
		else {
			if (_album.offsetValue > _album.scrollLength) {
				_album.offsetValue -= _album.scrollLength;
				value = _album.scrollLength;
			}
			else {
				value = _album.offsetValue;
				_album.offsetValue = 0;
			}
			_album.container.scrollLeft += value;
			window.setTimeout(_album._closeScroll, _album.scrollInterval);
		}
	};
};