const bodyParser = require('body-parser');
const express = require('express');

const feedbackService = require('../services/feedbackService');

const router = express.Router();
router.use(bodyParser.urlencoded({ extended: true }));

router.post('/', function gnomeRouteFeedback(req, res) {
    feedbackService.addFeedback(req.body, () => {
        console.log('Feedback added');
        res.render('index', { pageTitle: 'Feedback', entry: 'feedback' });
    });
});

module.exports = router;