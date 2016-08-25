## Build
`dotnet pack`

## Usage

``` csharp
using WkHtmlToX;

var settings = new PdfGlobalSettings();
settings.PageSize = PageSize.Letter;
settings.Orientation = Orientation.Landscape;
settings.ColorMode = ColorMode.Color;

var inFile = "template.html";
var outFile = "output.pdf";

using (var converter = new HtmlToPdfConverter(settings))
{
    var html = System.IO.File.ReadAllText(inFile);
    var data = converter.ConvertToPDF(html);
    System.IO.File.WriteAllBytes(outFile, data);
}
```