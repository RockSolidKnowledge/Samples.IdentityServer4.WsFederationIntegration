# WS-Federation Integration Samples

This repository contains sample implementations of WS-Federation identity providers using different IdentityServer implementations. These samples demonstrate how to integrate the Rock Solid Knowledge WS-Federation component with both Duende IdentityServer and IdentityServer4.

## Overview

The WS-Federation component is available from [identityserver.com](https://www.identityserver.com/products/ws-federation) and provides WS-Federation protocol support for IdentityServer implementations.

## Sample Projects

### Relying Party
- **rp** - Example relying party application using Microsoft's WS-Federation authentication libraries

### Duende IdentityServer Samples
- **idp** - Basic implementation with in-memory configuration
- **idpWithEf** - Advanced implementation using Entity Framework for persistence

### IdentityServer4 Samples
- **idp** - Basic implementation with in-memory configuration
- **idpWithEf** - Advanced implementation using Entity Framework for persistence

## Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022, VS Code, or JetBrains Rider
- Valid license key for the WS-Federation component

## Quick Start

1. Clone this repository
2. Obtain a license key (see below)
3. Configure the license key in the identity provider projects
4. Run the desired identity provider and relying party projects

## Documentation

- [Getting Started Guide](https://www.identityserver.com/articles/announcing-ws-federation-support-for-identityserver4-and-net-core/)
- [Full Documentation](https://www.identityserver.com/documentation/wsfed/)

## Obtaining a License

You can obtain a license key in one of two ways:
- Sign up for a demo license at our [products page](https://www.identityserver.com/products/ws-federation)
- Contact our sales team at sales@identityserver.com

## Support

For support questions:
- Check our [documentation](https://www.identityserver.com/documentation/wsfed/)
- Contact support@identityserver.com

## License

This sample code is licensed under the Apache License 2.0. See the [LICENSE](LICENSE) file for details.

The WS-Federation component requires a commercial license from Rock Solid Knowledge.
