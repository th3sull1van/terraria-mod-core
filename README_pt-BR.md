<div align="center">

# TerrariaModCore (TMC)

**Um framework de modding modular e motor de injeção em runtime de alta performance para o Vanilla Terraria 1.4.5.8 / 1.4.5.7 com zero dependência de tModLoader e 100% de integridade dos arquivos originais.**

<p align="center">
  <a href="README.md"><b>English</b></a> •
  <a href="README_pt-BR.md"><b>Português (Brasil)</b></a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Terraria-1.4.5.8%20%7C%201.4.5.7-22c55e?style=for-the-badge&logo=steam&logoColor=white" alt="Terraria 1.4.5.8 / 1.4.5.7">
  <img src="https://img.shields.io/badge/Framework-.NET%204.8-512bd4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET Framework 4.8">
  <img src="https://img.shields.io/badge/Patching-Harmony%202.4.2-6366f1?style=for-the-badge&logo=csharp&logoColor=white" alt="Harmony 2.4.2">
  <img src="https://img.shields.io/badge/Arquitetura-x86%20%2F%204GB%20LAA-f59e0b?style=for-the-badge" alt="x86 / 4GB LAA">
  <img src="https://img.shields.io/badge/Testes-391%20Aprovados-10b981?style=for-the-badge" alt="391 Testes Aprovados">
  <img src="https://img.shields.io/badge/Licen%C3%A7a-MIT-3b82f6?style=for-the-badge" alt="Licença MIT">
</p>

</div>

---

## Principais Recursos & Arquitetura

- **Zero Dependência de tModLoader & Integridade 100% dos Arquivos**:
  - Opera diretamente sobre a versão oficial de lançamento do **Vanilla Terraria 1.4.5.8 / 1.4.5.7** (Steam e GOG).
  - O executável original `Terraria.exe` permanece **100% intocado e sem modificações em disco** (SHA256 verificado).
  - Separação limpa de execução: execute `TerrariaModded.exe` para jogar com mods ou `Terraria.exe` para a experiência pura vanilla.

- **Isolamento de Patches Harmony & Gerenciamento de Conflitos**:
  - Gerenciador centralizado (`IPatchManager`) encapsula o Harmony 2.4.2, rastreando cada prefix, postfix e transpiler por ID de mod.
  - Reversão granular em tempo de execução: desativar ou descarregar um mod restaura o código IL original sem afetar outros mods ativos.

- **Resolução Topológica de Dependências (Algoritmo de Kahn)**:
  - Suporta dependências obrigatórias (`dependencies`), opcionais (`optionalDependencies`), ordem de carregamento (`loadBefore`, `loadAfter`) e prevenção explícita de conflitos (`incompatibleWith`).
  - Calcula automaticamente a ordem ideal de inicialização e detecta ciclos de dependência circular antes da execução.

- **Isolamento de Falhas & Modo de Segurança (Safe Mode)**:
  - Se um mod lançar uma exceção não tratada durante sua inicialização ou carregamento, a falha é isolada, o mod é marcado como `Faulted` e seus patches são revertidos sem travar o jogo.

- **Large Address Aware (4GB de Memória Virtual)**:
  - O Launcher é compilado com a flag PE `IMAGE_FILE_LARGE_ADDRESS_AWARE` (`0x0020`), fornecendo ao processo 32-bit os 4GB completos de espaço de endereço virtual necessários para eliminar `OutOfMemoryException`.

- **Proteção de Inicialização Gráfica Precoce**:
  - Inclui correções no motor protegendo a inicialização de configurações de vídeo contra condições de corrida antes do `GraphicsDevice` do XNA estar pronto.

---

## Mods de Produção Inclusos

| Mod | Recursos & Mecânicas | Documentação |
| :--- | :--- | :--- |
| **OreCascade** | Mineração em cadeia instantânea para minérios e pedras preciosas usando Busca em Largura (BFS), isolamento estrito de veios e preservação legítima de drops vanilla. | [OreCascade README](src/mods/OreCascade/README_pt-BR.md) |
| **AutoFishing** | Automação inteligente de pesca com arremesso automático, detecção nativa de fisgada (`ai[1] < 0`) e recolhimento sincronizado com o loop de 60 TPS do jogo. | [AutoFishing README](src/mods/AutoFishing/README_pt-BR.md) |
| **FishingLinePlus** | Múltiplas linhas de pesca funcionais e simultâneas com física de dispersão angular, sincronização de fisgada em dupla camada e captura múltipla. | [FishingLinePlus README](src/mods/FishingLinePlus/README_pt-BR.md) |
| **TurboExtractinator** | Acelera a velocidade de processamento do Extractinator e Chlorophyte Extractinator por um multiplicador configurável (padrão 5x) com suporte a lotes. | [TurboExtractinator README](src/mods/TurboExtractinator/README_pt-BR.md) |
| **AutoBuff** | Toma poções de buff e come alimentos do inventário e Void Bag automaticamente quando os buffs expiram, garantindo tempo ativo contínuo sem desperdício. | [AutoBuff README](src/mods/AutoBuff/README_pt-BR.md) |
| **AutoOpen** | Abertura contínua e rápida de recipientes, bolsas de tesouro, caixas de pesca, ostras, presentes e caixas trancadas segurando o botão direito (estilo Extractinator). | [AutoOpen README](src/mods/AutoOpen/README_pt-BR.md) |
| **AutoResearch** | Pesquisa e sacrifício automatizados de itens no modo Journey ao entrarem no inventário, preservando 100% das regras de quantidade vanilla sem cliques manuais. | [AutoResearch README](src/mods/AutoResearch/README_pt-BR.md) |
| **PiggyVault** | Coleta automática estilo Void Bag, criação direta de receitas, ações rápidas e acessórios informativos direto do Porquinho Cofrinho. | [PiggyVault README](src/mods/PiggyVault/README_pt-BR.md) |
| **TurboBucket** | Despejamento instantâneo a 60 TPS de baldes de líquidos, fluxo contínuo e aceleração de baldes sem fundo e esponjas. | [TurboBucket README](src/mods/TurboBucket/README_pt-BR.md) |
| **BossCursor** | Setas indicadoras de direção e ícones dos bosses em tempo real apontando para chefes e mini-chefes com escalonamento por proximidade. | [BossCursor README](src/mods/BossCursor/README_pt-BR.md) |

---

## Como Instalar e Usar

### 1. Compilar a partir do Código-Fonte
```powershell
# Compila a solução (Release|x86), executa a suíte com 391 testes e monta o pacote dist
powershell -ExecutionPolicy Bypass -File "build_dist.ps1"
```

### 2. Instalar no Terraria
```powershell
# Copia a distribuição compilada para a pasta do Terraria:
Copy-Item -Path "dist\*" -Destination "D:\Jogos\Steam\steamapps\common\Terraria" -Recurse -Force
```

### 3. Iniciar o Jogo
- **Com Mods**: Inicie pelo executável `TerrariaModded.exe` (ou pelo atalho criado na Área de Trabalho).
- **Vanilla Puro**: Inicie o `Terraria.exe` diretamente.

---

## Referência de Configuração do Host

Localizado em `TMC/config/core.json`:

```json
{
  "LogLevel": "Info",
  "DiagnosticBannerOnStartup": true
}
```

| Opção | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `LogLevel` | `string` | `"Info"` | Nível de detalhamento do log: `"Trace"`, `"Debug"`, `"Info"`, `"Warn"`, `"Error"`, `"Fatal"`. |
| `DiagnosticBannerOnStartup` | `bool` | `true` | Exibe banner de diagnóstico no console com a contagem de mods ativos. |

---

## Estrutura do Repositório

```text
terraria_mod_core/
├── build_dist.ps1                      # Script de build, testes automatizados e empacotamento
├── LICENSE                             # Licença MIT
├── TerrariaModCore.sln                 # Solução Visual Studio (.NET Framework 4.8 / x86)
├── README.md                           # Documentação Principal em Inglês
├── README_pt-BR.md                     # Documentação Principal em Português
│
├── docs/                               # Documentação Técnica Aprofundada
│   ├── ARCHITECTURE.md                 # Design técnico, modelo de memória e isolamento de patches
│   ├── MODDING.md                      # Guia do desenvolvedor para criação de plugins TMC
│   ├── COMPATIBILITY.md                # Matriz de hooks e validação de versões
│   ├── CONFIGURATION.md                # Referência completa de configurações do Core e dos mods
│   ├── TESTING.md                      # Detalhamento da suíte com 391 asserções automatizadas
│   └── TROUBLESHOOTING.md              # Resolução de problemas de memória, gráficos e logs
│
├── src/
│   ├── TerrariaModCore.API/            # API Pública de Modding (Contratos, Interfaces e Tipos)
│   │   ├── IMod.cs                     # Interface de ciclo de vida (Initialize, Load, Unload)
│   │   ├── IModContext.cs              # Sandbox de contexto do mod
│   │   ├── IPatchManager.cs            # Interface de gerenciamento de patches Harmony
│   │   ├── IConfigManager.cs           # Gerenciador genérico de configurações JSON
│   │   └── TerrariaModCore.API.csproj
│   │
│   ├── TerrariaModCore/                # Motor Host do TMC (Injetor de Runtime e Ciclo de Vida)
│   │   ├── ModEngine.cs                # Descoberta, resolução topológica e execução de mods
│   │   ├── Patching/                   # Gerenciador de patches Harmony e correções centrais
│   │   ├── Dependencies/               # Algoritmo de Kahn e detecção de ciclos
│   │   └── TerrariaModCore.csproj
│   │
│   ├── TerrariaModCore.Launcher/       # Bootstrapper Modded (TerrariaModded.exe)
│   │   ├── Program.cs                  # Dynamic AssemblyResolver e ponto de entrada
│   │   ├── App.config                  # Configurações de GC e objetos de grande porte
│   │   └── TerrariaModCore.Launcher.csproj
│   │
│   └── mods/                           # Mods de Produção Integrados
│       ├── OreCascade/                 # Plugin VeinMiner / Escavação de Minérios
│       ├── AutoFishing/                # Plugin de automação inteligente de pesca
│       ├── FishingLinePlus/            # Plugin de múltiplas linhas de pesca simultâneas
│       ├── TurboExtractinator/         # Plugin de aceleração do Extractinator
│       ├── AutoBuff/                   # Plugin de reposição automática de poções e buffs
│       ├── AutoOpen/                   # Plugin de abertura acelerada de recipientes
│       ├── AutoResearch/               # Plugin de sacrifício/pesquisa no modo Journey
│       ├── PiggyVault/                 # Plugin de recursos do Void Bag para o Porquinho Cofrinho
│       ├── TurboBucket/                # Plugin de despejo acelerado de baldes de líquidos
│       └── BossCursor/                 # Plugin de seta indicadora de bosses em tempo real
│
└── tests/
    └── TerrariaModCore.Tests/          # Suíte Automatizada com 391 Asserções
        ├── Program.cs                  # Executor autônomo de testes
        ├── DependencyResolverTests.cs  # Testes de resolução de dependências e ciclos
        ├── PatchManagerTests.cs        # Testes de prefix/postfix e reversão Harmony
        ├── FaultIsolationTests.cs      # Testes de contenção de falhas e SafeMode
        ├── ConfigManagerTests.cs       # Testes de serialização e GameVersionChecker
        ├── OreCascadePluginTests.cs    # Testes de algoritmo BFS e poder de picareta
        ├── AutoFishingPluginTests.cs   # Testes da máquina de estados de pesca
        ├── FishingLinePlusPluginTests.cs # Testes de física de dispersão e captura múltipla
        ├── TurboExtractinatorPluginTests.cs # Testes de escala de velocidade e lotes
        ├── AutoBuffPluginTests.cs      # Testes de seleção de poções e buffs
        ├── AutoOpenPluginTests.cs      # Testes de abertura contínua e recipientes
        ├── AutoResearchPluginTests.cs  # Testes de pesquisa e sacrifício no modo Journey
        ├── PiggyVaultPluginTests.cs    # Testes de coleta, criação e ações no Piggy Bank
        ├── TurboBucketPluginTests.cs   # Testes de despejamento instantâneo e esponjas
        ├── BossCursorPluginTests.cs    # Testes de detecção de bosses, rotação e proximidade
        └── ModCoexistenceTests.cs      # 16 cenários de coexistência entre mods
```

---

## Suíte de Testes & Validação Automatizada

O repositório conta com uma suíte de testes com 391 asserções automatizadas:

```powershell
# Compilar e executar a suíte de testes diretamente:
dotnet build tests/TerrariaModCore.Tests/TerrariaModCore.Tests.csproj -c Release -p:Platform="x86"
& "tests/TerrariaModCore.Tests/bin/Release/TerrariaModCore.Tests.exe"
```

---

## Documentação Estendida

- **[Especificação Técnica Mestra](SPECIFICATION.md)**: Especificação técnica e arquitetural formal baseada nos padrões do framework.
- **[Arquitetura Técnica e Design](docs/ARCHITECTURE.md)**: Mecanismos de injeção em runtime, modelo de memória e gerenciamento de patches.
- **[Requisitos de Dependências e Ambiente](docs/DEPENDENCIES.md)**: Pré-requisitos de sistema, toolchains .NET, bibliotecas e dependências de mods.
- **[Guia do Desenvolvedor de Mods](docs/MODDING.md)**: Tutorial completo para criar plugins personalizados para o TMC.
- **[Compatibilidade e Matriz de Patches](docs/COMPATIBILITY.md)**: Métodos IL interceptados e validação de versão.
- **[Referência de Configurações](docs/CONFIGURATION.md)**: Opções e presets para o host e todos os plugins.
- **[Estratégia de Testes](docs/TESTING.md)**: Detalhamento dos 391 testes automatizados.
- **[Guia de Resolução de Problemas](docs/TROUBLESHOOTING.md)**: Diagnóstico para limites de memória, gráficos e telemetria de logs.

---

## Licença

MIT © [th3sull1van](https://github.com/th3sull1van)
