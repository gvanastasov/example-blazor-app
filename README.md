# Example Blazor WASM Application

A simple web application and a helper library showcasing the functionality of Blazor WebAssembly (blazorwasm). The project includes a Blazor app, a reusable component library, and PowerShell scripts for handling repetitive tasks. This README provides instructions on setting up the project and describes key aspects of Blazor WebAssembly.

## Blazor WebAssembly Overview

Blazor WebAssembly allows the execution of C# code directly in the browser using WebAssembly. Here are some key considerations for Blazor WebAssembly:

- **Execution Location:** The application code runs in the client's browser, enabling the execution of UI logic and components on the client side.
  
- **Size of Initial Download:** The initial download size is larger due to including the .NET runtime and application code. Subsequent interactions require minimal data transfer for updates.

- **Performance:** Execution speed is limited by the performance of WebAssembly in the browser, with ongoing advancements in WebAssembly performance.

- **Scalability:** More suitable for scenarios where the client handles a larger portion of the application workload, potentially making it more scalable for certain use cases.

- **Offline Support:** Can be designed to work in an offline or partially connected scenario as the entire application is downloaded to the client.

## Getting Started

### Prerequisites

Before you begin, ensure you have the following installed:

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Visual Studio Code](https://code.visualstudio.com/) with the Blazor extension

### Project Structure

All packages are organized neatly under the `./src` subdirectory. The project includes a helper library with reusable components and scripts for generic tasks.

### Library

1. **Generate a (semi/fake) design system as a separate component library:**
   ```bash
   dotnet new blazorclasslib -n SomeLibName -o ./src/SomeLibName
   ```
2. **Generate a component inside ./src/SomeLibName using the script:**
    ```bash
    .\Run.ps1 -task CreateComponent -arguments <componentName>
    ```

### Application

1. **Ensure .NET CLI is working**
    ```bash
    dotnet --version
    ```
2. **Install Blazor extension:**
Install the Blazor extension for Visual Studio Code.
3. **Generate a new Blazor WebAssembly project:**
    ```bash
    dotnet new blazorwasm -n SomeName -o ./src/SomeName
    ```
4. **Optional: Disable HTTPS in launchSettings.json (for demo purposes):**
Remove the https:// URL from profiles.SomeName.applicationUrl in launchSettings.json within the app project. This is suitable for local development.
5. **Reference the component library inside the app:**
dotnet add ./src/ExampleBlazorApp/ExampleBlazorApp.csproj reference ./src/SomeLibName/SomeLibName.csproj

### Scripts

The project includes PowerShell scripts for common tasks. These scripts simplify common development tasks, making it easy to generate components and run the Blazor application. Adjust the script parameters as needed for your specific use case.

- Generate a component:
    ```bash
    .\Run.ps1 -task CreateComponent -arguments <componentName>
    ```
- Run the application:
    ```bash
    .\Run.ps1 -task Start
    ```

## Testing

1. generate test project with xunit framework
    ```bash
    dotnet new xunit -n ./test/ExampleBlazorDS.Tests
    ```
2. add reference to respective library
    ```bash
    dotnet add reference ../../src/ExampleBlazorDS/ExampleBlazorDS.csproj
    ```
3. add BUnit & Playwright
    ```bash
    dotnet add package Bunit
    dotnet add package Microsoft.Playwright
    ```
4. write tests
5. run tests
    ```bash
    dotnet test
    ```

## License

This project is licensed under the [MIT License](./LICENSE).
