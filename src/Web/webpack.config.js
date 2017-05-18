const path = require('path');
const webpack = require('webpack');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const CheckerPlugin = require('awesome-typescript-loader').CheckerPlugin;

var extractCSS = new ExtractTextPlugin('vendor.bundle.css');
const clientBundleOutputDir = './wwwroot/dist';

var clientBundleConfig = {
    stats: { modules: false },
    resolve: { extensions: ['.js', '.jsx', '.ts', '.tsx'] },
    entry: {
        'main': './ClientApp/boot-client.tsx',
        vendor: [
            'classnames'
            , 'react'
            , 'react-addons-css-transition-group'
            , 'react-bootstrap'
            , 'react-dom'
            , 'react-file-drop'
            , 'react-select'
            , 'moment'
        ]
    },
    module: {
        rules: [
            { test: /\.tsx?$/, include: /ClientApp/, use: 'babel-loader' },
            { test: /\.tsx?$/, include: /ClientApp/, use: 'awesome-typescript-loader?silent=true' },
            { test: /\.css$/, use: ExtractTextPlugin.extract({ use: 'css-loader' }) },
            { test: /\.(png|jpg|jpeg|gif|svg)$/, use: 'url-loader?limit=25000' }
        ]
    },
    output: {
        path: path.join(__dirname, clientBundleOutputDir),
        filename: '[name].js',
        publicPath: '/dist/' // Webpack dev middleware, if enabled, handles requests for this URL prefix
    },
    plugins: [
        new CheckerPlugin(),
        new ExtractTextPlugin('vendor.bundle.css'),
        new webpack.optimize.CommonsChunkPlugin({ name: 'vendor', filename: 'vendor.bundle.js' }) // Moves vendor content out of other bundles
    ]
};

module.exports = [clientBundleConfig];