# Changelog

All notable changes to this project are documented here. The format is based on
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to
[Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [2.0.0] - 2026-11-19

First release published to NuGet. Earlier versions, up to the `v/1.0.4` tag, were consumed only as
a project reference from the one site that used them, so there is no 1.0.x package on nuget.org.

Supports Umbraco 17 and Umbraco Forms 17.

### Breaking

A major version rather than a first minor, because anything that was referencing the project
directly has to change:

- **The assembly is renamed** from `integrious.umbraco.forms.signaturefield` to
  `Integrious.Umbraco.Forms.SignatureField`. Any explicit reference to the old file name, or any
  `appsettings` entry naming the assembly, needs updating.
- **`RenderView` returns `"signature"` instead of a view path**, so displaying a submitted
  signature is now the consumer's job - Umbraco Forms 13 resolved the old path to a view that
  produced the `img` tag for you. See the README for what to do instead.
- **Umbraco Forms 13 is no longer supported.** This release targets `net10.0` and Forms 17.

### Added

- Published as `Integrious.Umbraco.Forms.SignatureField`, with the `buildTransitive` targets that
  copy the Forms theme partial into the consuming site and the `App_Plugins` assets shipped as
  static web assets.
- **A property editor UI, so the Forms entry view shows the signature rather than its stored
  value.** Umbraco Forms resolves a field type's `EditView` against the registered
  `propertyEditorUi` extensions and falls back to `Umb.PropertyEditorUi.Label` when it is unset,
  which rendered around 10KB of base64 as text. This package previously registered no back office
  extensions at all - it had no `umbraco-package.json`, and the only client-side files it shipped
  were two Umbraco Forms 13 AngularJS views that nothing loads.
  This covers the entry view's **edit mode** only. Its read-only mode goes through
  `EntryRenderHelper.getRenderValue`, a hard coded switch over `date`, `datetime`, `number` and
  `file` that falls through to raw text, and Forms 17 exposes no way to extend it.
- **A minimal default front end stylesheet**, included on any page with a signature field via
  `RequiredCssFiles`, and switched off with `Integrious:SignatureField:IncludeDefaultStylesheet`.
  One rule, sizing the canvas - which is not cosmetic, because the capture script scales the
  drawing surface from the canvas' measured size, so an unsized canvas captures at the browser
  default of 300x150. See *Styling* in the README for why the switch is site-wide rather than
  per-form, unlike Umbraco Forms' own `DisableDefaultStylesheet`.
- **A field preview, so the form designer shows a signature pad where the field sits.** Previously
  it showed nothing at all: the field named an Umbraco Forms 13 AngularJS design view, and v17 does
  not read design views - it reads `PreviewView`, which was unset, and renders an empty template
  when the alias matches no registered extension.
- A test project covering the field type's external contract: the field type id and view name
  (both persisted in consumers' form definitions), the data type, and that the script path the
  field declares matches where the build actually writes it.
- The pack now fails if the webpack and sass output is missing, rather than producing a package
  that installs cleanly and then does nothing.

### Changed

- **`RenderView` now returns `"signature"` instead of an AngularJS view path.** Umbraco Forms 13
  resolved it to `BackOffice/Common/RenderTypes/signature.html`, which rendered the stored value
  into an `img` tag. Forms 17 has no AngularJS and instead copies `RenderView` onto the record
  schema as the column's view, where consumers match on short names like `"date"` and `"file"`. A
  path matched nothing, so a submitted signature was rendered as text - around 10KB of base64.
- **The actions wrapper in the theme partial is now `signature-pad-actions`**, not
  `signature-pad--actions`. The double hyphen was a typo rather than a BEM modifier - nothing had
  ever selected on it, here or in the one site using the package.
- Assembly renamed from `integrious.umbraco.forms.signaturefield` to
  `Integrious.Umbraco.Forms.SignatureField`, matching the package id and namespace.
- Umbraco and Forms dependencies are now version ranges (`[17.0.0, 18.0.0)`) rather than exact
  pins, so a consuming site can resolve any 17.x.
- **The client toolchain moved from Yarn and webpack to npm, Vite and TypeScript**, matching
  Umbraco's own `dotnet new umbraco-extension` template: a `Client/` folder beside the project,
  `@umbraco-cms` left external for the back office bundle, and fixed rather than hashed output
  names so the static web assets glob stays stable between builds. Nothing a consumer installs
  changes shape; this is about being able to add back office components without inventing a build
  each time.

### Fixed

- The package version was `1.0.0` regardless of the intended version, because `Version` read
  `$(assemblyVersion)` on the line above where `AssemblyVersion` was defined.
- `package.json`, `package-lock.json` (88KB) and the sass/webpack sources were all shipped inside
  the package. The stale `package-lock.json` has been deleted - `yarn.lock` is authoritative.
- The `build` script chained its two steps with `&`, which runs them sequentially on Windows but
  backgrounds the first on Linux, so a CI build could pack a stylesheet that sass had not finished
  writing.

### Removed

- **The stylesheet, and the two Umbraco Forms 13 AngularJS views it styled.** That stylesheet was a
  *back office* one, injected by an `IManifestFilter` which the v17 port deleted along with the
  views - so nothing has loaded any of it since. Its selectors look like front end ones, but they
  targeted the v13 design view, whose markup happens to share class names with the theme partial.
  The back office styling it used to provide is now carried inside the property editor UI component,
  and the front end styling it never provided is now a deliberate, much smaller stylesheet - see
  *Added* above.
- `GetDesignView()` is no longer overridden. It named one of those deleted views, and the base
  implementation names an equally dead path inside Forms' own App_Plugins folder - neither is right
  in v17, and neither matters, because the designer does not read design views. Its replacement is
  the `PreviewView` and `formsFieldPreview` pair described under *Added*.
