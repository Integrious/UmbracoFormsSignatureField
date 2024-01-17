import SignaturePad from 'signature_pad';

var signaturePad = (function () {
    'use strict';

    function init() {
        var signatureFields = document.querySelectorAll('.itg-signature-field');
        signatureFields.forEach(fieldWrapper => {
          var canvas = fieldWrapper.querySelector('canvas');
          var field = fieldWrapper.querySelector('input[type="hidden"]');
          var clearButton = fieldWrapper.querySelector('[data-action=clear]');
          var signaturePad = new SignaturePad(canvas);
          signaturePad.addEventListener('endStroke', () => {
              field.value = signaturePad.toSVG();
          });

          clearButton.addEventListener('click', function () {
              signaturePad.clear();
              field.value = "";
          });
        });
    }

    return {
        init: init
    };
}());
signaturePad.init();
