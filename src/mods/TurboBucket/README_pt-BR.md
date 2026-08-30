<div align="center">

# TurboBucket

**Acelera a velocidade de despejo e manipulação de baldes de líquidos e baldes sem fundo no Vanilla Terraria com fluxo contínuo a 60 TPS e zero modificação de arquivos.**

<p align="center">
  <a href="README.md"><b>English</b></a> •
  <a href="README_pt-BR.md"><b>Português (Brasil)</b></a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Terraria-1.4.5.8%20%7C%201.4.5.7-22c55e?style=for-the-badge&logo=steam&logoColor=white" alt="Terraria 1.4.5.8 / 1.4.5.7">
  <img src="https://img.shields.io/badge/Framework-Harmony%202.4.2-6366f1?style=for-the-badge&logo=csharp&logoColor=white" alt="Harmony 2.4.2">
  <img src="https://img.shields.io/badge/Tipo-Plugin%20TMC-06b6d4?style=for-the-badge" alt="Plugin TMC">
  <img src="https://img.shields.io/badge/Velocidade-10x%20%2F%2060%20TPS-f59e0b?style=for-the-badge" alt="Velocidade 10x / 60 TPS">
  <img src="https://img.shields.io/badge/Licen%C3%A7a-MIT-3b82f6?style=for-the-badge" alt="Licença MIT">
</p>

</div>

---

## Principais Recursos

- **Multiplicador de Velocidade Configurável**:
  - Acelera o despejamento de baldes do padrão de 10 ticks para até 2 ticks (30 despejos/seg a 5x) ou 1 tick (60 despejos/seg a 10x).

- **Aceleração para Mel, Lava e Água**:
  - Esvazia e derrama mel instantaneamente sem lentidão.
  - Cria rapidamente pontes de obsidiana ou preenche hellevators com lava.
  - Criação rápida de lagos artificiais e restauração de oceanos.

- **Suporte a Baldes Sem Fundo**:
  - Totalmente compatível com Baldes Sem Fundo de Água, Lava, Mel e Shimmer.

- **Aceleração Opcional para Baldes Vazios & Esponjas**:
  - Aceleração opcional para coleta de líquidos com baldes vazios e secagem com esponjas.

---

## Referência de Configuração

Localizado em `mods/TurboBucket/config.json`:

```json
{
  "Enabled": true,
  "SpeedMultiplier": 5,
  "AffectsWater": true,
  "AffectsLava": true,
  "AffectsHoney": true,
  "AffectsBottomlessBuckets": true,
  "AffectsEmptyBuckets": false,
  "AffectsSponges": false
}
```

| Opção | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Ativa ou desativa todas as funções de aceleração do TurboBucket. |
| `SpeedMultiplier` | `int` | `5` | Multiplicador de velocidade (1 a 10). 5 = 5x mais rápido (2 ticks/despejo), 10 = 60 TPS (1 tick/despejo). |
| `AffectsWater` | `bool` | `true` | Acelera o despejamento de Baldes de Água. |
| `AffectsLava` | `bool` | `true` | Acelera o despejamento de Baldes de Lava. |
| `AffectsHoney` | `bool` | `true` | Acelera o despejamento de Baldes de Mel. |
| `AffectsBottomlessBuckets` | `bool` | `true` | Acelera Baldes Sem Fundo (Água, Lava, Mel, Shimmer). |
| `AffectsEmptyBuckets` | `bool` | `false` | Acelera a coleta de líquidos com Baldes Vazios. |
| `AffectsSponges` | `bool` | `false` | Acelera a absorção de líquidos com esponjas. |

---

## Métodos em Runtime Interceptados

| Classe Alvo | Método Alvo | Tipo de Hook | Função |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `ItemCheck_UseBuckets(Item sItem)` | `Postfix` | Reduz os tempos de recarga de `player.itemTime` e `player.itemAnimation` pelo fator `SpeedMultiplier`. |

---

## Estrutura do Plugin

```text
mods/TurboBucket/
├── manifest.json       # Identidade, dependências e metadados
├── TurboBucket.dll     # Assembly compilado do plugin
├── TurboBucket.pdb     # Símbolos de depuração
├── README.md           # Documentação em inglês
├── README_pt-BR.md     # Documentação em português
└── config.json         # Configurações em tempo de execução
```

---

## Licença

MIT © [th3sull1van](https://github.com/th3sull1van)
