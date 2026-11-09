const path = require('path');

// The build tooling stays at the repository root; the project it builds into lives under src/.
const project = path.resolve(__dirname, 'src', 'Integrious.Umbraco.Forms.SignatureField');

module.exports = {
  entry: {
    signatureField:
      './src/Integrious.Umbraco.Forms.SignatureField/App_Plugins_Src/UmbracoFormsSignatureField/FrontEnd/Js/init.js'
  },
  output: {
    path: path.resolve(project, 'wwwroot', 'App_Plugins', 'UmbracoFormsSignatureField', 'FrontEnd'),
    filename: './[name].js'
  }
};
