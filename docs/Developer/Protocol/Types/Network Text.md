| Description | Type |
|-------------|------|
| [[#Modes\|Mode]]  | byte |
| Text                    | [[String]] |
| Substitutions Length[^1] | byte |
| Substitutions[^1]        | `NetworkText[Substitutions Length]` |

[^1]: Only present when `Mode` is _not_ Literal Text.

##### Modes
| Description | Value |
|-------------|------|
| Literal Text     | 0 |
| Formattable      | 1 |
| Localization Key | 2 |