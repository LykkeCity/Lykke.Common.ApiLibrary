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

What this invocation actualy do:

* Setups API version and description, using arguments passed by you
* Enables enum serialization as string values, not numbers
* Enables xms-enum extension, which allows Autorest to generate enums on client side
* Includes xml-documentation, if xml-file generation enabled in your project. (xml file name should equals assembly file name)
* Makes response value types to be not nullable, when client code is generated by autorest 

If you need individual swagger configuration, you can use next extensions:

### EnableXmsEnumExtension

```cs
// Enables "x-ms-enum" swagger extension, which allows Autorest tool generates enum or set of string constants for each server-side enum.
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

### MakeResponseValueTypesRequired

```cs
/// Makes response value types to be not nullable, when client code is generated by autorest
options.MakeResponseValueTypesRequired();
```

## Middleware

To configure default Lykke middleware, add next invocation to your Startup.Configure method:

```cs
app.UseLykkeMiddleware("Your main component name", ex => ErrorResponse.Create("Technical problem"), logClientErrors: true);
```

What this invocation actualy do:

* Registers global error handling middleware, which logs uncaught errors and sends json error response, which in turn specified by delegate.
* Registers client error handling middleware, which logs HTTP 4xx errors. It is optional, you can omit `logClientErrors` argument, if it's not necessary.

If you need individual middleware configuration, you can use next extensions:

### GlobalErrorHandlerMiddleware

```cs
// Adds global error handler, which logs uncaught errors and sends json error response, which in turn specified by delegate
app.UseMiddleware<GlobalErrorHandlerMiddleware>("Your main component name", ex => ErrorResponse.Create("Technical problem"));
```

### ClientErrorHandlerMiddleware

```cs
// Adds client error handler, which logs HTTP 4xx errors
app.UseMiddleware<ClientErrorHandlerMiddleware>("Your main component name");
```
