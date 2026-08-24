<div align="center">

# 🎣 AutoFishing — Automação Inteligente de Pesca para Terraria Vanilla

**Lançamento automatizado inteligente, detecção precisa de mordidas e recolhimento no Terraria Vanilla com máquina de estados nativa e zero modificação de arquivos.**

<p align="center">
  <a href="README.md"><b>English</b></a> •
  <a href="README_pt-BR.md"><b>Português (Brasil)</b></a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Terraria-1.4.5.8_|_1.4.5.7-22c55e?style=for-the-badge&logo=steam&logoColor=white" alt="Terraria 1.4.5.8 / 1.4.5.7">
  <img src="https://img.shields.io/badge/Framework-Harmony_2.4.2-6366f1?style=for-the-badge&logo=csharp&logoColor=white" alt="Harmony 2.4.2">
  <img src="https://img.shields.io/badge/Type-TMC_Plugin-06b6d4?style=for-the-badge" alt="TMC Plugin">
  <img src="https://img.shields.io/badge/License-MIT-10b981?style=for-the-badge" alt="License MIT">
</p>

</div>

---

## 🌟 Principais Funcionalidades

- **🎮 Ciclo de Vida Iniciado pelo Jogador (Início/Parada Manual)**:
  - Selecionar a vara de pesca no hotbar **não** lança automaticamente.
  - A automação inicia somente após o jogador selecionar a vara e realizar o primeiro clique manual de lançamento.
  - A automação para imediatamente assim que o jogador clica manualmente para recolher ou cancelar a linha.
  - Trocar de item no hotbar reinicia o estado de automação.

- **⚡ Zero Macros Externos e Sincronia a 60 TPS**:
  - Executa diretamente dentro do método nativo `Player.Update` sincronizado ao ciclo de 60 TPS do jogo.
  - Sem leituras frágeis de pixels ou macros externos de mouse.

- **🎯 Detecção Precisa de Mordidas em Engine**:
  - Monitora as boias ativas pertencentes ao jogador local (`bobber.ai[1] < 0f && bobber.localAI[1] != 0f`).
  - Fisga o peixe no instante exato em que o motor de física do Terraria confirma a mordida.

- **🛡️ Verificação de Iscas no Inventário**:
  - Escaneia os slots de inventário 0–57 em busca de iscas válidas antes de lançar.
  - Pausa a automação de forma segura caso a isca acabe quando `RequireBait` estiver ativo.

- **⏱️ Temporizadores Naturais de Reação**:
  - Atraso configurável para puxar a linha (`ReelDelayTicks`), simulando tempos de reação humanos.
  - Cooldown configurável entre capturas (`CastDelayTicks`) antes de relançar a linha.

---

## ⚙️ Configuração (`config.json`)

O arquivo de configuração está localizado em `mods/AutoFishing/config.json`:

```json
{
  "Enabled": true,
  "AutoCast": true,
  "AutoReel": true,
  "CastDelayTicks": 30,
  "ReelDelayTicks": 2,
  "RequireBait": true
}
```

### Referência de Opções

| Opção | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Ativa ou desativa toda a funcionalidade do mod. |
| `AutoCast` | `bool` | `true` | Relança a linha automaticamente após cada captura. |
| `AutoReel` | `bool` | `true` | Puxa a linha automaticamente quando uma mordida é detectada. |
| `CastDelayTicks` | `int` | `30` | Tempo de espera (em frames a 60 TPS, 30 ticks = 0,5s) antes de relançar. |
| `ReelDelayTicks` | `int` | `2` | Atraso (em ticks) entre a mordida e o recolhimento. |
| `RequireBait` | `bool` | `true` | Verifica se o jogador possui isca antes de realizar novo lançamento. |

---

## 🔧 Métodos em Runtime Interceptados

| Classe Alvo | Método Alvo | Tipo de Hook | Função |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `Update(int i)` | `Postfix` | Processa a máquina de estados de pesca para o jogador local. |
| `Terraria.Player` | `ItemCheck_Shoot(...)` | `Postfix` | Detecta o lançamento inicial para registrar a boia no controlador. |
| `Terraria.Player` | `ItemCheck_PullFishingBobbers(Item)` | `Prefix` | Executa o recolhimento automático da boia com captura de item. |

---

## 📁 Estrutura do Plugin

```text
mods/AutoFishing/
├── manifest.json       # Identidade, dependências e metadados
├── AutoFishing.dll     # Assembly compilado do plugin
├── AutoFishing.pdb     # Símbolos de depuração
├── README.md           # Documentação em inglês
├── README_pt-BR.md     # Documentação em português
└── config.json         # Configurações em tempo de execução
```
