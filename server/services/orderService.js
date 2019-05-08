const config = require('../config').mongodb;
const mongoDBService = require('../db/mongoDBService');

class OrderService {

    constructor() { }

    async addOrder(order) {
        return await  mongoDBService.insertDocs(config.orderCollectionName, order);
    }

}

module.exports = new OrderService();