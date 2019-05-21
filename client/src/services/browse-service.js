import { createUpdateFailedAction, createItemsUpdatedAction } from '../actions/browse-actions';
import { createCookie } from '../utils/cookies';
import HttpService from './http-service';
import { config } from '../config';

class BrowseService {
    async updateItems(tags = []) {
        // Update cookie here
        createCookie('searchTags', JSON.stringify(tags), 30);
    
        try {
            const result = await HttpService.get(config.apiEndpoints.BROWSE, {tags: tags.join(',') });
            createItemsUpdatedAction(result.data.items);
        } catch (error) {
            console.error(error);
            createUpdateFailedAction();
        }
        
    }
    
}

export default new BrowseService();