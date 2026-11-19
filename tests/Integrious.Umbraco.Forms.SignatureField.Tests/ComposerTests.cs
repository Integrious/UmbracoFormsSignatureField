using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Providers;
using Xunit;

namespace Integrious.Umbraco.Forms.SignatureField.Tests;

/// <summary>
/// Tests that the field type is reachable through the container, configured as documented.
/// </summary>
/// <remarks>
/// These cover the one thing a unit test on the field type cannot: that Umbraco Forms can actually
/// build it. The field type takes a constructor dependency, and the repeating fields plugin carries
/// a comment claiming dependency injection "stops the site starting" - which turned out to be about
/// a circular dependency on IFieldTypeStorage, not a general rule. Umbraco Forms' own DatePicker and
/// Password field types both take IOptions. Resolving ours from a real container here is what makes
/// that evidence rather than reasoning, without booting a site.
/// </remarks>
public class ComposerTests
{
    /// <summary>
    /// The field type can be constructed by the container, with configuration bound.
    /// </summary>
    [Fact]
    public void Compose_registers_a_field_type_the_container_can_build()
    {
        var signature = Compose().GetRequiredService<Fields.Signature>();

        Assert.Equal(
            Fields.Signature.DefaultStylesheet,
            Assert.Single(signature.RequiredCssFiles(new Field())));
    }

    /// <summary>
    /// The section path is a documented contract - it is in the README and in a consuming site's
    /// appsettings.json - so a rename here is a breaking change rather than a refactor.
    /// </summary>
    [Fact]
    public void Compose_binds_from_the_documented_appsettings_section()
    {
        var provider = Compose(new Dictionary<string, string?>
        {
            ["Integrious:SignatureField:IncludeDefaultStylesheet"] = "false",
        });

        Assert.Empty(provider.GetRequiredService<Fields.Signature>().RequiredCssFiles(new Field()));
    }

    /// <summary>
    /// An absent section has to leave the stylesheet on, which is what makes the package work on a
    /// site that installs it and configures nothing.
    /// </summary>
    [Fact]
    public void Compose_with_no_configuration_includes_the_default_stylesheet()
    {
        var provider = Compose();

        Assert.True(
            provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<SignatureFieldOptions>>()
                .Value.IncludeDefaultStylesheet);
    }

    /// <summary>
    /// Builds the container the way Umbraco would: compose, then let the collection builder register
    /// the types it was given.
    /// </summary>
    private static ServiceProvider Compose(Dictionary<string, string?>? configuration = null)
    {
        var services = new ServiceCollection();

        var configurationRoot = new ConfigurationBuilder()
            .AddInMemoryCollection(configuration ?? [])
            .Build();

        // Umbraco hands composers a real builder whose collection builders are realised during
        // Build(). Substituting the builder and realising the one collection we care about keeps
        // this a unit test while still exercising the registration itself.
        var fields = new FieldCollectionBuilder();
        var builder = Substitute.For<IUmbracoBuilder>();
        builder.Services.Returns(services);
        builder.Config.Returns(configurationRoot);
        builder.WithCollectionBuilder<FieldCollectionBuilder>().Returns(fields);

        new Composer().Compose(builder);

        ((ICollectionBuilder)fields).RegisterWith(services);

        return services.BuildServiceProvider();
    }
}
