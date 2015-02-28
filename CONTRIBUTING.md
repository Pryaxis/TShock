### Issue Guidelines
Please follow these simple requirements before posting an issue:

- TShock version number
- Any stack traces that may have happened when the issue occurred
- How to reproduce the issue
- Screenshots of the issue (if applicable)

### TShock Additions

If something is better suited to be a plugin for TShock, rather than a TShock core feature, it should not be added!

### Pull Request Dev Guidelines

These guidelines are for all contributors.

Required:
- Push code to the general-devel branch. Do not push it anywhere else.
- Use tabs, not spaces.
- Use UpperCamelCase for public function names.
- Prior to developing, make sure your clone is up to date with general-devel. This means that we don't get merge commits in your pull request.

### Dev Team Guidelines

These guidelines are to be followed by all developers with commit level access to this repository:

- Prior to posting any version on the website, you must tick the version in AssemblyInfo.cs. This is the versioning formula:
 - Major.Minor.Revision
- Do not release any development builds on the forums without consulting another developer first.
- This is not a professional software product. Your results may vary with code quality, buginess, etc. Do not complain about something -- just fix it and move on.
- __Do not force push the repo__, or you will be removed.
- __Do not revert commits__, unless you have sign-off from one other developer (the two-man rule), or you will be removed.
- __This is not a meritocracy.__

#### Pull Request Acceptance Guidelines

- Don't ruin someone's first time sending a pull request. They feel demotivated, and then they won't want to push any more code for us.
- Don't accept untested pull requests from the outside world. Bamboo and Travis will at least make sure that something compiles, but actual code and execution tests are required.
- Pull request acceptance from internal contributors (anyone with write access) requires only one other approval to merge.
- Pull request acceptance from external contributors (anyone without write access) requires the [two-man rule](https://en.wikipedia.org/wiki/Two-man_rule) to be followed. If another man/woman/child in the two-man rule cannot be found within seven days, then this requirement is exempted.
