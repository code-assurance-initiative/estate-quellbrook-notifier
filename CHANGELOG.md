# Changelog

All notable changes to this project are documented here. The format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and the project uses [Semantic Versioning](https://semver.org/).

## [0.2.0] - 2026-08-21

### Added
- SMS through the SMS gateway.
- "Arrives today" (e-mail and SMS) and "delivered" (e-mail) notifications from the dispatch events.

### Fixed
- A unit test of the consumer could stop it before it had started.

## [0.1.0] - 2026-08-07

### Added
- Booking confirmations by e-mail for `orders.order-placed.v1`, through an inbox and a notification log.
- Worker host with heartbeat probes, container image, manifests, CI, CodeQL, release and deploy workflows.
