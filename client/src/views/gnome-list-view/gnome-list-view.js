import React from 'react';
import PropTypes from 'prop-types';
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
    createExpandItemAction: PropTypes.func,
    items: PropTypes.arrayOf(PropTypes.object).isRequired
}