<div align="center">

# TurboExtractinator

**Acelera drasticamente a velocidade de processamento do Extractinator e Chlorophyte Extractinator por um multiplicador configurável (padrão 5x) com suporte a lotes e zero modificação de arquivos.**

<p align="center">
  <a href="README.md"><b>English</b></a> •
  <a href="README_pt-BR.md"><b>Português (Brasil)</b></a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Terraria-1.4.5.8%20%7C%201.4.5.7-22c55e?style=for-the-badge&logo=steam&logoColor=white" alt="Terraria 1.4.5.8 / 1.4.5.7">
  <img src="https://img.shields.io/badge/Framework-Harmony%202.4.2-6366f1?style=for-the-badge&logo=csharp&logoColor=white" alt="Harmony 2.4.2">
  <img src="https://img.shields.io/badge/Tipo-Plugin%20TMC-06b6d4?style=for-the-badge" alt="Plugin TMC">
  <img src="https://img.shields.io/badge/Velocidade-5x%20Configur%C3%A1vel-f59e0b?style=for-the-badge" alt="Velocidade 5x Configurável">
  <img src="https://img.shields.io/badge/Licen%C3%A7a-MIT-3b82f6?style=for-the-badge" alt="Licença MIT">
</p>

</div>

---

## Principais Recursos

- **Aceleração de Velocidade Configurável (Padrão 5x)**:
  - Acelera o consumo e a taxa de geração de drops para todos os itens extraíveis (Lodo, Neve com Terra, Fóssil do Deserto, Musgo Brilhante e conversões de Clorofita).
  - Converte milhares de blocos em gemas, moedas, minérios e fósseis em segundos sem esperas demoradas.

- **Suporte Duplo a Extractinators**:
  - Totalmente compatível tanto com o **Extractinator** padrão (`TileID.Extractinator` / 219) quanto com o **Chlorophyte Extractinator** do Hardmode (`TileID.ChlorophyteExtractinator` / 642).

- **100% de Legitimidade nos Drops Vanilla**:
  - Executa as rotinas nativas `Player.ExtractinatorUse` e `ExtractinatorHelper.RollExtractinatorDrop`.
  - Drops de moedas, gemas, minérios, fósseis pré-históricos e itens raros respeitam 100% das distribuições de probabilidade e efeitos sonoros originais.

- **Suporte a Extração em Lote**:
  - Suporte opcional a processamento em lote por ciclo de interação para limpeza ultra-rápida de grandes inventários.

---

## Referência de Configuração

Localizado em `mods/TurboExtractinator/config.json`:

```json
{
  "Enabled": true,
  "SpeedMultiplier": 5,
  "AffectsChlorophyteExtractinator": true,
  "BatchExtractionSize": 1
}
```

| Opção | Tipo | Padrão | Intervalo / Formato | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | `true` / `false` | Chave geral para ativar a aceleração de extração. |
| `SpeedMultiplier` | `int` | `5` | `1` – `60` | Fator de multiplicação de velocidade (5 significa extração 5x mais rápida). |
| `AffectsChlorophyteExtractinator` | `bool` | `true` | `true` / `false` | Quando `true`, aplica a aceleração também ao Chlorophyte Extractinator. |
| `BatchExtractionSize` | `int` | `1` | `1` – `50` | Quantidade de itens processados por ciclo de tick de extração. |

---

## Métodos em Runtime Interceptados

| Classe Alvo | Método Alvo | Tipo de Hook | Função |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `PlaceThing_ItemInExtractinator(Item sItem)` | `Postfix` | Reduz os tempos de recarga de `player.itemTime` e `player.itemAnimation` pelo fator `SpeedMultiplier` e processa extrações em lote adicionais. |

---

## Estrutura do Plugin

```text
mods/TurboExtractinator/
├── manifest.json               # Identidade, dependências e metadados
├── TurboExtractinator.dll      # Assembly compilado do plugin
├── TurboExtractinator.pdb      # Símbolos de depuração
├── README.md                   # Documentação em inglês
├── README_pt-BR.md             # Documentação em português
└── config.json                 # Configurações em tempo de execução
```

---

## Licença

MIT © [th3sull1van](https://github.com/th3sull1van)
