// Pulls in Umbraco's module augmentations for the global UmbExtensionManifestMap, which is what
// makes the global UmbExtensionManifest union resolve to the real manifest types. Without it every
// manifest fails to typecheck on its `meta` property.
//
// This deliberately lives in a .d.ts. Putting the same side-effect import in a .ts file means vite
// emits it as a real runtime import, and '@umbraco-cms/backoffice/extension-types' is NOT in the
// back office's runtime importmap - it is a types-only entry point. The browser then cannot resolve
// the specifier, the whole bundle fails to load, and nothing at all gets registered. A declaration
// file is never emitted, so the augmentations reach the compiler at no runtime cost.
import '@umbraco-cms/backoffice/extension-types';
