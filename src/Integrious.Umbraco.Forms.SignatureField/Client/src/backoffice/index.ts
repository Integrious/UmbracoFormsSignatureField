/**
 * Everything this package contributes to the Umbraco back office.
 *
 * umbraco-package.json declares a single `bundle` extension pointing at the built file; Umbraco
 * imports it and registers whatever this array holds, so new extensions get added here rather than
 * to the json manifest.
 *
 * The aliases below are the ones Signature.SignatureEditView and Signature.SignaturePreviewView
 * name on the server. Nothing but those strings joins the two sides, so tests pin them together.
 */
export const manifests: Array<UmbExtensionManifest> = [
    {
        type: 'propertyEditorUi',
        alias: 'Integrious.PropertyEditorUi.Signature',
        name: 'Signature',
        element: () => import('./signature-property-editor-ui.element.js'),
        meta: {
            label: 'Signature',
            icon: 'icon-edit',
            group: 'Forms',
            supportsReadOnly: true,
        },
    },
    {
        type: 'formsFieldPreview',
        alias: 'Integrious.FormsFieldPreview.Signature',
        name: 'Signature Field Preview',
        element: () => import('./signature-field-preview.element.js'),
    },
];
