<div align="center">

# AutoResearch

**Pesquisa e sacrifício automáticos de itens no modo Journey assim que entram no inventário, preservando 100% das regras de quantidade vanilla com zero modificação de arquivos.**

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

- **Pesquisa Instantânea na Coleta**:
  - Pesquisa e consome itens no instante exato em que são coletados no mundo (`Player.GetItem`), eliminando arrastar menus manuais e cliques no slot de sacrifício.
  - Se a pilha coletada ultrapassar o número necessário para pesquisa (ex.: pegar 100 Madeiras quando restam 40), exatamente 40 Madeiras são sacrificadas para desbloquear a duplicação e as 60 restantes vão para o inventário.

- **Varredura Automática no Inventário & Void Bag**:
  - Varredura periódica em segundo plano analisa o inventário do jogador, o item ativo no cursor (`Main.mouseItem`) e o Void Bag (`bank4`) para itens obtidos via criação (crafting), compras em NPCs ou baús.

- **Preservação Rigorosa de Quantidades Vanilla**:
  - **Não** altera as quantidades ou limites do catálogo vanilla (ex.: 100 Madeiras, 25 Minérios de Ferro, 1 Espada).
  - Itens com pesquisas parciais acumulam progresso de forma exata até atingir o limite de desbloqueio.

- **Isolamento Estrito do Modo Journey**:
  - Automaticamente inerte em personagens dos modos Clássico, Mediumcore ou Hardcore (`player.difficulty != 3`), mantendo a jogabilidade fora do Journey totalmente intocada.

- **Efeitos Sonoros e Visuais Nativos**:
  - Executa sons nativos de pesquisa (`SoundID.Research` e `SoundID.ResearchComplete`).
  - Exibe notificações coloridas no chat do jogo ao contribuir e ao desbloquear a duplicação infinita.

---

## Referência de Configuração

Localizado em `mods/AutoResearch/config.json`:

```json
{
  "Enabled": true,
  "AutoResearchOnPickup": true,
  "AutoResearchInventory": true,
  "ScanIntervalTicks": 5,
  "IncludeVoidBag": true,
  "PlaySound": true,
  "ShowNotifications": true,
  "ExcludedItemIds": []
}
```

| Opção | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Chave geral para ativar ou desativar o AutoResearch. |
| `AutoResearchOnPickup` | `bool` | `true` | Pesquisa itens automaticamente no momento em que são coletados no mundo. |
| `AutoResearchInventory` | `bool` | `true` | Varre e pesquisa itens que entram no inventário via criação, compras ou baús. |
| `ScanIntervalTicks` | `int` | `5` | Intervalo em ticks de jogo para varredura em segundo plano (~12 checagens/seg). |
| `IncludeVoidBag` | `bool` | `true` | Varre e pesquisa itens dentro do Void Bag (Void Vault). |
| `PlaySound` | `bool` | `true` | Toca os efeitos sonoros originais de pesquisa e desbloqueio. |
| `ShowNotifications` | `bool` | `true` | Exibe notificações no chat do jogo quando itens são pesquisados ou desbloqueados. |
| `ExcludedItemIds` | `int[]` | `[]` | Lista de IDs de itens excluídos da pesquisa automática. |

---

## Métodos em Runtime Interceptados

| Classe Alvo | Método Alvo | Tipo de Hook | Função |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `GetItem(Item newItem, GetItemSettings settings)` | `Prefix` | Intercepta a obtenção de itens e os pesquisa instantaneamente, reduzindo pilhas ou consumindo-os antes de entrar no inventário. |
| `Terraria.Player` | `Update(int i)` | `Postfix` | Varre slots de inventário, item no cursor e cofre do void em segundo plano. |

---

## Estrutura do Plugin

```text
mods/AutoResearch/
├── manifest.json       # Identidade, dependências e metadados
├── AutoResearch.dll    # Assembly compilado do plugin
├── AutoResearch.pdb    # Símbolos de depuração
├── README.md           # Documentação em inglês
├── README_pt-BR.md     # Documentação em português
└── config.json         # Configurações em tempo de execução
```

---

## Licença

MIT © [th3sull1van](https://github.com/th3sull1van)
