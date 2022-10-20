#!/usr/bin/env python3

# SPDX-FileCopyrightText: 2019 Corentin Noël <tintou@noel.tf>
#
# SPDX-License-Identifier: GPL-3.0-only

# Taken from https://github.com/elementary/actions/blob/master/gettext-template/check-diff.py

import git
import io

def commit_to_repo(repo):
    print('There are translation changes, committing changes to repository!')
    files = repo.git.diff(None, name_only=True)
    for f in files.split('\n'):
        if f.endswith ('.po') or f.endswith ('.pot'):
            repo.git.add(f)
    repo.git.commit('-m', 'Update translation template')
    infos = repo.remotes.origin.push()
    has_error=False
    error_msg=''
    for info in infos:
        if info.flags & git.remote.PushInfo.ERROR == git.remote.PushInfo.ERROR:
            has_error=True
            error_msg += info.summary
    if has_error:
        raise NameError('Unable to push to repository: ' + error_msg)

print('Checking the repository for new translations...')
repo = git.Repo('.')
t = repo.head.commit.tree
files = repo.git.diff(None, name_only=True)
needs_commit=False

for f in files.split('\n'):
    if f.endswith ('.pot'):
        raw_diff = repo.git.diff(t, f)
        output = io.StringIO()
        for line in raw_diff.splitlines():
            if line.startswith ('+++'):
                continue
            if line.startswith ('---'):
                continue
            if line.startswith ('diff'):
                continue
            if line.startswith ('index'):
                continue
            if line.startswith ('@@'):
                continue
            if line.startswith (' '):
                continue
            if line.startswith ('+#:'):
                continue
            if line.startswith ('-#:'):
                continue
            if line.startswith ('-"'):
                continue
            if line.startswith ('+"'):
                continue
            if not line.strip():
                continue
            print(line, file=output)
        if output.getvalue().strip():
            print(f + " has changed!")
            needs_commit = True
        output.close()

if needs_commit:
    commit_to_repo(repo)
else:
    print('The translations are up-to-date!')
