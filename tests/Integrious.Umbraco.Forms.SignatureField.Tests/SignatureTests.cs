using System.Text.Json;
using Integrious.Umbraco.Forms.SignatureField.Fields;
using Microsoft.Extensions.Options;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Xunit;

namespace Integrious.Umbraco.Forms.SignatureField.Tests;

/// <summary>
/// Tests for the field type's contract with the outside world.
/// </summary>
/// <remarks>
/// Deliberately not "tests that the constructor assigns fields". Every value asserted here is one
/// that something outside this assembly depends on, and that cannot be changed without breaking it:
/// a consuming back office, or a form definition already stored in a customer's database.
/// </remarks>
public class SignatureTests
{
    /// <summary>
    /// The view name a back office uses to decide how to display a submitted signature.
    /// </summary>
    /// <remarks>
    /// This is a regression test for a real bug. RenderView used to return the Umbraco Forms 13
    /// AngularJS view path, "~/App_Plugins/.../RenderTypes/signature.html". Forms 17 has no
    /// AngularJS: it copies a field type's RenderView straight onto the record schema as the
    /// column's view, and its own field types report short names there - "date", "file" - which
    /// consumers match on. A path matches nothing, so a submitted signature fell through to being
    /// rendered as text: about 10KB of base64 on screen.
    /// </remarks>
    [Fact]
    public void RenderView_is_the_short_name_a_v17_consumer_matches_on()
    {
        Assert.Equal("signature", CreateSignature().RenderView);
    }

    /// <summary>
    /// Guards the shape of the value, not just its current text. A path-shaped RenderView is the
    /// specific mistake that broke this before, and it would pass a test that only checked the
    /// value was non-empty.
    /// </summary>
    [Fact]
    public void RenderView_is_not_a_view_path()
    {
        var renderView = CreateSignature().RenderView;

        Assert.DoesNotContain("/", renderView);
        Assert.DoesNotContain("~", renderView);
        Assert.DoesNotContain(".html", renderView);
    }

    /// <summary>
    /// The field type id is written into every form definition that uses this field, and is stored
    /// in the consuming site's database. Changing it silently detaches every existing signature
    /// field from its type, so it is a breaking change rather than a refactor.
    /// </summary>
    [Fact]
    public void Id_is_unchanged()
    {
        Assert.Equal(new Guid("4ec97b32-1801-4c84-acab-e541bee39c60"), CreateSignature().Id);
    }

    /// <summary>
    /// Also persisted per field, and resolved by Umbraco Forms at render time by file name, so a
    /// rename breaks existing forms rather than this assembly.
    /// </summary>
    [Fact]
    public void FieldTypeViewName_is_unchanged()
    {
        Assert.Equal("FieldType.Signature.cshtml", CreateSignature().FieldTypeViewName);
    }

    /// <summary>
    /// The stored value is a JPEG data URL of roughly 10KB, so the column has to be a long string.
    /// Narrowing this would truncate signatures on save.
    /// </summary>
    [Fact]
    public void DataType_is_LongString()
    {
        Assert.Equal(FieldDataType.LongString, CreateSignature().DataType);
    }

    /// <summary>
    /// The script path the field asks Umbraco Forms to include on the front end has to match where
    /// the build actually puts the file, and that file is Vite output. The two have no compiler
    /// relationship, so nothing else would catch them drifting apart - including renaming the Vite
    /// entry, which is configured in a file C# never sees.
    /// </summary>
    [Fact]
    public void RequiredJavascriptFiles_points_at_a_file_the_build_actually_produces()
    {
        var declared = CreateSignature().RequiredJavascriptFiles(new Field());

        var path = Assert.Single(declared);
        Assert.Equal("~/App_Plugins/UmbracoFormsSignatureField/FrontEnd/signatureField.js", path);

        // Resolve the declared virtual path against the project's wwwroot and check it exists.
        // Skipped when the asset has not been generated, because the client build is a separate
        // step and a missing asset is the pack guard's job to report, not this test's.
        var onDisk = Path.Combine(
            ProjectRoot(),
            "wwwroot",
            path.Replace("~/", string.Empty).Replace('/', Path.DirectorySeparatorChar));

        Assert.SkipUnless(
            Directory.Exists(Path.GetDirectoryName(onDisk)!),
            "Front end assets have not been built; run `npm run build` in Client/.");

        Assert.True(File.Exists(onDisk), $"The declared script path does not exist on disk: {onDisk}");
    }

    /// <summary>
    /// The field type's EditView has to name a property editor UI that the client bundle actually
    /// registers, or Umbraco Forms silently falls back to Umb.PropertyEditorUi.Label and the entry
    /// view shows 10KB of base64 instead of the signature.
    /// </summary>
    /// <remarks>
    /// Nothing but this string joins the C# to the TypeScript - no compiler, no schema - so the
    /// two are pinned together here. This is the same class of break as the RenderView path above:
    /// it fails silently and looks like a styling problem.
    /// </remarks>
    [Fact]
    public void EditView_names_the_property_editor_ui_the_client_bundle_registers()
    {
        Assert.Equal(Fields.Signature.SignatureEditView, CreateSignature().EditView);

        var manifestSource = Path.Combine(ClientRoot(), "src", "backoffice", "index.ts");
        Assert.True(File.Exists(manifestSource), $"The client manifest source is missing: {manifestSource}");

        Assert.Contains(
            $"'{Fields.Signature.SignatureEditView}'",
            File.ReadAllText(manifestSource));
    }

    /// <summary>
    /// The package manifest points at the built bundle by path, and a package ships whatever is on
    /// disk - so a renamed entry or an unbuilt client would register a bundle that cannot load,
    /// taking every extension in it with it.
    /// </summary>
    [Fact]
    public void The_package_manifest_points_at_the_bundle_the_client_build_produces()
    {
        var manifestPath = Path.Combine(
            ProjectRoot(), "wwwroot", "App_Plugins", "UmbracoFormsSignatureField", "umbraco-package.json");
        Assert.True(File.Exists(manifestPath), $"The package manifest is missing: {manifestPath}");

        using var manifest = JsonDocument.Parse(File.ReadAllText(manifestPath));
        var bundle = manifest.RootElement
            .GetProperty("extensions")
            .EnumerateArray()
            .Single(e => e.GetProperty("type").GetString() == "bundle")
            .GetProperty("js")
            .GetString()!;

        // The vite config names this entry explicitly, so the two have to agree.
        var viteConfig = File.ReadAllText(Path.Combine(ClientRoot(), "vite.config.backoffice.ts"));
        Assert.Contains($"'{Path.GetFileName(bundle)}'", viteConfig);

        // The manifest path is site-absolute (/App_Plugins/...); resolve it against the project's
        // wwwroot, which is where the build puts it. Skipped when the client has not been built,
        // because that is the pack guard's job to report, not this test's.
        var onDisk = Path.Combine(
            ProjectRoot(), "wwwroot", bundle.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

        Assert.SkipUnless(
            Directory.Exists(Path.GetDirectoryName(onDisk)!),
            "The client has not been built; run `npm run build` in Client/.");
        Assert.True(File.Exists(onDisk), $"The bundle the manifest registers does not exist: {onDisk}");
    }

    /// <summary>
    /// The default stylesheet is included by default, and names a file the build actually produces.
    /// </summary>
    /// <remarks>
    /// The same joint as the script path above, with the same absence of any compiler relationship:
    /// the C# names signatureField.css and the Vite config's <c>cssFileName</c> decides what the
    /// build writes. A mismatch is a 404 on every form page, which is easy to miss because the
    /// field still works - it just captures at the browser's default 300x150.
    /// </remarks>
    [Fact]
    public void RequiredCssFiles_defaults_to_a_stylesheet_the_build_actually_produces()
    {
        var declared = CreateSignature().RequiredCssFiles(new Field());

        var path = Assert.Single(declared);
        Assert.Equal("~/App_Plugins/UmbracoFormsSignatureField/FrontEnd/signatureField.css", path);
        Assert.Equal(Fields.Signature.DefaultStylesheet, path);

        var viteConfig = File.ReadAllText(Path.Combine(ClientRoot(), "vite.config.frontend.ts"));
        Assert.Contains(
            $"cssFileName: '{Path.GetFileNameWithoutExtension(path)}'",
            viteConfig,
            StringComparison.Ordinal);

        var onDisk = Path.Combine(
            ProjectRoot(),
            "wwwroot",
            path.Replace("~/", string.Empty).Replace('/', Path.DirectorySeparatorChar));

        Assert.SkipUnless(
            Directory.Exists(Path.GetDirectoryName(onDisk)!),
            "Front end assets have not been built; run `npm run build` in Client/.");

        Assert.True(File.Exists(onDisk), $"The declared stylesheet does not exist on disk: {onDisk}");
    }

    /// <summary>
    /// The stylesheet has to style the markup the shipped theme partial actually produces. These
    /// two files have no relationship a compiler can check, and the package has already shipped a
    /// stylesheet whose selectors matched a different view entirely - nesting the footer inside the
    /// body, and naming a canvas class the partial does not use - which is how it came to be
    /// believed broken when it had simply never been loaded.
    /// </summary>
    [Fact]
    public void The_default_stylesheet_selects_the_markup_the_theme_partial_renders()
    {
        var css = File.ReadAllText(
            Path.Combine(ClientRoot(), "src", "frontend", "signature-field.css"));
        var partial = File.ReadAllText(Path.Combine(
            ProjectRoot(),
            "Views", "partials", "forms", "themes", "default", "fieldtypes",
            "FieldType.Signature.cshtml"));

        // Every class the stylesheet selects on, which must appear in the partial. Checked this way
        // round deliberately: the partial may carry classes for a site to hook, but a selector with
        // nothing to match is dead weight at best and a wrong assumption at worst.
        foreach (var className in new[] { "itg-signature-field", "signature-pad-body" })
        {
            Assert.Contains($".{className}", css, StringComparison.Ordinal);
            Assert.Contains($"\"{className}\"", partial, StringComparison.Ordinal);
        }

        // The double-hyphen form was the class name until it was corrected; nothing should resurrect
        // it, in either file.
        Assert.DoesNotContain("signature-pad--actions", css, StringComparison.Ordinal);
        Assert.DoesNotContain("signature-pad--actions", partial, StringComparison.Ordinal);
    }

    /// <summary>
    /// Turning the option off has to produce no stylesheet at all, not an empty-stringed one: the
    /// point of the switch is that a site styling the canvas itself gets nothing to override.
    /// </summary>
    [Fact]
    public void RequiredCssFiles_is_empty_when_the_default_stylesheet_is_turned_off()
    {
        Assert.Empty(CreateSignature(includeDefaultStylesheet: false).RequiredCssFiles(new Field()));
    }

    /// <summary>
    /// Builds the field type the way the container does. There is no parameterless constructor by
    /// design - see the remarks on <see cref="Fields.Signature(IOptions{SignatureFieldOptions})"/>.
    /// </summary>
    private static Signature CreateSignature(bool includeDefaultStylesheet = true) =>
        new(Options.Create(new SignatureFieldOptions
        {
            IncludeDefaultStylesheet = includeDefaultStylesheet,
        }));

    /// <summary>The client build's source directory.</summary>
    private static string ClientRoot() => Path.Combine(ProjectRoot(), "Client");

    /// <summary>Walks up from the test binaries to the project being tested.</summary>
    private static string ProjectRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !Directory.Exists(Path.Combine(dir.FullName, "src")))
        {
            dir = dir.Parent;
        }

        Assert.NotNull(dir);
        return Path.Combine(dir.FullName, "src", "Integrious.Umbraco.Forms.SignatureField");
    }
}
