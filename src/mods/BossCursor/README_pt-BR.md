<div align="center">

# BossCursor

**Setas indicadoras de direção e ícones da cabeça dos bosses em tempo real para o Vanilla Terraria com escalonamento por proximidade e zero modificação de arquivos.**

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

- **Seta Direcional & Ícone da Cabeça do Boss**:
  - Aponta diretamente para bosses e inimigos rastreados em tempo real.
  - Renderiza o ícone oficial da cabeça do boss junto à ponta da seta.

- **Escalonamento & Transparência Dinâmicos por Proximidade**:
  - Conforme o boss se aproxima do jogador, a seta e o ícone tornam-se maiores e mais opacos.
  - Conforme o boss se afasta ou fica fora da tela, o indicador diminui de tamanho suavemente e fica translúcido.

- **Suporte a Poção de Gravidade & Inversão Vertical**:
  - Detecta automaticamente gravidade invertida (`gravDir == -1f`) e ajusta todos os ângulos e coordenadas para manter os ponteiros 100% precisos.

- **Ocultação em Mapa de Tela Cheia**:
  - Oculta os indicadores automaticamente sempre que o mapa em tela cheia estiver aberto (`Main.mapStyle == 2`).

- **Sempre Ativo & Integrado**:
  - Funciona continuamente em segundo plano sempre que bosses ou mini-bosses estiverem presentes no mundo.
  - Zero poluição de teclas de atalho ou interrupções acidentais.

- **Lista Branca & Lista Negra Personalizáveis**:
  - Filtra os 4 Pilares Celestiais (Solar, Nebula, Vortex, Stardust) por padrão.
  - Permite adicionar qualquer ID de NPC à lista branca (ex.: Dreadnautilus, Mourning Wood, Pumpking, Martian Saucer) ou à lista negra.

- **API Extensível para Modders**:
  - Permite registrar ou desregistrar NPCs programaticamente via `BossCursorAPI`.

---

## Referência de Configuração

Localizado em `mods/BossCursor/config.json`:

```json
{
  "Enabled": true,
  "HideOnScreen": false,
  "CursorDistance": 150,
  "CursorSize": 1.0,
  "HeadOffset": 45.0,
  "BlacklistPillars": true,
  "ExcludedNpcIds": [],
  "IncludedNpcIds": []
}
```

| Opção | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Chave geral para ativar ou desativar o Boss Cursor (sempre ativo enquanto true). |
| `HideOnScreen` | `bool` | `false` | Se `true`, oculta o cursor caso o boss já esteja visível dentro da tela. |
| `CursorDistance` | `int` | `150` | Distância radial (em pixels) do centro do jogador até o cursor (`0` a `500`). |
| `CursorSize` | `float` | `1.0` | Multiplicador de escala para a seta e cabeça do boss (`0.1` a `2.0`). |
| `HeadOffset` | `float` | `45.0` | Distância radial em pixels entre a seta indicadora e a cabeça do boss. |
| `BlacklistPillars` | `bool` | `true` | Se `true`, oculta indicadores dos 4 Pilares Celestiais. |
| `ExcludedNpcIds` | `int[]` | `[]` | Lista personalizada de IDs de NPC que nunca exibirão cursor. |
| `IncludedNpcIds` | `int[]` | `[]` | Lista personalizada de IDs de NPC que sempre exibirão cursor (mini-bosses, eventos). |

---

## API para Desenvolvedores (`BossCursorAPI`)

Outros plugins TMC podem interagir com o Boss Cursor em tempo de execução:

```csharp
using BossCursor;

// Adicionar um NPC personalizado com textura de cabeça opcional
BossCursorAPI.AddToWhitelist(npcId, customHeadTexture);

// Remover um NPC da lista branca
BossCursorAPI.RemoveFromWhitelist(npcId);

// Adicionar um NPC à lista negra
BossCursorAPI.AddToBlacklist(npcId);

// Verificar se um NPC está sendo rastreado
bool isTracked = BossCursorAPI.IsBossTracked(npc);

// Alternar estado do Boss Cursor
BossCursorAPI.SetEnabled(true);
```

---

## Métodos em Runtime Interceptados

| Classe Alvo | Método Alvo | Tipo de Hook | Função |
| :--- | :--- | :--- | :--- |
| `Terraria.Main` | `DrawInterface_36_Cursor()` | `Postfix` | Renderiza setas direcionais e ícones de bosses sobre a camada de interface gráfica. |

---

## Estrutura do Plugin

```text
mods/BossCursor/
├── manifest.json       # Identidade, dependências e metadados
├── BossCursor.dll      # Assembly compilado do plugin
├── BossCursor.pdb      # Símbolos de depuração
├── README.md           # Documentação em inglês
├── README_pt-BR.md     # Documentação em português
└── config.json         # Configurações em tempo de execução
```

---

## Licença

MIT © [th3sull1van](https://github.com/th3sull1van)
