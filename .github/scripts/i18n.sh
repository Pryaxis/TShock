#!/usr/bin/env bash

# SPDX-FileCopyrightText: 2019 Corentin Noël <tintou@noel.tf>
# SPDX-FileCopyrightText: 2022 Janet Blackquill <uhhadd@gmail.com>
#
# SPDX-License-Identifier: GPL-3.0-only

# adapted from https://github.com/elementary/actions/blob/master/gettext-template/entrypoint.sh

set -e

export DEBIAN_FRONTEND="noninteractive"

# if a custom token is provided, use it instead of the default github token.
if [ -n "$GIT_USER_TOKEN" ]; then
  GITHUB_TOKEN="$GIT_USER_TOKEN"
fi

if [ -z "${GITHUB_TOKEN}" ]; then
  echo "\033[0;31mERROR: The GITHUB_TOKEN environment variable is not defined.\033[0m"  && exit 1
fi

# Git repository is owned by another user, mark it as safe
git config --global --add safe.directory /github/workspace

# get default branch, see: https://davidwalsh.name/get-default-branch-name
DEFAULT_BRANCH="$(git remote show origin | grep 'HEAD branch' | cut -d' ' -f5)"

if [ -z "${INPUT_TRANSLATION_BRANCH}" ]; then
  TRANSLATION_BRANCH="${DEFAULT_BRANCH}"
else
  TRANSLATION_BRANCH="${INPUT_TRANSLATION_BRANCH}"
fi

# default email and username to github actions user
if [ -z "$GIT_USER_EMAIL" ]; then
  GIT_USER_EMAIL="action@github.com"
fi
if [ -z "$GIT_USER_NAME" ]; then
  GIT_USER_NAME="GitHub Action"
fi

# make sure branches are up-to-date
echo "Setting up git credentials..."
git remote set-url origin https://x-access-token:"$GITHUB_TOKEN"@github.com/"$GITHUB_REPOSITORY".git
git config --global user.email "$GIT_USER_EMAIL"
git config --global user.name "$GIT_USER_NAME"
echo "Git credentials configured."

# get the project's name:
PROJECT="$(basename "$GITHUB_REPOSITORY")"
echo "Project: $PROJECT"

sudo apt-get -qq update
sudo apt-get -qq install python3-git

dotnet tool install --global GetText.NET.Extractor --version 1.6.6

GetText.Extractor --order -s TShock.sln -t i18n/template.pot

python3 .github/scripts/check-diff.py
