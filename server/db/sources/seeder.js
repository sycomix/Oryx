const config = require('../../config').mongodb;
const mongoDBService = require('../mongoDBService');
const gnomesService = require('../../services/gnomesService');

exports.initializeDatabase = () => {
    const initialData = require('../initial-data');
    
    return mongoDBService.getConnection(config.url)
        .then((db) => db.dropDatabase())
        .then(() => gnomesService.addGnomes(initialData))
        .then(mongoDBService.disconnect);
};

exports.initializeDatabaseCloud = () => {
    const initialData = require('../initial-data-cloud');
    
    return mongoDBService.getConnection(config.url)
        .then((db) => db.dropDatabase())
        .then(() => gnomesService.addGnomes(initialData))
        .then(mongoDBService.disconnect);
};