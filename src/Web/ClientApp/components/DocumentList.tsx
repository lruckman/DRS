import * as React from 'react';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import classNames from 'classnames';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as SearchFormStore from '../store/SearchForm';
import { Document, Library } from '../models';

type ResultProps = {
    isSelected: boolean
    , id: number
    , selfLink: string
    , title: string
    , abstract: string
    , thumbnailLink: string
    , viewLink: string
    , onClick: (document: Document) => void
    , onDblClick: (document: Document) => void
}

const Result = (props: ResultProps) => {

    let { isSelected, id, selfLink, title, abstract, thumbnailLink, onClick, onDblClick } = props;

    let resultClass = classNames({
        'active': isSelected,
        'card pulse': true
    });

    let clickStatus: number;
    let clickTimer: number;

    let document: Document = { ...props };

    let handleClick = (e: React.FormEvent<HTMLAnchorElement>) => {
        e.preventDefault();
        clickStatus = 1;
        clickTimer = setTimeout(() => {
            if (clickStatus === 1) {
                onClick(document);
            }
        }, 200);
    }

    let handleDblClick = (e: React.FormEvent<HTMLAnchorElement>) => {
        e.preventDefault();
        clearTimeout(clickTimer);
        clickStatus = 0;
        onDblClick(document);
    }

    return <div className={resultClass}>
        <a href="#" target="_blank" className="thumbnail-link" onClick={handleClick.bind(this)} onDoubleClick={handleDblClick.bind(this)}>
            <div className="thumbnail" style={{ backgroundImage: 'url(' + thumbnailLink + ')' }}>
            </div>
        </a>
        <div className="title-container">
            <a href={selfLink} title={title} className="title" onClick={handleClick.bind(this)}>
                {title}
            </a>
            <span className="abstract">
                {abstract}
            </span>
        </div>
    </div>
}

type ResultListProps = {
    keywords: string
    , libraryIds: number[]
    , libraryOptions: Library[]
    , error: Error
    , isFetching: boolean
    , documents: Document[]
    , selected: number[]
    , nextPage: string
    , onSelect: (id: number) => void
}

const ResultList = (props: ResultListProps) => {

    let { onSelect, documents, nextPage, selected } = props;

    let handleClick = (document: Document) =>
        onSelect(document.id);

    let handleDblClick = (document: Document) =>
        window.open(document.viewLink, '_blank');

    return <div data-next-page={nextPage}>
        <ReactCSSTransitionGroup transitionName="result" transitionEnterTimeout={500} transitionLeaveTimeout={300}>
            {documents.map(document => <Result {...document} key={document.id} isSelected={selected.indexOf(document.id) !== -1}
                onClick={handleClick} onDblClick={handleDblClick} />)}
        </ReactCSSTransitionGroup>
    </div>
}

export default ResultList;

//export default connect(
//    (state: ApplicationState) => state.documentResults, // Selects which state properties are merged into the component's props
//    IDocumentResultsState.actionCreators                 // Selects which action creators are merged into the component's props
//)(ResultList);