using Integrious.Umbraco.Forms.SignatureField.Manifest;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Forms.Core.Providers;

namespace Integrious.Umbraco.Forms.SignatureField;

public class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.ManifestFilters().Append<ManifestFilter>();
        builder.WithCollectionBuilder<FieldCollectionBuilder>()
            .Add<Fields.Signature>();
    }
}
