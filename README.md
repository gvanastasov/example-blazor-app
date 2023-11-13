# example-blazor-app
 A simple application built with Blazor

What?
A simple webapp and a helper library with some reusable components to showcase how Blazor WebAssembly (short for blazorwasm, or simply put client) application works althogher.
Key notes for blazorwasm:
- Execution Location - The application code is executed in the browser. The entire Blazor application, including the UI logic and components, is downloaded to the client's browser, and the execution happens there. This enables running C# code directly in the browser using WebAssembly.
- Size of Initial Download - The initial download size is larger because it includes the .NET runtime and the application code. Subsequent interactions may require minimal data transfer for updates and data.
- Performance - Generally, the execution speed is limited by the performance of WebAssembly in the browser. However, advancements in WebAssembly performance are ongoing.
- Scalability - More suitable for scenarios where the client can handle a larger portion of the application workload, making it potentially more scalable for certain use cases.
- Offline Support - Can be designed to work in an offline or partially connected scenario because the entire application is downloaded to the client.

How?
1. C# dev kit
- ensure dotnet cli is working via '?'
2. Blazor extension
3. Generate new project
- dotnet new blazorwasm -n SomeName -o ./src/SomeName
4. Optional: Disable TSL by simply removing removing https:// url from launchSettings.json -- profiles.SomeName.applicationUrl. Since this is for a demo purpose and you are suppose to run this only on your local evnironment is perfectly safe to do so. The other option is to generate a developer cert, trust it and use it during development. This is a topic outside of the scope of this demo.
5. Runt the project
- cd ./src/SomeName
- dotnet run
