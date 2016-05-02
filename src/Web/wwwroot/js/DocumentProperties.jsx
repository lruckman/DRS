var React = require('react');
var moment = require('moment');

var DocumentProperties = React.createClass({
    propTypes: {
        document: React.PropTypes.shape({
            abstract: React.PropTypes.string,
            createdOn: React.PropTypes.string,
            file: React.PropTypes.shape({
                createdOn: React.PropTypes.string,
                modifiedOn: React.PropTypes.string,
                pageCount: React.PropTypes.number,
                size: React.PropTypes.number,
                thumbnailLink: React.PropTypes.string,
                versionNum: React.PropTypes.number
            }),
            libraryIds: React.PropTypes.array,
            location: React.PropTypes.string,
            modifiedOn: React.PropTypes.string,
            title: React.PropTypes.string
        }),
        onClose: React.PropTypes.func
    },
    getDefaultProps: function() {
        return {
            document: {
                abstract: '',
                createdOn: new Date().toString(),
                file: {
                    createdOn: new Date().toString(),
                    modifiedOn: new Date().toString(),
                    pageCount: 0,
                    size: 0,
                    thumbnailLink: '',
                    versionNum: 1
                },
                libraryIds: [],
                location: '',
                modifiedOn: new Date().toString(),
                title: ''
            },
            onClose: function () { }
        }    
    },
    getInitialState () {
        return { };
    },
    render: function () {
        var addedOn = moment(this.props.document.createdOn).local().format();
        var updatedOn = moment(this.props.document.modifiedOn).local().format();
        return (
            <div>
                <img src={this.props.document.file.thumbnailLink} className="img-responsive thumbnail" />
                <h3>{this.props.document.title}</h3>
                <p>{this.props.document.abstract}</p>
                <dl>
                    <dt>Size:</dt>
                    <dd>{this.props.document.file.size} KB</dd>
                    <dt>Page Count:</dt>
                    <dd>{this.props.document.file.pageCount}</dd>
                    <dt>Added On</dt>
                    <dd>{addedOn}</dd>
                    <dt>Updated On</dt>
                    <dd>{updatedOn}</dd>
                    <dt>Version Num</dt>
                    <dd>{this.props.document.file.versionNum}</dd>
                </dl>
            </div>
        );
    }
});

module.exports = DocumentProperties;