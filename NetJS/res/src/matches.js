var calculateRating = function(r1: number, r2: number): number[] {
	var R1 = Math.pow(10, r1 / 400);
	var R2 = Math.pow(10, r2 / 400);

	var E1 = R1 / (R1 + R2);
	var E2 = R2 / (R1 + R2);

	var S1 = 1;
	var S2 = 0;

	var newR1 = r1 + K * (S1 - E1);
	var newR2 = r2 + K * (S2 - E2);

	return [newR1, newR2];
};

if(request.path[1] == "create"){
	var data = SQL.execute("Data", `
		SELECT winner.ID AS winnerID, winner.Rating AS winnerRating, loser.ID AS loserID, loser.Rating AS loserRating 
		FROM Player winner, Player loser 
		WHERE winner.Name = '${request.params.winner}' 
		AND loser.Name = '${request.params.loser}'
	`);

	var newRatings = calculateRating(data.winnerRating, data.loserRating);

	SQL.execute("Data", `
		INSERT INTO Match (Winner, Loser) (${data.winnerID}, ${data.loserID});
		UPDATE Player SET Rating = ${newRatings[0]} WHERE ID = ${data.winnerID};
		UPDATE Player SET Rating = ${newRatings[1]} WHERE ID = ${data.loserID};
	`);
	redirect("/matches");
}else{
	<form action="/matches/create" method="get">
		"Winnaar: "<input type="text" name="winner"/><br/>
		"Verliezer: "<input type="text" name="loser"/>
		<button type="submit">"Toevoegen"</button>
	</form>
	
	var matches = SQL.execute("Data", "SELECT pw.Name AS WinnerName, pl.Name AS LoserName, DatePlayed FROM Match JOIN Player pw ON pw.ID = Match.Winner JOIN Player pl ON pl.ID = Match.Loser ORDER BY Match.DatePlayed DESC");
	for(var match of matches){
		<div class="match">
			<span class="date">match.DatePlayed + " - "</span>
			match.WinnerName + " heeft gewonnen van " + match.LoserName
		</div>
	}
}