<div align="center">

# AutoOpen

**Abertura contínua e acelerada de recipientes, caixas de pesca, ostras, bolsas de tesouro, caixas trancadas e presentes ao segurar o botão direito (estilo Extractinator) com zero modificação de arquivos.**

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

- **Abertura Contínua Rápida ao Segurar o Botão**:
  - Elimina a exigência de clicar e soltar repetidamente o botão do mouse.
  - Basta segurar o Botão Direito sobre qualquer pilha de caixas ou bolsas para abri-las continuamente em alta velocidade (estilo Extractinator).

- **Amplo Suporte a Recipientes**:
  - **Caixas de Pesca**: Madeira, Ferro, Ouro, Sagrada, Calabouço, Oceano, Selva, Céu, Corrupção, Carmim e variantes do Hardmode.
  - **Bolsas de Tesouro de Bosses**: Todas as bolsas de chefes do modo Perito e Mestre.
  - **Bolsas Especiais e Recipientes**: Bolsa de Ervas, Lata de Minhocas, Ostras, Bolsas de Brindes, Presentes e Ovos de Chillet.
  - **Caixas Trancadas**: Caixas Trancadas Douradas (consome Chaves Douradas automaticamente) e Caixas Trancadas de Obsidiana (requer Chave das Sombras no inventário ou Void Bag).

- **Suporte a Processamento em Lote**:
  - `BatchSize` configurável para processar múltiplos recipientes por ciclo de tick para descompactação instantânea de pilhas.

- **Modo Opcional Mãos Livres de Auto-Abertura**:
  - Modo `AutoOpenInventory` descompacta automaticamente bolsas no inventário ou Void Bag em segundo plano sem necessidade de cliques.

- **Segurança de Chaves & Lista de Exclusão**:
  - Interrompe a abertura de forma segura caso chaves obrigatórias (ex.: Chaves Douradas) acabem.
  - Lista de exclusão personalizada `ExcludedItemIds` no `config.json` para reservar recipientes específicos.

---

## Referência de Configuração

Localizado em `mods/AutoOpen/config.json`:

```json
{
  "Enabled": true,
  "RapidRightClickOpen": true,
  "OpenDelayTicks": 3,
  "BatchSize": 1,
  "PlaySound": true,
  "AutoOpenInventory": false,
  "AutoOpenIntervalTicks": 10,
  "IncludeVoidBag": true,
  "ExcludedItemIds": []
}
```

| Opção | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Chave geral para ativar ou desativar o AutoOpen. |
| `RapidRightClickOpen` | `bool` | `true` | Ativa a abertura contínua e rápida ao segurar o botão direito. |
| `OpenDelayTicks` | `int` | `3` | Intervalo em ticks entre aberturas ao segurar o botão direito (3 ticks = 20 aberturas/seg). |
| `BatchSize` | `int` | `1` | Quantidade de recipientes abertos por ciclo (1 a 50). |
| `PlaySound` | `bool` | `true` | Toca o efeito sonoro vanilla de abertura de recipiente. |
| `AutoOpenInventory` | `bool` | `false` | Abertura totalmente automática em segundo plano para bolsas no inventário. |
| `AutoOpenIntervalTicks` | `int` | `10` | Frequência em ticks para varredura do inventário em segundo plano. |
| `IncludeVoidBag` | `bool` | `true` | Varre e abre recipientes guardados no Void Bag. |
| `ExcludedItemIds` | `int[]` | `[]` | Lista de IDs de itens excluídos da abertura automática. |

---

## Métodos em Runtime Interceptados

| Classe Alvo | Método Alvo | Tipo de Hook | Função |
| :--- | :--- | :--- | :--- |
| `Terraria.UI.ItemSlot` | `RightClick(Item[] inv, int context, int slot)` | `Prefix` | Intercepta o clique direito contínuo em recipientes para abertura rápida e previne divisão de pilha no cursor. |
| `Terraria.Player` | `Update(int i)` | `Postfix` | Processa a descompactação automática no inventário quando `AutoOpenInventory` está ativado. |

---

## Estrutura do Plugin

```text
mods/AutoOpen/
├── manifest.json       # Identidade, dependências e metadados
├── AutoOpen.dll        # Assembly compilado do plugin
├── AutoOpen.pdb        # Símbolos de depuração
├── README.md           # Documentação em inglês
├── README_pt-BR.md     # Documentação em português
└── config.json         # Configurações em tempo de execução
```

---

## Licença

MIT © [th3sull1van](https://github.com/th3sull1van)
