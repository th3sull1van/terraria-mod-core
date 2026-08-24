<div align="center">

# 🧪 AutoBuff — Automação Inteligente de Poções e Alimentos para Terraria Vanilla

**Consome automaticamente poções de buff, alimentos e frascos do inventário, Void Bag e Piggy Bank quando as durações expiram, mantendo 100% de atividade dos buffs com zero modificação de arquivos.**

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

- **⚡ Detecção Nativa de Expiração de Buffs**:
  - Avalia os buffs ativos e durações restantes do jogador durante `Player.Update`.
  - Consome as poções de reposição no exato momento em que um buff ativo se encerra.

- **🍱 Seleção Inteligente de Nível de Alimento**:
  - Detecta o status de alimentação (*Bem Alimentado / Muito Satisfeito / Extremamente Saciado*).
  - Consome automaticamente o alimento de maior nível disponível no inventário assim que a nutrição acaba.

- **🗡️ Renovação Automática de Frascos**:
  - Mantém frascos de arma corpo a corpo (Ichor, Chamas Amaldiçoadas, Fogo, Ouro, Veneno, etc.) ativos continuamente.

- **🐷 Integração com Void Bag e Piggy Bank**:
  - Procura itens de buff não apenas no inventário principal, mas também dentro da Void Bag e do Piggy Bank (quando usado com o PiggyVault).

- **🚫 Lista de Exclusão Configurável**:
  - Permite ignorar buffs específicos (ex: Poção de Invisibilidade, Gravidade, etc.) para que não sejam consumidos automaticamente.

---

## ⚙️ Configuração (`config.json`)

O arquivo de configuração está localizado em `mods/AutoBuff/config.json`:

```json
{
  "Enabled": true,
  "CheckIntervalTicks": 15,
  "IncludeFood": true,
  "IncludeFlasks": true,
  "IncludeVoidBag": true,
  "IncludePiggyBank": true,
  "MinBuffTimeThresholdTicks": 0,
  "ExcludedBuffIds": [18, 119, 120],
  "ExcludedItemIds": [1344, 2756]
}
```

### Referência de Opções

| Opção | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Ativa ou desativa a reposição automática de buffs. |
| `CheckIntervalTicks` | `int` | `15` | Intervalo de verificação em ticks (15 ticks = 4 vezes por segundo a 60 TPS). |
| `IncludeFood` | `bool` | `true` | Renova automaticamente buffs de comida. |
| `IncludeFlasks` | `bool` | `true` | Renova automaticamente frascos de arma corpo a corpo. |
| `IncludeVoidBag` | `bool` | `true` | Procura poções e alimentos dentro da Void Bag. |
| `IncludePiggyBank` | `bool` | `true` | Procura poções e alimentos dentro do Piggy Bank. |
| `ExcludedBuffIds` | `int[]` | `[18, 119, 120]` | Lista de IDs de buffs que nunca devem ser renovados automaticamente. |
| `ExcludedItemIds` | `int[]` | `[1344, 2756]` | Lista de IDs de itens ignorados pela automação. |

---

## 🔧 Métodos em Runtime Interceptados

| Classe Alvo | Método Alvo | Tipo de Hook | Função |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `Update(int i)` | `Postfix` | Executa a verificação periódica de buffs e reposição para o jogador local. |

---

## 📁 Estrutura do Plugin

```text
mods/AutoBuff/
├── manifest.json       # Identidade, dependências e metadados
├── AutoBuff.dll        # Assembly compilado do plugin
├── AutoBuff.pdb        # Símbolos de depuração
├── README.md           # Documentação em inglês
├── README_pt-BR.md     # Documentação em português
└── config.json         # Configurações em tempo de execução
```
