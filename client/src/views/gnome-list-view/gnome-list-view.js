import React from 'react';
import GnomeView from '../gnome-view/gnome-view';
import './gnome-list-view.css';

export default React.createClass({

    displayName: 'gnome-list-view',

    propTypes: {
        createExpandItemAction: React.PropTypes.func,
        items: React.PropTypes.arrayOf(React.PropTypes.object).isRequired
    },

    render() {
        const items = this.props.items.map((item, key) => <GnomeView key={key} item={item} createExpandItemAction={this.props.createExpandItemAction} />);
        return (
            <div className="gs-gnomelist">
                {items}
            </div>
        );
    }
});
