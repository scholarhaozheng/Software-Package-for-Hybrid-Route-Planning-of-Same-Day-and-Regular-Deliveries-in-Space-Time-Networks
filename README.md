# Optimization Integration System

## Project Overview

This project aims to develop an integrated optimization system for dynamic vehicle routing. The system is developed in C# and uses the Gurobi Optimizer to solve the mixed-integer programming models formulated during the process.

## Environment Dependencies

### Development Language and Platform

- **C#** (compatible with either .NET Framework or .NET Core, depending on your requirements)
- **Windows Platform** (the project uses Windows Forms components)

### Optimization Solver

- **Gurobi Optimizer**  
  Please ensure that Gurobi is installed and that you have obtained the necessary license. The project must reference the Gurobi .NET libraries correctly.

### Other Dependencies

- Standard .NET libraries such as `System.Windows.Forms`, `System.Drawing`, etc.

## Compilation and Execution

### Compilation Steps

#### Environment Setup

- First, install Visual Studio or another C# IDE that supports C# development, and install the appropriate .NET SDK.
- Ensure that Gurobi environment variables and the license file are properly configured.

#### Project File Setup

- Add all source files (e.g., `Program.cs` along with all related module files) to your solution.
- Add a reference to Gurobi (typically `Gurobi.dll`).

#### Build

- In Visual Studio, select **Build Solution** or use command-line tools (for example, `dotnet build` or MSBuild) to compile the project.

### Running Steps

#### Preparing Input Files

- Place all input data files (coordinates, time windows, service status files, etc.) in the correct relative directories.

#### Running the Application

- Execute the generated executable file. Upon startup, the program will sequentially perform static model solving, dynamic model optimization, branch-and-bound node generation, and result statistics.

#### Reviewing the Output

- The program outputs information about each solving stage to the console, and multiple output files are generated. You can check the log information to verify the runtime process and the solution results.
