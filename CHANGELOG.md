# Changelog

All notable changes to this project are documented here. The format is based on
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to
[Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.1.0] - 2026-11-09

First release published to NuGet. Earlier versions, up to the `v/1.0.4` tag, were consumed only as
a project reference from the one site that used them, so there is no 1.0.x package on nuget.org.

Supports Umbraco 17 and Umbraco Forms 17.

### Added

- Published as `Integrious.Umbraco.Forms.SignatureField`, with the `buildTransitive` targets that
  copy the Forms theme partial into the consuming site and the `App_Plugins` assets shipped as
  static web assets.
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
- Assembly renamed from `integrious.umbraco.forms.signaturefield` to
  `Integrious.Umbraco.Forms.SignatureField`, matching the package id and namespace.
- Umbraco and Forms dependencies are now version ranges (`[17.0.0, 18.0.0)`) rather than exact
  pins, so a consuming site can resolve any 17.x.

### Fixed

- The package version was `1.0.0` regardless of the intended version, because `Version` read
  `$(assemblyVersion)` on the line above where `AssemblyVersion` was defined.
- `package.json`, `package-lock.json` (88KB) and the sass/webpack sources were all shipped inside
  the package. The stale `package-lock.json` has been deleted - `yarn.lock` is authoritative.
- The `build` script chained its two steps with `&`, which runs them sequentially on Windows but
  backgrounds the first on Linux, so a CI build could pack a stylesheet that sass had not finished
  writing.

### Known issues

- `GetDesignView()` still returns an Umbraco Forms 13 AngularJS path, so the field has no design
  view in the v17 form designer. Unchanged by this release; it needs the v17 equivalent, which is
  a separate piece of work.
