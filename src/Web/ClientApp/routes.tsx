import * as React from 'react';
import { Route } from 'react-router';
import { Layout } from './components';
//import { App } from './containers';
import { Search } from './containers';
import { actionCreators as LibraryStore } from './store/entity/Library';

export const getRoutes = (store) => {
    const { dispatch } = store;

    const loadThings = (nextState, replace, callback) => dispatch(LibraryStore.fetch())
        .catch(console.log)
        .then(() => callback());

    return <Route component={Layout}>
        <Route path='*' components={{ body: Search }} onEnter={loadThings} />
    </Route>;

}

// Enable Hot Module Replacement (HMR)
if (module.hot) {
    module.hot.accept();
}