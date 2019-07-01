import HttpService from './http-service';
import { config } from '../config';
import { createSearchFailedAction, createSearchSucceededAction } from '../actions/create-actions';

class ImageService {
    async searchImage(keyword) {
        try {
            const result = await HttpService.get(config.apiEndpoints.SEARCH, { keyword });
            createSearchSucceededAction(result.data.items);
        } catch (error) {
            createSearchFailedAction();
        }
    }
    
}

export default new ImageService();