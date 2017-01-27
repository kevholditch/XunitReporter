# XunitReporter  ![Appveyor build status](https://ci.appveyor.com/api/projects/status/2s9xqbjgnab8tkbs?svg=true)
Convert Xunit XBehave xml reports to html using razor

XUnit reporter is a console app to extract the Gherkin (Given, When, Then) syntax from XBehave tests and then render them to html using Razor.

XBehave is great but the problem with it is that there is no nice way (that I know of) of extracting your tests from the source code.  This parser parses the xml output from the XUnit console runner to a TestAssemblyModel.  This can then be used to render html however you wish.  Here is an example view:

```
<html>
    <head>
        <meta charset="utf-8" />
        <title>@Model.PageTitle</title>
    </head>
    <body>
        <p>
            @foreach (var testAssembly in Model.TestAssemblyModels)
            {
                <h1>@testAssembly.Name</h1>

                foreach (var testCollection in testAssembly.TestCollections)
                {
                    <h2>@testCollection.Name</h2>
                    <p>Total: @testCollection.Total, Passed: @testCollection.Passed, Failed: @testCollection.Failed, Skipped: @testCollection.Skipped</p>

                    foreach (var test in testCollection.Tests)
                    {
                        <h3>@test.Name   -   @test.TestResult</h3>
                        foreach (var step in test.TestSteps)
                        {
                            <p>@step.Step</p>
                        }

                    }

               }
            }
        </p>
    </body>
</html>
```

If you had a test that looked like:
```
public class MyTest
{
    [Scenario]
    public void MyScenario()
    {
        "Given something"
            ._(() => { });

        "When something"
            ._(() => { });

        "Then something should be true"
            ._(() => { });

        "And then another thing"
           ._(() => { });
    }
}
```
If you ran the xunit console runner using the -Xml switch and then parsed the resultant xml to the XUnit.Reporter console app with the view given above then you would get the following html:

```
<html>
    <head>
        <meta charset="utf-8" />
        <title>MyTests</title>
    </head>
    <body>
        <p>
                <h1>C:\projects\CommandScratchpad\CommandScratchpad\bin\Debug\CommandScratchpad.EXE</h1>
                    <h2>Test collection for RandomNamespace.MyTest</h2>
                    <p>Total: 1, Passed: 1, Failed: 0, Skipped: 0</p>
                        <h3>MyScenario   -   Pass</h3>
                            <p>Given something</p>
                            <p>When something</p>
                            <p>Then something should be true</p>
                            <p>And then another thing</p>
        </p>


    </body>
</html>
```



