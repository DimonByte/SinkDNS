<p align="center">
    <img src="SinkDNS\Resources\SinkDNSIconImage.png" alt="SinkDNS" />
</p>

SinkDNS is a Windows-based DNSCrypt tool that interfaces with the DNSCrypt service to provide encrypted DNS traffic filtering that has auto-updating blocklists. It allows users to block unwanted domains via host files or individual user defined domains while maintaining the ability to bypass filtering when needed. The tool supports customizable blocklists and provides options for automatic updates of blocklists.

[![GitHub issues](https://img.shields.io/github/issues/DimonByte/SinkDNS)](https://github.com/DimonByte/SinkDNS/issues)
[![GitHub stars](https://img.shields.io/github/stars/DimonByte/SinkDNS)](https://github.com/DimonByte/SinkDNS/stargazers)
[![GitHub license](https://img.shields.io/github/license/DimonByte/SinkDNS)](https://github.com/DimonByte/SinkDNS/blob/master/LICENSE)
![GitHub CodeQL](https://github.com/DimonByte/SinkDNS/actions/workflows/codeql.yml/badge.svg)
![GitHub DOTNET](https://github.com/DimonByte/SinkDNS/actions/workflows/dotnet.yml/badge.svg)

## Why SinkDNS?

Traditional Windows host files, when filled with hundreds or even millions of entries, slow DNS lookups dramatically, causing delays across the entire system.

Instead of relying on the Windows host file, SinkDNS is a program that integrates directly with  **DNSCrypt‑Proxy**, allowing you to use massive blocklists. And with SinkDNS, your hostlists are updated in the background to ensure you get the latest entries. This makes SinkDNS ideal for:

- Users who want Pi‑hole‑level filtering without needing a dedicated device
- Students or people in restricted networks (e.g., university dorms) who cannot modify router settings
- Anyone who wants encrypted DNS + powerful filtering without running a full DNS server
- People who want a lightweight, Windows-native solution that “just works”

## Features

### DNS Filtering Without Slowdowns
- Handles **millions** of blocked domains without badly degrading DNS performance
- Uses DNSCrypt‑Proxy’s high‑performance filtering instead of the Windows host file

### Blocklist & Whitelist Management
- Auto‑updates blocklists and whitelists on a schedule
- Uses **HostlistDirectory**, another open source project, giving users a curated list of:
  - Adblock lists  
  - Telemetry blockers  
  - Malware/security lists  
  - Tracking and analytics blockers  
- Merge multiple lists into a single configuration and deletes duplicate entries.
- Add custom block or allow entries easily

### Automatic Updates
- Auto‑updates **blocklists**, **whitelists**, and checks for **DNSCrypt‑Proxy** updates.
- When configured, blocklist and whitelist updates occur in the background without restarting DNSCrypt‑Proxy (Changes will be made upon restart)
- Non‑intrusive: no admin prompts unless absolutely necessary (e.g. updating DNSCrypt-Proxy)

### Query Monitoring
- View **blocked** and **allowed** DNS queries
- Helps diagnose issues or identify domains to whitelist

### Easy Bypass Mode
- Instantly disable filtering if a website or app breaks
- Restore previous DNS settings automatically
- Add problematic domains to the whitelist with one click

### Host File Safety
- Automatically backs up existing host files
- Keeps user-defined host entries safe
- Only DNS settings are changed. So SinkDNS is easy to uninstall.

### System Tray Integration
- Clean, lightweight tray icon  
- Notifications for updates, errors, and status changes

### Free & Open Source
- 100% free
- SinkDNS is MIT licensed
- No telemetry, no ads, no data collection, no nonsense

## Requirements

- Windows 10/11 operating system
- .NET Core runtime
- Administrative privileges for DNS configuration changes
- DNSCrypt service (will be installed with SinkDNS setup)
  - **Important:** Do *not* run another DNSCrypt‑Proxy instance — it will conflict. If you have another instance, please uninstall DNSCrypt-Proxy first.

## Installation

Currently, this is an early development project without a formal installation process. In the future, the tool will automatically download and configure DNSCrypt if it is not already present on the system.

## Contributing

Contributions to SinkDNS are welcome. Please submit pull requests or issues through the GitHub repository.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

This project uses other projects and assets that are under different licenses, for example DNSCrypt-Proxy. For the list of credits, visit SinkDNS/Credits.txt or click the "About SinkDNS" button on the system tray icon.