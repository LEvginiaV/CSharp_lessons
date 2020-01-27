const path = require("path");
const webpack = require("webpack");
const HtmlWebpackPlugin = require("html-webpack-plugin");

const _DEV_ = process.env.NODE_ENV !== "production";
const port = 3000;

const config = {
  entry: [
    "@babel/polyfill",
    "./react-selenium-testing.js",
    "./src/shellForTests/root.tsx",
  ],
  mode: _DEV_ ? "development" : "production",
  output: {
    path: path.resolve(__dirname, "shellForTests"),
    filename: "root.js",
  },
  devtool: _DEV_ ? "source-map" : "eval",
  module: {
    rules: [
      {
        test: /\.(ts|tsx)$/,
        exclude: /node_modules/,
        use: [
          { loader: "babel-loader" },
          { loader: "ts-loader" },
        ],
        include: [path.join(__dirname, "src")],
      },
      {
        test: /\.(js|jsx)$/,
        exclude: /node_modules/,
        loader: "babel-loader",
        include: [path.join(__dirname, "src"), path.join(__dirname, "react-selenium-testing.js")],
      },
      {
        test: /\.(c|le)ss$/,
        use: [
          { loader: "style-loader" },
          { loader: "css-loader", options: { importLoaders: 1, localIdentName: "[name]__[local]--[hash:base64:5]" } },
          { loader: "less-loader" },
        ],
        include: [path.join(__dirname, "src")],
      },
      {
        test: /\.(woff|woff2|eot|png|gif|ttf|jpg|svg)$/,
        loader: "url-loader",
        include: [path.join(__dirname, "src")],
      },


      {
        test: /\.css$/,
        use: [
          { loader: "style-loader" },
          { loader: "css-loader" },
        ],
        include: [/react\-ui|react\-icons/],
      },
      {
        test: /\.less$/,
        use: [
          { loader: "style-loader" },
          { loader: "css-loader" },
          { loader: "less-loader" },
        ],
        include: [/market\-ui/],
      },
      {
        test: /\.(woff|woff2|eot|png|gif|ttf|jpg|svg)$/,
        loader: "url-loader",
        include: [/react\-ui|react\-icons|market\-ui/],
      },

    ],
  },
  resolve: {
    extensions: [".js", ".jsx", ".ts", ".tsx", ".less", ".css"],
  },
  plugins: [
    new HtmlWebpackPlugin({
      template: "src/index.html",
    }),
    (_DEV_
      ? new webpack.DefinePlugin({
        "process.env.enableReactTesting": JSON.stringify(true),
      })
      : null),
  ].filter(plugin => plugin != null),
  devServer: {
    port: port,
    proxy: {
      "/customersApi": "http://localhost:10946",
      "/marketapi": "http://localhost:2543",
    },
  },
};

module.exports = config;
