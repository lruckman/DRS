import * as React from 'react';
import { renderToString } from 'react-dom/server';
import { createServerRenderer } from 'aspnet-prerendering';
import App from './app';

export default createServerRenderer(params => {
    return new Promise((resolve, reject) => {
        const app = <App libraries={params.data.libraries} searchUrl={params.data.searchUrl}/>;
        const html = renderToString(app);
        resolve({
            html: html,
            globals: {
                addDocumentUrl: params.data.addDocumentUrl,
                libraries: params.data.libraries,
                searchUrl: params.data.searchUrl
            }
        });
    });
});