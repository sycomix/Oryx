const configMongo = require('../config').mongodb;

const getFormattedMongoURL = () => {
    const mongodbUri = require('mongodb-uri');
    const url = process.env.MONGO_URL || `mongodb://${configMongo.host}:${configMongo.port}`;
    const urlObject = mongodbUri.parse(url);
    urlObject.database = configMongo.dbName;

    return mongodbUri.format(urlObject);
}

module.exports = {
    getFormattedMongoURL,
}