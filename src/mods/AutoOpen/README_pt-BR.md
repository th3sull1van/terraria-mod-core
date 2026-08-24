<div align="center">

# 📦 AutoOpen — Abertura Rápida e Automatizada de Recipientes para Terraria Vanilla

**Abertura rápida e contínua de bolsas de itens, caixas de pesca, ostras, bolsas de bosses, baús trancados e presentes ao segurar o botão direito (estilo Extrator) com zero modificação de arquivos vanilla.**

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

- **⚡ Abertura Rápida Contínua ao Segurar o Clique**:
  - Elimina a necessidade frustrante do jogo vanilla de soltar e clicar o botão direito do mouse repetidamente centenas de vezes.
  - Basta manter pressionado o botão direito sobre qualquer pilha de caixas ou bolsas para abri-las continuamente em alta velocidade.

- **📦 Amplo Suporte a Recipientes**:
  - **Caixas de Pesca**: Madeira, Ferro, Ouro, Sagrada, Masmorra, Oceano, Selva, Céu, Corrupção, Carmesim e variantes do Hardmode.
  - **Bolsas de Bosses**: Todas as bolsas de tesouro do modo Expert / Master.
  - **Recipientes e Sorteios**: Ostras, Bolsas de Ervas, Bolsas de Pesca, Presentes, Caixas Trancadas da Masmorra Dourada e Masmorra de Obsidiana.

- **🛡️ Drops 100% Legítimos com Áudio Nativo**:
  - Os itens são gerados utilizando as chamadas nativas de desempacotamento do Terraria, preservando todas as probabilidades de itens, moedas, pets e efeitos sonoros originais.

---

## ⚙️ Configuração (`config.json`)

O arquivo de configuração está localizado em `mods/AutoOpen/config.json`:

```json
{
  "Enabled": true,
  "RapidRightClickOpen": true,
  "OpenDelayTicks": 3,
  "BatchSize": 1,
  "PlaySound": true,
  "AutoOpenInventory": false,
  "AutoOpenIntervalTicks": 10,
  "IncludeVoidBag": true,
  "ExcludedItemIds": []
}
```

### Referência de Opções

| Opção | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Ativa ou desativa o mod de abertura de recipientes. |
| `RapidRightClickOpen` | `bool` | `true` | Ativa o modo contínuo de abertura ao manter o clique direito pressionado. |
| `OpenDelayTicks` | `int` | `3` | Intervalo em ticks entre aberturas (3 ticks = 20 recipientes abertos por segundo). |
| `BatchSize` | `int` | `1` | Quantidade de recipientes abertos por ciclo. |
| `PlaySound` | `bool` | `true` | Reproduz os efeitos sonoros originais ao abrir recipientes. |
| `IncludeVoidBag` | `bool` | `true` | Permite desempacotar recipientes armazenados na Void Bag. |
| `ExcludedItemIds` | `int[]` | `[]` | Lista de IDs de itens excluídos da abertura rápida. |

---

## 🔧 Métodos em Runtime Interceptados

| Classe Alvo | Método Alvo | Tipo de Hook | Função |
| :--- | :--- | :--- | :--- |
| `Terraria.UI.ItemSlot` | `RightClick(Item[], int, int)` | `Prefix` | Detecta o clique direito mantido sobre recipientes válidos para armar o loop de abertura rápida. |
| `Terraria.Player` | `Update(int i)` | `Postfix` | Executa o loop de abertura contínua para o jogador local. |

---

## 📁 Estrutura do Plugin

```text
mods/AutoOpen/
├── manifest.json       # Identidade, dependências e metadados
├── AutoOpen.dll        # Assembly compilado do plugin
├── AutoOpen.pdb        # Símbolos de depuração
├── README.md           # Documentação em inglês
├── README_pt-BR.md     # Documentação em português
└── config.json         # Configurações em tempo de execução
```
