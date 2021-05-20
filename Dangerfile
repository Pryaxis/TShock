def changelog_was_not_updated?
  !git.modified_files.include? "CHANGELOG.md"
end

def complicated?
  !github.pr_body.include? "#trivial"
end

if changelog_was_not_updated? && complicated?
  fail "You need to update the changelog. Your pull request will not be merged until this is done."
  markdown "ProTip: Even if you think something is super simple, it should be in the changelog. Literally the only exception to this rule is if your commit isn't touching code."
end
