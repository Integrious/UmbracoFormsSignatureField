const path = require('path');

module.exports = {
  entry: {
    signatureField: './App_Plugins_Src/UmbracoFormsSignatureField/FrontEnd/Js/init.js'
  },
  output: {
    path: path.resolve(__dirname, 'App_Plugins', 'UmbracoFormsSignatureField', 'FrontEnd'),
    filename: './[name].js'
  }
};
