# Narato.ObjectExtensions
This library contains some utils extensions placed on Object. A QOL library


GIT
===
We use git as source control.

To get started with git, follow the hands-on tutorials [here](https://try.github.io).

You should also watch following [video](https://www.youtube.com/watch?v=1ffBJ4sVUb4).

Branching strategy
---
We use following branching startegy: [git flow](http://nvie.com/posts/a-successful-git-branching-model/).

![git-flow](http://nvie.com/img/git-model@2x.png)

basicly to implement a new feature, you do the following:

1. You make a new branch **your-feature** from the develop branch 
```bash
git checkout -b **your-feature**
```
2. You make your code changes in your feature branch
3. Commit and push your changes in your feature branch 
```bash
git commit -m "**your-message**"
git push origin **your-feature**
```
4. Merge the develop branch back in your feature branch. 
```bash
git merge develop
git push origin **your-feature**
```
5. Squash merge your branch in the develop branch. 
```bash
git checkout develop
git merge --squash **your-feature**
git commit -m "**your-message**"
git push origin develop
```

Some git conventions we use
---------------------------
1. We don't merge feature branches. We **squash** merge them (see obove). This keeps the git history on the develop branch very clean and tidy.
2. We don't delete feature branches. Even after we completed the feature. This is because we squash merge. We still want to be able to see the exact history of a certain feature. We can find this in the git log of a feature branch.
3. We avoid unnecessary merge commits. We do this by always doing git pull --rebase rather than git pull (see below).
4. **NEVER** do a force push on develop, master, rc!

### How to avoid merge commits in the develop/Test branches
When you are working on the develop or Test branch, and you finished a feature/bugfix,
but someone else already pushed a commit of their own, you won't be able to push
your changes to the remote without first merging. To avoid this do the following:
```bash
git pull --rebase origin your_current_branch
git push origin your_current_branch
```