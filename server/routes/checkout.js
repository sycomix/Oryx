const bodyParser = require('body-parser');
const express = require('express');

const orderService = require('../services/orderService');
const cartService = require('../services/cartService');

const router = express.Router();
router.use(bodyParser.urlencoded({ extended: true }));

router.post('/', function gnomeRouteCheckout(req, res) {
    if (!req.body.token) {
        res.status(401).send('Unauthorized');
        return;
    }
    orderService.addOrder({
        items: req.body['checkout-items'],
        name: req.body['checkout-name'],
        email: req.body['checkout-email'],
        token: req.body.token
    }).then(() => {
        console.log('Order added');
        return cartService.clearCart(req.body.token);
    }).then(() => {
        res.render('index', { pageTitle: 'Checkout', entry: 'checkout' });
    });
});

module.exports = router;