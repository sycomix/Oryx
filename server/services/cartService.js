const config = require('../config').mongodb;
const mongoDBService = require('../data/mongoDBService');

class CartService {
    
    constructor() { }

    addToCart(token, itemId) {
        return mongoDBService.findDoc(config.cartCollectionName, { _id: token }).then((cart) => {
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

    clearCart(token) {
        return mongoDBService.removeDoc(config.cartCollectionName, { _id: token });
    }

    getCart(token) {
        console.log('token', token)
        return mongoDBService.findDoc(config.cartCollectionName, { _id: token }).then((cart) => {
            return cart;
        });
    }

    removeFromCart(token, itemId) {
        return mongoDBService.findDoc(config.cartCollectionName, { _id: token }).then((cart) => {
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