const { MongoClient } = require('mongodb');
const config = require('../config').mongodb;

class MongoDBService {
    constructor() {
        this.getConnection = this.getConnection.bind(this);
        this.disconnect = this.disconnect.bind(this);
        this.connection = null;
    }

    async getConnection() {
        if (!this.connection) {
            this.connection = await MongoClient.connect(config.url);
        }

        return this.connection;
    }

    async disconnect() {
        if (this.connection) {
            await this.connection.close();
            this.connection = null;
        }
    }

    async findDoc(collectionName, criteria) {
        const connection = await this.getConnection();
        return await connection.collection(collectionName).findOne(criteria);
    }

    async findDocs(collectionName, criteria) {        
        const connection = await this.getConnection();
        return await connection.collection(collectionName).find(criteria).toArray();
    }

    async insertDocs(collectionName, docs) {
        const connection = await this.getConnection();
        return await connection.collection(collectionName).insert(docs);  
    }

    async removeDoc(collectionName, criteria) {
        const connection = await this.getConnection();
        return await connection.collection(collectionName).remove(criteria, {
            single: true,
            w: 1
        });
    }

    async updateDocs(collectionName, criteria, docs) {
        const connection = await this.getConnection();
        return await connection.collection(collectionName).update(criteria, docs, {
            multi: true,
            upsert: false,
            w: 1
        });             
    }

}

module.exports = new MongoDBService();