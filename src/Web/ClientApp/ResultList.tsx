import * as React from 'react';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import classNames from 'classnames';

export interface IResult {
    abstract: string;
    id: number;
    selfLink: string;
    viewLink: string;
    thumbnailLink: string;
    title: string;
}

interface IResultListProp {
    data: IResult[];
    nextLink: string;
    onSelect: (location: string) => void;
}

interface IResultListState {
    activeIndex: number[];
}

export default class ResultList extends React.Component<IResultListProp, IResultListState> {

    static defaultProps: IResultListProp;

    clickStatus: number;
    clickTimer: any;

    constructor() {
        super();

        //this.resultHandleClick = this.resultHandleClick.bind(this);
        //this.resultHandleDblClick = this.resultHandleDblClick.bind(this);

        this.state = {
            activeIndex: []
        }
    }

    resultHandleClick(index, e) {
        e.preventDefault();

        var activeIndex = e.shiftKey
            ? this.state.activeIndex
            : [];

        activeIndex.push(index);

        this.setState({ activeIndex: activeIndex });

        this.clickStatus = 1;
        this.clickTimer = setTimeout(function () {
            if (this.clickStatus === 1) {
                this.props.onSelect(this.props.data[index].selfLink);
            }
        }.bind(this), 200);
    }

    resultHandleDblClick(index, e) {
        e.preventDefault();
        clearTimeout(this.clickTimer);
        this.clickStatus = 0;
        window.open(this.props.data[index].viewLink, '_blank');
    }

    render() {
        //return <span>hi</span>;
        var resultNodes = this.props.data.map(function (result: IResult, index: number) {
            var resultClass = classNames({
                'active': this.state.activeIndex.indexOf(index) !== -1,
                'card pulse': true
            });

            var boundClick = this.resultHandleClick.bind(this, index);
            var boundDblClick = this.resultHandleDblClick.bind(this, index);

            return <div key={result.id} className={resultClass}>
                    <a href="#" target="_blank" className="thumbnail-link" onClick={boundClick} onDoubleClick={boundDblClick}>
                        <div className="thumbnail" style={{ backgroundImage: 'url(' + result.thumbnailLink + ')' }}>
                        </div>
                    </a>
                    <div className="title-container">
                        <a href={result.selfLink} title={result.title} className="title" onClick={boundClick}>
                            {result.title}
                        </a>
                        <span className="abstract">
                            {result.abstract}
                        </span>
                    </div>
                </div>;
        }, this);
        return <div data-next-link={this.props.nextLink}>
                <ReactCSSTransitionGroup
                    transitionName="result"
                    transitionEnterTimeout={500}
                    transitionLeaveTimeout={300}>
                    {resultNodes}
                </ReactCSSTransitionGroup>
            </div>;
    }
}

ResultList.defaultProps = {
    data: [],
    nextLink: '',
    onSelect: function (location: string) { }
}