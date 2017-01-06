import * as React from 'react';
import * as ReactDOM from 'react-dom';
import App from './app';

ReactDOM.render(<App addDocumentUrl={window.addDocumentUrl}
    libraries={window.libraries}
    searchUrl={window.searchUrl} />, document.getElementById('my-spa'));