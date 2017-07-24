import * as React from 'react';
import ConfirmationModal from '../containers/ConfirmationModal';

export interface LayoutProps {
    body: React.ReactElement<any>;
}

class Layout extends React.Component<LayoutProps, null> {
    public render() {
        return <div>
            {this.props.body}
            <ConfirmationModal />
        </div>;
    }
}

export default Layout;