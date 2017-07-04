import * as React from 'react';
import classNames from 'classnames';
import { ModelError } from './models';

interface IValidationSummaryProp {
    errors: ModelError[];
    message?: string;
}

export default class ValiationSummary extends React.Component<IValidationSummaryProp, undefined> {

    static defaultProps: IValidationSummaryProp;

    constructor() {
        super();
    }

    render() {
        var rows = Object.keys(this.props.errors).map((key) =>
            this.props.errors[key].map((error, i) =>
                <li key={i}>{error}</li>
            ), this);

        var cssClass = classNames({
            'alert': true,
            'alert-danger': true,
            'hidden': rows.length === 0
        });
        return <div className={cssClass}>
            {this.props.message}
            <ul>
                {rows}
            </ul>
        </div>;
    }
}

ValiationSummary.defaultProps = {
    errors: [],
    message: 'You must correct the following errors and try again.'
}