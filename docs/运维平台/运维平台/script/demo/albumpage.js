/*Set Album Photo*/
var setAlbumPhoto = function(id, url) {
	var photo = document.getElementById(id);
	if(photo && photo.src) photo.src = url;
};