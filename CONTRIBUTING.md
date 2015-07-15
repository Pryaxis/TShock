### Issue Guidelines
Please follow these simple requirements before posting a bug report:

- TShock version number
- Any stack traces that may have happened when the issue occurred
- How to reproduce the issue
- Screenshots of the issue (if applicable)

### TShock Additions

If something is better suited to be a plugin for TShock, rather than a TShock core feature, it should not be added!

### Pull Request Dev Guidelines

These guidelines are for all contributors. Please join our Slack instance and ask about your idea first, if you're implementing a new feature, system, or changing an existing implementation. Pull requests that change large feature sets or swathes of code will be dissected for quality and purpose prior to approval, and requests that conflict with a team developer's work may be declined if the project is already being worked on internally, but not released. In addition, issues assigned to Nyx developers that are recent and fresh should be considered a no-go zone, while that developer works on their solution outside the scope of Github.

Even if you have write access to the repository, follow [Github flow](https://guides.github.com/introduction/flow/) when sending commits. Don't send commits directly to either ```master``` or ```general-devel``` unless those commits modify either the deploy scripts or non-code components. If it compiles, follow Github Flow.

Required:
- Push code to the general-devel branch. Do not push it anywhere else.
- Use tabs, not spaces.
- Use UpperCamelCase for public function names.
- Prior to developing, make sure your clone is up to date with general-devel. This means that we don't get merge commits in your pull request.
- Document all code with public access at minimum. Use the ```<param name="Name">Description</param>``` style -- do not repeat the param type or name in the xml comment field.
- Document functions that can throw exceptions with ```<exception cref="NameOfException"/>```.
- Use explicit type declaration (```int, long, float```) when the type is not easily inferred (such as return types).
- Use implicit type declaration (```var```) when the type is easily inferred (such as private lists, temporary values, etc).
- Use Microsoft code conventions on variable naming (including ```_name``` for private members).
- When using static methods on primitives, use the CLR type. E.g. ```String.Format``` instead of ```string.Format```.
- Always use properties, not public fields.
- Document deprecations and fail compilation if they're included with ```[Obsolete("Use blah instead of blahx...", true)]```.

### Dev Team Guidelines

These guidelines are to be followed by all developers with commit level access to this repository:

- Prior to posting any version on the website, you must tick the version in AssemblyInfo.cs. This is the versioning formula:
 - Major.Minor.Revision
- Do not release any development builds on the forums without consulting another developer first.
- This is not a professional software product. Your results may vary with code quality, buginess, etc. Do not complain about something -- just fix it and move on.
- __Do not force push the repo__, or you will be removed.
- __Do not revert commits__, unless you have sign-off from one other developer (the two-man rule), or you will be removed.
- Deprecation of a function guarantees that it will exist in the given release, but may be removed, at any time following, from subsequent releases without further warning. Warning should be given in the release thread prior to it going live.
- Updates should be discussed, via pull request of a version tick, prior to release. Only after consensus from active contributing community members has been given can a release happen.
- Breaking API changes (excluding removal of already deprecated and warned code) should be forewarned with a one week notice on the forums, which may be given at any time prior to release (as soon as a pull request for a version push has been made, an update can be warned).
- Push less compiler warnings than you started with.
- Push more documentation than you started with.

#### Pull Request Acceptance Guidelines

- Don't ruin someone's first time sending a pull request. Be civil and welcoming.
- Don't accept untested pull requests from the outside world. Bamboo and Travis will at least make sure that something compiles, but actual code and execution tests are required.
- Pull request acceptance from internal contributors (anyone with write access) requires only one other approval to merge.
- Pull request acceptance from external contributors (anyone without write access) requires the [two-man rule](https://en.wikipedia.org/wiki/Two-man_rule) to be followed. If another man/woman/child in the two-man rule cannot be found within seven days, then this requirement is exempted.
