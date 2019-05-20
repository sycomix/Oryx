import React from 'react';
import PropTypes from 'prop-types';
import { searchImage } from '../../utils/api/create-api';

import './search-box-view.css';

const SEARCH_BOX_SIZE = 40;

export default class SearchBoxView extends React.Component {
    constructor(props, context) {
        super(props, context);
    }

    onKeyPressed(event) {
        if (event.key === 'Enter') {
            searchImage(event.target.value);
        }
    }

    render() {
        return (
            <div className="gs-create-search">
                <input
                    type="text"
                    className="gs-create-search-input"
                    placeholder={this.props.placeholder}
                    size={SEARCH_BOX_SIZE}
                    onKeyPress={this.onKeyPressed}
                    />
            </div>
        );
    }
}

SearchBoxView.propTypes = {
    placeholder: PropTypes.string
}
