const path = require('path');
const TerserPlugin = require('terser-webpack-plugin');
 
module.exports = {
    optimization: {
        minimize: false,
        minimizer: [
            new TerserPlugin({
                terserOptions: {
                    format: {
                        comments: false,
                    },
                },
                extractComments: false,
            }),
        ],
    },
    entry: {
        ins_document: './src/document/main.ts',
    },
    module: {
        rules: [{
            test: /\.ts$/,
            include: [path.resolve(__dirname, 'src')],
            use: 'ts-loader',
        }, ],
    },
    resolve: {
        extensions: ['.ts', '.js'],
    },
    devtool: 'inline-source-map',
    output: {
        filename: '[name].js',
        libraryTarget: 'var',
        library: 'fx'
    },
};