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
├── Activities/
│   ├── GetSalesforceToken.xaml
│   ├── RefreshToken.xaml
│   ├── ValidateToken.xaml
│   ├── ErrorHandling.xaml
├── Code/
│   ├── JWTGenerator.cs
│   ├── TokenParser.cs
├── Config/
│   ├── JWT-Config.json
│   └── Certificate.pem.example
├── Examples/
│   ├── BasicUsage.xaml
│   ├── AdvancedUsage.xaml
│   ├── MigrationExample.xaml
│   ├── ProductionExample.xaml
└── Integration/
    └── ReplaceSOAPAuth.xaml
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
             │   GetSalesforceToken.xaml     │
             └────────────┬──────────────────┘
                          │
          ┌───────────────┼──────────────────────────────┐
          │               │                              │
          ▼               ▼                              ▼
 ┌────────────────┐ ┌────────────────┐          ┌────────────────┐
 │ ValidateToken  │ │ RefreshToken   │          │ ErrorHandling  │
 │ .xaml          │ │ .xaml          │          │ .xaml          │
 └──────┬─────────┘ └──────┬─────────┘          └───────┬────────┘
        │                  │                            │
        └──────────┬───────┘                            │
                   ▼                                    │
          ┌────────────────────┐                        │
          │ ProductionExample  │<───────────────────────┘
          │ .xaml              │
          └────────────────────┘
```

---

## ⚙️ Required Packages

### 🧩 UiPath Activity Packages (install via Manage Packages)

| Package | Purpose |
|----------|----------|
| **UiPath.System.Activities** | Core activities such as LogMessage, Assign, If, InvokeWorkflowFile |
| **UiPath.WebAPI.Activities** | Provides HttpRequest activity used for REST and JWT calls |

> These two are required to open and run any `.xaml` file without “missing activity” errors.  
> Without them, UiPath may interpret the project as Windows (.NET 6).

### 🧰 .NET Libraries (for C# code)

These are used in the C# helper files for JWT generation and parsing:

- `System.IdentityModel.Tokens.Jwt`
- `Microsoft.IdentityModel.Tokens`
- `System.Text.Json`
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
  }
}
```

> The `.pem` file contains the private key that signs your JWT.  
> Make sure the path matches exactly what’s defined in your JSON file.

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
1. Read the config file  
2. Load the private key  
3. Generate the JWT  
4. Request a Salesforce access token  
5. Log and return the response

---

## 📂 Workflow Descriptions & Usage Order

| File | Purpose | Order |
|------|----------|--------|
| **BasicUsage.xaml** | Runs the core JWT authentication sequence using config values. | ① |
| **AdvancedUsage.xaml** | Adds retry logic, configurable delays, and enhanced logging. | ② |
| **RefreshToken.xaml** | Refreshes expired Salesforce access tokens. | ③ |
| **ValidateToken.xaml** | Validates tokens before API calls. | ④ |
| **ErrorHandling.xaml** | Centralized reusable workflow for catching, logging, and rethrowing authentication and HTTP errors. Invoked by other workflows. | 🔁 |
| **ProductionExample.xaml** | End-to-end orchestration combining validation, refresh, and error handling. | ⑤ |
| **MigrationExample.xaml** | Demonstrates replacing SOAP-based login with JWT. | Optional |
| **ReplaceSOAPAuth.xaml** | Legacy reference (commented-out SOAP example). | — |

---

## 🧩 Using the C# Code

The `Code` directory contains reusable C# logic for JWT generation and response parsing.

### Option A — Compile as a DLL (Recommended)

You can build a DLL and call its methods from UiPath using an **Invoke Method** activity.

#### 1. Build with .NET CLI
```bash
mkdir SalesforceJWT
cd SalesforceJWT
dotnet new classlib -f net461 -n SalesforceJWT
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
    <PackageReference Include="System.Text.Json" Version="7.0.3" />
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

`ErrorHandling.xaml` is now located under `Activities/` and acts as a shared workflow to catch and manage runtime exceptions.

- Automatically invoked by `ProductionExample.xaml` and `AdvancedUsage.xaml`.
- Handles network issues, authentication failures, and bad HTTP responses.
- Logs all exceptions using `Log Message` and throws them back to the parent for controlled handling.

If you create new workflows that perform Salesforce API calls, invoke it via:

```
Activities/ErrorHandling.xaml
```

and wire its arguments (`AccessToken`, `StatusCode`, `ErrorMessage`, etc.) accordingly.

---

## 🧠 Troubleshooting

| Issue | Cause | Fix |
|--------|--------|-----|
| Missing activities | UiPath.System or UiPath.WebAPI not installed | Install packages via Manage Packages |
| Workflow opens as “Windows” type | Project compatibility not set to Legacy | Set Compatibility = Windows-Legacy |
| JWT fails to generate | Incorrect PEM or invalid JSON path | Verify `JWT-Config.json` and file names |
| Missing DLL reference | File not in project path | Add DLL manually and rebuild dependencies |

---

## ✅ Summary

- Compatible with **Windows-Legacy** UiPath projects  
- Requires only `UiPath.System.Activities` and `UiPath.WebAPI.Activities`  
- Supports both **DLL invocation** and **inline Invoke Code** methods  
- Fully configurable via JSON and PEM files  
- Prebuilt example XAMLs included for fast setup  
- Includes centralized, reusable **ErrorHandling.xaml** workflow

---

© 2025 UiPath JWT Authentication Library  
Licensed for educational and internal automation use.
