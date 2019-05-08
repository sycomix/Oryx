const config = require('../config').mongodb;
const mongoDBService = require('../data/mongoDBService');

class OrderService {

    constructor() { }

    addOrder(order) {
        return mongoDBService.insertDocs(config.orderCollectionName, order);
    }

}

module.exports = new OrderService();