import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { browserHistory, Router } from 'react-router';
import { Provider } from 'react-redux';
import { syncHistoryWithStore } from 'react-router-redux';
import * as Routes from './routes';
import configureStore from './configureStore';
import { ApplicationState } from './store';

// Get the application-wide store instance, prepopulating with state from the server where available.
const initialState = (window as any).initialReduxState as ApplicationState;
const store = configureStore(initialState);
const history = syncHistoryWithStore(browserHistory, store);

// This code starts up the React app when it runs in a browser. It sets up the routing configuration
// and injects the app into a DOM element.
ReactDOM.render(
    <Provider store={store}>
        <Router history={history}>
            {Routes.getRoutes(store)}
        </Router>
    </Provider>,
    document.getElementById('my-spa')
);