// Define callback for HTTP requests
HTTPServer.on("connection", function(){
	// Get articles from example JSON API
	HTTP.get("https://jsonplaceholder.typicode.com/posts").then(result => {
		// Parse the JSON result
		var posts = JSON.parse(result);
		
		// Render the posts in html
		var html = load("template.html", {posts: posts});
		
		// Write the result to the browser
		end(html);
	});
});