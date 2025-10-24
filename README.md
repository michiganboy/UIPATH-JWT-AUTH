# UiPath JWT Authentication Library

A complete **C#-based** JWT authentication library for UiPath Studio that replaces Salesforce SOAP authentication with secure JWT Bearer Token flow. Uses native C# code with NuGet packages for full JWT functionality.

## Features

- ✅ **Native UiPath Support**: C# code integrates directly with UiPath
- ✅ **Full JWT Capability**: Handles all JWT requirements including RSA signing
- ✅ **No External Dependencies**: Everything runs within UiPath
- ✅ **Hands-off Authentication**: No manual login required
- ✅ **Secure**: Uses JWT with private key signing
- ✅ **Automatic Refresh**: Handles token expiry automatically
- ✅ **Drop-in Replacement**: Minimal changes to existing workflows
- ✅ **Production Ready**: Handles all edge cases and errors

## Prerequisites

- UiPath Studio installed
- .NET Framework 4.6.1 or higher
- Salesforce Connected App with JWT enabled
- Private key certificate (.pem file)

## Quick Start

### 1. Install Required NuGet Packages
In UiPath Studio, install these packages:

#### **Core JWT Libraries**
- `System.IdentityModel.Tokens.Jwt` (v6.0.0+)
- `Microsoft.IdentityModel.Tokens` (v6.0.0+)

#### **JSON Processing**
- `System.Text.Json` (v6.0.0+)

#### **Cryptography**
- `System.Security.Cryptography` (v4.3.0+)

### 2. Copy Files to Your UiPath Project
1. **Create a new UiPath project** in UiPath Studio
2. **Copy all XAML files** to your project directory
3. **Copy the Code folder** with C# files
4. **Copy the Config folder** with your settings

### 3. Configure Salesforce
Update `Config/JWT-Config.json`:
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

### 4. Add Your Certificate
Place your `.pem` certificate file in `Config/Certificate.pem`

### 5. Test the Library
Run `Examples/BasicUsage.xaml` to test JWT authentication

## Project Structure

```
UiPath-JWT-Auth/
├── Main.xaml                           # Main workflow orchestrator
├── Activities/
│   ├── GetSalesforceToken.xaml         # JWT token generation
│   ├── RefreshToken.xaml                # Token refresh logic
│   └── ValidateToken.xaml               # Token validation
├── Code/
│   ├── JWTGenerator.cs                 # JWT generation logic
│   ├── TokenParser.cs                  # Response parsing
│   └── README.md                       # Code documentation
├── Config/
│   ├── JWT-Config.json                 # Configuration file
│   └── Certificate.pem.example         # Certificate template
├── Examples/
│   ├── BasicUsage.xaml                 # Simple usage example
│   ├── AdvancedUsage.xaml              # Advanced with retry logic
│   ├── MigrationExample.xaml           # SOAP to JWT migration
│   ├── ErrorHandling.xaml              # Comprehensive error handling
│   ├── ProductionExample.xaml          # Production-ready implementation
└── Integration/
    └── ReplaceSOAPAuth.xaml             # Migration example
```

## How It Works

### **C# + UiPath Integration**
- **C# code** handles JWT generation and parsing
- **UiPath workflows** orchestrate the authentication process
- **NuGet packages** provide JWT and cryptographic functionality
- **Native integration** through InvokeMethod activities

### **JWT Generation Process**
1. **C# JWTGenerator** creates JWT token with RSA signing
2. **UiPath workflow** calls the C# method
3. **HTTP request** exchanges JWT for access token
4. **C# TokenParser** processes Salesforce response
5. **UiPath workflow** uses the access token for API calls

## Usage

### Basic Authentication
```xml
<InvokeMethod DisplayName="Generate JWT" MethodName="GenerateJWT">
  <InvokeMethod.TargetObject>
    <InArgument x:TypeArguments="x:Object">[New SalesforceJWT.JWTGenerator()]</InArgument>
  </InvokeMethod.TargetObject>
  <InvokeMethod.Parameters>
    <Argument x:TypeArguments="x:String" Name="clientId" Value="[ClientId]" />
    <Argument x:TypeArguments="x:String" Name="username" Value="[Username]" />
    <Argument x:TypeArguments="x:String" Name="loginUrl" Value="[LoginUrl]" />
    <Argument x:TypeArguments="x:String" Name="privateKey" Value="[PrivateKey]" />
  </InvokeMethod.Parameters>
  <InvokeMethod.Result>
    <OutArgument x:TypeArguments="x:String">[JWT]</OutArgument>
  </InvokeMethod.Result>
</InvokeMethod>
```

### Making API Calls
```xml
<HttpRequest DisplayName="Make API Call" Method="GET" Url="[InstanceUrl + '/services/data/v58.0/sobjects/Account/describe']">
  <HttpRequest.Headers>
    <Dictionary x:TypeArguments="x:String, x:String">
      <KeyValuePair x:Key="Authorization" x:Value="[&quot;Bearer &quot; + AccessToken]" />
      <KeyValuePair x:Key="Content-Type" x:Value="application/json" />
    </Dictionary>
  </HttpRequest.Headers>
</HttpRequest>
```

### Token Refresh
```xml
<InvokeWorkflowFile DisplayName="Refresh Token" WorkflowFileName="Activities\RefreshToken.xaml">
  <Arguments>
    <Argument x:TypeArguments="x:String" Name="CurrentToken" Value="[AccessToken]" />
    <Argument x:TypeArguments="x:DateTime" Name="TokenExpiry" Value="[TokenExpiry]" />
    <Argument x:TypeArguments="x:Boolean" Name="IsRefreshed" Value="[IsRefreshed]" />
    <Argument x:TypeArguments="x:String" Name="NewToken" Value="[NewToken]" />
    <Argument x:TypeArguments="x:DateTime" Name="NewExpiry" Value="[NewExpiry]" />
  </Arguments>
</InvokeWorkflowFile>
```

## Migration from SOAP

### Before (SOAP)
```xml
<HttpRequest>
  <Headers>
    <Header Name="SOAPAction" Value="login" />
  </Headers>
</HttpRequest>
```

### After (JWT)
```xml
<HttpRequest>
  <Headers>
    <Header Name="Authorization" Value="Bearer " + AccessToken />
  </Headers>
</HttpRequest>
```

## Examples

### 1. BasicUsage.xaml
Simple JWT authentication and API call
- Basic token retrieval
- Simple API call
- Response validation

### 2. AdvancedUsage.xaml
Advanced authentication with retry logic
- JWT authentication with retry
- Token refresh checking
- API call retry mechanism

### 3. MigrationExample.xaml
SOAP to JWT migration demonstration
- Side-by-side comparison
- Header replacement examples
- Migration patterns

### 4. ErrorHandling.xaml
Comprehensive error handling
- JWT authentication errors
- Token validation errors
- API call errors
- Specific error codes (401, 403, etc.)

### 5. ProductionExample.xaml
Production-ready implementation
- Retry logic for authentication
- Automatic token refresh
- Comprehensive error handling
- Production logging

## Configuration

### JWT-Config.json
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

## Error Handling

### Try-Catch Blocks
```xml
<TryCatch DisplayName="JWT Authentication with Error Handling">
  <Try>
    <InvokeMethod DisplayName="Generate JWT" MethodName="GenerateJWT">
      <InvokeMethod.TargetObject>
        <InArgument x:TypeArguments="x:Object">[New SalesforceJWT.JWTGenerator()]</InArgument>
      </InvokeMethod.TargetObject>
    </InvokeMethod>
  </Try>
  <Catch>
    <LogMessage DisplayName="Error Logging" Level="Error" Message="[Exception.Message]" />
  </Catch>
</TryCatch>
```

### Retry Logic
```xml
<RetryScope DisplayName="JWT Authentication Retry Logic">
  <MaxRetryNumber>3</MaxRetryNumber>
  <RetryInterval>00:00:10</RetryInterval>
  <InvokeMethod DisplayName="Generate JWT" MethodName="GenerateJWT">
    <InvokeMethod.TargetObject>
      <InArgument x:TypeArguments="x:Object">[New SalesforceJWT.JWTGenerator()]</InArgument>
    </InvokeMethod.TargetObject>
  </InvokeMethod>
</RetryScope>
```

## Installation Guide

### Step 1: Create New UiPath Project
1. Open UiPath Studio
2. Create new project: `Salesforce-JWT-Auth`
3. Set project type to **Library** or **Process**
4. Choose .NET Framework 4.6.1 or higher

### Step 2: Install NuGet Packages
1. **Go to Manage Packages** in UiPath Studio
2. **Install the required packages**:
   - `System.IdentityModel.Tokens.Jwt` (v6.0.0+)
   - `Microsoft.IdentityModel.Tokens` (v6.0.0+)
   - `System.Text.Json` (v6.0.0+)
   - `System.Security.Cryptography` (v4.3.0+)

### Step 3: Copy Files
1. **Copy all XAML files** to your project directory
2. **Copy Code folder** with C# files
3. **Copy Config folder** with your settings
4. **Add your certificate** to Config folder

### Step 4: Configure Salesforce
1. Create Connected App in Salesforce
2. Enable JWT authentication
3. Upload your certificate
4. Update configuration file

### Step 5: Test Implementation
1. Run `Examples/BasicUsage.xaml`
2. Check logs for errors
3. Verify token generation

## Troubleshooting

### Common Issues

1. **Package Installation Fails**
   - Check .NET Framework version (4.6.1+)
   - Verify internet connection
   - Try installing packages individually

2. **Certificate Issues**
   - Verify .pem file format
   - Check file path in configuration
   - Ensure certificate is properly formatted

3. **Authentication Failures**
   - Verify Connected App settings
   - Check username and client ID
   - Ensure certificate matches Connected App

4. **C# Compilation Errors**
   - Verify all NuGet packages are installed
   - Check .NET Framework version
   - Ensure C# files are included in project

### Debug Steps

1. **Enable detailed logging** in configuration
2. **Test C# methods** individually
3. **Check package versions** for compatibility
4. **Verify certificate format** (PEM)
5. **Test with simple JWT** first

## Best Practices

### 1. Always Use Error Handling
- Wrap JWT authentication in try-catch blocks
- Handle specific error codes
- Implement retry logic for transient failures

### 2. Check Token Refresh
- Always check if token needs refresh before API calls
- Implement automatic token refresh
- Handle token expiry gracefully

### 3. Log Everything
- Log authentication attempts
- Log API call results
- Log error details
- Use appropriate log levels

### 4. Test Thoroughly
- Test with valid credentials
- Test with invalid credentials
- Test network failures
- Test token expiry scenarios

## Key Benefits

- **Native UiPath Support** - C# integrates directly with UiPath
- **Full JWT Capability** - Handles all JWT requirements
- **No External Dependencies** - Everything runs in UiPath
- **Production Ready** - Handles all edge cases
- **Easy Maintenance** - Standard C# development

## Support

For issues and questions:
1. Check the troubleshooting section
2. Review the error logs
3. Verify configuration settings
4. Test with a simple workflow first

## License

This library is provided as-is for internal use. Please ensure compliance with Salesforce terms of service and your organization's security policies.