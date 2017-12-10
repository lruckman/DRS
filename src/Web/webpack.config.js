const path = require('path');
const webpack = require('webpack');
const ExtractTextPlugin = require('extract-text-webpack-plugin');

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
            { test: /\.tsx?$/, include: /ClientApp/, use: 'ts-loader' },
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
        new ExtractTextPlugin('vendor.bundle.css')
        , new webpack.optimize.CommonsChunkPlugin({ name: 'vendor', filename: 'vendor.bundle.js' }) // Moves vendor content out of other bundles
        , new webpack.optimize.ModuleConcatenationPlugin()
        /*
        , new webpack.DefinePlugin({
            'process.env': {
                NODE_ENV: JSON.stringify('production')
            }
        })
        , new webpack.optimize.UglifyJsPlugin()
        */
    ]
};

module.exports = [clientBundleConfig];