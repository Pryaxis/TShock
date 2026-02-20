# TShock — Compilar e rodar para Terraria 1.4.5.5

Documento de referência para compilar e executar o TShock com alvo **Terraria 1.4.5.5**.

## Objetivo

Compilar e rodar o servidor TShock para uma versão alvo do Terraria (ex.: **1.4.5.5**), a partir do repositório `TShock-general-devel`.

## Pré-requisitos

- **.NET 9 SDK** — a solução usa `net9.0`.
- **Submodule TerrariaServerAPI** — deve estar inicializado (ver abaixo).

## Inicializar submodule TerrariaServerAPI

Se o clone foi feito sem `--recurse-submodules`, a pasta `TerrariaServerAPI/` pode estar vazia e o build falha. Na raiz do repositório:

```bash
git submodule update --init --recursive
```

Confirme que existem arquivos em `TerrariaServerAPI/` (incluindo `TerrariaServerAPI/TerrariaServerAPI.csproj`).

## Versão Terraria (OTAPI.Upcoming)

A versão do Terraria é controlada pelo pacote NuGet **OTAPI.Upcoming** em `TShockLauncher/TShockLauncher.csproj`.

- **Para Terraria 1.4.5.5:** o projeto usa **OTAPI.Upcoming 3.3.7** (verificar [NuGet OTAPI.Upcoming](https://www.nuget.org/packages/OTAPI.Upcoming/) ou o repositório [Open-Terraria-API](https://github.com/SignatureBeef/Open-Terraria-API) para confirmar o mapeamento versão OTAPI ↔ versão do jogo).
- Para alterar a versão do Terraria, atualize o `PackageReference` de `OTAPI.Upcoming` em `TShockLauncher/TShockLauncher.csproj` e refaça `dotnet restore` e `dotnet build`.

## Build

Na raiz do repositório:

```bash
dotnet restore TShock.sln
dotnet build TShock.sln
```

Ou apenas:

```bash
dotnet build TShock.sln
```

Para Release:

```bash
dotnet build TShock.sln -c Release
```

## Run

O executável é produzido pelo projeto **TShockLauncher** (assembly de saída: `TShock.Server`). Na raiz:

```bash
dotnet run --project TShockLauncher
```

O servidor sobe na pasta de saída do projeto (ex.: `TShockLauncher/bin/Debug/net9.0/`), com dependências em `bin/` conforme os targets em `TShockLauncher.csproj`. Ver também [README.md](README.md).

## Publicar (opcional)

Para gerar um executável publicado (ex.: Windows x64, single-file, framework-dependent):

```bash
dotnet publish -p TShockLauncher -r win-x64 -f net9.0 -c Release -p:PublishSingleFile=true --self-contained false
```

A partir da raiz; ou de dentro de `TShockLauncher`:

```bash
dotnet publish -r win-x64 -f net9.0 -c Release -p:PublishSingleFile=true --self-contained false
```

## Resumo de arquivos principais

| Ação              | Arquivo |
|-------------------|--------|
| Referência build  | [TShock.sln](TShock.sln) |
| Projeto executável| [TShockLauncher/TShockLauncher.csproj](TShockLauncher/TShockLauncher.csproj) |
| Versão Terraria   | `PackageReference` OTAPI.Upcoming em `TShockLauncher/TShockLauncher.csproj` |
| Submodule         | [TerrariaServerAPI/](TerrariaServerAPI/) |

## Observação

Este **SKILLS.md** é um documento de projeto na raiz do repositório para humanos e agentes (como compilar e rodar para 1.4.5.5). Não é um Cursor Skill em `.cursor/skills/` com frontmatter tipo SKILL.md. Para que o Cursor aplique isso automaticamente, pode-se criar um skill em `.cursor/skills/` baseado neste conteúdo.
