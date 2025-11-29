# EasyBinder

EasyBinder is a command-line utility designed to merge multiple Windows executables into a single distributable file. The tool automatically creates a temporary .NET project, embeds the provided executables as resources, generates the extraction logic, and compiles everything into one final binary.

This README is based directly on the program logic defined in the provided C# source code.

## Table of Contents

1. [Overview](#overview)
2. [Features](#features)
3. [How It Works](#how-it-works)
4. [Requirements](#requirements)
5. [Usage](#usage)

   * [Available Arguments](#available-arguments)
   * [Command Examples](#command-examples)
6. [Internal Workflow](#internal-workflow)
7. [Notes and Limitations](#notes-and-limitations)
8. [License](#license)

## Overview

EasyBinder takes a list of executable files and produces a single output executable capable of extracting and running all bundled executables at runtime. It relies on the .NET SDK to dynamically generate and compile a temporary project.

The tool validates input arguments, checks file existence, embeds each executable, then builds a standalone binary using `dotnet publish`.

## Features

* Bind multiple executables into a single output file
* Automatic project generation and cleanup
* Optional console hiding (WinExe mode)
* Optional exclusion of the .NET runtime (requires runtime installed on target machine)
* Custom work directory and output directory selection
* Input validation and real-time logging

## How It Works

From the source code, the program performs the following steps:

1. Parse arguments and validate the distribution file name.
2. Read options such as `--workdir`, `--outputdir`, `--bind-exe`, and more.
3. Ensure at least two executables are provided.
4. Create necessary directories.
5. Generate a temporary .NET console project.
6. Copy all executables into the project and embed them as resources.
7. Inject extraction and execution logic into `Program.cs`.
8. Compile the project using `dotnet publish` with single-file mode.
9. Clean up temporary files.

## Requirements

* .NET SDK (compatible with `dotnet new console`)
* Windows environment recommended (due to .exe payloads)

## Setup Script

The project includes a `Quick Setup.bat` script intended to restore the .NET dependencies before building or using the dotnet cli, run :

```
dotnet restore
```

It simply restores all required NuGet packages for the project. 

* .NET SDK (compatible with `dotnet new console`)
* Windows environment recommended (due to .exe payloads)

## Usage

Basic syntax:

```
ezbinder [Options] <DistFileName>
```

### Available Arguments

* `--exclude-runtime`
  Excludes the .NET runtime from the final executable.

* `--noconsole`
  Builds using `WinExe` mode instead of `Exe` (no console window).

* `--workdir=<path>`
  Sets the temporary working directory used during the build.

* `--outputdir=<path>`
  Sets the directory where the final executable will be produced.

* `--bind-exe=<path>`
  Adds an executable to the list of files to bind. Must exist on disk.

### Command Examples

Bind two executables into one:

```
ezbinder finalname.exe --bind-exe=app1.exe --bind-exe=app2.exe
```

Custom directories and no console window:

```
ezbinder merged.exe --bind-exe=a.exe --bind-exe=b.exe --noconsole --workdir=./tmp --outputdir=./dist
```

## Internal Workflow

A high‑level breakdown of what the code does:

1. **Argument Parsing**
   Distinguishes options from the output file name and handles unknown arguments with warnings.

2. **Validation**
   Ensures the distribution filename is valid and all executables exist.

3. **Project Generation**
   Creates a temporary directory and runs:

   ```
   dotnet new console -o <workdir>/__SIGMA_CS__
   ```

4. **Resource Embedding**
   Each executable is copied into the project and registered inside the generated `.csproj` file.

5. **Payload Loader Injection**
   The code replaces placeholders in `Payloads.BaseMainPayload` to insert extraction logic for each file.

6. **Compilation**
   Runs:

   ```
   dotnet publish <project> -o <outputdir> /p:PublishSingleFile=true /p:SelfContained=<true/false>
   ```

7. **Cleanup**
   Removes generated `.csproj` and `Program.cs` after successful compilation.

## Notes and Limitations

* At least **two** executables must be supplied.
* The output file size scales directly with the size of all embedded executables.
* Some antivirus programs may flag the final binary due to resource extraction behavior.
* The tool does not support non‑executable file binding.

