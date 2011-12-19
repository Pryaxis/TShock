TShock is a server modification based upon High6's mod API that allows for basic server administration commands.

__Constant builds__: http://ci.tshock.co/
__Bug Reporting__: http://ci.tshock.co:8080/
__Support Forums__: http://tshock.co/

----

### Helping out

If you'd like to help out, the best thing you can do is to fork this repository, add changes, and request a pull back in. Try to make your changes on the latest code possible so that merging doesn't take ages, but other than that we accept any improvements or changes.

----

### Teamspeak

We communicate on the ShankShock Temspeak server whilst programming.

__IP__: ts3.shankshock.com

__Port__: 9987

### IRC

We love IRC (although a little less than Teamspeak). If you need support, or just want to hang around, feel free to join.

__IP__: irc.shankshock.com

__Channel__: #terraria-dev or #terraria

### Pull Request Dev Guidelines

These guidelines are for contributors. If you do not follow these guidelines your commits will be reverted.

Required:
- Follow the code style. We generally use microsofts except for m_ infront of private variables.
- Do not push unfinished features to the master branch, instead create a remote branch and push to that.
- Do not push untested code to the master branch, instead push to the test branch.
- Document all compatibility issues in the COMPATIBILITY file. (IE file formats changing)
- DO NOT MASS COMMIT. Commit changes as you go (without pushing). That way when you push we don't get a thousand changes with a 1-3 line commit message.

Optional:
- Build Version Increment (http://autobuildversion.codeplex.com/).

### Dev Team Guidelines

These guidelines are to be followed by all developers with commit level access to this repository:

- Do not, for any reason, submit code to the master branch before it hits the development branch first. If the development branch is far ahead, and a new bug fix is going out, branch master, then merge with master and remove your branch.
 - If you are found to do this, you will be the person merging and rebasing your code to fit general-devel.
- Prior to posting any version on the website, you must tick the version in AssemblyInfo.cs. This is the versioning formula:
 - Major.Minor.Revision.BuildDate (tick Revision if you're fixing prior to an actual planned release)
- Do not release any development builds on the forums without consulting another developer first.
- __Document code prior to marking it done in JIRA__
- Move any un-tested code to the "Needs Validation" section on JIRA prior to marking it as done.
- Do not push changes to any branch without a proper issue being assigned in JIRA. If a feature isn't planned for this release, __it shouldn't be in the repo about to be released__.
- Submit all pull requests to the general-devel branch prior to the master branch, or you will be ignored.