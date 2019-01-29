import React from 'react';
import GnomeView from '../gnome-view/gnome-view';
import LoadingSpinner from '../loading-spinner/loading-spinner';
import './gnome-list-view.css';

export default React.createClass({

    displayName: 'gnome-list-view',

    propTypes: {
        createExpandItemAction: React.PropTypes.func,
        items: React.PropTypes.arrayOf(React.PropTypes.object).isRequired
    },

    render() {
        const items = this.props.items.map((item, key) => <GnomeView key={key} item={item} createExpandItemAction={this.props.createExpandItemAction} />);
        const itemsView = this.props.items.length === 0 ? <LoadingSpinner /> : items
        return (
            <div className="gs-gnomelist">
                {itemsView}
            </div>
        );
    }
});
