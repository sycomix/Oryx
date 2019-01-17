'use strict';

const path = require('path');
const fs = require('fs');
const config = require('../config');

const sourcePath = path.join(__dirname, 'sources', config.dataSource + '.js');
if (!fs.existsSync(sourcePath)) {
    throw new Error(`Unknown dataSource source type "${config.dataSource}"`);
}

const dataSource = require(sourcePath);

module.exports = {
    getGnomes: dataSource.getGnomes,
    getGnome: dataSource.getGnome,
    addGnomes: dataSource.addGnomes,
    addOrder: dataSource.addOrder,
    addFeedback: dataSource.addFeedback,
    getCart: dataSource.getCart,
    addToCart: dataSource.addToCart,
    removeFromCart: dataSource.removeFromCart,
    clearCart: dataSource.clearCart
};
