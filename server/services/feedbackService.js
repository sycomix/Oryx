const config = require('../config').mongodb;
const mongoDBService = require('../data/mongoDBService');

class FeedbackService {

    constructor() { }

    addFeedback(feedbackCollectionName) {
        return mongoDBService.insertDocs(config.feedbackCollectionName, feedbackCollectionName);
    }
}

module.exports = new FeedbackService();