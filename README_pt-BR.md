<div align="center">

# 🌌 TerrariaModCore (TMC) — Framework de Modding de Alta Performance para Terraria 1.4.5.7 Vanilla

**Um framework de plugins modular, sem dependência de tModLoader, com injeção em tempo de execução, isolamento de patches Harmony, resolução topológica de dependências, gerenciamento de memória 4GB LAA e mods integrados.**

<p align="center">
  <a href="README.md"><b>English</b></a> •
  <a href="README_pt-BR.md"><b>Português (Brasil)</b></a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Terraria-1.4.5.7-22c55e?style=for-the-badge&logo=steam&logoColor=white" alt="Terraria 1.4.5.7">
  <img src="https://img.shields.io/badge/Framework-Harmony_2.4.2-6366f1?style=for-the-badge&logo=csharp&logoColor=white" alt="Harmony 2.4.2">
  <img src="https://img.shields.io/badge/Vanilla-Zero_tModLoader-06b6d4?style=for-the-badge" alt="Zero tModLoader">
  <img src="https://img.shields.io/badge/Memória-4GB_LAA_Ativo-f59e0b?style=for-the-badge" alt="4GB LAA Ativo">
  <img src="https://img.shields.io/badge/Testes-183_Passando-10b981?style=for-the-badge" alt="183 Testes Passando">
  <img src="https://img.shields.io/badge/Licença-MIT-3b82f6?style=for-the-badge" alt="Licença MIT">
</p>

<br>

<img src="https://terraria.org/static/media/logo.734118ae.png" width="360" alt="Terraria Logo">

<br>
<br>

</div>

---

## 🌟 Arquitetura e Principais Capacidades

- **⚡ Zero Dependência de tModLoader e Integridade 100% dos Arquivos**:
  - Opera diretamente sobre a versão oficial do **Terraria 1.4.5.7** (Steam e GOG).
  - O executável original `Terraria.exe` permanece **100% intocado no disco** (SHA256 preservado).
  - Separação limpa: execute `TerrariaModded.exe` para jogar com mods ou `Terraria.exe` para a experiência pura vanilla.

- **🛡️ Isolamento e Gerenciamento de Patches Harmony**:
  - Gerenciador centralizado (`IPatchManager`) encapsula o Harmony 2.4.2, rastreando cada prefix, postfix e transpiler por ID de mod.
  - Reversão granular em tempo de execução: desativar ou descarregar um mod restaura o código IL original sem afetar outros mods ativos.

- **🔀 Resolução Topológica de Dependências (Algoritmo de Kahn)**:
  - Suporta dependências obrigatórias (`dependencies`), opcionais (`optionalDependencies`), ordem de carregamento (`loadBefore`, `loadAfter`) e prevenção explícita de conflitos (`incompatibleWith`).
  - Calcula automaticamente a ordem de inicialização ideal e detecta bloqueios por dependência circular.

- **🛡️ Isolamento de Falhas e Modo de Segurança (Safe Mode)**:
  - Se um mod lançar uma exceção não tratada durante sua inicialização ou carregamento, a falha é isolada, o mod é marcado como `Faulted` e seus patches são revertidos sem travar o jogo.

- **🧠 Large Address Aware (4GB de Memória Virtual)**:
  - O Launcher é compilado com a flag PE `IMAGE_FILE_LARGE_ADDRESS_AWARE` (`0x0020`), fornecendo ao processo 32-bit os 4GB completos de memória virtual necessários para evitar `OutOfMemoryException`.

- **🎨 Proteção de Inicialização Gráfica Precoce**:
  - Inclui correções no Core que protegem a inicialização precoce de configurações de vídeo contra condições de corrida antes do `GraphicsDevice` do XNA estar pronto.

---

## 🎮 Mods de Produção Inclusos

| Mod | Descrição | Documentação |
| :--- | :--- | :--- |
| **⛏️ OreCascade** | Mineração em cadeia instantânea para minérios e pedras preciosas usando Busca em Largura (BFS), isolamento estrito de veios e preservação legítima de drops vanilla. | [OreCascade README](src/mods/OreCascade/README.md) |
| **🎣 AutoFishing** | Automação inteligente de pesca com arremesso automático, detecção nativa de fisgada (`ai[1] < 0`) e recolhimento sincronizado com o loop de 60 TPS do jogo. | [AutoFishing README](src/mods/AutoFishing/README.md) |
| **🎣 FishingLinePlus** | Múltiplas linhas de pesca funcionais e simultâneas com física de dispersão angular, sincronização de fisgada em dupla camada e captura múltipla. | [FishingLinePlus README](src/mods/FishingLinePlus/README.md) |
| **⚡ TurboExtractinator** | Acelera a velocidade de processamento do Extractinator e Chlorophyte Extractinator por um multiplicador configurável (padrão 5x) com suporte a lotes. | [TurboExtractinator README](src/mods/TurboExtractinator/README.md) |
| **🧪 AutoBuff** | Toma poções de buff e come alimentos do inventário e Void Bag automaticamente quando os buffs expiram, garantindo tempo ativo contínuo sem desperdício. | [AutoBuff README](src/mods/AutoBuff/README.md) |
| **📦 AutoOpen** | Abertura contínua e acelerada de bolsas de tesouro, caixas de pesca, ostras, presentes e baús trancados segurando o botão direito (estilo Extractinator). | [AutoOpen README](src/mods/AutoOpen/README.md) |

---

## 🚀 Início Rápido

### 1. Compilar a partir do Código-Fonte
```powershell
# Compila a solução (Release|x86), executa a suíte com 85 testes e monta o pacote dist
powershell -ExecutionPolicy Bypass -File "build_dist.ps1"
```

### 2. Instalar no Terraria
```powershell
# Copia a distribuição compilada para a pasta do Terraria:
Copy-Item -Path "dist\*" -Destination "D:\Jogos\Steam\steamapps\common\Terraria" -Recurse -Force
```

### 3. Iniciar o Jogo
- 🎮 **Com Mods**: Inicie pelo executável `TerrariaModded.exe` (ou pelo atalho criado na Área de Trabalho).
- 🛡️ **Vanilla Puro**: Inicie o `Terraria.exe` diretamente.

---

## ⚙️ Referência de Configuração

### Host Engine do TMC (`TMC/config/core.json`)

```json
{
  "LogLevel": "Info",
  "DiagnosticBannerOnStartup": true,
  "StrictCompatibilityCheck": true,
  "SafeModeOnModFailure": true,
  "ModsDirectoryName": "mods"
}
```

| Opção | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `LogLevel` | `string` | `"Info"` | Nível de detalhamento do log: `"Trace"`, `"Debug"`, `"Info"`, `"Warn"`, `"Error"`, `"Fatal"`. |
| `DiagnosticBannerOnStartup` | `bool` | `true` | Exibe banner de diagnóstico no console com a contagem de mods ativos. |
| `StrictCompatibilityCheck` | `bool` | `true` | Valida se a versão do Terraria é exatamente a 1.4.5.7 antes de iniciar. |
| `SafeModeOnModFailure` | `bool` | `true` | Isola mods com erro e continua carregando os mods saudáveis. |
| `ModsDirectoryName` | `string` | `"mods"` | Nome da pasta que contém os diretórios dos mods. |

---

## 📁 Estrutura do Repositório

```text
terraria_mod_core/
├── .gitignore                          # Regras do gitignore para C#, Visual Studio e binários
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
│   ├── TESTING.md                      # Detalhamento da suíte com 85 testes automatizados
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
│       └── TurboExtractinator/         # Plugin de aceleração do Extractinator
│
└── tests/
    └── TerrariaModCore.Tests/          # Suíte Automatizada com 97 Testes
        ├── Program.cs                  # Executor autônomo de testes
        ├── DependencyResolverTests.cs  # Testes de resolução de dependências e ciclos
        ├── PatchManagerTests.cs        # Testes de prefix/postfix e reversão Harmony
        ├── FaultIsolationTests.cs      # Testes de contenção de falhas e SafeMode
        ├── OreCascadePluginTests.cs    # Testes de algoritmo BFS e poder de picareta
        ├── AutoFishingPluginTests.cs   # Testes da máquina de estados de pesca
        ├── FishingLinePlusPluginTests.cs # Testes de física de dispersão e captura múltipla
        ├── TurboExtractinatorPluginTests.cs # Testes de escala de velocidade e lotes
        └── ModCoexistenceTests.cs      # 8 cenários de coexistência entre mods
```

---

## 📖 Documentação Estendida

- 📐 **[Arquitetura Técnica e Design](docs/ARCHITECTURE.md)**: Mecanismos de injeção em runtime, modelo de memória e gerenciamento de patches.
- 📦 **[Requisitos de Dependências e Ambiente](docs/DEPENDENCIES.md)**: Pré-requisitos de sistema, toolchains .NET, bibliotecas e dependências de mods.
- 🛠️ **[Guia do Desenvolvedor de Mods](docs/MODDING.md)**: Tutorial completo para criar plugins personalizados para o TMC.
- 🔍 **[Compatibilidade e Matriz de Patches](docs/COMPATIBILITY.md)**: Métodos IL interceptados e validação de versão.
- ⚙️ **[Referência de Configurações](docs/CONFIGURATION.md)**: Opções e presets para o host e todos os plugins.
- 🧪 **[Estratégia de Testes](docs/TESTING.md)**: Detalhamento dos 85 testes automatizados.
- 🔧 **[Guia de Resolução de Problemas](docs/TROUBLESHOOTING.md)**: Diagnóstico para limites de memória, gráficos e telemetria de logs.

---

## 📄 Licença

Este projeto é de código aberto sob a [Licença MIT](LICENSE).
