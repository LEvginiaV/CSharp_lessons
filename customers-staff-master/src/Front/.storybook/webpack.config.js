module.exports = (config, env) => {

  config.module.rules.push({
    test: /\.(c|le)ss$/,
    use: [
      {loader: 'style-loader'},
      {loader: 'css-loader', options: {importLoaders: 1, localIdentName: '[name]__[local]--[hash:base64:5]'}},
      {loader: 'less-loader'}
    ],
  });
  config.module.rules.push({
    test: /\.(woff|woff2|eot|png|gif|ttf|jpg|svg)$/,
    loader: "url-loader"
  });
  config.module.rules.push({
    test: /\.(ts|tsx)$/,
    loader: "ts-loader"
  });

  config.resolve.extensions = [".js", ".jsx", ".ts", ".tsx", ".less", ".css"];

  config.externals = [
    {
      './cptable': 'var cptable'
    }
  ];

  config.node = {
    fs: 'empty'
  };

  return config;
};
