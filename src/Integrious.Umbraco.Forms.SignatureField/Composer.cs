using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Forms.Core.Providers;

namespace Integrious.Umbraco.Forms.SignatureField;

/// <summary>
/// Registers the signature field type with Umbraco Forms, and binds its options.
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
        ArgumentNullException.ThrowIfNull(builder);

        // No validator, unlike the sane database availability check: a single bool has no invalid
        // value to reject. Binding alone means an absent section yields the defaults.
        builder.Services.AddOptions<SignatureFieldOptions>()
            .Bind(builder.Config.GetSection(SignatureFieldOptions.SectionName));

        builder.WithCollectionBuilder<FieldCollectionBuilder>()
            .Add<Fields.Signature>();
    }
}
