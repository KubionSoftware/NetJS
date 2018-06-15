try{
	var recursive = function(){
		recursive();
	}
	recursive();
}catch(e){
	assert(() => true, "Recursive error - " + e);
}