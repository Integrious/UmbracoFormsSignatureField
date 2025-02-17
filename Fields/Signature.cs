using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;

namespace Integrious.Umbraco.Forms.SignatureField.Fields;

public class Signature : FieldType
{
    /// <inheritdoc />
    public override string GetDesignView() =>
        "~/App_Plugins/UmbracoFormsSignatureField/BackOffice/Common/FieldTypes/signature.html";

    /// <inheritdoc />
    public override string RenderView => "~/App_Plugins/UmbracoFormsSignatureField/BackOffice/Common/RenderTypes/signature.html";

    /// <inheritdoc />
    public override IEnumerable<string> RequiredJavascriptFiles(Field field) => new[]
        { "~/App_Plugins/UmbracoFormsSignatureField/FrontEnd/signatureField.js?v=1.0.3" };

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
