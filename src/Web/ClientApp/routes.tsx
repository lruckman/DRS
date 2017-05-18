import * as React from 'react';
import { Router, Route, HistoryBase } from 'react-router';
import { Layout } from './components/Layout';
import App from './App';
import * as SearchFormStore from './store/SearchForm';

export const getRoutes = (store) => {
    const { dispatch } = store;

    const loadThings = (nextState, replace, callback) => Promise.all([
        dispatch(SearchFormStore.actionCreators.getLibraries())
    ])
        .catch(console.log)
        .then(() => callback());

    return <Route component={Layout}>
        <Route path='*' components={{ body: App }} onEnter={loadThings} />
    </Route>;

}

// Enable Hot Module Replacement (HMR)
if (module.hot) {
    module.hot.accept();
}