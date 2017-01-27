using System;
using System.Collections.Generic;
using System.IO;
using PowerArgs;
using RazorEngine;
using RazorEngine.Templating;
using Vigilant.IntegrationTests;

namespace XunitReporter
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var reporterArgs = Args.Parse<ReporterArgs>(args);

            
            Engine.Razor.Compile(AssemblyResource.InThisAssembly("TestView.cshtml").GetText(), "testTemplate", typeof(List<TestAssemblyModel>));

            var model = TestPageModelBuilder.Create()
                                            .WithPageTitle(reporterArgs.PageTitle)
                                            .WithTestXmlFromPath(reporterArgs.Xml)
                                            .Build();

            var output = new StringWriter();
            Engine.Razor.Run("testTemplate", output, typeof(List<TestAssemblyModel>), model);

            File.WriteAllText(reporterArgs.Html, output.ToString());

            Console.WriteLine(output.ToString());

            Console.ReadKey();

        }
    }
}
