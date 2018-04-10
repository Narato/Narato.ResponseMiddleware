# Narato.ResponseMiddleware
This library contains Middleware/actionfilters for global exception handling, timing and modelstate filtering

It does 3 things:
1) Catches all exceptions within your action and maps them to error models
2) Times the execution of your request and places it in your response headers (or response model if you're using legacy mode)
3) Automatically validates the modelstate, and if invalid, will return a validation error model. No more modelstate checking in your controllers, yay!

This all is done in a very unintrusive way. The only things you have to do is add some configuration in your setup, and use [these Exceptions](https://github.com/Narato/Narato.ResponseMiddleware.Models/tree/master/src/Narato.ResponseMiddleware.Models/Exceptions).  

If you don't want to use our exceptions, or you want to extend the mapping, you can do so by implementing `IExceptionToActionResultMapper` or extending
`ExceptionToActionResultMapper` (the Map method is virtual)

Getting started
==========
### 1. Add dependency in your project's csproj file

```xml
<PackageReference Include="Narato.ResponseMiddleware.Models" Version="2.0.0" />
```

### 2. Configure Startup.cs
#### 2.1 Response model
You have 2 options here: Wrap your response into a response model, or don't wrap it at all.  
An example of the response model is as follows:
```json
{
  "data": {
    "id": "50a62ebd-3683-4618-80b2-7f893c917521",
    "name": "Narato",
    "city": "Kontich",
  },
  "self": "/api/companies/50a62ebd-3683-4618-80b2-7f893c917521",
  "generation": {
    "timeStamp": "2017-06-07T13:16:54.22727+02:00",
    "duration": 52
  },
  "status": 200
}
```

If you do **not** want to use the response model (recommended, as the response model is our legacy model) add following config when adding Mvc support in ConfigureServices

```C#
services.AddMvc(
    config =>
    {
        config.AddResponseFilters(false); // default value is false
    })
);
```
If you **do** want to use the response model (**not recommended**), use `config.AddResponseFilters(true);`

#### 2.2 error model
Certain Exceptions automatically get mapped to certain actionresults
* ValidationException => Validation error model with statuscode 400
* EntityNotFoundException => If a message is set, a normal error model with statuscode 404, otherwise empty body with statuscode 404
* UnauthorizedException => 401 statuscode
* ForbiddenException => 403 statuscode
* ExceptionWithFeedback => error model with statuscode 500. **Important** to note here is that these messages will always be shown, even when running in a production environment
* any other exception => error model with statuscode 500. **Important** to note here is that the message on these exceptions will **only** be shown when running in a development environment. In anything else, a generic "please contact support" message will be shown

These mappings can be extended/replaced. see section [2.2.3](#223-exception-mapping-hooks) for more info
##### 2.2.1 recommended error model
An example of an error model:
```json
{
  "code": "ENF",
  "message": "dossier type with id be5f2a37-e0f9-4534-ab8b-154778527b6e doesn't exist"
}
```
and an example of a validation error model:
```json
{
  "validationMessages": {
    "version": [
      "version number can't be lower than 0"
    ],
    "schema": [
      "supplied value is not a valid json schema: Unexpected token encountered when reading schema. Expected StartObject, Boolean, got Integer. Path '', line 1, position 1."
    ]
  }
}
```
If you want to use this error model, add following line to ConfigureServices
```C#
services.AddResponseMiddleware();
```
##### 2.2.2 Legacy error model
We do not reccomend using this error model. We simply offer it so our legacy projects can keep their API contract  
An example of an error model:
```json
{
  "identifier": "577600ce-ba88-47af-8190-e8fd305edd7d",
  "title": "Entity could not be found.",
  "self": "/api/DossierType/be5f2a37-e0f9-4534-ab8b-154778527b6e",
  "generation": {
    "timeStamp": "2017-06-08T14:03:19.6107431+02:00",
    "duration": 155
  },
  "feedback": [
    {
      "type": 3,
      "description": "dossier type with id be5f2a37-e0f9-4534-ab8b-154778527b6e doesn't exist"
    }
  ],
  "status": 404
}
```
and an example of a validation error model:
```json
{
  "identifier": "6e49110a-e425-43cf-8e27-4c8ebc632c24",
  "title": "Validation failed.",
  "self": "/api/DossierType",
  "generation": {
    "timeStamp": "2017-06-08T14:14:53.2675821+02:00",
    "duration": 280
  },
  "feedback": [
    {
      "type": 4,
      "description": "version number can't be lower than 0"
    },
    {
      "type": 4,
      "description": "supplied value is not a valid json schema: Unexpected token encountered when reading schema. Expected StartObject, Boolean, got Integer. Path '', line 1, position 1."
    }
  ],
  "status": 400
}
```
If you want to use this error model, add following line to ConfigureServices
```C#
services.AddResponseMiddleware(true);
```

##### 2.2.3 Exception mapping hooks
If the default Exception to ActionResult mappings aren't good enough, you have 2 options.  
Either extend `ExceptionToActionResultMapper` (or `LegacyExceptionToActionResultMapper`) and override the `Map(Exception ex)` method (make sure you call `base.Map(ex)` after your code).  
Or create a (or several) new implementation of `IExceptionToActionResultMapperHook`. An example can be found [here](https://github.com/Narato/Narato.ResponseMiddleware/blob/master/test/Narato.ResponseMiddleware.IntegrationTest/Mappers/TestClasses/ConflictMapperHook.cs).
Don't forget to add it in your Startup.cs!

##### 2.2.4 Dealing with AggregateExceptions
Sometimes Exceptions get wrapped in an AggregateException (for example when running async methods synchronously, or when using Service Fabric with remoting). In such cases you could write an extra [Exception mapping hook](#223-exception-mapping-hooks), or you could use the built-in AggregateExceptionUnwrappingFilter. This filter will unwrap the AggregateException (**only if there is only 1 inner exception**), so that the ExceptionToActionResultMapper can do its job as usual.  
To use the Filter add following config when adding Mvc support in ConfigureServices

```C#
services.AddMvc(
    config =>
    {
        config.AddResponseFilters();
        config.AddAggregateExceptionUnwrappingFilter(); // make sure to add this line *BELOW* the AddResponseFilters line!
    })
);
```

#### 2.3 Execution timing
When using legacy mode (legacy response models) you don't need to do anything in this step (the Execution timing happens in an ActionFilter here).  
When using non-legacy mode however, you need to register a middleware  
In Startup.cs Configure, add following line
```C#
app.UseExecutionTiming();
```
This line has to be **above** `app.UseMvc();`

### 3. Custom Validation message model
In most cases, you will use `IValidationException<string>`, but when you want a more complex model, you can always change the generic part to be any other model you want. Everything will work out of the box, except if you use the legacy response models.  
In this case you have to make sure to add an Exception mapping hook for your custom model. e.g.: `IValidationException<MyCustomModel>`

# Helping out

If you want to help out, please read [this wiki page](https://github.com/Narato/Narato.ResponseMiddleware/wiki/Helping-out)
