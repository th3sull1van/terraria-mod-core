<div align="center">

# AutoBuff

**Consome automaticamente poções de buff e alimentos do inventário e Void Bag quando as durações expiram, garantindo 100% de tempo ativo com zero modificação de arquivos.**

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

- **Detecção Nativa de Expiração de Buffs**:
  - Avalia automaticamente os buffs ativos do jogador e o tempo restante durante o `Player.Update`.
  - Consome as poções de buff correspondentes no instante em que o efeito expira.

- **Consumo Inteligente de Alimentos por Nível**:
  - Detecta o status de alimentação (*Bem Alimentado / Muito Satisfeito / Extremamente Satisfeito*).
  - Escolhe e consome automaticamente o alimento de maior nível disponível no inventário assim que a nutrição acaba.

- **Renovação Automática de Frascos / Encantamentos de Armas**:
  - Mantém frascos de combate corpo a corpo (Ichor, Chamas Malditas, Fogo, Ouro, Veneno, Peçonha, Nanites, Confete) ativos continuamente.

- **Integração Completa com Void Bag & Piggy Bank**:
  - Varre perfeitamente itens guardados no Void Bag (`bank4`) e no Porquinho Cofrinho (`bank`) quando carregados ou abertos.

- **Lista de Exclusão Configurável & Travas de Segurança**:
  - Exclusões padrão seguras para itens situacionais ou perigosos (como *Poção de Gravidade* ou *Poção Vermelha* em mundos normais).
  - Listas personalizáveis de Buffs e Itens excluídos no `config.json`.

---

## Referência de Configuração

Localizado em `mods/AutoBuff/config.json`:

```json
{
  "Enabled": true,
  "CheckIntervalTicks": 15,
  "IncludeFood": true,
  "IncludeFlasks": true,
  "IncludeVoidBag": true,
  "IncludePiggyBank": true,
  "MinBuffTimeThresholdTicks": 0,
  "ExcludedBuffIds": [
    18,
    119,
    120
  ],
  "ExcludedItemIds": [
    1344,
    2756
  ]
}
```

| Opção | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Chave geral para ativar ou desativar o AutoBuff. |
| `CheckIntervalTicks` | `int` | `15` | Intervalo em ticks de jogo (60 ticks = 1s) entre checagens do inventário (15 ticks = 4 checagens/seg). |
| `IncludeFood` | `bool` | `true` | Consome automaticamente o melhor alimento quando o efeito Bem Alimentado expirar. |
| `IncludeFlasks` | `bool` | `true` | Renova automaticamente frascos e encantamentos de armas. |
| `IncludeVoidBag` | `bool` | `true` | Varre poções e comidas armazenadas no Void Bag aberto. |
| `IncludePiggyBank` | `bool` | `true` | Varre poções e comidas no Piggy Bank quando carregado ou aberto. |
| `MinBuffTimeThresholdTicks` | `int` | `0` | Reaplica o buff se o tempo restante for menor que este limite (0 = apenas ao expirar). |
| `ExcludedBuffIds` | `int[]` | `[18, 119, 120]` | Lista de IDs de Buffs excluídos do consumo automático. |
| `ExcludedItemIds` | `int[]` | `[1344, 2756]` | Lista de IDs de Itens excluídos do consumo automático. |

---

## Métodos em Runtime Interceptados

| Classe Alvo | Método Alvo | Tipo de Hook | Função |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `Update(int i)` | `Postfix` | Avalia durações de buffs e dispara o consumo seguro de poções e comidas para o jogador local (`i == Main.myPlayer`). |

---

## Estrutura do Plugin

```text
mods/AutoBuff/
├── manifest.json       # Identidade, dependências e metadados
├── AutoBuff.dll        # Assembly compilado do plugin
├── AutoBuff.pdb        # Símbolos de depuração
├── README.md           # Documentação em inglês
├── README_pt-BR.md     # Documentação em português
└── config.json         # Configurações em tempo de execução
```

---

## Licença

MIT © [th3sull1van](https://github.com/th3sull1van)
