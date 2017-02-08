import * as React from 'react';
import moment from 'moment';
import { IDocumentDetails } from "./IDocumentDetails";

interface IDocumentPropertiesProp {
    onClose?: () => void;
    document: IDocumentDetails;
}

export default class DocumentProperties extends React.Component<IDocumentPropertiesProp, undefined> {

    static defaultProps: IDocumentPropertiesProp;

    constructor() {
        super();
    }

    render() {
        var addedOn = moment(this.props.document.createdOn).local().format();
        var updatedOn = moment(this.props.document.modifiedOn).local().format();
        return <div>
                <img src={this.props.document.thumbnailLink} className="img-responsive thumbnail" />
                <h3>{this.props.document.title}</h3>
                <p>{this.props.document.abstract}</p>
                <dl>
                    <dt>Size:</dt>
                    <dd>{this.props.document.size} KB</dd>
                    <dt>Page Count:</dt>
                    <dd>{this.props.document.pageCount}</dd>
                    <dt>Added On</dt>
                    <dd>{addedOn}</dd>
                    <dt>Updated On</dt>
                    <dd>{updatedOn}</dd>
                    <dt>Version Num</dt>
                    <dd>{this.props.document.versionNum}</dd>
                </dl>
            </div>;
    }
}

DocumentProperties.defaultProps = {
    document: {
        createdOn: null,
        modifiedOn: null,
        abstract: '',
        title: '',
        libraryIds: [],
        resourceUri: '',
        pageCount: 0,
        size: 0,
        thumbnailLink: '',
        versionNum: 1
    },
    onClose: function () { }
}