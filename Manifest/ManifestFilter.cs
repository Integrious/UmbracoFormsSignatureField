using Umbraco.Cms.Core.Manifest;

namespace Integrious.Umbraco.Forms.SignatureField.Manifest;

public class ManifestFilter : IManifestFilter
{
    public void Filter(List<PackageManifest> manifests)
    {
        manifests.Add(new PackageManifest
        {
            PackageName = "Umbraco Forms Signature Field",
            Version = "1.0.4",
            AllowPackageTelemetry = true,
            PackageId = "39248ea3-22b0-4804-ace6-e90fcf3304e8",
            Stylesheets = new [] {"/App_Plugins/UmbracoFormsSignatureField/BackOffice/Css/signature.css"}
        });
    }
}
