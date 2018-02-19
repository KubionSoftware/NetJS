if(request.path[0] == "players"){
	include("players", {request: request});
}else if(request.path[0] == "matches"){
	include("matches", {request: request});
}