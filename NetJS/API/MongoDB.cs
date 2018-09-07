using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.IO;
using System.Linq;

namespace NetJS.API {
    public class MongoDBAPI {

        public static dynamic insertOne(string connection, string collection, string json) {
            var application = State.Application;
            var state = State.Get();

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    var db = application.Connections.GetMongoDBConnection(connection);

                    var document = BsonDocument.Parse(json);
                    db.GetCollection<BsonDocument>(collection).InsertOne(document);

                    application.AddCallback(resolve, null, state);
                } catch (Exception e) {
                    application.AddCallback(reject, $"{e.Message}", state);
                }
            });
        }

        public static dynamic insertMany(string connection, string collection, string json) {
            var application = State.Application;
            var state = State.Get();

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    var db = application.Connections.GetMongoDBConnection(connection);

                    var documents = BsonSerializer.Deserialize<BsonArray>(json).Select(d => d.AsBsonDocument);
                    db.GetCollection<BsonDocument>(collection).InsertMany(documents);

                    application.AddCallback(resolve, null, state);
                } catch (Exception e) {
                    application.AddCallback(reject, $"{e.Message}", state);
                }
            });
        }

        public static dynamic updateOne(string connection, string collection, string query, string json) {
            var application = State.Application;
            var state = State.Get();

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    var db = application.Connections.GetMongoDBConnection(connection);

                    var document = BsonDocument.Parse(json);
                    db.GetCollection<BsonDocument>(collection).UpdateOne(query, document);

                    application.AddCallback(resolve, null, state);
                } catch (Exception e) {
                    application.AddCallback(reject, $"{e.Message}", state);
                }
            });
        }

        public static dynamic updateMany(string connection, string collection, string query, string json) {
            var application = State.Application;
            var state = State.Get();

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    var db = application.Connections.GetMongoDBConnection(connection);

                    var document = BsonDocument.Parse(json);
                    db.GetCollection<BsonDocument>(collection).UpdateMany(query, document);

                    application.AddCallback(resolve, null, state);
                } catch (Exception e) {
                    application.AddCallback(reject, $"{e.Message}", state);
                }
            });
        }

        public static dynamic deleteOne(string connection, string collection, string query) {
            var application = State.Application;
            var state = State.Get();

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    var db = application.Connections.GetMongoDBConnection(connection);

                    db.GetCollection<BsonDocument>(collection).DeleteOne(query);

                    application.AddCallback(resolve, null, state);
                } catch (Exception e) {
                    application.AddCallback(reject, $"{e.Message}", state);
                }
            });
        }

        public static dynamic deleteMany(string connection, string collection, string query) {
            var application = State.Application;
            var state = State.Get();

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    var db = application.Connections.GetMongoDBConnection(connection);

                    db.GetCollection<BsonDocument>(collection).DeleteMany(query);

                    application.AddCallback(resolve, null, state);
                } catch (Exception e) {
                    application.AddCallback(reject, $"{e.Message}", state);
                }
            });
        }

        public static IFindFluent<BsonDocument, BsonDocument> find (string connection, string collection, string query) {
            var application = State.Application;

            var db = application.Connections.GetMongoDBConnection(connection);
            return db.GetCollection<BsonDocument>(collection).Find(query);
        }

        public static IFindFluent<BsonDocument, BsonDocument> sort(IFindFluent<BsonDocument, BsonDocument> fluent, string sort) {
            return fluent.Sort(sort);
        }

        public static IFindFluent<BsonDocument, BsonDocument> limit(IFindFluent<BsonDocument, BsonDocument> fluent, int limit) {
            return fluent.Limit(limit);
        }

        public static IFindFluent<BsonDocument, BsonDocument> skip(IFindFluent<BsonDocument, BsonDocument> fluent, int amount) {
            return fluent.Skip(amount);
        }

        public static IAsyncCursor<BsonDocument> aggregate(string connection, string collection, string json) {
            var application = State.Application;

            var db = application.Connections.GetMongoDBConnection(connection);
            var documents = BsonSerializer.Deserialize<BsonArray>(json).Select(d => d.AsBsonDocument).ToArray();
            return db.GetCollection<BsonDocument>(collection).Aggregate<BsonDocument>(documents);
        }

        public static dynamic count(IFindFluent<BsonDocument, BsonDocument> fluent) {
            var application = State.Application;
            var state = State.Get();

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    int result = (int)fluent.CountDocuments();
                    application.AddCallback(resolve, result, state);
                } catch (Exception e) {
                    application.AddCallback(reject, $"{e.Message}", state);
                }
            });
        }

        public static dynamic toArray (IFindFluent<BsonDocument, BsonDocument> fluent) {
            var cursor = fluent.ToCursor();
            return toArray(cursor);
        }

        public static dynamic toArray(IAsyncCursor<BsonDocument> cursor) {
            var application = State.Application;
            var state = State.Get();

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    var list = cursor.ToList();
                    var result = new StringWriter();
                    result.Write("[");
                    for (var i = 0; i < list.Count; i++) {
                        if (i != 0) result.Write(",");

                        using (var bsonWriter = new JsonWriter(result, new JsonWriterSettings() { OutputMode = JsonOutputMode.Strict, Indent = false })) {
                            var context = BsonSerializationContext.CreateRoot(bsonWriter);
                            var nominalType = typeof(BsonDocument);
                            var serializer = BsonSerializer.LookupSerializer(nominalType);

                            var args = default(BsonSerializationArgs);
                            args.NominalType = nominalType;
                            serializer.Serialize(context, args, list[i]);
                        }
                    }
                    result.Write("]");
                    application.AddCallback(resolve, result, state);
                } catch (Exception e) {
                    application.AddCallback(reject, $"{e.Message}", state);
                }
            });
        }
    }
}
