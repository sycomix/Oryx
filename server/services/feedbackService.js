const config = require('../config').mongodb;
const mongoDBService = require('../data/mongoDBService');

class FeedbackService {

    constructor() { }

    async addFeedback(feedbackCollectionName) {
        return await mongoDBService.insertDocs(config.feedbackCollectionName, feedbackCollectionName);
    }
}

module.exports = new FeedbackService();