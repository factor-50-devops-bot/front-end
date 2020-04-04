const path = require("path");

module.exports = {
  context: path.resolve(__dirname, "src"),
  entry: "./js/app.js",
  output: {
    path: path.resolve(__dirname, "wwwroot")
  },
  module: {
    rules: [
      {
        test: /\.s[ac]ss$/i,
        use: [
          // Creates `style` nodes from JS strings
          "style-loader",
          // Translates CSS into CommonJS
          "css-loader",
          // Compiles Sass to CSS
          "sass-loader"
        ]
      },
      {
        test: /\.js$/, //using regex to tell babel exactly what files to transcompile
        exclude: /node_modules/, // files to be ignored
        use: {
          loader: "babel-loader" // specify the loader
        }
      }
    ]
  }
};