var stopwords = {"‘t": true, "a": true, "aan": true, "aangaande": true, "aangezien": true, "achter": true, "achterna": true, "aen": true, "af": true, "afd": true, "afgelopen": true, "agter": true, "al": true, "aldaar": true, "aldus": true, "alhoewel": true, "alias": true, "all’": true, "alle": true, "allebei": true, "alleen": true, "alleenlyk": true, "allen": true, "alles": true, "als": true, "alsnog": true, "altijd": true, "altoos": true, "altyd": true, "ander": true, "andere": true, "anderen": true, "anders": true, "anderszins": true, "anm": true, "b": true, "behalve": true, "behoudens": true, "beide": true, "beiden": true, "ben": true, "beneden": true, "bent": true, "bepaald": true, "beter": true, "betere": true, "betreffende": true, "bij": true, "bijna": true, "bijvoorbeeld": true, "binnen": true, "binnenin": true, "bizonder": true, "bizondere": true, "bl": true, "blz": true, "boven": true, "bovenal": true, "bovendien": true, "bovengenoemd": true, "bovenstaand": true, "bovenvermeld": true, "buiten": true, "by": true, "daar": true, "daarheen": true, "daarin": true, "daarna": true, "daarnet": true, "daarom": true, "daarop": true, "daarvanlangs": true, "daer": true, "dan": true, "dat": true, "de": true, "deeze": true, "den": true, "der": true, "ders": true, "derzelver": true, "des": true, "deszelfs": true, "deszelvs": true, "deze": true, "dezelfde": true, "dezelve": true, "dezelven": true, "dezen": true, "dezer": true, "dezulke": true, "die": true, "dien": true, "dikwijls": true, "dikwyls": true, "dit": true, "dl": true, "doch": true, "doen": true, "doet": true, "dog": true, "door": true, "doorgaand": true, "doorgaans": true, "dr": true, "dra": true, "ds": true, "dus": true, "echter": true, "ed": true, "een": true, "een’": true, "eene": true, "eenen": true, "eener": true, "eenig": true, "eenige": true, "eens": true, "eer": true, "eerdat": true, "eerder": true, "eerlang": true, "eerst": true, "eerste": true, "eersten": true, "effe": true, "egter": true, "eigen": true, "eigene": true, "elk": true, "elkanderen": true, "elkanderens": true, "elke": true, "en": true, "enig": true, "enige": true, "enigerlei": true, "enigszins": true, "enkel": true, "enkele": true, "enz": true, "er": true, "erdoor": true, "et": true, "etc": true, "even": true, "eveneens": true, "evenwel": true, "ff": true, "gauw": true, "ge": true, "gebragt": true, "gedurende": true, "geen": true, "geene": true, "geenen": true, "gegeven": true, "gehad": true, "geheel": true, "geheele": true, "gekund": true, "geleden": true, "gelijk": true, "gelyk": true, "gemoeten": true, "gemogen": true, "geven": true, "geweest": true, "gewoon": true, "gewoonweg": true, "geworden": true, "gezegt": true, "gij": true, "gt": true, "gy": true, "haar": true, "had": true, "hadden": true, "hadt": true, "haer": true, "haere": true, "haeren": true, "haerer": true, "hans": true, "hare": true, "heb": true, "hebben": true, "hebt": true, "heeft": true, "hele": true, "hem": true, "hen": true, "het": true, "hier": true, "hierbeneden": true, "hierboven": true, "hierin": true, "hij": true, "hoe": true, "hoewel": true, "hun": true, "hunne": true, "hunner": true, "hy": true, "ibid": true, "idd": true, "ieder": true, "iemand": true, "iet": true, "iets": true, "ii": true, "iig": true, "ik": true, "ikke": true, "ikzelf": true, "in": true, "indien": true, "inmiddels": true, "inz": true, "inzake": true, "is": true, "ja": true, "je": true, "jezelf": true, "jij": true, "jijzelf": true, "jou": true, "jouw": true, "jouwe": true, "juist": true, "jullie": true, "kan": true, "klaar": true, "kon": true, "konden": true, "krachtens": true, "kunnen": true, "kunt": true, "laetste": true, "lang": true, "later": true, "liet": true, "liever": true, "like": true, "m": true, "maar": true, "maeken": true, "maer": true, "mag": true, "martin": true, "me": true, "mede": true, "meer": true, "meesten": true, "men": true, "menigwerf": true, "met": true, "mezelf": true, "mij": true, "mijn": true, "mijnent": true, "mijner": true, "mijzelf": true, "min": true, "minder": true, "misschien": true, "mocht": true, "mochten": true, "moest": true, "moesten": true, "moet": true, "moeten": true, "mogelijk": true, "mogelyk": true, "mogen": true, "my": true, "myn": true, "myne": true, "mynen": true, "myner": true, "myzelf": true, "na": true, "naar": true, "nabij": true, "nadat": true, "naer": true, "net": true, "niet": true, "niets": true, "nimmer": true, "nit": true, "no": true, "noch": true, "nog": true, "nogal": true, "nooit": true, "nr": true, "nu": true, "o": true, "of": true, "ofschoon": true, "om": true, "omdat": true, "omhoog": true, "omlaag": true, "omstreeks": true, "omtrent": true, "omver": true, "onder": true, "ondertussen": true, "ongeveer": true, "ons": true, "onszelf": true, "onze": true, "onzen": true, "onzer": true, "ooit": true, "ook": true, "oorspr": true, "op": true, "opdat ": true, "opnieuw": true, "opzij": true, "opzy": true, "over": true, "overeind": true, "overigens": true, "p": true, "pas": true, "pp": true, "precies": true, "pres": true, "prof": true, "publ": true, "reeds": true, "rond": true, "rondom": true, "rug": true, "s": true, "sedert": true, "sinds": true, "sindsdien": true, "sl": true, "slechts": true, "sommige": true, "spoedig": true, "st": true, "steeds": true, "sy": true, "t": true, "tamelijk": true, "tamelyk": true, "te": true, "tegen": true, "tegens": true, "ten": true, "tenzij": true, "ter": true, "terwijl": true, "terwyl": true, "thans": true, "tijdens": true, "toch": true, "toe": true, "toen": true, "toenmaals": true, "toenmalig": true, "tot": true, "totdat": true, "tusschen": true, "tussen": true, "tydens": true, "u": true, "uit": true, "uitg": true, "uitgezonderd": true, "uw": true, "uwe": true, "uwen": true, "uwer": true, "vaak": true, "vaakwat": true, "vakgr": true, "van": true, "vanaf": true, "vandaan": true, "vanuit": true, "vanwege": true, "veel": true, "veeleer": true, "veelen": true, "verder": true, "verre": true, "vert": true, "vervolgens": true, "vgl": true, "vol": true, "volgens": true, "voor": true, "vooraf": true, "vooral": true, "vooralsnog": true, "voorbij": true, "voorby": true, "voordat": true, "voordezen": true, "voordien": true, "voorheen": true, "voorop": true, "voort": true, "voortgez": true, "voorts": true, "voortz": true, "vooruit": true, "vrij": true, "vroeg": true, "vry": true, "waar": true, "waarom": true, "wanneer": true, "want": true, "waren": true, "was": true, "wat": true, "we": true, "weer": true, "weg": true, "wege": true, "wegens": true, "weinig": true, "weinige": true, "wel": true, "weldra": true, "welk": true, "welke": true, "welken": true, "welker": true, "werd": true, "werden": true, "werdt": true, "wezen": true, "wie": true, "wiens": true, "wier": true, "wierd": true, "wierden": true, "wij": true, "wijzelf": true, "wil": true, "wilde": true, "worden": true, "wordt": true, "wy": true, "wyze": true, "wyzelf": true, "zal": true, "ze": true, "zeer": true, "zei": true, "zeker": true, "zekere": true, "zelf": true, "zelfde": true, "zelfs": true, "zelve": true, "zelven": true, "zelvs": true, "zich": true, "zichzelf": true, "zichzelve": true, "zichzelven": true, "zie": true, "zig": true, "zij": true, "zijn": true, "zijnde": true, "zijne": true, "zijner": true, "zo": true, "zo’n": true, "zoals": true, "zodra": true, "zommige": true, "zommigen": true, "zonder": true, "zoo": true, "zou": true, "zoude": true, "zouden": true, "zoveel": true, "zowat": true, "zulk": true, "zulke": true, "zulks": true, "zullen": true, "zult": true, "zy": true, "zyn": true, "zynde": true, "zyne": true, "zynen": true, "zyner": true, "zyns": true, "beste": true, "groet": true, "vriendelijke": true, "groeten": true};

function getKeywords(text: string): string[] {
	var tokens: string[] = text.toLowerCase().split(/[^a-zàèìòùáéíóúýâêîôûäëïöü]/g);
	return tokens.filter(token => token.length > 0 && !(token in stopwords));
}

function getSummary(text: string, numSentences: number): string {
	var sentences: object[] = text.match(/[^\.!\?\n]+[\.!\?\n]+/g).map(sentence => { return {
		text: sentence.trim(),
		keywords: getKeywords(sentence),
		score: 0
	}}).filter(sentence => sentence.keywords.length > 0);

	// count keywords
	var keywords: object = {};
	sentences.forEach(sentence => {
		sentence.keywords.forEach(keyword => {
			if(keyword in keywords){
				keywords[keyword]++;
			}else{
				keywords[keyword] = 1;
			}
		});
	});

	// get best sentences
	var summary: string[] = [];
	while(summary.length < numSentences && sentences.length > 0){
		// score sentences
		sentences.forEach(sentence => {
			var total: number = 0, highest: number = 0;

			sentence.keywords.forEach(keyword => {
				var s: number = keywords[keyword];
				total += s;
				if(s > highest) highest = s;
			});

			var average: number = total / sentence.keywords.length;
			sentence.score = average * highest;
		});

		// get best
		var best: object = sentences.reduce((a, b) => a.score > b.score ? a : b);
		sentences.splice(sentences.indexOf(best), 1);
		best.keywords.forEach(keyword => keywords[keyword]--);
		summary.push(best.text);
	}
	
	return summary.join("<br>");
}

return getSummary(`Kleine stappen gezet voor aanpak klimaatverandering tijdens top in Bonn 
  
Delegaties uit bijna tweehonderd landen hebben in Bonn nieuwe afspraken gemaakt over de uitwerking van het klimaatakkoord van Parijs, en de aanpak van klimaatverandering. Desondanks zijn niet alle landen tevreden over het tempo van de strijd tegen de opwarming van de aarde. 
De conferentie in de Duitse stad duurde twee weken. Vertegenwoordigers van de aanwezige landen hielden zich onder meer bezig met voorbereidingen voor volgend jaar. Dan vindt in december een grote klimaattop plaats in het Poolse Katowice.

Hoewel het Amerikaanse besluit zich terug te trekken uit het akkoord van Parijs een schaduw wierp over de top, sprak voorzitter Fiji over een groot succes.

De delegaties spraken onder meer af bestaande afspraken over het terugdringen van de uitstoot van broeikasgassen nog eens tegen het licht te houden. Dat proces moet in 2018 van start gaan en krijgt de naam "Talanoa-dialoog". 

Ook maakten de landen afspraken over het opstellen van een gedetailleerd draaiboek voor het uitvoeren van het klimaatverdrag van Parijs. Eind 2015 werd tijdens een grote klimaattop in de Franse hoofdstad afgesproken dat de gemiddelde temperatuur op aarde niet meer dan twee graden mag stijgen. In de huidige situatie koersen we volgens verschillende modellen af op een temperatuurstijging van zo'n drie graden.

Om de temperatuurstijging in toom te houden willen de landen vastleggen hoe de uitstoot van broeikasgassen in verschillende landen moet worden gemeten en gemeld. Die afspraken moeten eind volgend jaar zijn uitgewerkt.

Niet op alle punten zijn knopen doorgehakt. Zo zijn nog geen definitieve afspraken gemaakt over financiële steun aan ontwikkelingslanden die worstelen met de gevolgen van klimaatverandering. Rijke landen beloofden eerder dat ze jaarlijks miljarden ter beschikking gaan stellen, maar die toezegging moet nader worden uitgewerkt.

Niet alle landen zijn tevreden over de kleine stappen die tijdens de top in Bonn zijn gezet. "We gaan momenteel in een stevig wandeltempo vooruit, dus landen zullen voortaan echt vaart moeten maken", zei de Braziliaanse minister van Milieu Jose Sarney Filho.`, 4)