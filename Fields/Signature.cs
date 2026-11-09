using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;

namespace Integrious.Umbraco.Forms.SignatureField.Fields;

public class Signature : FieldType
{
    /// <inheritdoc />
    public override string GetDesignView() =>
        "~/App_Plugins/UmbracoFormsSignatureField/BackOffice/Common/FieldTypes/signature.html";

    /// <summary>
    /// The name the back office uses to decide how to display a submitted signature.
    /// </summary>
    /// <remarks>
    /// A short name, not a path. Umbraco Forms 13 resolved this to an AngularJS view - here
    /// BackOffice/Common/RenderTypes/signature.html, which rendered the stored value straight into
    /// an <c>img</c> tag. Forms 17 has no AngularJS, and its own field types now report short names
    /// ("date", "file") which the back office matches on; a path matches nothing, so the signature
    /// fell through to being rendered as text - about 10KB of base64 on screen.
    ///
    /// The stored value is a complete data URL, because the front end saves
    /// <c>canvas.toDataURL("image/jpeg", 0.7)</c>, so a consumer can use it as an image source
    /// directly.
    /// </remarks>
    public override string RenderView => "signature";

    /// <inheritdoc />
    public override IEnumerable<string> RequiredJavascriptFiles(Field field) => new[]
        { "~/App_Plugins/UmbracoFormsSignatureField/FrontEnd/signatureField.js" };

    public Signature()
    {
        Id = new Guid("4ec97b32-1801-4c84-acab-e541bee39c60");
        Name = "Signature";
        Description = "Displays a canvas to capture a signature";
        Icon = "icon-edit";
        SortOrder = 100;
        FieldTypeViewName = "FieldType.Signature.cshtml";
        DataType = FieldDataType.LongString;
    }
}
