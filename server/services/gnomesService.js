const config = require('../config').mongodb;
const mongoDBService = require('../data/mongoDBService');

class GnomesService {
    constructor() { }

    addGnomes(items) {
        return mongoDBService.insertDocs(config.gnomeCollectionName, items);
    }

    getGnome(id) {
        return mongoDBService.findDoc(config.gnomeCollectionName, { id }).then((gnome) => {
            return gnome;
        });
    }

    getGnomes() {
        return mongoDBService.findDocs(config.gnomeCollectionName, {}).then((gnomes) => {
            return gnomes;
        });
    }
}

module.exports = new GnomesService();