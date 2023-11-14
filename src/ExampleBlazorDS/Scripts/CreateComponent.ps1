param (
    [string]$componentName
)

# Check if componentName is provided as a command-line argument
if (-not $componentName) {
    Write-Host "Usage: $MyInvocation.MyCommand -componentName <YourComponentName>"
    Exit 1
}

# Get the directory of the script
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Specify the component directory
$componentDir = "$scriptDir\..\Components\$componentName"

# Ensure the component directory exists
if (-not (Test-Path -Path $componentDir -PathType Container)) {
    New-Item -Path $componentDir -ItemType Directory | Out-Null
}

# Create the component markup file
@"
@inherits ExampleBlazorDS.Components.$($componentName)Base

<div class=".$componentName">Hello, $componentName!</div>
"@ | Out-File -FilePath "$componentDir\$componentName.razor" -Force

# Create the component code-behind file
@"
using Microsoft.AspNetCore.Components;

namespace ExampleBlazorDS.Components
{
    public class $($componentName)Base : ComponentBase
    {
    }
}
"@ | Out-File -FilePath "$componentDir\$($componentName)Base.cs" -Force

# Create the component styles file
@"
.$componentName {
    display: block;
}
"@ | Out-File -FilePath "$componentDir\$componentName.razor.css" -Force

Write-Host "Razor split component '$componentName' created successfully in directory '$componentDir'."