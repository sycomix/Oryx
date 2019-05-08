const config = require('../config').mongodb;
const mongoDBService = require('../data/mongoDBService');

class CartService {
    
    constructor() { }

    async addToCart(token, itemId) {
        return await mongoDBService.findDoc(config.cartCollectionName, { _id: token }).then((cart) => {
            if (!cart) {
                return mongoDBService.insertDocs(config.cartCollectionName, {
                    _id: token,
                    items: [ itemId ]
                });
            } 
            
            return mongoDBService.updateDocs(config.cartCollectionName, { _id: token }, {
                $addToSet: { items: itemId }
            });            
        })
    }

    async clearCart(token) {
        return await mongoDBService.removeDoc(config.cartCollectionName, { _id: token });
    }

    async getCart(token) {
        return await mongoDBService.findDoc(config.cartCollectionName, { _id: token }).then((cart) => {
            return cart;
        });
    }

    async removeFromCart(token, itemId) {
        return await mongoDBService.findDoc(config.cartCollectionName, { _id: token }).then((cart) => {
            if (!cart) {
                return;
            } 
            
            return mongoDBService.updateDocs(config.cartCollectionName, { _id: token }, {
                $pull: { items: itemId }
            });
        })
    }

}

module.exports = new CartService();