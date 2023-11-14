param (
    [string]$task,
    [string[]]$arguments
)

if (-not $task) {
    Write-Host "Usage: .\Run.ps1 -task <taskName> [-arguments <arguments>]"
    Exit 1
}

if ($task -eq "CreateComponent") {
    $taskPath = Join-Path -Path $PSScriptRoot -ChildPath "src\ExampleBlazorDS\Scripts\CreateComponent.ps1"

    if (Test-Path $taskPath) {
        if ($arguments.Count -ge 1) {
            $componentName = $arguments[0]
            Invoke-Expression -Command "$taskPath -componentName $componentName"
        }
        else {
            Write-Host "Usage: .\Run.ps1 -task CreateComponent -arguments <componentName>"
        }
    }
    else {
        Write-Host "Task script '$taskPath' not found."
    }
}
else {
    Write-Host "Task '$task' not found."
}