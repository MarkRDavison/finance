const cssnano = require('cssnano');
const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

const isProduction = process.argv[process.argv.indexOf('--mode') + 1] === 'production';

const config = {
    mode: isProduction ? 'production' : 'development',
    entry: './src/Index.tsx',
    resolve: {
        extensions: ['.ts', '.tsx', '.js'],
        alias: {
            "@": path.resolve(__dirname, "src"),
            "@images": path.resolve(__dirname, "public/images"),
        }
    },
    module: {
        rules: [
            {
                test: /\.(ts|tsx)$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                },
            },
            {
                test: /\.(jpg|png|svg|gif)$/,
                type: 'asset/resource',
            },
        ],
    },
    devServer: {
        historyApiFallback: true
    },
    devtool: 'source-map',
    output: {
        filename: '[name].bundle.[contenthash].js',
        publicPath: '/',
        path: path.resolve(__dirname, 'build'),
    },
    plugins: [
        new HtmlWebpackPlugin({
            template: "public/index.html",
            hash: true, // This is useful for cache busting
            filename: 'index.html',
            favicon: "./public/favicon.ico"
        }),
    ],
};

if (isProduction) {
    config.module.rules = config.module.rules.concat(
        { test: /\.css$/i, use: [MiniCssExtractPlugin.loader, "css-loader"], }
    )
    config.plugins = config.plugins.concat(
        new MiniCssExtractPlugin({ filename: "[name].[contenthash].css", })
    )
}
else {
    config.module.rules = config.module.rules.concat(
        { test: /\.css$/i, use: ["style-loader", "css-loader"], }
    )
}

module.exports = config;