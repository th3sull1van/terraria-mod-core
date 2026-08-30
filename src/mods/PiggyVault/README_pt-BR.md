<div align="center">

# PiggyVault

**Concede ao Porquinho Cofrinho (`player.bank`) todas as capacidades modernas do Void Bag (`player.bank4`) — incluindo auto-coleta de transbordamento, criação direta de itens, Buff/Cura/Mana Rápida, consumo de munição/isca e acessórios informativos — com 100% de integridade dos arquivos originais.**

<p align="center">
  <a href="README.md"><b>English</b></a> •
  <a href="README_pt-BR.md"><b>Português (Brasil)</b></a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Terraria-1.4.5.8%20%7C%201.4.5.7-22c55e?style=for-the-badge&logo=steam&logoColor=white" alt="Terraria 1.4.5.8 / 1.4.5.7">
  <img src="https://img.shields.io/badge/Framework-Harmony%202.4.2-6366f1?style=for-the-badge&logo=csharp&logoColor=white" alt="Harmony 2.4.2">
  <img src="https://img.shields.io/badge/Tipo-Plugin%20TMC-06b6d4?style=for-the-badge" alt="Plugin TMC">
  <img src="https://img.shields.io/badge/Licen%C3%A7a-MIT-3b82f6?style=for-the-badge" alt="Licença MIT">
</p>

</div>

---

## Principais Recursos

- **Auto-Coleta de Transbordamento / Aspirador**:
  - Captura automaticamente itens e moedas do mundo quando o inventário principal (slots 0..49) estiver cheio, enviando-os diretamente para o Porquinho Cofrinho (`player.bank.item`).
  - Exibe notificações visuais de texto de coleta e efeitos sonoros compatíveis com o cofre do void.

- **Criação Direta a partir do Piggy Bank**:
  - Integra-se a `Recipe.CollectItemsFromChests` para que estações de criação reconheçam materiais guardados no Porquinho sem precisar colocá-lo no chão ou retirar itens manualmente.

- **Buff Rápido, Cura Rápida & Mana Rápida**:
  - **Cura Rápida (`H`)**: Consome poções de vida do Piggy Bank caso faltem no inventário.
  - **Mana Rápida (`M`)**: Consome poções de mana do Piggy Bank durante conjurações intensas de magia.
  - **Buff Rápido (`B`)**: Identifica buffs ausentes e consome poções e melhores alimentos diretamente do Piggy Bank.

- **Consumo Direto de Munições, Fios & Iscas**:
  - Dispara flechas, balas, foguetes e utiliza fios, atuadores e iscas guardadas no Piggy Bank quando ausentes do inventário principal.

- **Ativação de Acessórios Informativos**:
  - Celular, PDA, Bússola, Medidor de Profundidade, Relógio, GPS, Medidor de DPS, Detector de Metais, Radar e outros acessórios informativos funcionam normalmente dentro do Piggy Bank.

- **Poções de Retorno / Teleporte de Equipe (Wormhole)**:
  - Teleporta para companheiros de equipe no mapa em tela cheia usando Poções de Teleporte guardadas no Piggy Bank.

- **100% Seguro & em Memória**:
  - Preserva todas as funções originais do Piggy Bank (guardar moedas, compras com mercadores, Money Trough, Chester) com zero modificação em disco.

---

## Referência de Configuração

Localizado em `mods/PiggyVault/config.json`:

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

| Opção | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Chave geral para ativar ou desativar o PiggyVault. |
| `RequirePiggyItemInInventory` | `bool` | `true` | Exige carregar um Porquinho Cofrinho, Money Trough ou Chester no inventário para ativar as funções. Quando `false`, fica sempre ativo. |
| `AutoPickupToPiggyBank` | `bool` | `true` | Envia itens coletados para o Piggy Bank quando o inventário está cheio. |
| `CraftFromPiggyBank` | `bool` | `true` | Permite usar ingredientes do Piggy Bank em receitas de criação. |
| `QuickBuffFromPiggyBank` | `bool` | `true` | Ativa consumo de buffs e comidas do Piggy Bank com Buff Rápido. |
| `QuickHealFromPiggyBank` | `bool` | `true` | Ativa Cura Rápida usando poções do Piggy Bank. |
| `QuickManaFromPiggyBank` | `bool` | `true` | Ativa Mana Rápida usando poções do Piggy Bank. |
| `ConsumeAmmoAndBaitFromPiggyBank` | `bool` | `true` | Permite consumir munições, fios e iscas do Piggy Bank. |
| `InfoAccessoriesInPiggyBank` | `bool` | `true` | Mantém acessórios informativos ativos dentro do Piggy Bank. |
| `WormholePotionFromPiggyBank` | `bool` | `true` | Permite usar Poções de Teleporte (Wormhole) guardadas no Piggy Bank. |
| `PlayPickupSound` | `bool` | `true` | Toca efeito sonoro ao armazenar itens no Piggy Bank. |
| `ShowPickupText` | `bool` | `true` | Mostra texto flutuante ao sugar itens para o Piggy Bank. |

---

## Métodos em Runtime Interceptados

| Classe Alvo | Método Alvo | Tipo de Hook | Função |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `GetItem` | `Postfix` | Redireciona itens e moedas de transbordamento para o Piggy Bank. |
| `Terraria.Player` | `ItemSpaceForCofveve` | `Postfix` | Sinaliza elegibilidade de coleta do mundo quando o Piggy Bank tem espaço. |
| `Terraria.Recipe` | `CollectItemsFromChests` | `Postfix` | Adiciona o Piggy Bank às fontes de materiais disponíveis para criação. |
| `Terraria.Player` | `QuickHeal_GetItemToUse` | `Postfix` | Busca alternativa por poções de cura no Piggy Bank. |
| `Terraria.Player` | `QuickMana_GetItemToUse` | `Postfix` | Busca alternativa por poções de mana no Piggy Bank. |
| `Terraria.Player` | `QuickBuff_PickBestFoodItem` | `Postfix` | Busca alternativa pelo melhor alimento no Piggy Bank. |
| `Terraria.Player` | `QuickBuff` | `Postfix` | Aplica buffs de poções ausentes diretamente do Piggy Bank. |
| `Terraria.Player` | `ConsumeItem` | `Postfix` | Consome munição, fios e iscas do Piggy Bank. |
| `Terraria.Player` | `HasUnityPotion` | `Postfix` | Checa presença de Poções de Teleporte no Piggy Bank. |
| `Terraria.Player` | `TakeUnityPotion` | `Prefix/Postfix` | Consome Poções de Teleporte do Piggy Bank. |
| `Terraria.Player` | `RefreshInfoAccs` | `Postfix` | Atualiza a interface de acessórios informativos guardados no Piggy Bank. |

---

## Estrutura do Plugin

```text
mods/PiggyVault/
├── manifest.json       # Identidade, dependências e metadados
├── PiggyVault.dll      # Assembly compilado do plugin
├── PiggyVault.pdb      # Símbolos de depuração
├── README.md           # Documentação em inglês
├── README_pt-BR.md     # Documentação em português
└── config.json         # Configurações em tempo de execução
```

---

## Licença

MIT © [th3sull1van](https://github.com/th3sull1van)
