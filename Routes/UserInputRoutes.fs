namespace Routes

open Giraffe
open Handlers.UserInputHandler

module UserInputRoutes =
    let routes : HttpHandler =
        choose [
            // Route for micro-ML conversion
            route "/api/convert-ml" >=> choose [
                POST >=> convertMicroML
            ]
        ]