import { createSearchFailedAction, createSearchSucceededAction } from '../../actions/create-actions';
import HttpService from '../../services/http-service';

export async function searchImage(keyword) {
    try {
        const result = await HttpService.get('create/api/search', { keyword });
        createSearchSucceededAction(result.data.items);
    } catch (error) {
        createSearchFailedAction();
    }
}
