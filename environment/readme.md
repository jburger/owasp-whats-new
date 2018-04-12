# Centralised logging server example

This folder contains an example Graylog2 server setup for Docker CE

It will help you play with graylog and maybe help show your team how awesome centralised, correlated logging is.

> PLEASE NOTE: 
> This compose script isn't setup for production. 
> It doesn't mount any volumes, so it has amnesia..
> It doesn't load balance elastic search, mongodb or the web tier so it isn't likely to be very resilient to transient failures. YMMV 

## How to use this

1. Install Docker CE for your environment, cd into this directory
2. docker-compose up
3. browse to localhost:9000
4. send GELF compliant UDP packets laced with log data to 12201/udp
