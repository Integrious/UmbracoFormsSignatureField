import { defineConfig } from 'vite';

// The public website script, included by Signature.RequiredJavascriptFiles.
//
// Nothing is external here, unlike the back office bundle: a visitor's browser has no importmap,
// so signature_pad has to be bundled in. Names are fixed for the same static web assets reason as
// the back office config, and because RequiredJavascriptFiles names this file explicitly.
export default defineConfig({
  // signature_pad is MIT, and bundling it means carrying its copyright notice. webpack extracted
  // that to a sidecar signatureField.js.LICENSE.txt; esbuild strips /*! */ banners when minifying
  // unless told otherwise, so without this the notice vanishes from the build entirely. Inline
  // keeps it in the one file that actually contains the code.
  esbuild: {
    legalComments: 'inline',
  },
  build: {
    target: 'es2022',
    outDir: '../wwwroot/App_Plugins/UmbracoFormsSignatureField/FrontEnd',
    emptyOutDir: false,
    sourcemap: true,
    lib: {
      entry: 'src/frontend/signature-field.ts',
      formats: ['es'],
      // The default stylesheet, imported by the entry and extracted here rather than injected.
      // Named explicitly for the same two reasons as the JS: the static web assets glob needs a
      // stable name, and Signature.RequiredCssFiles names this file. Without this it would be
      // called after the package name, which is not what the C# asks Umbraco Forms to load.
      cssFileName: 'signatureField',
    },
    rollupOptions: {
      output: {
        entryFileNames: 'signatureField.js',
        chunkFileNames: '[name].js',
      },
    },
  },
});
