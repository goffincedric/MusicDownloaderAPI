# MusicDownloader

## Git hooks
To enable git hooks stored in the .githooks, run the following command to configure git:
```shell
git config core.hooksPath .githooks
```
This enables the following git hooks:
- pre-commit: This hook automatically formats the code with CSharpier.

## Code style
Code style is configured and enforced by [CSharpier](https://csharpier.com/docs/About). Editor support is available when paired with the accompanying [plugin for you IDE](https://csharpier.com/docs/Editors).
On each build, all files will be styled  automatically. CSharpier can be configured by editing the .csharpierrc file. To ignore files, add them to .csharpierignore.

CSharpier is added to the local project manifest. To install the tools, run:
```shell
dotnet tool install
```

To manually format files, run the following command in the project root:
```shell
dotnet csharpier .
```
