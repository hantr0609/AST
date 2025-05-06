namespace Handlers

open Giraffe
open Microsoft.AspNetCore.Http
open System
open Parse
open Fun

module UserInputHandler =
    // Type definition for the request body
    type MicroMLRequest = {
        expression: string
    }
    
    // Handler for micro-ML to syntax tree conversion
    let convertMicroML : HttpHandler =
        fun next ctx ->
            task {
                try
                    // Bind the JSON request body to our MicroMLRequest type
                    let! request = ctx.BindJsonAsync<MicroMLRequest>()
                    
                    if String.IsNullOrWhiteSpace(request.expression) then
                        ctx.SetStatusCode 400
                        return! json {| success = false; error = "Expression cannot be empty" |} next ctx
                    else
                        // Parse the micro-ML expression to create an AST
                        let expr = Parse.fromString request.expression
                        
                        // Convert the AST to syntax tree notation
                        let syntaxTree = Fun.print expr
                        
                        // Return the syntax tree notation
                        return! json {| success = true; syntaxTree = syntaxTree |} next ctx
                with
                | ex ->
                    // Return detailed error message
                    ctx.SetStatusCode 400
                    return! json {| success = false; error = ex.Message |} next ctx
            }