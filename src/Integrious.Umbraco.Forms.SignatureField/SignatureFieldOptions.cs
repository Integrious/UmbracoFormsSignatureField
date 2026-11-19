namespace Integrious.Umbraco.Forms.SignatureField;

/// <summary>
/// Configures the signature field type.
/// </summary>
/// <remarks>
/// Bound from the <c>Integrious:SignatureField</c> section of <c>appsettings.json</c> by
/// <see cref="Composer"/>. A site that configures nothing gets the defaults here.
/// </remarks>
public sealed class SignatureFieldOptions
{
    /// <summary>
    /// The configuration section these options are bound from.
    /// </summary>
    public const string SectionName = "Integrious:SignatureField";

    /// <summary>
    /// Gets or sets a value indicating whether the package's default front end stylesheet is
    /// included on pages containing a signature field. Defaults to <c>true</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Set to <c>false</c> on a site that styles the signature canvas itself, which is the usual
    /// case once a form has a designed theme. Nothing else in the package is affected: the field
    /// still works, it just renders at the browser's default canvas size until something sizes it.
    /// </para>
    /// <para>
    /// This is a site-wide switch rather than a per-form one, which is a deliberate mismatch with
    /// Umbraco Forms' own <c>Form.DisableDefaultStylesheet</c>. Honouring that would be the better
    /// fit, but <c>FieldType.RequiredCssFiles</c> is handed only the <c>Field</c>, and a field has
    /// no back-reference to its form, so the flag cannot be reached from where the decision has to
    /// be made.
    /// </para>
    /// </remarks>
    public bool IncludeDefaultStylesheet { get; set; } = true;
}
