<div align="center">

# FishingLinePlus

**Lance e controle múltiplas linhas de pesca simultâneas e independentes com física de dispersão de velocidade, sincronização de fisgada em dupla camada e legitimidade total de drops vanilla.**

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

- **Múltiplas Linhas Simultâneas Configuráveis**:
  - Supera a limitação padrão do Terraria de apenas 1 boia por jogador, suportando até **4 (ou mais)** linhas funcionais simultâneas.

- **Dispersão em Leque Realista & Variação de Velocidade**:
  - Calcula automaticamente offsets angulares (`SpreadAngleDegrees`) e variações de velocidade (`VelocitySpread`) para que todas as boias se espalhem naturalmente na água em vez de sobreporem o mesmo ponto.

- **Sincronização de Fisgada em Dupla Camada**:
  - **Sincronização Dinâmica na Água (`BobberSyncPatch`)**: Quando qualquer uma das boias ativas do jogador recebe uma fisgada (`ai[1] < 0`), as demais boias na água realizam rolagens com `FishingCheck()`. Visualmente, todas as boias espirram água e afundam em uníssono.
  - **Garantia de Captura Múltipla ao Recolher (`BobberPullPatch`)**: Ao puxar a linha (manualmente ou via `AutoFishing`), todas as boias flutuando na água (`ai[0] == 0f`) checam suas tabelas de loot de pesca antes do retorno, capturando e puxando itens simultaneamente.

- **Regras Legítimas de Iscas & Caixa de Pesca (Tackle Box)**:
  - Cada peixe ou caixa capturada consome sua respectiva isca do inventário, respeitando integralmente a chance da Caixa de Pesca (`accTackleBox`).

---

## Referência de Configuração

Localizado em `mods/FishingLinePlus/config.json`:

```json
{
  "Enabled": true,
  "MaxActiveFishingLines": 4,
  "LinesPerCast": 4,
  "SpreadAngleDegrees": 7.0,
  "VelocitySpread": 0.08
}
```

| Opção | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Ativa ou desativa as capacidades de múltiplas linhas de pesca. |
| `MaxActiveFishingLines` | `int` | `4` | Limite máximo total de boias que o jogador pode manter ativas simultaneamente (de 1 a 20). |
| `LinesPerCast` | `int` | `4` | Quantidade de boias lançadas a cada clique de arremesso (limitado de 1 a `MaxActiveFishingLines`). |
| `SpreadAngleDegrees` | `double` | `7.0` | Abertura angular de dispersão entre as trajetórias das boias em graus. |
| `VelocitySpread` | `double` | `0.08` | Porcentagem aleatória de variação de velocidade por boia para distribuição natural. |

---

## Métodos em Runtime Interceptados

| Classe Alvo | Método Alvo | Tipo de Hook | Função |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `ItemCheck_Shoot(int i, Item sItem, int weaponDamage)` | `Postfix` | Dispara projéteis de boia adicionais com dispersão angular ao arremessar a vara. |
| `Terraria.Player` | `ItemCheck_PullFishingBobbers(Item sItem)` | `Prefix` | Garante a checagem das tabelas de loot de pesca em todas as boias ativas na água antes de recolhê-las. |
| `Terraria.Projectile` | `AI_061_FishingBobber()` | `Postfix` | Sincroniza estados de mordida e animações de respingo de água entre todas as boias ativas. |

---

## Estrutura do Plugin

```text
mods/FishingLinePlus/
├── manifest.json            # Identidade, dependências e metadados
├── FishingLinePlus.dll      # Assembly compilado do plugin
├── FishingLinePlus.pdb      # Símbolos de depuração
├── README.md                # Documentação em inglês
├── README_pt-BR.md          # Documentação em português
└── config.json              # Configurações em tempo de execução
```

---

## Licença

MIT © [th3sull1van](https://github.com/th3sull1van)
