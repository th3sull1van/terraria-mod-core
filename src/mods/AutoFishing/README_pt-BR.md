<div align="center">

# AutoFishing

**Automação inteligente de pesca com arremesso automático, detecção nativa de fisgada e recolhimento no Vanilla Terraria com máquina de estados e zero modificação de arquivos.**

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

- **Ciclo de Vida Iniciado pelo Jogador (Início & Parada Manuais)**:
  - Selecionar uma vara de pescar na hotbar **não** arremessa automaticamente.
  - A automação inicia apenas após o jogador selecionar a vara e realizar o primeiro clique de arremesso manual.
  - A automação é interrompida imediatamente sempre que o jogador clica para puxar a linha ou cancelá-la.
  - Trocar o slot selecionado na hotbar redefine a automação com segurança.

- **Zero Macros Externos & Sincronização Nativa com o Game Loop**:
  - Executa diretamente dentro de `Player.Update`, em sincronia com o ciclo de 60 TPS do motor do jogo.
  - Zero captura externa de mouse ou leitura instável de pixels da tela.

- **Detecção Precisa de Fisgada no Motor Vanilla**:
  - Monitora as boias ativas pertencentes ao jogador local (`bobber.ai[1] < 0f && bobber.localAI[1] != 0f`).
  - Pesca o item no instante exato em que a física vanilla confirma a fisgada.

- **Verificação de Iscas no Inventário**:
  - Varre automaticamente os slots 0–57 do inventário em busca de iscas válidas antes de cada arremesso.
  - Pausa a automação de forma segura caso as iscas acabem quando `RequireBait` estiver ativo.

- **Temporizadores Naturais de Reação Configuráveis**:
  - Tempo de atraso para recolhimento (`ReelDelayTicks`) simulando reflexo humano.
  - Cooldown configurável entre capturas (`CastDelayTicks`) antes do próximo arremesso.

- **Totalmente Compatível com Múltiplas Linhas**:
  - Integração perfeita com `FishingLinePlus`. Monitora todas as boias ativas na água e recolhe assim que qualquer boia fisgar um item.

---

## Referência de Configuração

Localizado em `mods/AutoFishing/config.json`:

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

| Opção | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Ativa ou desativa toda a automação de pesca. |
| `AutoCast` | `bool` | `true` | Arremessa a vara de pescar automaticamente enquanto a automação estiver ativa. |
| `AutoReel` | `bool` | `true` | Recolhe a linha automaticamente quando um peixe ou item fisgar. |
| `CastDelayTicks` | `int` | `30` | Intervalo em ticks de jogo (60 ticks = 1 segundo) após o recolhimento antes de arremessar novamente. |
| `ReelDelayTicks` | `int` | `2` | Tempo de reação em ticks entre a detecção de fisgada e o recolhimento. |
| `RequireBait` | `bool` | `true` | Impede novos arremessos caso não haja iscas no inventário do jogador. |

---

## Métodos em Runtime Interceptados

| Classe Alvo | Método Alvo | Tipo de Hook | Função |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `Update(int i)` | `Postfix` | Executa a máquina de estados do controlador de pesca para o jogador local (`i == Main.myPlayer`). |
| `Terraria.Player` | `ItemCheck_Shoot(int i, Item sItem, int weaponDamage)` | `Postfix` | Intercepta o arremesso manual para ativar a automação. |
| `Terraria.Player` | `ItemCheck_PullFishingBobbers(Item sItem)` | `Prefix` | Intercepta o recolhimento manual para desativar a automação. |

---

## Estrutura do Plugin

```text
mods/AutoFishing/
├── manifest.json       # Identidade, dependências e metadados
├── AutoFishing.dll     # Assembly compilado do plugin
├── AutoFishing.pdb     # Símbolos de depuração
├── README.md           # Documentação em inglês
├── README_pt-BR.md     # Documentação em português
└── config.json         # Configurações em tempo de execução
```

---

## Licença

MIT © [th3sull1van](https://github.com/th3sull1van)
