module Ap

open Browser.Dom
open Fable.React
open Fable.React.Props
open Feliz

[<ReactComponent>]
let App () =
    let h1Class = (if true then "show" else "")

    div [ ClassName(if true then "show" else "") ] [
        h1 [ ClassName h1Class ] [ str "Hello World" ]
    ]

ReactDom.render (App(), document.querySelector "body")
