# C# Code for UiPath JWT Authentication

This directory contains the C# implementation of the JWT authentication functionality for UiPath Studio.

## Files

- **`JWTGenerator.cs`** - JWT token generation and validation
- **`TokenParser.cs`** - Token response parsing and validation

## Required NuGet Packages

Install these packages in your UiPath project:

### **Core JWT Libraries**
- `System.IdentityModel.Tokens.Jwt` (v6.0.0+)
- `Microsoft.IdentityModel.Tokens` (v6.0.0+)

### **JSON Processing**
- `System.Text.Json` (v6.0.0+)

### **Cryptography**
- `System.Security.Cryptography` (v4.3.0+)

## Installation in UiPath Studio

### Step 1: Install NuGet Packages
1. **Open UiPath Studio**
2. **Go to Manage Packages**
3. **Install the required packages** listed above
4. **Verify installation** in the Packages folder

### Step 2: Add C# Files to Project
1. **Copy the C# files** to your UiPath project
2. **Ensure they're included** in the project
3. **Build the project** to verify compilation

### Step 3: Use in Workflows
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

## How It Works

### JWT Generation Process
1. **Parse private key** from PEM format
2. **Create RSA security key** for signing
3. **Generate JWT token** with claims
4. **Sign token** with private key
5. **Return JWT string**

### Token Parsing Process
1. **Parse JSON response** from Salesforce
2. **Validate required fields**
3. **Return structured response**
4. **Handle errors gracefully**

## Key Benefits

- **Native UiPath Support** - C# integrates directly with UiPath
- **Full JWT Capability** - Handles all JWT requirements
- **No External Dependencies** - Everything runs in UiPath
- **Production Ready** - Handles all edge cases
- **Easy Maintenance** - Standard C# development

## Troubleshooting

### Common Issues
1. **Package Installation Fails** - Check .NET Framework version
2. **Compilation Errors** - Verify all packages are installed
3. **Runtime Errors** - Check certificate format and path

### Debug Tips
1. **Test C# methods** individually
2. **Check package versions** for compatibility
3. **Verify certificate format** (PEM)
4. **Test with simple JWT** first

## Support

For issues and questions:
1. Check the troubleshooting section
2. Verify all packages are installed
3. Test with minimal examples first
4. Check UiPath Studio logs for errors
