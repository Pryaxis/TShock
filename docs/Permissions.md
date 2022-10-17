---
aliases: [permissions, permission]
---

The permissions subsystem is a core subsystem of [[TShock]].

The basic idea behind the permission system is that each [[account]] can be given access to a part of TShock, a special ability on the server, the ability to use some part of Terraria, or the right to bypass certain protections, like the anticheat subsystem.

Permissions in TShock are applied at the [[group]] level. A [[player]] obtains their permissions through their [[account]]'s group. If the [[group]] is a child of another group, the parent's group is also evaluated when checking to see if an account has a permission.

Unlike some other permissions systems in games, permission cannot easily be applied to an individual user on an "override" basis. That is to say, they're group exclusive in TShock.

Permissions on the [[preset permission group mappings]] table, as well as in general, are referred to by their *permission node*. These nodes can have subgroups. For example, [[tshock.admin.ban]] can be granted to a group, but `tshock.admin.*` can also be granted to a group. If it is, the group gains all permissions that start with `tshock.admin` too. Ending a permission node definition with a `*` (a wildcard) means that all permission names in that node subset are valid too.

A permission can also be negated by prefixing it with `!`. This means that if the group would normally have a permission, a child group can revoke that permission intentionally. This can also be used if using wildcard permissions to create complex relationships. For example, a group might grant `tshock.admin.*` but also have a negated permission `!tshock.admin.ban`. If this is the case, all of the players logged into accounts with the group with this permission set would have the ability to use all `tshock.admin` features, except the ban system.