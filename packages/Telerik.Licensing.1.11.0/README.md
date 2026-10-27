Telerik.Licensing
=================

This tool/software/dependency shall be used ONLY in combination with a respective 
Progress product/bundle. It is part of Progress' proprietary software package and 
is protected by the respective EULA of the product it is part of.

For license information, visit: https://www.telerik.com/purchase/license-agreements

Description
------------------

An utility package used to detect Telerik products and implement build and runtime license validation.

Part of the [Telerik DevCraft](https://www.telerik.com/devcraft) product suite.

Please, read and accept the [End User License Agreement](https://www.telerik.com/purchase/license-agreement/progress-devcraft-ui-developer-license) before using this product.

License Activation
------------------

> [To download a license key file, visit https://www.telerik.com/account/your-licenses/license-keys](https://www.telerik.com/account/your-licenses/license-keys)

The packages comes with two major parts that facilitate build time and runtime license validation:
 - `Telerik.Licensing.Tasks.dll` - MSBuild task
 - `Telerik.Licensing.Runtime.dll` - runtime library


Build Task
----------

At compile time, the build task will evaluate the references in your project, and will query license information from the Telerik licensing file stored on your developer machine. The task will then embed a runtime key using code generation.

The Runtime
-----------

At runtime, Telerik components will query `Telerik.Licensing.Runtime.dll` about the license state.

FAQ
---

See the [Purchasing & Licensing FAQs](https://www.telerik.com/purchase/faq/licensing-purchasing) and the Licensing documentation of the specific Telerik product you're using.

Known Issues
------------

* Automatic license activation is not supported for [ASP.NET Web Site Projects](https://learn.microsoft.com/en-us/previous-versions/aspnet/dd547590(v=vs.110)). See the [Telerik UI for ASP.NET Ajax - Installing a License Key in Projects without NuGet References](https://www.telerik.com/products/aspnet-ajax/documentation/licensing/license-key#installing-a-license-key-in-projects-without-nuget-references).

## Change Log

### 1.9.3
 - fix: Add missing binding-redirect detection during .NET Framework builds

### 1.9.2
 - fix: Performance improvements

### 1.8.0
 - fix: dotnet watch in 10.0.201
 - fix: Support plugins NWPlugin, Roamer
 - fix: Performance issues due to hanging process on error

### 1.7.6
 - fix: Log to obj/Debug/*

### 1.7.5
 - fix: Runtime differentiate between missing: license vs product in license
 - fix: Spawned process sometimes stall on stderr

### 1.7.0
 - feat: Portable report server messages
 - feat: Telerik.Product ontology attributes

### 1.6.36
 - fix: Read version agnostic attributes and avoid loading unused dependencies
 - fix: Runtime diagnostics will log assemblies in AppDomain

### 1.6.35
 - fix: Add support for MicroStation plugins

### 1.6.33
 - fix: Add support for PRIMAVERA Executive ERP plugins

### 1.6.32
 - fix: Add net10.0 to multitarget

### 1.6.31
 - fix: Add support for PABLO.AddIn and PFCLScan plugins

### 1.6.29
 - fix: Add support for AutoCAD plugins

### 1.6.26
 - fix: Add support for plugins of within Sentinel License Development Kit

### 1.6.21
 - fix: Add support for ReSharper test runner

### 1.6.20
 - feat: Add runtime diagnostic capabilities

### 1.6.17
 - fix: Add support for plugins of Microsoft Dynamics 365, TopSolid, Mercator

### 1.6.9
 - fix: Use culture invariant timestamps
 - fix: Add the entry assembly name to license setup notice

### 1.6.8
 - feat: non-SDK style projects will use attributes to detect transitive Telerik dependencies
 - fix: Improved edge-case warning messages

### 1.6.6
 - feat: Use TelerikLicensing.Register() in plugin assembly entrypoints - Microsoft Excel, AutoCAD, AWS Lambda, Azure Function;

### 1.6.1
 - fix: Timeouts
 - feat: Setting `-p:TelerikLicensingStrict=true` command line or `<TelerikLicensingStrict>true</TelerikLicensingStrict>` in msbuild will generate warnings as errors
 - feat: Setting `-p:TelerikLicensingVerbosity=quiet` or `<TelerikLicensingVerbosity>quiet</TelerikLicensingVerbosity> will suppress diagnostics on success

### 1.4.16
 - fix: F# generated code compilation order

### 1.4.15
 - fix: fail to locate ResolveTelerikProducts

### 1.4.14
 - fix: Do not require Telerik.Licensing.Runtime.dll in .NET Framework projects with transitive XAML libraries.

### 1.4.11
 - fix: Multitarget frameworks list: _net6_, _net7_, _net8_, _net9_, _net462_, _netstandard2.0_;
 - fix: Support MSBuild properties with task options: _TelerikLicensingTaskArchitecture_, _TelerikLicensingTaskRuntime_, _TelerikLicensingTaskFactory_;

### 1.4.10
 - fix: WPF command-line MSBuild clean up error after intermediate XAML compilation

### 1.4.9
 - fix: allow TELERIK_LICENSE_PATH to point to a license file for CI
 - fix: watermarks appear in updated MVC used with earlier versions of Kendo UI
 - fix: build times
 - fix: concurrent access to license status

### 1.4.7
 - fix: license activation fails with "TKL001 - No Telerik or Kendo UI product references detected in project" in some cases.
 - fix: interactive prompt is triggered on Windows
