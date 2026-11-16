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
  these are the back office bundle and the front end capture script. **No stylesheet** - see
  *Styling* below;
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

## Styling

The package ships no front end stylesheet, so the signature canvas renders at the browser default
of 300×150 until you style it. The markup the theme partial produces is:

```html
<div class="itg-signature-field">
    <div class="signature-pad-body">
        <canvas></canvas>
    </div>
    <div class="signature-pad-footer">
        <div class="signature-pad--actions">
            <div><button class="itg-signature-clear" data-action="clear">Clear</button></div>
        </div>
    </div>
    <input type="hidden" />
</div>
```

A reasonable starting point:

```css
.itg-signature-field .signature-pad-body { display: flex; flex-direction: column; }
.itg-signature-field .signature-pad-body canvas {
    border: 1px solid #ccc; width: 100%; max-width: 650px; height: 300px;
    background: #fff; margin-bottom: 5px;
}
.itg-signature-field .signature-pad-footer { display: flex; justify-content: flex-end; }
```

Note that `signature-pad-footer` is a **sibling** of `signature-pad-body`, not a child. Sizing the
canvas matters for more than looks: the script measures `offsetWidth`/`offsetHeight` to scale the
drawing surface for the device pixel ratio, so an unsized canvas captures at 300×150.

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

Client assets live in `src/Integrious.Umbraco.Forms.SignatureField/Client` and are built with npm
and Vite, following Umbraco's own `dotnet new umbraco-extension` layout. They are committed
nowhere — the build generates them into the project's `wwwroot`, which is gitignored.

```sh
cd src/Integrious.Umbraco.Forms.SignatureField/Client
npm ci
npm run build       # back office bundle and front end script
cd ../../..
dotnet build
dotnet test
```

There are two separate bundles, because they have opposite needs:

- **`src/backoffice`** → the property editor UI the Forms entry view uses. Everything under
  `@umbraco-cms` is left **external**, because those specifiers are in the back office's own
  importmap and the browser resolves them; bundling them would ship a second copy of Umbraco.
- **`src/frontend`** → the script the public form loads. Nothing is external here, because a
  visitor's browser has no importmap and `signature_pad` has to be bundled in.

Output names are fixed rather than hashed. The Web SDK globs `wwwroot` into static web assets at
evaluation time, before any target runs, so the set of files on disk has to stay stable between
builds.

`dotnet build` re-runs the client build for you when `Client/node_modules` is present; set
`SkipClientBuild=true` to suppress that. `dotnet pack` fails if the generated assets are missing,
rather than producing a package that installs cleanly and then does nothing.

## Licence

MIT — see [LICENSE](LICENSE).
