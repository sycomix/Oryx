const config = require('../config').mongodb;
const mongoDBService = require('../db/mongoDBService');

class GnomesService {
    constructor() { }

    async addGnomes(items) {
        return await mongoDBService.insertDocs(config.gnomeCollectionName, items);
    }

    async getGnome(id) {
        return await mongoDBService.findDoc(config.gnomeCollectionName, { id }).then((gnome) => {
            return gnome;
        });
    }

    async getGnomes() {
        return await mongoDBService.findDocs(config.gnomeCollectionName, {}).then((gnomes) => {
            return gnomes;
        });
    }
}

module.exports = new GnomesService();