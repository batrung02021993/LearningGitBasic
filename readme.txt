1. Commit file change content to git:
- git add [file_name]
- git commit -m "comment"
- git push origin master
2. Add user:
- https://github.com/batrung02021993/LearningGitBasic:
  -> Setting -> Collaborators -> add collaborator
3. Delete file:
- git rm -r [file_name]
- git commit -m "comment"
- git push origin master
3. Rename file:
- git mv [file_name] [file_name_new]
- git commit -m "comment"
- git push origin master
4. Revert file has commit:
- git checkout 7f3e4c1 readme.txt
5. Log commit:
- git log --author=account@gmail.com
- git config --global alias.lg "log --color --graph 
--pretty=format:'%Cred%h%Creset -%C(yellow)%d%Creset %s %Cgreen(%cr) 
%C(bold blue)<%an>%Creset' --abbrev-commit"
- git lg
6. Revert version old in local:
- git reset --hard [id_revert]
7. Remark Tag show log:
- git add [file_name]
- git commit -a -m "comment"
- git tag [ver...]
- git tag
- git show [ver...]
