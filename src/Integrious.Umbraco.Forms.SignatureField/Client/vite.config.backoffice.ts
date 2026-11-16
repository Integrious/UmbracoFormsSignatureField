import { defineConfig } from 'vite';

// The back office bundle, following Umbraco's own shipped base config
// (staticwebassets/umbraco/backoffice/vite-config-base.js): ES target, library build, and
// everything under @umbraco-cms left external so it resolves through the back office's runtime
// importmap rather than being bundled in. Bundling it would ship a second copy of Umbraco.
//
// Two departures from that base, both to keep MSBuild happy. The output lands inside this
// project's wwwroot, which the Web SDK globs into static web assets AT EVALUATION TIME, before any
// target runs - so the set of files on disk has to stay stable across builds:
//   * fixed, unhashed names for the entry and for every chunk, so the glob can never point at a
//     file a later build renamed;
//   * emptyOutDir off, because this folder also holds hand written source and the sass output,
//     which this build does not own.
export default defineConfig({
  build: {
    target: 'es2022',
    outDir: '../wwwroot/App_Plugins/UmbracoFormsSignatureField/BackOffice',
    emptyOutDir: false,
    sourcemap: true,
    lib: {
      entry: 'src/backoffice/index.ts',
      formats: ['es'],
    },
    rollupOptions: {
      external: [/^@umbraco-cms/],
      output: {
        entryFileNames: 'signature-field.js',
        chunkFileNames: '[name].js',
      },
    },
  },
});
