import { css, customElement, html, LitElement, nothing, property } from '@umbraco-cms/backoffice/external/lit';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

/**
 * A stored signature as an image source, or null if the value is not one.
 *
 * The signature field stores what `canvas.toDataURL('image/jpeg', 0.7)` returns, so the value is
 * already a complete data URL and needs no wrapping.
 *
 * The `data:image/` prefix is required rather than assumed. These are values submitted by the
 * public, and while a `data:text/html` value cannot execute as a document in `<img src>`, a value
 * that is not an image has no business being handed to one: anything else falls through to being
 * shown as text, which is safe and more use for working out what went wrong.
 */
export function signatureSrc(value: unknown): string | null {
    const text = typeof value === 'string' ? value : '';
    return /^data:image\/[a-z0-9.+-]+;base64,[A-Za-z0-9+/=]+$/i.test(text) ? text : null;
}

/**
 * Shows a submitted signature in the Umbraco Forms entry view.
 *
 * Umbraco Forms resolves a field type's EditView against the registered propertyEditorUi
 * extensions, falling back to Umb.PropertyEditorUi.Label when it is unset - which rendered the
 * stored value as text, around 10KB of base64.
 *
 * Deliberately offers no editing. A signature is evidence of what someone signed, not a value to
 * retype, and there is no sensible way to re-draw one here. The value is held and handed back
 * untouched so that saving an entry cannot blank a signature.
 */
@customElement('itg-signature-property-editor-ui')
export class IntegriousSignaturePropertyEditorUiElement
    extends LitElement
    implements UmbPropertyEditorUiElement
{
    @property({ attribute: false })
    value: unknown = '';

    @property({ type: Boolean, reflect: true })
    readonly = false;

    override render() {
        if (!this.value) {
            return nothing;
        }

        const src = signatureSrc(this.value);

        return src
            ? html`<img src=${src} alt="Signature" />`
            : html`<span class="not-a-signature">${String(this.value)}</span>`;
    }

    static override styles = css`
        /*
         * Capped because the signature is drawn on a canvas as wide as the form was. The border
         * matters: the strokes are dark on transparent, so without it there is nothing to show
         * that an empty looking box is a signature that did not capture.
         */
        img {
            display: block;
            max-width: 320px;
            height: auto;
            background: #fff;
            border: 1px solid var(--uui-color-border);
            border-radius: 3px;
        }

        .not-a-signature {
            word-break: break-all;
            color: var(--uui-color-text-alt);
        }
    `;
}

export default IntegriousSignaturePropertyEditorUiElement;

declare global {
    interface HTMLElementTagNameMap {
        'itg-signature-property-editor-ui': IntegriousSignaturePropertyEditorUiElement;
    }
}
