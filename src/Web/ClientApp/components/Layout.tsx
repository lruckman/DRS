import * as React from 'react';

export interface LayoutProps {
    body: React.ReactElement<any>;
}

export class Layout extends React.Component<LayoutProps, void> {
    public render() {
        return <div>
            {this.props.body}
        </div>;
    }
}