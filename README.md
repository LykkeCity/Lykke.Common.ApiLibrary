# Lykke.Common.ApiLibrary

Utility libraries for DotNetCore WebAPI projects

Nuget package: [link](https://www.nuget.org/packages/Lykke.Common.ApiLibrary/)

# Usage 

## Swagger

To configure swagger with default Lykke options, add next invocation to your Startup.ConfigureServices method:

```cs
services.AddSwaggerGen(options =>
{
    options.DefaultLykkeConfiguration("v1", "Your API name");
});
```

If you need individual swagger configuration, you can use next extensions:

### EnableXmsEnumExtension

```cs
// Enables "x-ms-enum" swagger extension, wich allows Autorest tool generates enum or set of string constants for each server-side enum.
// Optionaly you can specify XmsEnumExtensionsOptions to configure extension
options.EnableXmsEnumExtension();
```

### EnableXmlDocumentation

```cs
// Includes source code's XML documentation into swagger document.
// Documentation will be included to swagger document only if assembly's 
// XML documentation file generation enabled and it's name corresponds to the assembly name.
options.EnableXmlDocumentation();
```

## Middleware

To configure default Lykke middleware, add next invocation to your Startup.Configure method:

```cs
app.UseLykkeMiddleware("Your main component name", () => ErrorResponse.Create("Technical problem"));
```

If you need individual middleware configuration, you can use next extensions:

```cs
// Adds global error handler, wich logs uncaught errors and sends json error response, wich specified by delegate
app.UseMiddleware<GlobalErrorHandlerMiddleware>("Your main component name", () => ErrorResponse.Create("Technical problem"));
```
