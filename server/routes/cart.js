const bodyParser = require('body-parser');
const express = require('express');

const gnomesService = require('../services/gnomesService');
const cartService = require('../services/cartService');

const router = express.Router();
router.use(bodyParser.json());

router.get('/', function gnomeRouteCart(req, res) {
    res.render('index', { pageTitle: 'Cart', entry: 'cart' });
});

function sendItems(token, res) {
    cartService.getCart(token).then((cart) => {
        if (!cart || !cart.items || cart.items.length === 0) {
            return res.send({ items: [] });
        }

        gnomesService.getGnomes().then((gnomes) => {
            res.send({ items: cart.items.map((id) => gnomes.filter((gnome) => gnome.id.toString() === id)[0]) });
        });
    }, () => {
        res.send({ items: [] });
    });
}

router.get('/api/items', (req, res) => {
    if (!req.query.token) {
        res.status(401).send('Unauthorized');
        return;
    }
    sendItems(req.query.token, res);
});

router.put('/api/items/:item_id', (req, res) => {
    if (!req.body.token) {
        res.status(401).send('Unauthorized');
        return;
    }

    console.log('Item targetted %s', req.params.item_id);

    cartService.addToCart(req.body.token, req.params.item_id).then(() => {
        return gnomesService.getGnome(req.params.item_id);
    }).then((gnome) => {
        if (!gnome) {
            gnomesService.addGnomes([ req.body.item ]).then(() => {
                sendItems(req.body.token, res);
            });
        } else {
            sendItems(req.body.token, res);
        }
    });
});

router.delete('/api/items/:item_id', (req, res) => {
    if (!req.body.token) {
        res.status(401).send('Unauthorized');
        return;
    }

    console.log('Item targetted', req.params.item_id);

    cartService.removeFromCart(req.body.token, req.params.item_id).then(() => {
        sendItems(req.body.token, res);
    });
});

module.exports = router;