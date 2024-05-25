# Simaira.Serilog.Testing.Application
Welcome to the Simaira.Serilog.Testing.Application repository. This repository serves as a boilerplate for implementing Serilog with .NET 8, providing a foundational setup for both Console and Web applications.
## Overview
Simaira.Serilog.Testing.Application aims to streamline the process of integrating Serilog into your .NET 8 projects. Serilog is a powerful and flexible logging library that makes structured logging easy and efficient. This repository offers a ready-to-use template to help you get started quickly, with examples and configurations for both Console and Web applications.

## Features
- <b>Pre-configured Serilog setup:</b> Simplifies the integration of Serilog into .NET 8 applications.
- <b>Support for Console Applications: </b>Provides example code and configuration for using Serilog in a console app.
- <b>Support for Web Applications:</b> Includes setup and examples for integrating Serilog into ASP.NET Core web applications.
- <b>Structured Logging:</b> Facilitates structured logging, making it easier to search and analyze logs.
- <b>Extensible and Customizable:</b> Easily extend and customize the configuration to suit your specific needs.

## Getting Started
### Prerequisites
Ensure you have the following installed:
- .NET 8 SDK
- Visual Studio or any other compatible IDE

### Installation
#### Clone the repository:

```
git clone https://github.com/yourusername/Simaira.Serilog.Testing.Application.git
```

#### Navigate to the project directory:

```
cd Simaira.Serilog.Testing.Application
```
#### Restore dependencies:
```
dotnet restore
```
### Running the Console Application
#### Navigate to the console application directory:

```
cd Serilog.Console.Testing.Application
```

#### Run the application:

```
dotnet run
```

### Running the Web Application
#### Navigate to the web application directory:

```
cd Serilog.WebAPI.Testing.Application
```

#### Run the application:

```
dotnet run
```

### Configuration
The Serilog configuration is defined in the appsettings.json file for both Console and Web applications. Here is an example configuration:

```
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.txt",
          "rollingInterval": "Day"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
    "Properties": {
      "Application": "Simaira.Serilog.Testing.Application"
    }
  }
}

```
Feel free to modify the settings as per your requirements.



## Contributing
We welcome contributions to improve this boilerplate repository. Please fork the repository and submit pull requests.

## License
This project is licensed under the MIT License - see the LICENSE file for details.

## Contact
For any inquiries or issues, please contact us at
- [https://www.linkedin.com/in/firstcrazydeveloper/]
- [support@firstcrazydeveloper.in].
