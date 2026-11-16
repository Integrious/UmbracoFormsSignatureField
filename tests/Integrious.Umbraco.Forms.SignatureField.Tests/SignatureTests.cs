using System.Text.Json;
using Integrious.Umbraco.Forms.SignatureField.Fields;
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
        Assert.Equal("signature", new Signature().RenderView);
    }

    /// <summary>
    /// Guards the shape of the value, not just its current text. A path-shaped RenderView is the
    /// specific mistake that broke this before, and it would pass a test that only checked the
    /// value was non-empty.
    /// </summary>
    [Fact]
    public void RenderView_is_not_a_view_path()
    {
        var renderView = new Signature().RenderView;

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
        Assert.Equal(new Guid("4ec97b32-1801-4c84-acab-e541bee39c60"), new Signature().Id);
    }

    /// <summary>
    /// Also persisted per field, and resolved by Umbraco Forms at render time by file name, so a
    /// rename breaks existing forms rather than this assembly.
    /// </summary>
    [Fact]
    public void FieldTypeViewName_is_unchanged()
    {
        Assert.Equal("FieldType.Signature.cshtml", new Signature().FieldTypeViewName);
    }

    /// <summary>
    /// The stored value is a JPEG data URL of roughly 10KB, so the column has to be a long string.
    /// Narrowing this would truncate signatures on save.
    /// </summary>
    [Fact]
    public void DataType_is_LongString()
    {
        Assert.Equal(FieldDataType.LongString, new Signature().DataType);
    }

    /// <summary>
    /// The script path the field asks Umbraco Forms to include on the front end has to match where
    /// the build actually puts the file, and that file is webpack output. The two have no compiler
    /// relationship, so nothing else would catch them drifting apart - including moving the webpack
    /// output directory, which is configured in a different repository root file.
    /// </summary>
    [Fact]
    public void RequiredJavascriptFiles_points_at_a_file_the_build_actually_produces()
    {
        var declared = new Signature().RequiredJavascriptFiles(new Field());

        var path = Assert.Single(declared);
        Assert.Equal("~/App_Plugins/UmbracoFormsSignatureField/FrontEnd/signatureField.js", path);

        // Resolve the declared virtual path against the project's wwwroot and check it exists.
        // Skipped when the asset has not been generated, because `yarn build` is a separate step
        // and a missing asset is the pack guard's job to report, not this test's.
        var onDisk = Path.Combine(
            ProjectRoot(),
            "wwwroot",
            path.Replace("~/", string.Empty).Replace('/', Path.DirectorySeparatorChar));

        Assert.SkipUnless(
            Directory.Exists(Path.GetDirectoryName(onDisk)!),
            "Front end assets have not been built; run `yarn build` at the repository root.");

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
        Assert.Equal(Fields.Signature.SignatureEditView, new Signature().EditView);

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
