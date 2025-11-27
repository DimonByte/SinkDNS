# SinkDNS
SinkDNS is a Windows-based DNSCrypt tool that interfaces with the DNSCrypt service to provide encrypted DNS traffic filtering that has auto-updating blocklists. It allows users to block unwanted domains via host files or individual user defined domains while maintaining the ability to bypass filtering when needed. The tool supports customizable blocklists and provides options for automatic updates of blocklists.

## Features

### Host File Management
- Manages blocklists and whitelists for domain blocking from a user defined URL list
- Downloads all blocklists and whitelists automatically
- Supports merging multiple host files into a single configuration
- Automatically backs up existing host files before modifications
- Allows custom domains to be added to block or whitelist

### Easy Disable
- Simple mechanism to bypass filtering by restoring original DNS settings if a websites functionality is broken (To be implemented)
- Automatically saves previous DNS configuration for restoration (To be implemented)

## Requirements

- Windows operating system
- .NET Core runtime
- Administrative privileges for DNS configuration changes
- DNSCrypt service (to be downloaded and configured automatically in future versions)

## Installation

Currently, this is an early development project without a formal installation process. In the future, the tool will automatically download and configure DNSCrypt if it is not already present on the system.

## Configuration

### Blocklists
Blocklists are configured through the configuration files located in the `config` directory. Users can specify which blocklists to use and enable automatic updates.

## Usage

### System Tray Integration
Notifications will appear in the system tray for important events and status updates.

### Host File Management
The tool manages host files located in the `hostfiles/` directory, with separate subdirectories for blocklists and whitelists.

## Directory Structure

- `logs/` - Contains application log files with timestamped entries
- `config/` - Configuration files for DNSCrypt settings and blocklist management
- `resolvers/` - DNS resolver configuration files
- `hostfiles/` - Host file management directory
- `backup/` - Backup copies of important configuration files
- `hostfiles/blocklist/` - Blocklist files
- `hostfiles/whitelist/` - Whitelist files

## Technical Details

### Architecture
The application uses a modular architecture with separate components for:
- DNSCrypt service management and integration
- Host file management and updates
- System network configuration
- Logging and notification systems
- Configuration management and scheduling
- File I/O and backup operations

### Core Components
- `DownloadManager` - Handles downloading of DNSCrypt and blocklists
- `UpdateManager` - Manages version checking and updates
- `IOManager` - Handles file operations and directory management
- `NotificationManager` - Provides system tray notifications
- `SystemNetworkManager` - Manages DNS configuration changes
- `TraceLogger` - Provides detailed logging with caller information
- `TaskScheduler` - Manages scheduled tasks for automatic updates

## Contributing

Contributions to SinkDNS are welcome. Please submit pull requests or issues through the GitHub repository.

## License

This project is licensed under the MIT License - see the LICENSE file for details.