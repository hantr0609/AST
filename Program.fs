open Giraffe
open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Routes

open Parse
open Fun

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    
    // Add Giraffe to the service collection
    builder.Services.AddGiraffe() |> ignore
    
    // Configure CORS to allow API requests
    builder.Services.AddCors(fun options ->
        options.AddPolicy("AllowAll", fun builder ->
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                |> ignore
        )
    ) |> ignore

    let app = builder.Build()
    
    // Configure middleware
    app.UseStaticFiles() |> ignore
    app.UseCors("AllowAll") |> ignore

    // Configure routing
    let webApp =
        choose [
            route "/" >=> htmlFile "./wwwroot/index.html"
            UserInputRoutes.routes  // This includes our convert-ml route
            // Fallback for other routes
            setStatusCode 404 >=> text "Not Found"
        ]
    app.UseGiraffe(webApp)

    // Output some examples to verify our parsing works
    printfn "%A" Parse.e1
    printfn "%A" Parse.e2

    eval Parse.e1 [] |> printfn "%A"    
    eval Parse.e2 [] |> printfn "%A"

    print Parse.e1 |> printfn "%A"
    print Parse.e2 |> printfn "%A"

    app.Run()

    0 // Exit code