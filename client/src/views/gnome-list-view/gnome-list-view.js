import React from 'react';
import GnomeView from '../gnome-view/gnome-view';
import LoadingSpinner from '../loading-spinner/loading-spinner';
import './gnome-list-view.css';

export default class GnomeListView extends React.Component {
    constructor(props, context) {
        super(props, context);
    }

    render() {
        const items = this.props.items.map((item, key) => <GnomeView key={key} item={item} createExpandItemAction={this.props.createExpandItemAction} />);
        const itemsView = this.props.items.length === 0 ? <LoadingSpinner /> : items
        return (
            <div className="gs-gnomelist">
                {itemsView}
            </div>
        );
    }
};

GnomeListView.propTypes = {
    createExpandItemAction: React.PropTypes.func,
    items: React.PropTypes.arrayOf(React.PropTypes.object).isRequired
}