<div align="center">

# 🐷 PiggyVault — Automação e Funcionalidades da Void Bag no Porquinho (Piggy Bank)

**Concede ao Porquinho (`player.bank`) todas as funcionalidades modernas da Void Bag (`player.bank4`) — incluindo coleta automática por transbordamento (vácuo), criação direta de itens (crafting), Quick Buff/Cura/Mana, consumo de munições/iscas e acessórios de informação — preservando 100% das funções clássicas do Porquinho e integridade dos arquivos.**

<p align="center">
  <img src="https://img.shields.io/badge/Terraria-1.4.5.7-22c55e?style=for-the-badge&logo=steam&logoColor=white" alt="Terraria 1.4.5.7">
  <img src="https://img.shields.io/badge/Framework-Harmony_2.4.2-6366f1?style=for-the-badge&logo=csharp&logoColor=white" alt="Harmony 2.4.2">
  <img src="https://img.shields.io/badge/Type-Plugin_TMC-06b6d4?style=for-the-badge" alt="Plugin TMC">
  <img src="https://img.shields.io/badge/Licen%C3%A7a-MIT-10b981?style=for-the-badge" alt="Licença MIT">
</p>

</div>

---

## 🌟 Principais Funcionalidades

- **📦 Coleta Automática (Vácuo) no Porquinho**:
  - Quando o inventário principal (slots 0..49) estiver cheio, itens e moedas coletados do chão são automaticamente transferidos e empilhados dentro do seu Porquinho (`player.bank.item`).
  - Apresenta feedback visual e sonoro nativo.

- **🔨 Criação Direta (Crafting) Usando Itens do Porquinho**:
  - Integra-se ao `Recipe.CollectItemsFromChests` para que as bancadas de trabalho e receitas de criação usem materiais armazenados dentro do seu Porquinho sem você precisar abri-lo ou colocá-lo no chão.

- **🧪 Cura Rápida, Mana Rápida e Buff Rápido**:
  - **Cura Rápida (`H`)**: Bebe poções de cura guardadas no Porquinho se não houver no inventário.
  - **Mana Rápida (`M`)**: Bebe poções de mana guardadas no Porquinho durante o uso de magias.
  - **Buff Rápido (`B`)**: Detecta buffs ausentes e consome poções e a melhor comida guardada no Porquinho.

- **🏹 Consumo Automático de Munições, Fios e Iscas**:
  - Dispara flechas, balas, foguetes e usa fios, atuadores e iscas de pesca armazenados no Porquinho quando o inventário não tiver esses itens.

- **🧭 Ativação de Acessórios de Informação**:
  - Celular (Cell Phone), PDA, Bússola, Medidor de Profundidade, Relógio, GPS, Medidor de DPS, Detector de Metais, Radar e outros acessórios fornecem informações na tela mesmo guardados dentro do Porquinho.

- **🌀 Poção de Teleporte para Amigos (Wormhole / Unity)**:
  - Permite clicar e teleportar para companheiros de equipe no mapa usando Poções de Wormhole guardadas no Porquinho.

- **🛡️ 100% Seguro e Não-Destrutivo**:
  - Mantém todas as funções originais do Porquinho (guardar moedas, compras com NPCs, Calha de Dinheiro / Money Trough, Chester) sem nenhuma perda.

---

## ⚙️ Configuração (`config.json`)

O arquivo de configuração fica localizado em `mods/PiggyVault/config.json`:

```json
{
  "Enabled": true,
  "RequirePiggyItemInInventory": true,
  "AutoPickupToPiggyBank": true,
  "CraftFromPiggyBank": true,
  "QuickBuffFromPiggyBank": true,
  "QuickHealFromPiggyBank": true,
  "QuickManaFromPiggyBank": true,
  "ConsumeAmmoAndBaitFromPiggyBank": true,
  "InfoAccessoriesInPiggyBank": true,
  "WormholePotionFromPiggyBank": true,
  "PlayPickupSound": true,
  "ShowPickupText": true
}
```

### Referência das Opções

| Opção | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Interruptor mestre que ativa ou desativa o PiggyVault. |
| `RequirePiggyItemInInventory` | `bool` | `true` | Exige carregar o Porquinho, Calha de Dinheiro (Money Trough) ou Osso Ocular (Chester) no inventário. Se `false`, funciona sempre. |
| `AutoPickupToPiggyBank` | `bool` | `true` | Envia itens para o Porquinho quando o inventário estiver cheio. |
| `CraftFromPiggyBank` | `bool` | `true` | Permite usar ingredientes do Porquinho em receitas de criação. |
| `QuickBuffFromPiggyBank` | `bool` | `true` | Permite consumo de poções e comida do Porquinho pelo Buff Rápido (`B`). |
| `QuickHealFromPiggyBank` | `bool` | `true` | Permite uso de poções de vida do Porquinho pela Cura Rápida (`H`). |
| `QuickManaFromPiggyBank` | `bool` | `true` | Permite uso de poções de mana do Porquinho pela Mana Rápida (`M`). |
| `ConsumeAmmoAndBaitFromPiggyBank` | `bool` | `true` | Permite consumir munições, fios e iscas do Porquinho. |
| `InfoAccessoriesInPiggyBank` | `bool` | `true` | Ativa informações de acessórios guardados no Porquinho. |
| `WormholePotionFromPiggyBank` | `bool` | `true` | Permite usar Poções de Wormhole guardadas no Porquinho no mapa. |
| `PlayPickupSound` | `bool` | `true` | Toca som ao armazenar itens automaticamente no Porquinho. |
| `ShowPickupText` | `bool` | `true` | Exibe texto popup indicando coleta para o Porquinho. |
