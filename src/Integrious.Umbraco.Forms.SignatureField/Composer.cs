using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Forms.Core.Providers;

namespace Integrious.Umbraco.Forms.SignatureField;

/// <summary>
/// Registers the signature field type with Umbraco Forms.
/// </summary>
/// <remarks>
/// Discovered automatically by Umbraco's type scanning, so a consuming site needs no startup code
/// of its own.
/// </remarks>
public class Composer : IComposer
{
    /// <inheritdoc />
    public void Compose(IUmbracoBuilder builder)
    {
        builder.WithCollectionBuilder<FieldCollectionBuilder>()
            .Add<Fields.Signature>();
    }
}
