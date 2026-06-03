# DsGestor.Api

C# .NET API with Oracle Database, Clean Architecture and TOTVS WinThor ERP integration.

## Overview

DsGestor.Api is a portfolio version of a real business API developed by Dinâmica SYS.  
The project demonstrates the structure, architecture and patterns used in a real ERP integration environment.

Sensitive data, credentials, customer information, production endpoints and proprietary business details were removed or replaced with sample values.

## Business Context

This API was designed to support business operations, ERP integrations and management workflows connected to Oracle Database and TOTVS WinThor environments.

## Main Features

- C# .NET API
- Oracle Database integration
- TOTVS WinThor ERP integration
- Clean Architecture
- Repository pattern
- Service/Application layer
- Domain layer separation
- REST API structure
- Dependency Injection
- Portfolio-safe configuration using `appsettings.Example.json`

## Architecture

The solution follows Clean Architecture principles:

- **Domain**: entities and core business rules
- **Application**: use cases, services, DTOs and interfaces
- **Infrastructure**: database access, repositories and external integrations
- **API**: controllers, endpoints, dependency injection and application startup

## Project Structure

```text
DsGestor.Api
DsGestor.Application
DsGestor.Domain
DsGestor.Infrastructure
