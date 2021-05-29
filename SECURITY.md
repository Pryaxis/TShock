# Security policy

TShock aims to improve Terraria's multiplayer security. Security issues
for us are a little different compared to other projects.

Our "most normal" response criteria for security reports involve TShock
itself. For security issues in TShock, TSAPI, or other Pryaxis
projects, all issues can be reported to us directly, and we'll issue
fixes as appropriate. Depending on the nature of the issue, we will
either issue an update and note the issue in the changelog, or
coordinate a case specific security response.

For example, in [GHSA-q776-cv3j-4q6m](https://github.com/Pryaxis/TShock/security/advisories/GHSA-q776-cv3j-4q6m),
we gave many server operators advanced warning about the issue,
provided server specific patch guidance, and announced the disclosure
in a predictable way. This is because the issue was primarily one
introduced by TShock. Since we attempted to fix a problem with Terraria
and left a gap, we considered it a higher priority to fix and disclose
than other issues we've had reported.

If you operate a server with a large player base, you can contact us for
advanced details about a security vulnerability when we're coordinating
the disclosure. The best way to learn about upcoming issues is to keep
an eye on the announcements category of discussions, and subscribe to
our Discord's announcements feed.

When issues are discovered in the Terraria protocol directly, we add
guards to TShock to prevent their abuse. Depending on the severity of
the issue, we may choose to release versions which account for protocol
defects differently. Because there are so many protocol defects,
running a TShock server (and by extension, a Terraria server) is
inherently risky. Therefore, we strongly advise updating to the latest
versions of TShock at all times.

Some types of issues may not be directly patched by TShock, after
reporting. For example, esoteric attack types may not be disclosed
because they're too difficult to protect against, represent a low risk,
or otherwise may adversely affect servers if disclosed. This is usually
the case with minor protocol defects in Terraria, where patching an
issue may start an "arm's race" or where the attack is theoretical, but
not occurring in practice, and poses minimal risk.

## Supported Terraria protocol versions

TShock maintains protocol patches and associated protection services for
the the most recent versions of Terraria. We may remove protection
mechanisms or update them in response to protocol changes.

| Version | Supported          |
| ------- | ------------------ |
| 1.4.2.1 | :white_check_mark: |
| 1.4.0.5 | :x:                |

It is important to remember that Terraria is a clientside game with
serverside networking "added on." If you're familiar with hosting
Minecraft or other primarily serverside games, please be aware that
integrity cannot be maintained with Terraria in the same way. The
network design has improved over the years, but is fundamentally
difficult to fully secure. Even in a fully patched, supported version
of TShock, protocol defects leading to client and server crashes, item
duplication, and denial of service still exist in some way or another.

When feasible, Pryaxis works with Re-Logic to address security issues.
However, due to the nature of these types of issues, we cannot always
disclose the status of certain issues which have been reported to
Re-Logic.

## Supported TShock versions

Beginning with TShock 4.5, versions with odd numbers are
considered "unstable" for the purposes of operating a public server,
and may contain issues with the Terraria protocol in terms of patching,
danger, or other similar things. Versions which are considered "stable"
are even numbered releases, which offer typical security measures.

When running unstable versions of TShock, make regular backups of your
worlds, characters, configurations, and databases. This is because the
Terraria protocol may be dangerous in this version, and data loss may
occur. More commonly, attackers may perform denial of service attacks,
cheat items into the game, or perform other types of griefing on
servers. You stand a better chance to defend against these protocol
issues by using updated versions of TShock that are stable, not
unstable releases.

## Bug bounties

Pryaxis may offer bug bounties for defects found in Terraria or TShock,
but this is evaluated on a case by case basis. Bounties should not be
expected, and are only awarded to those who do not ask for them.

## Reporting issues

To report issues, join Discord and mention a staff member, or post that
you have critical information in the #tshock channel. You can also
contact hakusaro (argo@hey.com) directly.
