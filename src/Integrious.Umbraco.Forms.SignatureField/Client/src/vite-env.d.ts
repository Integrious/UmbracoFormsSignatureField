/// <reference types="vite/client" />

// Declares the non-TypeScript module types Vite understands, which is what lets
// `import './signature-field.css'` typecheck. `tsc` runs before `vite build`, so without this the
// build fails on the CSS import rather than at bundle time.
