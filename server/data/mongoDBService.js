const { MongoClient } = require('mongodb');
const config = require('../config').mongodb;

class MongoDBService {
    constructor() {
        this.getConnection = this.getConnection.bind(this);        
        this.connection = null;
    }

    async getConnection() {
        if (!this.connection) {
            await MongoClient.connect(config.url).then((db) => {
                this.connection = db;                
            });
        }

        return this.connection;
    }

    async disconnect() {
        if (this.connection) {
            await this.connection.close().then(() => {
                this.connection = null;
            });
        }
    }

    async findDoc(collectionName, criteria) {        
        return await this.getConnection().then((db) => {
            return db.collection(collectionName).findOne(criteria);
        });
    }

    async findDocs(collectionName, criteria) {        
        return await this.getConnection().then((db) => {
            return db.collection(collectionName).find(criteria).toArray();
        });
    }

    async insertDocs(collectionName, docs) {
        return await this.getConnection().then((db) => {
            return db.collection(collectionName).insert(docs);
        });            
    }

    async removeDoc(collectionName, criteria) {
        return await this.getConnection().then((db) => {
            return db.collection(collectionName).remove(criteria, {
                single: true,
                w: 1
            });    
        });
    }

    async updateDocs(collectionName, criteria, docs) {
        return await  this.getConnection().then((db) => {
            return db.collection(collectionName).update(criteria, docs, {
                multi: true,
                upsert: false,
                w: 1
            });
        });             
    }

}

module.exports = new MongoDBService();