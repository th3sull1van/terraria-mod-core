<div align="center">

# 🔬 AutoResearch — Pesquisa e Sacrifício Automáticos no Modo Journey para Terraria Vanilla

**Pesquisa automática no Modo Journey sem esforço manual: pesquisa e consome itens assim que entram no inventário, preservando 100% dos requisitos de quantidade vanilla com zero modificação de arquivos.**

<p align="center">
  <a href="README.md"><b>English</b></a> •
  <a href="README_pt-BR.md"><b>Português (Brasil)</b></a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Terraria-1.4.5.8_|_1.4.5.7-22c55e?style=for-the-badge&logo=steam&logoColor=white" alt="Terraria 1.4.5.8 / 1.4.5.7">
  <img src="https://img.shields.io/badge/Framework-Harmony_2.4.2-6366f1?style=for-the-badge&logo=csharp&logoColor=white" alt="Harmony 2.4.2">
  <img src="https://img.shields.io/badge/Type-TMC_Plugin-06b6d4?style=for-the-badge" alt="TMC Plugin">
  <img src="https://img.shields.io/badge/License-MIT-10b981?style=for-the-badge" alt="License MIT">
</p>

</div>

---

## 🌟 Principais Funcionalidades

- **⚡ Pesquisa Instantânea ao Coletar Itens**:
  - Sacrifica e pesquisa itens automaticamente no momento em que são recolhidos do chão, eliminando arrastes manuais para o slot de sacrifício do menu.
  - Se a pilha recolhida exceder o número necessário (ex: pegar 100 Madeiras quando são necessárias apenas 40), exatamente 40 Madeiras são sacrificadas para desbloquear duplicação e as 60 restantes permanecem no inventário.

- **🔄 Varredura Contínua no Inventário e Void Bag**:
  - Uma varredura periódica em segundo plano inspeciona o inventário do jogador, o item sob o cursor (`Main.mouseItem`) e a Void Bag em busca de itens obtidos por criação, compras em NPCs ou baús.

- **🛡️ 100% Fiel às Regras Vanilla**:
  - Consulta o catálogo oficial `CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId` para garantir que as cotas oficiais de pesquisa sejam rigorosamente respeitadas.
  - Emite notificações visuais e efeitos sonoros originais de pesquisa.

---

## ⚙️ Configuração (`config.json`)

O arquivo de configuração está localizado em `mods/AutoResearch/config.json`:

```json
{
  "Enabled": true,
  "ScanIntervalTicks": 1,
  "IncludeVoidBag": true,
  "PlaySound": true,
  "ShowNotifications": true,
  "ExcludedItemIds": []
}
```

### Referência de Opções

| Opção | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Ativa ou desativa a pesquisa automática no modo Journey. |
| `ScanIntervalTicks` | `int` | `1` | Intervalo em ticks entre varreduras (1 tick = 60 verificações por segundo). |
| `IncludeVoidBag` | `bool` | `true` | Pesquisa e sacrifica itens armazenados na Void Bag. |
| `PlaySound` | `bool` | `true` | Reproduz os efeitos sonoros originais ao pesquisar itens. |
| `ShowNotifications` | `bool` | `true` | Exibe textos de notificação flutuantes ao concluir a pesquisa de um item. |
| `ExcludedItemIds` | `int[]` | `[]` | Lista de IDs de itens excluídos do sacrifício automático. |

---

## 🔧 Métodos em Runtime Interceptados

| Classe Alvo | Método Alvo | Tipo de Hook | Função |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `Update(int i)` | `Postfix` | Executa a rotina de varredura periódica e sacrifício para o jogador local. |

---

## 📁 Estrutura do Plugin

```text
mods/AutoResearch/
├── manifest.json            # Identidade, dependências e metadados
├── AutoResearch.dll         # Assembly compilado do plugin
├── AutoResearch.pdb         # Símbolos de depuração
├── README.md                # Documentação em inglês
├── README_pt-BR.md          # Documentação em português
└── config.json              # Configurações em tempo de execução
```
