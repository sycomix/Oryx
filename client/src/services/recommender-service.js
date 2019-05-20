import { createRecommendationsUpdatedAction } from '../actions/cart-actions';
import axios from 'axios';
import HttpService from './http-service';

class RecommenderService {
    async getRecommendations() {
        try {
            const urlRecommenderResult = await HttpService.get('recommender/api/recommender');
            
            const axiosWithoutInterceptor = axios.create();
            const recommendationsResult = await axiosWithoutInterceptor.get(urlRecommenderResult.data);
            createRecommendationsUpdatedAction(recommendationsResult.data);
    
        } catch (error) {
            console.error(error)
        }
    
    }
}

export default new RecommenderService();