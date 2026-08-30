<div align="center">

# OreCascade

**Mineração instantânea em cadeia para minérios e pedras preciosas com injeção IL em runtime, isolamento estrito de veios, preservação legítima de drops e zero modificação de arquivos.**

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

- **Zero Dependência de tModLoader & Integridade 100% dos Arquivos**:
  - Executa nativamente através do framework **TerrariaModCore (TMC)** no Terraria oficial 1.4.5.8 / 1.4.5.7.
  - O executável original `Terraria.exe` permanece 100% intacto no disco.

- **Busca em Largura Iterativa (BFS)**:
  - Descobre veios de minério contíguos dinamicamente com complexidade temporal e espacial $O(V)$.
  - **Isolamento Estrito de Veios**: Veios adjacentes de materiais diferentes (ex.: Ouro encostado em Cobre) são rigorosamente isolados quando `RequireSameOreType` está ativo.
  - **Conexão Diagonal**: Suporte a exploração opcional em 8 direções para formações complexas de veios.

- **Drops Legítimos do Motor Vanilla & Segurança de Picareta**:
  - Blocos são destruídos via `WorldGen.KillTile`, preservando tabelas de drop vanilla, moedas da sorte, partículas, conquistas e efeitos sonoros.
  - Respeita rigorosamente o poder de picareta vanilla (ex.: Cobalto requer 100% de picareta, Clorofita requer 200%).
  - Guard de reentrância `[ThreadStatic] bool _isCascading` elimina riscos de recursão infinita.

- **Sincronizado no Multiplayer**:
  - Transmite automaticamente pacotes de manipulação de blocos (`NetMessage.SendData(17, ...)`) em sessões multiplayer de clientes, sincronizando a destruição em tempo real.

---

## Referência de Configuração

Localizado em `mods/OreCascade/config.json`:

```json
{
  "Enabled": true,
  "MaxBlocksPerActivation": 100,
  "AllowDiagonalConnections": false,
  "RequireSameOreType": true,
  "IncludeGems": true,
  "IncludeExtractables": true
}
```

| Opção | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Ativa ou desativa todas as funções de mineração em cascata. |
| `MaxBlocksPerActivation` | `int` | `100` | Número máximo de blocos minerados por ativação (limitado de 1 a 500). |
| `AllowDiagonalConnections` | `bool` | `false` | Quando `true`, pesquisa vizinhos em 8 direções em vez de apenas 4 ortogonais. |
| `RequireSameOreType` | `bool` | `true` | Quando `true`, restringe a mineração estritamente ao mesmo tipo de minério e identidade de frame. |
| `IncludeGems` | `bool` | `true` | Quando `true`, ativa a mineração em cadeia para pedras preciosas (Ametista, Diamante, Âmbar, etc.). |
| `IncludeExtractables` | `bool` | `true` | Quando `true`, ativa mineração em cadeia para blocos extraíveis (Lodo, Neve com Terra, Fóssil do Deserto). |

---

## Minérios, Gemas e Extraíveis Suportados

| Categoria | Blocos Incluídos |
| :--- | :--- |
| **Pré-Hardmode** | Cobre, Estanho, Ferro, Chumbo, Prata, Tungstênio, Ouro, Platina, Meteorito, Demonita, Carminita, Obsidiana, Pedra do Inferno |
| **Extraíveis e Fósseis** | Bloco de Lodo, Neve com Terra, Fóssil do Deserto, Minério de Fóssil |
| **Hardmode (Tiers 1-3)** | Cobalto, Paládio, Mithril, Oricalco, Adamantita, Titânio |
| **Endgame e Celestial** | Clorofita, Luminita (Minério Lunar) |
| **Pedras Preciosas** | Ametista, Topázio, Safira, Esmeralda, Rubi, Diamante, Âmbar |

---

## Métodos em Runtime Interceptados

| Classe Alvo | Método Alvo | Tipo de Hook | Função |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `PickTile(int x, int y, int pickPower)` | `Prefix` & `Postfix` | Captura o estado do bloco antes do impacto e aciona a BFS iterativa quando a destruição é confirmada. |

---

## Estrutura do Plugin

```text
mods/OreCascade/
├── manifest.json       # Identidade, dependências e metadados
├── OreCascade.dll      # Assembly compilado do plugin
├── OreCascade.pdb      # Símbolos de depuração
├── README.md           # Documentação em inglês
├── README_pt-BR.md     # Documentação em português
└── config.json         # Configurações em tempo de execução
```

---

## Licença

MIT © [th3sull1van](https://github.com/th3sull1van)
