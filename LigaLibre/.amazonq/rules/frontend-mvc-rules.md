# Reglas Front-End MVC - Liga Libre

## Patrón MVC ASP.NET Core

### Estructura de Carpetas
- **Controllers/**: Controladores MVC (sufijo `Controller`)
- **Views/**: Vistas Razor organizadas por controlador
- **Models/**: ViewModels específicos para vistas
- **wwwroot/**: Archivos estáticos (css, js, images)

### Controllers
- Heredar de `Controller`
- Métodos retornan `IActionResult` o derivados
- Usar `[HttpGet]`, `[HttpPost]`, etc.
- Validar con `ModelState.IsValid`
- Usar ViewModels, no entidades de dominio

### Views
- Usar Razor syntax (`@model`, `@Html`, `@Url`)
- Layouts en `Views/Shared/_Layout.cshtml`
- Partials con prefijo `_` (ej: `_LoginPartial.cshtml`)
- Tag Helpers sobre HTML Helpers
- Validación client-side con `asp-validation-*`

### ViewModels
- Clases específicas para cada vista
- Atributos de validación (`[Required]`, `[StringLength]`)
- Propiedades solo necesarias para la vista
- Separar de DTOs de API

## Buenas Prácticas

### JavaScript
- Archivos en `wwwroot/js/`
- Usar `site.js` para código común
- Módulos separados por funcionalidad
- Evitar inline scripts
- Usar `fetch` o `axios` para llamadas AJAX

### CSS
- Archivos en `wwwroot/css/`
- `site.css` para estilos globales
- BEM o convención consistente
- Responsive design (mobile-first)
- Usar Bootstrap si está incluido

### Formularios
- Usar Tag Helpers: `asp-for`, `asp-action`, `asp-controller`
- CSRF token automático con `@Html.AntiForgeryToken()`
- Validación client y server-side
- Mensajes de error con `asp-validation-summary`

### Seguridad
- Sanitizar inputs (automático con Razor)
- Usar `[ValidateAntiForgeryToken]` en POST
- Autorización con `[Authorize]`
- No exponer información sensible en vistas

### Performance
- Bundling y minificación en producción
- Lazy loading de imágenes
- Cache de assets estáticos
- Usar CDN para librerías externas

### Accesibilidad
- Etiquetas semánticas HTML5
- Atributos `aria-*` cuando sea necesario
- Labels asociados a inputs
- Contraste de colores adecuado

### Código Limpio
- Nombres descriptivos en español
- Evitar lógica compleja en vistas
- Reutilizar partials y componentes
- Comentarios solo cuando sea necesario
- Consistencia en nomenclatura
