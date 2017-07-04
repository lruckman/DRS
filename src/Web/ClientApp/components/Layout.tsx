import * as React from 'react';

export interface LayoutProps {
    body: React.ReactElement<any>;
}

class Layout extends React.Component<LayoutProps, null> {
    public render() {
        return <div>
            {this.props.body}
        </div>;
    }
}

export default Layout;