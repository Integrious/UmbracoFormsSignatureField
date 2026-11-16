/**
 * Everything this package contributes to the Umbraco back office.
 *
 * umbraco-package.json declares a single `bundle` extension pointing at the built file; Umbraco
 * imports it and registers whatever this array holds, so new extensions get added here rather than
 * to the json manifest.
 *
 * The alias below is the one Signature.SignatureEditView names on the server. Nothing but that
 * string joins the two, so a test pins them together.
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
];
