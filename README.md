# owasp-presentation
ADNUG presentation - 'Whats New in OWASP 2017'

> WARNING: THIS IS DELIBERATELY VULNERABLE SOURCE CODE AND SHOULD ONLY BE USED 
> TO LEARN ABOUT BROKEN SECURITY PATTERNS

## How this repo works
This repo is tagged at different stages of development to help you see something go from completely vulnerable to being less completely vulnerable.

The way I demonstrated this was to deploy tag v1.0 to a guest linux VM, and then attack this from a seperate VM, but you could totally just do all of that on your development machine if thats how you want to roll.

I then just checked out at the various places that are represented by the version tags you'll read about below, in order to show the fixes and talk through the code changes

### Backstory
Piggy Bank Co. is a fictional business that came up with an awesome idea: payday loans with cryptocurrency on a machine learning platform with pictures of cute pigs.

*Their plan was simple: You give us your latest bank statement, and we will lend you money if we think you can handle it.*

They didn't quite figure out the machine learning part, and they ran out of money coming up with an expensive, yet awesome website branding.

They've asked us nicely to give them a free security code review. Do you accept?

#### Y U No React/Angular/Vue?
> I needed to get this out quickly and aspnet mvc is home turf for me, however I also wanted to make the security architecture the focus here, and I think the simplicity of the front end to show the flaws off clearly is important. It's a 'back to basics' thing.

### Release History
#### v1.0
This is their first cut, there are several vulnerabilities here and the code is atrocious!! Almost everything is so very wrong. Inspired by anti-patterns I've seen on bug bounties, advice on stack overflow, and real world things I've had to fix for people.    

>*Yes, I felt like having a shower after writing this code, but I guess it is for science...*

Check this tag out to begin testing things for yourself

#### v1.0.1
**Bug Fixes**

- fixed up a simple design flaw that allowed for account enumeration, a pre-cursor to Broken Authentication

#### v1.0.2
**Bug Fixes**
- fixes for Insecure Deserialisation, Broken Authentication and Broken Access Control.  Replaced with standard parts of dotnet core to achieve:
    - Claims based Authentication, with Cookies
    - Role based Authorisation policies

#### v1.0.3
**Bug Fixes**
- fixes for Insufficient Logging and Monitoring, using serilog and graylog2

#### v1.0.4
**Bug Fixes**
- fixes for an XXE vulnerability in the bank file processing code.

## Pre-requisites

To use / build /run this you'll need the following installed:

- [.NET Core SDK 2.1.4 or greater - click to goto download](https://www.microsoft.com/net/download/dotnet-core/sdk-2.1.4)

## Content
You'll find the following directories
### vulnerable

This directory contains a misconfigured aspnet core mvc application that has the following weaknesses:

- External Xml Entity vulnerabilities (XXE)
- Broken Authentication
- Insecure Deserialisation
- Broken Access Control
- Insufficient Logging and Monitoring
- Some mystery ones :)


- Apple OS X High Sierra
- Devuan Linux 'Jessie' stable (Debian Jesse without the systemd)
*should work on windows, but tell me about it if it doesn't*

### exploits

This directory contains example payloads to demonstrate the vulnerabilities in the application

> Several of these payloads require you to modify them to suit your target environment

### environment
This directory contains an example centralised logging server, so that we can start to solve A10: Insufficient Logging and Monitoring

### slides
Here you'll find a copy of my slide deck that I used to present this. PDF Format

## Build / Deploy

```
cd vulnerable
# To build
dotnet build

# Developing?
dotnet watch run

# To build and publish
dotnet publish

# To run a published build
# By default it will be published to /bin/Debug/netcoreapp2.0/publish
cd ./bin/Debug/netcoreapp2.0/publish
dotnet vulnerable.dll

# Browse to http://localhost:5000 to begin exploring the application
```
#### Tested on the following platforms
- Mac OS X High Sierra
- Devuan Linux (Jessie Stable)

## Normal operation
1. Click Apply Now
2. Upload example_bank_file.xml to learn if you qualify for an OinkCoin loan