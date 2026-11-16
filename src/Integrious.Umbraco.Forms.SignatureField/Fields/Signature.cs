using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;

namespace Integrious.Umbraco.Forms.SignatureField.Fields;

/// <summary>
/// An Umbraco Forms field that captures a signature drawn on a canvas.
/// </summary>
/// <remarks>
/// The value is stored on the record as a complete JPEG data URL, so it needs no file storage and
/// renders anywhere the record does. See <see cref="RenderView"/> for how a consumer displays it.
/// </remarks>
public class Signature : FieldType
{
    /// <inheritdoc />
    /// <remarks>
    /// TODO: still an Umbraco Forms 13 AngularJS path, so this field has no design view in the v17
    /// form designer. Needs the v17 equivalent; tracked in the CHANGELOG under known issues.
    /// </remarks>
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

    /// <summary>
    /// Initialises the field type's identity and storage shape.
    /// </summary>
    /// <remarks>
    /// <c>Id</c> and <c>FieldTypeViewName</c> are both persisted in consuming sites' form
    /// definitions, so changing either detaches existing signature fields from this type. They are
    /// covered by tests for that reason.
    /// </remarks>
    public Signature()
    {
        Id = new Guid("4ec97b32-1801-4c84-acab-e541bee39c60");
        Name = "Signature";
        Description = "Displays a canvas to capture a signature";
        Icon = "icon-edit";
        SortOrder = 100;
        FieldTypeViewName = "FieldType.Signature.cshtml";
        DataType = FieldDataType.LongString;
        EditView = SignatureEditView;
    }

    /// <summary>
    /// The property editor UI the Umbraco Forms entry view uses to show a submitted signature.
    /// </summary>
    /// <remarks>
    /// Must match the <c>propertyEditorUi</c> alias registered in
    /// wwwroot/App_Plugins/UmbracoFormsSignatureField/umbraco-package.json - the two are joined by
    /// nothing but this string, so a test pins them together.
    ///
    /// Without it, Forms falls back to <c>Umb.PropertyEditorUi.Label</c>, which rendered the stored
    /// value as text: around 10KB of base64.
    ///
    /// This covers the entry view's edit mode only. Its read-only mode goes through
    /// EntryRenderHelper.getRenderValue, a hard coded switch over "date", "datetime", "number" and
    /// "file" with everything else falling through to raw text, and Umbraco Forms 17 offers no way
    /// to extend it.
    /// </remarks>
    public const string SignatureEditView = "Integrious.PropertyEditorUi.Signature";
}
