import * as React from 'react';
import classNames from 'classnames';
import { IModelError } from './IModelError';

interface IValidationSummaryProp {
    errors: IModelError[];
    message?: string;
}

export default class ValiationSummary extends React.Component<IValidationSummaryProp, undefined> {

    static defaultProps: IValidationSummaryProp;

    constructor() {
        super();
    }

    render() {
        var rows = Object.keys(this.props.errors).map(function (key) {
            var row = this.props.errors[key].map(function (error, i) {
                return (<li key={i}>{error}</li>);
            });
            return (row);
        }, this);
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