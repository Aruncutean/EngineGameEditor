using Core.IO;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorAvalonia.service
{
    public class ScriptService
    {
        public ScriptService()
        {
        }


        public void CreateSlnProject(string scriptPath)
        {
            if (!File.Exists("GameScripts.sln"))
            {
                var createSln = Process.Start(new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"new sln -n GameScripts",
                    WorkingDirectory = scriptPath,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });

                createSln.WaitForExit();
            }
        }



        public void AddNewScrips(string scriptPath, string scriptName)
        {
            string baseDir = Path.Combine(scriptPath, scriptName);
            Directory.CreateDirectory(baseDir);

            string csprojPath = Path.Combine(baseDir, $"{scriptName}.csproj");
            string csFilePath = Path.Combine(baseDir, $"{scriptName}.cs");

            string coreDllPath = @"C:\Users\arunc\Desktop\GameEditor\Core\bin\Debug\net8.0\Core.dll";
            string relativeCoreDLLPath = Path.GetRelativePath(baseDir, coreDllPath);
            File.WriteAllText(Path.Combine(baseDir, $"{scriptName}.csproj"), $@"
            <Project Sdk=""Microsoft.NET.Sdk"">
              <PropertyGroup>
                <OutputType>Library</OutputType>
                <TargetFramework>net8.0</TargetFramework>
                <RootNamespace>{scriptName}</RootNamespace>
              </PropertyGroup>
              <ItemGroup>
                <Compile Include=""Scripts\*.cs"" />
              </ItemGroup>
            	<ItemGroup>
            	  <Reference Include=""Core"">
            	    <HintPath>{relativeCoreDLLPath}</HintPath>
            	  </Reference>
            	</ItemGroup>
            </Project>
            ");

            File.WriteAllText(Path.Combine(baseDir, "PlayerController.cs"), @"
            using Core.component;
            using Core.entity;

            public class PlayerController : ScriptComponent
            {
                public float Speed = 2.5f;

                public override void OnUpdate(float dt)
                {
                    var transform = Entity.GetComponent<TransformComponent>();
                }
            }
            ");

            var proc = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"sln \"{Path.Combine(scriptPath, "GameScripts.sln")}\" add \"{csprojPath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            proc.WaitForExit();
            if (proc.ExitCode == 0)
            {
                Console.WriteLine($"✅ Proiect '{scriptName}' adăugat cu succes.");
            }
            else
            {
                Console.WriteLine("❌ Eroare la adăugarea proiectului.");
            }

        }
        public void RunDotnetCommand()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "build GameScripts.sln",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                Console.WriteLine($"✅ [dotnet build GameScripts.sln] completed successfully.");
                Console.WriteLine(output);
            }
            else
            {
                Console.WriteLine($"❌ [dotnet build GameScripts.sln] failed:");
                Console.WriteLine(error);
            }
        }

    }


}



