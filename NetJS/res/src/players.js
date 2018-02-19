if(request.path[1] == "create"){
	SQL.insert("Data", "Player", {
		Name: request.params.name
	});
	redirect("/players");
}else{
	<form action="/players/create" method="get">
		"Naam: "<input type="text" name="name"/>
		<button type="submit">"Aanmaken"</button>
	</form>

	var players = SQL.get("Data", "Player");
	<div class="players">
		for(var player of players){
			<div class="player">
				player.Name
				<span class="rating">" (" + player.Rating + ")"</div>
			</div>
		}
	</div>
}