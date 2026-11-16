/**
 * Renders a submitted signature in the Umbraco Forms entry view.
 *
 * Umbraco Forms resolves a field type's EditView against the registered propertyEditorUi
 * extensions, so this is what the entry view shows for a signature instead of the
 * Umb.PropertyEditorUi.Label fallback - which rendered the stored value as text, around 10KB of
 * base64.
 *
 * Deliberately a plain custom element with no imports. UmbPropertyEditorUiElement is just an
 * HTMLElement with a `value` property, so nothing here needs Lit, and this package would otherwise
 * have to grow a bundler configured to treat @umbraco-cms/* as external - a lot of machinery to
 * draw one image.
 *
 * It does not offer editing. A signature is evidence of what someone signed, not a value to retype,
 * and there is no sensible way to re-draw one here. The value is held and handed back untouched so
 * that saving an entry in edit mode preserves it.
 */
class IntegriousSignaturePropertyEditorUi extends HTMLElement {
    #value;

    static get observedAttributes() {
        return ['readonly'];
    }

    set value(value) {
        this.#value = value;
        this.#render();
    }

    /** Returned unchanged, so saving an entry does not blank the signature. */
    get value() {
        return this.#value;
    }

    connectedCallback() {
        this.#render();
    }

    /**
     * Whether a stored value is safe to use as an image source.
     *
     * These are values submitted by the public, so the `data:image/` prefix is required rather than
     * assumed. A value that is not an image falls through to being shown as text, which is both
     * safe and more use for working out what went wrong than an empty box.
     */
    static #imageSource(value) {
        const text = typeof value === 'string' ? value : '';
        return /^data:image\/[a-z0-9.+-]+;base64,[A-Za-z0-9+/=]+$/i.test(text) ? text : null;
    }

    #render() {
        if (!this.#value) {
            this.replaceChildren();
            return;
        }

        const source = IntegriousSignaturePropertyEditorUi.#imageSource(this.#value);

        if (!source) {
            const fallback = document.createElement('span');
            fallback.textContent = String(this.#value);
            fallback.style.cssText = 'word-break:break-all;color:var(--uui-color-text-alt);';
            this.replaceChildren(fallback);
            return;
        }

        const image = document.createElement('img');
        image.src = source;
        image.alt = 'Signature';
        // Capped because the signature is drawn on a canvas as wide as the form was. The border
        // matters: the strokes are dark on transparent, so without it there is nothing to show
        // that an empty-looking box is a signature that did not capture.
        image.style.cssText =
            'display:block;max-width:320px;height:auto;background:#fff;' +
            'border:1px solid var(--uui-color-border);border-radius:3px;';

        this.replaceChildren(image);
    }
}

customElements.define('itg-signature-property-editor-ui', IntegriousSignaturePropertyEditorUi);

export default IntegriousSignaturePropertyEditorUi;
