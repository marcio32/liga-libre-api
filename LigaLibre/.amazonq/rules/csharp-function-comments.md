# Regla de Comentarios para Funciones en C#

Al crear o modificar funciones en C#, siempre agrega comentarios XML siguiendo estas pautas:

## Formato Requerido

```csharp
/// <summary>
/// Descripción breve de lo que hace la función
/// </summary>
/// <param name="nombreParametro">Descripción del parámetro</param>
/// <returns>Descripción de lo que retorna</returns>
```

## Reglas Específicas

1. **Métodos públicos y protegidos**: SIEMPRE deben tener comentarios XML completos
2. **Métodos privados**: Agregar comentarios solo si la lógica es compleja
3. **Parámetros**: Documentar cada parámetro con `<param>`
4. **Valores de retorno**: Documentar con `<returns>` excepto para métodos void
5. **Excepciones**: Si el método lanza excepciones, documentarlas con `<exception>`

## Ejemplos

### Método con parámetros y retorno
```csharp
/// <summary>
/// Obtiene un jugador por su identificador
/// </summary>
/// <param name="id">Identificador único del jugador</param>
/// <returns>DTO del jugador o null si no existe</returns>
public async Task<PlayerDto?> GetPlayerByIdAsync(int id)
```

### Método con múltiples parámetros
```csharp
/// <summary>
/// Actualiza la información de un jugador existente
/// </summary>
/// <param name="id">Identificador del jugador a actualizar</param>
/// <param name="playerDto">Datos actualizados del jugador</param>
/// <returns>DTO del jugador actualizado</returns>
public async Task<PlayerDto> UpdatePlayerAsync(int id, CreatePlayerDto playerDto)
```

### Método sin retorno
```csharp
/// <summary>
/// Elimina un jugador del sistema
/// </summary>
/// <param name="id">Identificador del jugador a eliminar</param>
public async Task DeletePlayerAsync(int id)
```

## Estilo de Escritura

- Usa lenguaje claro y conciso
- Escribe en tercera persona del presente
- No repitas el nombre del método en la descripción
- Enfócate en QUÉ hace, no en CÓMO lo hace
