const bodyParser = require('body-parser');
const express = require('express');

const router = express.Router();
router.use(bodyParser.json());

router.get('/', function gnomeRouteBrowse(req, res) {
    const renderData = { pageTitle: 'Browse', entry: 'browse' };
    console.log('Render values: ', renderData);
    res.render('index', renderData);
});

const db = require('../db');
router.get('/api/items', function gnomeRouteApiBrowse(req, res) {
    let tags;
    if (req.query.tags) {
        tags = req.query.tags.split(',');
    }

    db.getGnomes(tags).then((items) => {

        console.info('%d gnomes found', items.length);
        if (tags) {
            console.log('Tags used in filter: ', tags);
        }

        // remove scott gnome from list
        let scottItem = items.findIndex((item) => {
            return item.name.indexOf('Scott Gnome') >= 0;
        })

        if (scottItem >=0) {
            items.splice(scottItem, 1);
        }

        res.send({ items });
    }, () => {
        res.send({ items: [] });
    });
});

module.exports = router;