import SignaturePad from 'signature_pad';

var signaturePad = (function () {
    'use strict';

    function init() {
        var signatureFields = document.querySelectorAll('.itg-signature-field');
        signatureFields.forEach(fieldWrapper => {
          var canvas = fieldWrapper.querySelector('canvas');
          var field = fieldWrapper.querySelector('input[type="hidden"]');
          var clearButton = fieldWrapper.querySelector('[data-action=clear]');
          var signaturePad = new SignaturePad(canvas, {
            backgroundColor: 'rgb(255,255,255)'
          });
          signaturePad.addEventListener('endStroke', () => {
              field.value = signaturePad.toDataURL('image/jpeg', 0.7);
          });

          clearButton.addEventListener('click', function () {
              signaturePad.clear();
              field.value = "";
          });

          function resizeCanvas() {
            const ratio =  Math.max(window.devicePixelRatio || 1, 1);
            canvas.width = canvas.offsetWidth * ratio;
            canvas.height = canvas.offsetHeight * ratio;
            canvas.getContext("2d").scale(ratio, ratio);
            // Browser will clear the canvas when it is resized, so reload the existing
            // signature. Note that if the canvas is now smaller this may not all fit!
            signaturePad.fromData(signaturePad.toData());
          }

          window.addEventListener("resize", resizeCanvas);
          resizeCanvas();
        });
    }

    return {
        init: init
    };
}());
signaturePad.init();
