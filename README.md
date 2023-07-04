# Fable prop bug

## Steps to reproduce

```bash
dotnet fsi server.fsx
```

Browse to [http://localhost:4000/](http://localhost:4000/) and view the error in the console.

```
Error: Cannot infer key and value of ClassName show
    at u (MapUtil.js:34:15)
    at U (MapUtil.js:71:13)
    at App (Library.js:14:39)
    at $i (react-dom.production.min.js:167:137)
    at da (react-dom.production.min.js:290:337)
    at aa (react-dom.production.min.js:280:389)
    at vf (react-dom.production.min.js:280:320)
    at jr (react-dom.production.min.js:280:180)
    at mo (react-dom.production.min.js:273:245)
    at on (react-dom.production.min.js:127:105)
```