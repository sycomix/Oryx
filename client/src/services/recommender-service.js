import { createRecommendationsUpdatedAction } from '../actions/cart-actions';
import axios from 'axios';
import HttpService from './http-service';
import { config } from '../config';

class RecommenderService {
    async getRecommendations() {
        try {
            const urlRecommenderResult = await HttpService.get(config.apiEndpoints.RECOMMENDER);
            
            const axiosWithoutInterceptor = axios.create();
            const recommendationsResult = await axiosWithoutInterceptor.get(urlRecommenderResult.data);
            createRecommendationsUpdatedAction(recommendationsResult.data);
    
        } catch (error) {
            console.error(error)
        }
    
    }
}

export default new RecommenderService();