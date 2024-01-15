const path = require('path');

module.exports = {
  entry: {
    frontEnd: './App_Plugins_Src/UmbracoFormsSignatureField/FrontEnd/Js/init.js'
  },
  output: {
    path: path.resolve(__dirname, 'App_Plugins', 'UmbracoFormsSignatureField'),
    filename: './[name].js',
    clean: true
  }
};
