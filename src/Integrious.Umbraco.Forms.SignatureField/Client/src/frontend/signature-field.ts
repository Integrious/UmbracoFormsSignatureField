import SignaturePad from 'signature_pad';

/**
 * Wires up every signature field on the page.
 *
 * Umbraco Forms includes this via Signature.RequiredJavascriptFiles, once per page containing a
 * form with a signature field. The markup it expects is in
 * Views/partials/forms/themes/default/fieldtypes/FieldType.Signature.cshtml.
 *
 * The stored value is `canvas.toDataURL('image/jpeg', 0.7)` - a complete data URL, which is what
 * lets the back office render it straight into an `img` tag with no file storage involved.
 */
function init(): void {
    document.querySelectorAll<HTMLElement>('.itg-signature-field').forEach((wrapper) => {
        const canvas = wrapper.querySelector('canvas');
        const field = wrapper.querySelector<HTMLInputElement>('input[type="hidden"]');
        const clearButton = wrapper.querySelector('[data-action=clear]');

        // A form author can customise the theme partial, so none of these is guaranteed. Skipping
        // a malformed field is better than throwing and leaving the rest of the page's fields
        // unwired.
        if (!canvas || !field) {
            return;
        }

        const signaturePad = new SignaturePad(canvas, { backgroundColor: 'rgb(255,255,255)' });

        signaturePad.addEventListener('endStroke', () => {
            field.value = signaturePad.toDataURL('image/jpeg', 0.7);
        });

        clearButton?.addEventListener('click', () => {
            signaturePad.clear();
            field.value = '';
        });

        const resizeCanvas = () => {
            const ratio = Math.max(window.devicePixelRatio || 1, 1);
            canvas.width = canvas.offsetWidth * ratio;
            canvas.height = canvas.offsetHeight * ratio;
            canvas.getContext('2d')?.scale(ratio, ratio);
            // The browser clears the canvas when it is resized, so reload the existing signature.
            // If the canvas is now smaller, not all of it will fit.
            signaturePad.fromData(signaturePad.toData());
        };

        window.addEventListener('resize', resizeCanvas);
        resizeCanvas();
    });
}

init();
