import * as React from 'react';
import classNames from 'classnames';
import { ApplicationState } from '../../store';
import { DocumentFile } from '../../models';

type DocumentResultStateProps = {
    isSelected: boolean
} & DocumentFile;

type DocumentResultDispatchProps = {
    onSelect: (id: number) => void
}

type OwnProps = DocumentResultDispatchProps & DocumentResultStateProps;

class DocumentResult extends React.Component<OwnProps, null> {

    clickStatus: number;
    clickTimer: number;

    constructor(props: OwnProps) {
        super(props);

        this.handleClick = this.handleClick.bind(this);
        this.handleDblClick = this.handleDblClick.bind(this);
    }

    handleClick(e: React.SyntheticEvent<HTMLAnchorElement>) {
        e.preventDefault();
        
        this.clickStatus = 1;

        const clickHandler = () => {
            if (this.clickStatus === 1) {
                this.props.onSelect(this.props.id);
            }
        }

        this.clickTimer = setTimeout(clickHandler, 200);
    }

    handleDblClick(e: React.SyntheticEvent<HTMLAnchorElement>) {
        e.preventDefault();

        clearTimeout(this.clickTimer);

        this.clickStatus = 0;
        window.open(this.props.viewLink, '_blank');
    }

    public render() {

        const { isSelected, title, abstract, thumbnailLink } = this.props;

        const resultClass = classNames({
            'active': isSelected,
            'card pulse': true
        });

        return <div className={resultClass}>
            <a target="_blank" className="thumbnail-link" onClick={this.handleClick} onDoubleClick={this.handleDblClick}>
                <div className="thumbnail" style={{ backgroundImage: 'url(' + thumbnailLink + ')' }}>
                </div>
            </a>
            <div className="title-container">
                <a title={title} className="title" onClick={this.handleClick} onDoubleClick={this.handleDblClick}>
                    {title}
                </a>
                <span className="abstract">
                    {abstract}
                </span>
            </div>
        </div>;
    }
}

export default DocumentResult;