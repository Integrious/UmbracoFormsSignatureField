# Signature Field for Umbraco Forms

Adds a **Signature** field type to Umbraco Forms, letting someone sign a form with a mouse, finger
or stylus.

The signature is captured on an HTML canvas by
[signature_pad](https://github.com/szimek/signature_pad) and stored on the record as a JPEG data
URL. There is no file storage to configure and nothing to clean up: the value travels with the
record, so it renders anywhere the record does — the back office entry view, a workflow email, or
printed output.

## Installation

```sh
dotnet add package Integrious.Umbraco.Forms.SignatureField
```

Nothing else to do. The field appears in the Forms field type list as **Signature**, under the
Common category.

The package ships two things beyond the assembly:

- its `App_Plugins` assets, as static web assets, so there is nothing to copy into your `wwwroot`;
- `Views/partials/forms/themes/default/fieldtypes/FieldType.Signature.cshtml`, which is copied into
  your site on build. Umbraco Forms resolves theme partials from the file system rather than from
  an assembly, so this one has to exist as a physical file. Customise it in place if you want
  different markup — the copy only overwrites a file that is older than the packaged one.

## Compatibility

| Package | Umbraco CMS | Umbraco Forms |
| ------- | ----------- | ------------- |
| 2.0.x   | 17.x        | 17.x          |

The dependency range is capped at the next major deliberately. This field type uses a fair amount
of Umbraco Forms' own surface — `FieldType`, the record storage shape, and theme view resolution —
and that surface changed substantially between Forms 13 and 17. The range will be widened once it
has actually been tested against a newer major, not in anticipation of it.

## Displaying a submitted signature

The stored value is a complete data URL, so it can be used directly as an image source:

```html
<img src="@value" alt="Signature" />
```

In a back office extension, dispatch on the record schema's view name, which this field type sets
to `signature`:

```ts
if (column.view === 'signature') {
    // value is a data:image/jpeg;base64,... URL
}
```

Validate before rendering if the value could be anything other than one you wrote — it is, after
all, submitted by the public. Requiring the `data:image/` prefix is enough.

> **Note for anyone upgrading from a pre-release 1.0.x project reference:** `RenderView` used to
> return an AngularJS view path, which Forms 13 resolved to a view that produced the `img` tag for
> you. Forms 17 has no AngularJS and treats `RenderView` as a short name, so displaying the
> signature is now the consumer's job, as shown above.

## Building

Front end assets are built with Yarn and committed nowhere — they are generated into the project's
`wwwroot` and gitignored.

```sh
yarn install --immutable
yarn build          # sass + webpack
dotnet build
dotnet test
```

`dotnet build` re-runs `yarn build` for you when `node_modules` is present. `dotnet pack` fails if
the generated assets are missing, rather than producing a package that installs cleanly and then
does nothing.

## Licence

MIT — see [LICENSE](LICENSE).
