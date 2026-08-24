# 🎯 Boss Cursor — Plugin para TerrariaModCore (TMC)

O **Boss Cursor** adiciona setas indicadoras de direção e ícones da cabeça dos bosses ao redor do seu personagem em tempo real, apontando para bosses e mini-bosses ativos no **Vanilla Terraria 1.4.5.8 / 1.4.5.7**.

Portado do popular mod de tModLoader criado por **kgoyo** ([Steam Workshop #2816694149](https://steamcommunity.com/sharedfiles/filedetails/?id=2816694149)), esta versão roda nativamente dentro do **TerrariaModCore (TMC)** com zero dependência de tModLoader, zero modificação de arquivos no disco e injeção puramente em memória via Harmony IL.

---

## 🌟 Principais Recursos

1. **Seta Direcional e Cabeça do Boss**:
   - Aponta diretamente para bosses e inimigos rastreados em tempo real.
   - Renderiza o ícone oficial da cabeça do boss junto à ponta da seta.

2. **Escalonamento e Transparência Dinâmicos por Proximidade**:
   - Conforme o boss se aproxima do jogador, a seta e o ícone tornam-se maiores e mais opacos.
   - Conforme o boss se afasta ou fica fora da tela, o indicador diminui de tamanho suavemente e fica translúcido.

3. **Suporte a Poção de Gravidade e Inversão Vertical**:
   - Detecta automaticamente gravidade invertida (`gravDir == -1f`) e ajusta todos os ângulos e coordenadas para manter os ponteiros 100% precisos.

4. **Ocultação em Mapa de Tela Cheia**:
   - Oculta os indicadores automaticamente sempre que o mapa em tela cheia estiver aberto (`Main.mapStyle == 2`).

5. **Tecla de Atalho no Jogo**:
   - Ative ou desative o cursor a qualquer momento com a tecla **`B`** (configurável em `config.json`).
   - Exibe mensagem de confirmação na tela (`Boss Cursor enabled` / `Boss Cursor disabled`).
   - Ignora comandos automaticamente enquanto digita no chat, edita placas ou está no menu do jogo.

6. **Lista Branca e Lista Negra Personalizáveis**:
   - Filtra os 4 Pilares Celestiais (Solar, Nebula, Vortex, Stardust) por padrão.
   - Permite adicionar qualquer ID de NPC à lista branca (ex: Dreadnautilus, Mourning Wood, Pumpking, Martian Saucer) ou à lista negra.

7. **API para Desenvolvedores de Mods**:
   - Permite registrar ou desregistrar NPCs programaticamente via `BossCursorAPI`.

---

## ⚙️ Configuração (`config.json`)

Localizado em `<Terraria>/mods/BossCursor/config.json`:

```json
{
  "Enabled": true,
  "HideOnScreen": false,
  "CursorDistance": 150,
  "CursorSize": 1.0,
  "BlacklistPillars": true,
  "ToggleKey": "B",
  "ExcludedNpcIds": [],
  "IncludedNpcIds": []
}
```

| Configuração | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Ativa ou desativa o mod Boss Cursor. |
| `HideOnScreen` | `bool` | `false` | Se `true`, oculta o cursor caso o boss já esteja visível dentro da tela. |
| `CursorDistance` | `int` | `150` | Distância radial (em pixels) do centro do jogador até o cursor (`0` a `500`). |
| `CursorSize` | `float` | `1.0` | Multiplicador de escala para a seta e cabeça do boss (`0.1` a `2.0`). |
| `BlacklistPillars` | `bool` | `true` | Se `true`, oculta indicadores dos 4 Pilares Celestiais. |
| `ToggleKey` | `string` | `"B"` | Nome da tecla de atalho para alternar no jogo (use `"None"` para desativar). |
| `ExcludedNpcIds` | `int[]` | `[]` | Lista personalizada de IDs de NPC que nunca exibirão cursor. |
| `IncludedNpcIds` | `int[]` | `[]` | Lista personalizada de IDs de NPC que sempre exibirão cursor (mini-bosses, eventos). |

---

## 💻 API para Modders (`BossCursorAPI`)

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
