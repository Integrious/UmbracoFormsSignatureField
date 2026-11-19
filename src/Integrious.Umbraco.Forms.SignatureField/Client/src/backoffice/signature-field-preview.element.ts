import { css, customElement, html } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';

const elementName = 'integrious-signature-field-preview';

/**
 * What the form designer shows where a signature field sits.
 *
 * Deliberately extends UmbLitElement rather than Umbraco Forms'
 * `FormsFieldPreviewBaseElement`, which the documentation suggests. That base class is three
 * members - `settings`, `prevalues` and a `getSettingValue` lookup - and reaching it means
 * importing from `@umbraco-forms/backoffice`, a specifier that exists only in the back office's
 * runtime importmap and has no types to compile against. This bundle also carries the signature
 * property editor UI, and an import the browser cannot resolve takes down the whole bundle rather
 * than the one extension in it, so the trade is a bad one for a class with no behaviour. The
 * contract that actually matters - the manifest type and the property names - is unaffected.
 *
 * The signature field has no settings, so this preview needs neither of those properties. The
 * designer assigns them anyway, which is harmless.
 */
@customElement(elementName)
export class IntegriousSignatureFieldPreviewElement extends UmbLitElement {
    override render() {
        // Not to scale: the real canvas is 300px tall (see the package's default stylesheet), which
        // would dwarf every other field in the designer. This is a recognisable likeness - a white
        // bordered pad with a signature on it - at a size that sits alongside a text box preview.
        //
        // aria-hidden because it is decorative: the field's caption, type and mandatory state are
        // already rendered around this element by the designer, and a screen reader gains nothing
        // from a drawing of a squiggle.
        return html`<div id="pad" aria-hidden="true">
            <svg viewBox="0 0 220 64" preserveAspectRatio="none">
                <path
                    d="M12 46 C 26 16, 36 54, 50 32 S 66 8, 80 36 C 90 54, 98 22, 112 32
                       S 128 50, 142 26 C 150 12, 158 40, 170 34"
                />
            </svg>
        </div>`;
    }

    static override readonly styles = [
        css`
            #pad {
                /* White rather than --uui-color-surface: a signature is captured on an opaque
                   white canvas, and the preview should not imply it follows the editor's theme. */
                background: #fff;
                border: 1px solid var(--uui-color-border);
                border-radius: var(--uui-border-radius);
                width: 220px;
                height: 64px;
            }

            svg {
                display: block;
                width: 100%;
                height: 100%;
            }

            path {
                fill: none;
                stroke: var(--uui-color-text);
                stroke-width: 2;
                stroke-linecap: round;
            }
        `,
    ];
}

export default IntegriousSignatureFieldPreviewElement;

declare global {
    interface HTMLElementTagNameMap {
        [elementName]: IntegriousSignatureFieldPreviewElement;
    }
}
