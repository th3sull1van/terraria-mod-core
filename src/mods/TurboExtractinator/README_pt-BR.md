<div align="center">

# ⚡ TurboExtractinator — Extrator em Alta Velocidade para Terraria Vanilla

**Acelera drasticamente a velocidade de processamento do Extrator e do Extrator de Clorofita por um multiplicador configurável (padrão 5x) com zero modificação de arquivos vanilla.**

<p align="center">
  <a href="README.md"><b>English</b></a> •
  <a href="README_pt-BR.md"><b>Português (Brasil)</b></a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Terraria-1.4.5.8_|_1.4.5.7-22c55e?style=for-the-badge&logo=steam&logoColor=white" alt="Terraria 1.4.5.8 / 1.4.5.7">
  <img src="https://img.shields.io/badge/Framework-Harmony_2.4.2-6366f1?style=for-the-badge&logo=csharp&logoColor=white" alt="Harmony 2.4.2">
  <img src="https://img.shields.io/badge/Type-TMC_Plugin-06b6d4?style=for-the-badge" alt="TMC Plugin">
  <img src="https://img.shields.io/badge/Velocidade-5x_Configurável-f59e0b?style=for-the-badge" alt="5x Configurável">
  <img src="https://img.shields.io/badge/License-MIT-10b981?style=for-the-badge" alt="License MIT">
</p>

</div>

---

## 🌟 Principais Funcionalidades

- **⚡ Aceleração de Velocidade Configurável (Padrão 5x)**:
  - Acelera a taxa de consumo e geração de drops de todos os blocos extraíveis (Lodo, Neve com Terra, Fóssil do Deserto, Musgo Brilhante e conversões de Clorofita).
  - Transforma milhares de blocos extraíveis em pedras preciosas, moedas, minérios e fósseis em segundos.

- **🌿 Suporte aos Dois Extratores**:
  - Funciona tanto com o **Extrator Padrão** quanto com o **Extrator de Clorofita** do Hardmode.

- **🛡️ Preservação 100% Legítima de Drops**:
  - Não altera as tabelas de probabilidade de itens vanilla. O jogo continua sorteando itens exatamente como no jogo oficial, apenas em velocidade acelerada.

---

## ⚙️ Configuração (`config.json`)

O arquivo de configuração está localizado em `mods/TurboExtractinator/config.json`:

```json
{
  "Enabled": true,
  "SpeedMultiplier": 5,
  "AffectsChlorophyteExtractinator": true,
  "BatchExtractionSize": 1
}
```

### Referência de Opções

| Opção | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Ativa ou desativa a aceleração do extrator. |
| `SpeedMultiplier` | `int` | `5` | Multiplicador de velocidade (limitado entre 1x e 20x). |
| `AffectsChlorophyteExtractinator` | `bool` | `true` | Aplica a aceleração também ao Extrator de Clorofita. |
| `BatchExtractionSize` | `int` | `1` | Quantidade de itens consumidos por operação em lote. |

---

## 🔧 Métodos em Runtime Interceptados

| Classe Alvo | Método Alvo | Tipo de Hook | Função |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `PlaceThing_ItemInExtractinator(int, int)` | `Postfix` | Reduz proporcionalmente o `itemTime` e `itemAnimation` do jogador para acelerar o ciclo. |

---

## 📁 Estrutura do Plugin

```text
mods/TurboExtractinator/
├── manifest.json                  # Identidade, dependências e metadados
├── TurboExtractinator.dll         # Assembly compilado do plugin
├── TurboExtractinator.pdb         # Símbolos de depuração
├── README.md                      # Documentação em inglês
├── README_pt-BR.md                # Documentação em português
└── config.json                    # Configurações em tempo de execução
```
