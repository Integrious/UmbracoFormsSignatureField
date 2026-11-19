// Umbraco Forms' `formsFieldPreview` extension type, declared here because Forms ships no npm
// types package - at runtime it exposes `@umbraco-forms/backoffice` through an importmap entry, but
// there is nothing to install for the compiler.
//
// The shape below is taken from Forms' own source, read from the sourcemaps it ships
// (src/section/forms/field-previews/manifests.ts), and from the designer element that consumes it
// (src/section/forms/workspace/views/design/components/forms-form-field.design.element.ts), which
// does:
//
//   const manifest = umbExtensionsRegistry.getByAlias(fieldType.previewView);
//   if (manifest) html`<umb-extension-slot
//       type="formsFieldPreview"
//       .filter=${(ext) => ext.alias === fieldType.previewView}
//       .props=${{ settings: field.settings, prevalues: this._fieldPrevalues }}>`;
//   return html``;
//
// Two consequences worth knowing, both of which shape how this package uses it:
//
//   * a field type's PreviewView must equal the alias registered here. When it does not match
//     anything, the designer renders *nothing* - no error, no placeholder - so a typo looks exactly
//     like "this field type has no preview". A test pins the two strings together.
//   * the element is handed `settings` and `prevalues` as properties, and nothing else. There is no
//     caption, no field id, and no way to reach the form.
import type { ManifestElement } from '@umbraco-cms/backoffice/extension-api';

export interface ManifestFormsFieldPreview extends ManifestElement {
    type: 'formsFieldPreview';
}

declare global {
    interface UmbExtensionManifestMap {
        integriousFormsFieldPreview: ManifestFormsFieldPreview;
    }
}
