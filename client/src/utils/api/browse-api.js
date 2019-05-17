import { createUpdateFailedAction, createItemsUpdatedAction } from '../../actions/browse-actions';
import { createCookie } from '../cookies';
import HttpService from '../../services/HttpService';

export async function updateItems(tags = []) {
    // Update cookie here
    createCookie('searchTags', JSON.stringify(tags), 30);

    try {
        const result = await HttpService.get('browse/api/items', {tags: tags.join(',') });
        createItemsUpdatedAction(result.data.items);
    } catch (error) {
        console.error(error);
        createUpdateFailedAction();
    }
    
}
