# UiPath JWT Authentication Library

A complete **JWT Authentication** framework for UiPath Studio that implements secure Salesforce JWT Bearer Token authentication.  
Compatible with **Windows-Legacy** projects and includes full code, configuration, and usage instructions.

---

## 📘 Overview

This project provides a complete, ready-to-use implementation for authenticating with Salesforce using JWT Bearer tokens instead of SOAP-based credentials.

It supports:
- Full JWT generation (RSA-signed)
- Access token retrieval from Salesforce
- Token refresh and validation
- Config-driven setup (JSON + PEM certificate)
- Legacy-safe implementation for UiPath 2021+

Repository path:  
```
UiPath-JWT-Auth/
```

---

## 🪟 Windows-Legacy Compatibility

All `.xaml` files are written for **Windows-Legacy** UiPath projects using the **.NET Framework 4.6.1** runtime.

> ❗ Coded workflows (.cs files run directly in UiPath) are **not supported** in Windows-Legacy.  
> You can, however, invoke compiled DLL methods or use inline **Invoke Code** activities.

---

## 🧱 Project Structure

```
UiPath-JWT-Auth/
├── Main.xaml
├── project.json
├── Activities/
│   ├── GetSalesforceToken.xaml
│   ├── RefreshToken.xaml
│   ├── ValidateToken.xaml
│   ├── LoadConfiguration.xaml
│   └── HandleError.xaml
├── Code/
│   ├── JWTGenerator.cs
│   ├── TokenParser.cs
├── Config/
│   ├── JWT-Config.json
│   └── Certificate.pem.example
├── Examples/
│   ├── BasicUsage.xaml
│   ├── AdvancedUsage.xaml
│   └── MigrationExample.xaml
└── Libraries/
    └── SalesforceJWT.dll
```

---

### 🔄 Workflow Flow Overview

```text
                ┌────────────────────────────┐
                │        Main.xaml           │
                └────────────┬───────────────┘
                             │
                             ▼
             ┌───────────────────────────────┐
             │   LoadConfiguration.xaml      │
             └────────────┬──────────────────┘
                          │
                          ▼
             ┌───────────────────────────────┐
             │   GetSalesforceToken.xaml     │
             └────────────┬──────────────────┘
                          │
                          ▼
             ┌───────────────────────────────┐
             │   ValidateToken.xaml          │
             └───────────────────────────────┘
                          │
                          ▼
             ┌───────────────────────────────┐
             │   HandleError.xaml            │
             │   (invoked on errors)         │
             └───────────────────────────────┘
```

---

## ⚙️ Required Packages

### 🧩 UiPath Activity Packages (install via Manage Packages)

| Package | Purpose |
|----------|----------|
| **UiPath.System.Activities** | Core UiPath activities including HttpRequest, LogMessage, InvokeWorkflowFile |

> This package is required to open and run any `.xaml` file without "missing activity" errors.  
> Without it, UiPath may interpret the project as Windows (.NET 6).

### 📄 Project Configuration

The `project.json` file is **critical** for Windows-Legacy compatibility:

```json
{
  "projectProfile": "WindowsLegacy",
  "modernBehavior": false,
  "dependencies": {
    "UiPath.System.Activities": "24.10.3"
  }
}
```

> **Important**: This file ensures UiPath opens the project in Windows-Legacy mode instead of Windows mode.  
> Without it, the project may open in the wrong compatibility mode and cause parsing errors.

### 🧰 .NET Libraries (for C# code)

These are used in the C# helper files for JWT generation and parsing:

- `System.IdentityModel.Tokens.Jwt`
- `Microsoft.IdentityModel.Tokens`
- `Newtonsoft.Json`
- `System.Security.Cryptography`

---

## 🔧 Configuration

All configuration is handled via the `Config` folder.

### Example layout

```
Config/
├── JWT-Config.json
└── Certificate.pem
```

### Example `JWT-Config.json`

```json
{
  "Salesforce": {
    "LoginUrl": "https://login.salesforce.com",
    "ClientId": "YOUR_CONNECTED_APP_CLIENT_ID",
    "Username": "YOUR_SALESFORCE_USERNAME",
    "PrivateKeyPath": "Config/Certificate.pem",
    "TokenEndpoint": "/services/oauth2/token"
  },
  "Token": {
    "ExpiryBuffer": 300,
    "MaxRetries": 3,
    "RetryDelay": 5000
  },
  "Logging": {
    "Level": "Info",
    "EnableDetailedLogging": true
  }
}
```

> The `.pem` file contains the private key that signs your JWT.  
> Make sure the path matches exactly what's defined in your JSON file.

### Configuration Sections

| Section | Purpose | Required |
|---------|---------|----------|
| **Salesforce** | Core authentication settings | ✅ Yes |
| **Token** | Token management and retry settings | ❌ Optional |
| **Logging** | Logging configuration | ❌ Optional |

#### Salesforce Section
- `LoginUrl`: Salesforce login URL (production or sandbox)
- `ClientId`: Connected App Client ID from Salesforce
- `Username`: Salesforce username for JWT assertion
- `PrivateKeyPath`: Path to PEM certificate file
- `TokenEndpoint`: OAuth2 token endpoint (usually `/services/oauth2/token`)

#### Token Section (Optional)
- `ExpiryBuffer`: Seconds before expiry to consider token invalid (default: 300)
- `MaxRetries`: Maximum retry attempts for failed requests (default: 3)
- `RetryDelay`: Delay between retries in milliseconds (default: 5000)

#### Logging Section (Optional)
- `Level`: Log level (Info, Debug, Error, etc.)
- `EnableDetailedLogging`: Enable detailed exception logging (default: true)

---

## ▶️ Running the Workflows

### Step 1: Open the Project
Open `UiPath-JWT-Auth` in UiPath Studio and ensure:
- Compatibility = **Windows-Legacy**
- Required packages are installed

### Step 2: Configure Files
Drop your `.pem` and `JWT-Config.json` into the `Config` folder.

### Step 3: Run
Open and run `Examples/BasicUsage.xaml`.  
This workflow will:
1. Load configuration from `JWT-Config.json`  
2. Read the private key from PEM file  
3. Generate the JWT using RSA signing  
4. Exchange JWT for Salesforce access token  
5. Validate the token  
6. Log results and handle any errors

---

## 📂 Workflow Descriptions & Usage Order

| File | Purpose | Order |
|------|----------|--------|
| **BasicUsage.xaml** | Runs the core JWT authentication sequence using config values. | ① |
| **AdvancedUsage.xaml** | Adds retry logic, configurable delays, and enhanced logging. | ② |
| **RefreshToken.xaml** | Refreshes expired Salesforce access tokens. | ③ |
| **ValidateToken.xaml** | Validates tokens before API calls. | ④ |
| **LoadConfiguration.xaml** | Loads and validates configuration from JSON file. | 🔁 |
| **HandleError.xaml** | Centralized reusable workflow for catching, logging, and rethrowing authentication and HTTP errors. | 🔁 |
| **MigrationExample.xaml** | Demonstrates replacing SOAP-based login with JWT. | Optional |

---

## 🧩 Using the C# Code

The `Code` directory contains reusable C# logic for JWT generation and response parsing.

### Option A — Compile as a DLL (Recommended)

You can build a DLL and call its methods from UiPath using an **Invoke Method** activity.

#### 1. Build with .NET CLI
```bash
mkdir SalesforceJWT
cd SalesforceJWT
dotnet new classlib -f netstandard2.0 -n SalesforceJWT
cd SalesforceJWT
```

Replace contents of `SalesforceJWT.csproj` with:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net461</TargetFramework>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <RootNamespace>SalesforceJWT</RootNamespace>
    <AssemblyName>SalesforceJWT</AssemblyName>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="6.34.0" />
    <PackageReference Include="Microsoft.IdentityModel.Tokens" Version="6.34.0" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  </ItemGroup>
</Project>
```

Copy both `JWTGenerator.cs` and `TokenParser.cs` into the folder, then build:
```bash
dotnet build -c Release
```
Result: `bin\Release\net461\SalesforceJWT.dll`

#### 2. Add to UiPath
Copy `SalesforceJWT.dll` into your UiPath project folder (e.g., `Libraries\SalesforceJWT.dll`).  
In **Invoke Method**:

| Property | Value |
|-----------|--------|
| **TargetType** | `SalesforceJWT.JWTGenerator` |
| **MethodName** | `GenerateJWT` |
| **In Arguments** | clientId, username, loginUrl, privateKey |
| **Out Argument** | jwtToken |

---

### Option B — Use Invoke Code (Inline Alternative)

If you cannot compile the DLL, you can embed similar logic using an **Invoke Code** activity.

Steps:
1. Read PEM file using `Read Text File`
2. Read config using `Deserialize JSON`
3. Inside Invoke Code, create RSA signing and build the JWT manually
4. Return the JWT string as output

> This approach works directly in Legacy projects but is harder to maintain than the DLL method.

---

## ⚠️ Error Handling Integration

`HandleError.xaml` is located under `Activities/` and acts as a shared workflow to catch and manage runtime exceptions.

- Automatically invoked by `Main.xaml` and `GetSalesforceToken.xaml`.
- Handles network issues, authentication failures, and bad HTTP responses.
- Logs all exceptions using `Log Message` and throws them back to the parent for controlled handling.

If you create new workflows that perform Salesforce API calls, invoke it via:

```
Activities/HandleError.xaml
```

and wire its arguments (`Exception`, `Context`, `StatusCode`, `ResponseBody`, `EnableDetailedLogging`) accordingly.

---

## 🧠 Troubleshooting

| Issue | Cause | Fix |
|--------|--------|-----|
| Missing activities | UiPath.System.Activities not installed | Install package via Manage Packages |
| Workflow opens as "Windows" type | Project compatibility not set to Legacy | Set Compatibility = Windows-Legacy |
| JWT fails to generate | Incorrect PEM format or invalid JSON path | Verify `JWT-Config.json` and PEM file format |
| Configuration not loading | Missing required fields in JSON | Ensure all required fields are present in config |
| Token validation fails | Invalid token or expired credentials | Check token expiry and Salesforce credentials |
| Missing DLL reference | File not in project path | Add DLL manually and rebuild dependencies |

---

## ✅ Summary

- Compatible with **Windows-Legacy** UiPath projects  
- Requires only `UiPath.System.Activities`  
- Supports both **DLL invocation** and **inline Invoke Code** methods  
- Fully configurable via JSON and PEM files  
- Prebuilt example XAMLs included for fast setup  
- Includes centralized, reusable **HandleError.xaml** workflow
- Built-in configuration loading with **LoadConfiguration.xaml**

---

© 2025 UiPath JWT Authentication Library  
Licensed for educational and internal automation use.
