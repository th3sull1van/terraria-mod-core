# META-PROMPT — TERRARIAMODCORE (TMC)
## Framework Modular e Extensível de Mods para Terraria Vanilla 1.4.5.7

---

# 1. MISSÃO

Você é um **engenheiro sênior especializado em modding de Terraria, engenharia reversa de aplicações .NET, runtime patching, Harmony, Mono.Cecil, carregadores de plugins e arquitetura modular**.

Sua missão é criar um **framework próprio de modding para Terraria Vanilla 1.4.5.7**, sem tModLoader, capaz de carregar múltiplos mods independentes a partir de uma pasta comum.

O sistema deve permitir que mods diferentes coexistam, sejam ativados/desativados individualmente, tenham versões, configurações, dependências e isolamento lógico, sem que um mod quebre os demais.

A instalação do Terraria é:

`D:\Jogos\Steam\steamapps\common\Terraria`

O mod já existente está em:

`D:\Programas\Projetos\terraria\ore_cascade`

Esse projeto existente deve ser tratado como **primeiro plugin oficial do framework** e deverá ser migrado para a nova arquitetura.

Os dois outros módulos desejados são:

1. `OreCascade`
   - já existe;
   - deve ser migrado.

2. `AutoFishing`
   - automaticamente lança a linha/vara e realiza a pesca quando possível;
   - quando houver fisgada, deve executar automaticamente a ação apropriada;
   - deve ser um plugin independente.

3. `FishingLinePlus` ou nome equivalente
   - aumenta a **quantidade simultânea de linhas e hooks/bobbers de pesca** que o jogador pode utilizar;
   - deve permitir múltiplas linhas/hooks ativos ao mesmo tempo, em vez da limitação vanilla de uma única linha/hook;
   - deve ser independente do `AutoFishing`.

O framework deve ser projetado para suportar muitos outros mods futuramente.

---

# 2. PRINCÍPIO ARQUITETURAL

NÃO crie três mods acoplados entre si.

A arquitetura obrigatória deve ser conceitualmente:

```text
                    Terraria.exe
                         │
                         ▼
                TerrariaModCore
                    (TMC Host)
                         │
          ┌──────────────┼──────────────┐
          ▼              ▼              ▼
    OreCascade      AutoFishing   FishingLinePlus
```

O Core controla:

- descoberta;
- carregamento;
- ciclo de vida;
- dependências;
- configuração;
- logging;
- versionamento;
- compatibilidade;
- gerenciamento de Harmony;
- tratamento de erros;
- isolamento;
- ordem de inicialização;
- detecção de conflitos;
- comunicação entre plugins.

Os mods implementam somente sua própria funcionalidade.

---

# 3. RESTRIÇÕES

## NÃO UTILIZE tModLoader

Não utilizar:

- tModLoader;
- APIs `Mod`;
- `ModPlayer`;
- `ModTile`;
- `GlobalTile`;
- `ModContent`;
- ou qualquer infraestrutura dependente de tModLoader.

O sistema deve funcionar sobre Terraria Vanilla 1.4.5.7.

Não modifique permanentemente:

- `Terraria.exe`;
- DLLs vanilla;
- saves;
- arquivos do jogo;

salvo algum mecanismo externo de bootstrap/injection estritamente necessário.

Prefira:

```text
Injector
    ↓
TerrariaModCore
    ↓
Harmony
    ↓
Mods
```

e patches carregados em runtime.

---

# 4. PRIMEIRO PASSO: AUDITORIA

ANTES DE CODIFICAR:

1. Inspecione a instalação real do Terraria.
2. Inspecione `D:\Programas\Projetos\terraria\ore_cascade`.
3. Leia todo o código existente.
4. Entenda como `OreCascade` funciona atualmente.
5. Identifique:
   - entry point;
   - assemblies;
   - dependências;
   - Harmony;
   - patches;
   - configuração;
   - logs;
   - target framework;
   - build process.
6. Determine o mecanismo usado atualmente para carregar o mod.
7. Identifique o que pode ser reaproveitado.
8. Identifique o que precisa ser refatorado.
9. Não reescreva código funcional sem necessidade.

O objetivo é **migrar**, não destruir e reconstruir cegamente.

---

# 5. ESTRUTURA FINAL

Projete uma estrutura semelhante a:

```text
Terraria/
│
├── Terraria.exe
│
├── TMC/
│   ├── TerrariaModCore.dll
│   ├── Harmony.dll
│   ├── Mono.Cecil.dll
│   ├── config/
│   ├── logs/
│   └── cache/
│
└── mods/
    │
    ├── OreCascade/
    │   ├── manifest.json
    │   ├── OreCascade.dll
    │   ├── config.json
    │   └── assets/
    │
    ├── AutoFishing/
    │   ├── manifest.json
    │   ├── AutoFishing.dll
    │   ├── config.json
    │   └── assets/
    │
    └── FishingLinePlus/
        ├── manifest.json
        ├── FishingLinePlus.dll
        ├── config.json
        └── assets/
```

A estrutura exata pode ser alterada após inspeção do ambiente.

O princípio deve permanecer:

> **Um diretório por mod. Um assembly principal por mod. O Core permanece independente dos mods.**

---

# 6. MANIFEST

Cada mod deve possuir um `manifest.json`.

Exemplo:

```json
{
  "id": "ore_cascade",
  "name": "Ore Cascade",
  "version": "1.0.0",
  "author": "Silvio",
  "entryAssembly": "OreCascade.dll",
  "entryType": "OreCascade.OreCascadeMod",
  "targetGameVersion": "1.4.5.7",
  "coreVersion": "1.0.0",
  "dependencies": [],
  "loadAfter": [],
  "loadBefore": [],
  "enabled": true
}
```

O formato deve ser extensível.

Suportar futuramente:

```json
{
  "optionalDependencies": [],
  "incompatibleWith": [],
  "permissions": [],
  "supportedGameVersions": [],
  "configurationVersion": 1,
  "clientOnly": true,
  "serverCompatible": false
}
```

---

# 7. API DE PLUGIN

Crie uma API mínima e estável.

Cada mod deve implementar algo semelhante a:

```csharp
public interface IMod
{
    void Initialize(IModContext context);
    void Load();
    void Unload();
}
```

O design pode ser diferente caso a inspeção determine uma solução melhor.

O importante é estabelecer um lifecycle claro.

Considere eventos:

```text
Discovered
Validated
Loaded
Initialized
Enabled
Disabled
Unloaded
Failed
```

Evite expor ao mod mais poder do que ele precisa.

---

# 8. MOD CONTEXT

Cada plugin recebe seu próprio contexto.

O contexto deve permitir acesso controlado a:

- logger;
- configuração;
- informações do jogo;
- versão do Core;
- versão do Terraria;
- diretório do próprio mod;
- diretório de dados;
- serviços compartilhados;
- API de patching;
- eventos suportados.

Um mod deve poder obter:

```text
context.ModDirectory
context.ConfigDirectory
context.Logger
context.GameVersion
context.CoreVersion
```

sem precisar descobrir caminhos manualmente.

---

# 9. HARMONY CENTRALIZADO

Este é um ponto crítico.

**Não permita que cada mod crie seu próprio Harmony arbitrariamente.**

O Core deve possuir um gerenciador central:

```text
PatchManager
```

Todos os patches deverão ser registrados através dele.

Cada mod receberá um identificador próprio:

```text
ore_cascade
auto_fishing
fishing_line_plus
```

O Core deve conseguir:

- listar patches;
- saber qual mod criou cada patch;
- aplicar;
- remover;
- detectar conflitos;
- desativar patches de um único mod;
- desfazer todos os patches do mod durante unload.

---

# 10. CONFLITOS ENTRE MODS

O framework deve antecipar conflitos.

Exemplo:

```text
OreCascade → patch MiningMethod
AutoFishing → patch FishingMethod
FishingLinePlus → patch FishingMethod
```

Se dois mods alterarem o mesmo método:

1. registrar ambos;
2. preservar ambos quando possível;
3. definir prioridade;
4. respeitar `before` / `after`;
5. detectar conflitos impossíveis;
6. registrar warning;
7. nunca deixar um conflito silenciosamente destruir o outro mod.

O sistema deve ter diagnóstico semelhante a:

```text
[TMC] Patch conflict detected
Method: Terraria.Player.X
Mod A: AutoFishing
Mod B: FishingLinePlus
Resolution: priority-based execution
```

---

# 11. DEPENDÊNCIAS

Implemente:

```text
dependencies
optionalDependencies
loadBefore
loadAfter
incompatibleWith
```

Construa um grafo de dependências.

Determine automaticamente a ordem de carregamento.

Detecte:

- dependência inexistente;
- ciclo;
- versão incompatível;
- conflito;
- mod duplicado.

Exemplo:

```text
TMC
 ├── OreCascade
 ├── AutoFishing
 └── FishingLinePlus
```

Nenhum dos três deve depender diretamente dos outros.

---

# 12. FALHA ISOLADA

Um dos requisitos mais importantes:

> **Um mod quebrado não pode derrubar o loader inteiro.**

Se:

```text
OreCascade → OK
AutoFishing → EXCEPTION
FishingLinePlus → OK
```

o resultado deve ser:

```text
OreCascade       LOADED
AutoFishing      FAILED
FishingLinePlus  LOADED
```

Capture exceções nas fronteiras do plugin.

Registre stack trace completo.

Não use:

```csharp
catch { }
```

Nunca silencie uma falha.

---

# 13. DESABILITAÇÃO INDIVIDUAL

O usuário deve poder fazer:

```text
mods/
├── OreCascade/
│   └── manifest.json
├── AutoFishing/
│   └── manifest.json
└── FishingLinePlus/
    └── manifest.json
```

com:

```json
"enabled": false
```

para um mod específico.

Desabilitar:

```text
AutoFishing
```

não deve afetar:

```text
OreCascade
FishingLinePlus
```

---

# 14. CONFIGURAÇÃO

Cada mod possui configuração própria.

Exemplo:

```text
mods/
└── OreCascade/
    └── config.json
```

O Core deve fornecer uma API para:

- carregar;
- salvar;
- validar;
- fornecer defaults;
- detectar configuração inválida;
- migrar versões futuras.

Evite um grande arquivo global para todas as configurações dos mods.

---

# 15. LOGGING

Crie logger centralizado.

Formato:

```text
[TMC]
[TMC:OreCascade]
[TMC:AutoFishing]
[TMC:FishingLinePlus]
```

Exemplo:

```text
[18:42:01] [TMC] Loading mods...
[18:42:01] [TMC:OreCascade] Loaded v1.0.0
[18:42:01] [TMC:AutoFishing] Loaded v1.0.0
[18:42:01] [TMC:FishingLinePlus] Loaded v1.0.0
```

Cada mod deverá possuir:

- Info;
- Warning;
- Error;
- Debug.

Logs devem identificar sempre o mod responsável.

---

# 16. COMPATIBILIDADE DO TERRARIA

O Core deve verificar:

```text
Terraria version
Assembly identity
Game build
```

A versão alvo atual é:

```text
1.4.5.7
```

Se um mod exigir outra versão:

```text
[ERROR] OreCascade requires Terraria 1.4.5.7
[ERROR] Current version: 1.4.6.x
```

O loader deve impedir o carregamento inseguro quando necessário.

Não dependa apenas de `AssemblyVersion` se ele não representar corretamente a versão do jogo.

Use os identificadores disponíveis no assembly real.

---

# 17. API DE EVENTOS

Crie, quando tecnicamente viável, uma camada de eventos abstrata.

Exemplos futuros:

```text
OnPlayerUpdate
OnTileMined
OnItemUse
OnFishingStarted
OnFishingBite
OnFishingCaught
OnWorldLoaded
OnWorldUnloaded
```

Porém:

**não invente eventos artificiais desnecessários.**

Um evento deve existir quando houver um ponto de integração real no código do Terraria ou quando o Core puder implementá-lo de maneira segura.

---

# 18. SERVIÇOS COMPARTILHADOS

Crie serviços no Core para funcionalidades que todos os mods possam reutilizar.

Exemplos:

```text
ILogger
IConfigManager
IPatchManager
IGameVersionService
IModRegistry
IEventBus
```

Evite duplicação.

Por exemplo, se amanhã existir:

```text
50 mods
```

não queremos:

```text
50 cópias de Harmony
50 loggers
50 sistemas de configuração
```

---

# 19. COMPATIBILIDADE DE DEPENDÊNCIAS

Resolva dependências compartilhadas de forma centralizada.

Especialmente:

```text
Harmony
Mono.Cecil
Newtonsoft.Json
outras libraries
```

Evite que:

```text
OreCascade.dll
AutoFishing.dll
FishingLinePlus.dll
```

carreguem versões incompatíveis da mesma dependência.

Defina claramente:

```text
TMC owns shared dependencies
Mods reference the Core API
```

ou outra estratégia superior encontrada durante a implementação.

---

# 20. ORECASCADE COMO PRIMEIRO PLUGIN

Pegue:

`D:\Programas\Projetos\terraria\ore_cascade`

e faça a migração.

Preserve sua funcionalidade atual.

Refatore para:

```text
OreCascade
    ↓
IMod
    ↓
TMC
    ↓
PatchManager
```

O mod não deverá:

- criar seu próprio loader;
- inicializar Harmony globalmente;
- possuir um sistema paralelo de configuração;
- escrever logs de forma independente;
- assumir caminhos fixos;
- depender de outro mod.

Toda infraestrutura comum deve ser movida para o Core.

---

# 21. AUTOFISHING

Crie o `AutoFishing` como **segundo plugin de teste real**.

O objetivo funcional é:

1. detectar quando o jogador está utilizando uma vara de pesca;
2. lançar automaticamente;
3. aguardar a pescaria;
4. detectar uma fisgada válida;
5. executar automaticamente a ação necessária;
6. continuar a operação conforme configuração.

O mod deve possuir configuração para controlar o comportamento.

No entanto:

**não automatize mecanicamente o mouse/teclado se a lógica interna do Terraria permitir executar a ação de forma semanticamente correta.**

Primeiro investigue como Terraria processa:

```text
cast
fishing line
bobber
bite
reel/catch
```

e utilize os mecanismos internos apropriados.

A automação deve funcionar como modificação do gameplay, não como um macro externo frágil.

---

# 22. FISHINGLINEPLUS

Crie `FishingLinePlus` como terceiro plugin.

## OBJETIVO FUNCIONAL

O Terraria vanilla normalmente limita o jogador a **uma linha/hook de pesca ativo por vez**.

Este mod NÃO deve aumentar o comprimento da linha.

Ele deve modificar o sistema de pesca para permitir **múltiplas linhas/hooks simultaneamente**, de acordo com uma quantidade configurável.

Exemplo:

```text
Vanilla:
Player
 └── Hook/Line #1

FishingLinePlus:
Player
 ├── Hook/Line #1
 ├── Hook/Line #2
 ├── Hook/Line #3
 ├── Hook/Line #4
 └── ...
```

O número máximo deve ser configurável.

Exemplo:

```text
MaxActiveFishingLines = 4
```

### IMPORTANTE

Não implemente isso simplesmente duplicando visualmente o hook.

As múltiplas linhas/hooks devem ser **funcionais e independentes**, dentro das limitações do motor.

Investigue o código real do Terraria 1.4.5.7 para descobrir como são controlados:

- estado atual da pesca;
- hook/bobber;
- projétil associado à pesca;
- owner do hook;
- quantidade de hooks ativos;
- criação de novos hooks;
- limite vanilla;
- detecção de colisão;
- posição de cada hook;
- linha entre jogador e hook;
- estado de bite;
- captura;
- remoção do hook;
- cancelamento da pesca;
- sincronização multiplayer.

O patch deve alterar **a limitação de quantidade**, não simplesmente a distância.

### COMPORTAMENTO ESPERADO

Se:

```text
MaxActiveFishingLines = 4
```

o jogador deverá conseguir possuir até quatro hooks/linhas ativos simultaneamente.

Cada hook deve:

- possuir seu próprio estado;
- poder alcançar uma posição própria;
- permanecer associado ao jogador;
- ser desenhado corretamente;
- poder receber uma fisgada;
- poder ser recolhido;
- não invalidar os outros hooks;
- não duplicar indevidamente eventos ou drops.

O sistema deve investigar como representar múltiplos estados de pesca quando o código vanilla assume apenas um.

Se o Terraria utilizar uma única estrutura/estado global por jogador, adapte a arquitetura do mod para representar **uma coleção de estados de pesca**, sem corromper o estado vanilla.

### INTERAÇÃO COM AUTOFISHING

`AutoFishing` e `FishingLinePlus` devem permanecer **independentes**.

Porém, quando ambos estiverem ativos:

```text
FishingLinePlus
       ↓
cria/permite múltiplos hooks
       ↓
AutoFishing
       ↓
pode administrar automaticamente esses hooks
```

O `AutoFishing` não deve assumir que existe somente um hook quando `FishingLinePlus` estiver instalado.

Caso o Core ofereça uma API/evento de pesca, prefira fazê-los se comunicar através dessa abstração em vez de um mod acessar diretamente as classes internas do outro.

---

# 23. COEXISTÊNCIA ENTRE OS MODS

Teste explicitamente:

```text
OreCascade
```

sozinho.

Depois:

```text
AutoFishing
```

sozinho.

Depois:

```text
FishingLinePlus
```

sozinho.

Depois:

```text
OreCascade + AutoFishing
```

Depois:

```text
OreCascade + FishingLinePlus
```

Depois:

```text
AutoFishing + FishingLinePlus
```

Finalmente:

```text
OreCascade + AutoFishing + FishingLinePlus
```

O último cenário é o teste principal.

---

# 24. ORDEM DE PATCHES

Os três mods podem potencialmente tocar métodos relacionados à pesca.

Portanto, determine automaticamente:

```text
quem toca qual método;
qual tipo de patch;
qual prioridade;
qual ordem;
```

Não use prioridades arbitrárias apenas para "fazer funcionar".

Documente a razão de cada ordem.

---

# 25. UNLOAD

O framework deve suportar:

```text
Load
Unload
Reload
```

quando tecnicamente seguro.

Ao remover um mod:

- remover Harmony patches;
- liberar recursos;
- cancelar eventos;
- cancelar timers;
- restaurar estados;
- remover referências;
- impedir callbacks posteriores.

Um mod descarregado não pode continuar executando código.

---

# 26. SEGURANÇA

O framework deve assumir que qualquer DLL externa pode:

- lançar exceções;
- depender de assembly inexistente;
- apresentar versão incompatível;
- executar código durante load;
- corromper estado global.

Não é necessário criar um sandbox de segurança completo, pois DLL .NET carregada no mesmo processo possui privilégios equivalentes ao jogo.

Mas deve existir **isolamento operacional**, especialmente:

- lifecycle;
- logging;
- configuração;
- patches;
- registro;
- tratamento de exceções.

---

# 27. DESCOBERTA AUTOMÁTICA

Ao iniciar:

```text
TMC
 ↓
scan /mods
 ↓
encontrar manifest.json
 ↓
validar
 ↓
resolver dependências
 ↓
ordenar
 ↓
carregar
```

Deve ser possível simplesmente criar:

```text
mods/NewMod/
```

e adicionar:

```text
manifest.json
NewMod.dll
```

sem modificar o Core.

Esse é um requisito fundamental.

---

# 28. METADADOS E REGISTRO

O Core deve manter um:

```text
ModRegistry
```

capaz de responder:

```text
GetMods()
GetMod(id)
IsLoaded(id)
IsEnabled(id)
GetVersion(id)
GetDependencies(id)
```

No futuro isso poderá alimentar uma UI.

---

# 29. DIAGNÓSTICO

Ao iniciar, apresente algo semelhante a:

```text
========================================
 TerrariaModCore 1.0.0
 Terraria 1.4.5.7
========================================

Discovered mods: 3

✓ OreCascade          1.0.0
✓ AutoFishing         1.0.0
✓ FishingLinePlus     1.0.0

All mods loaded successfully.
```

Em caso de erro:

```text
✗ AutoFishing 1.0.0
  Reason: Patch target not found
```

Não permita que o usuário precise procurar logs obscuros para entender por que o mod não carregou.

---

# 30. TESTE DE RESILIÊNCIA

Crie deliberadamente:

```text
BrokenTestMod
```

ou equivalente temporário.

Faça-o lançar uma exceção durante `Load()`.

Resultado esperado:

```text
BrokenTestMod     FAILED
OreCascade        LOADED
AutoFishing       LOADED
FishingLinePlus   LOADED
```

Depois remova o mod de teste.

---

# 31. BUILD SYSTEM

Crie uma solução organizada:

```text
TerrariaModCore.sln

src/
├── Core/
├── Bootstrap/
├── API/
├── OreCascade/
├── AutoFishing/
└── FishingLinePlus/
```

ou estrutura equivalente.

O build deverá:

1. compilar o Core;
2. compilar os mods;
3. copiar DLLs;
4. gerar manifests;
5. montar uma distribuição em:

```text
dist/
```

Exemplo:

```text
dist/
├── TMC/
└── mods/
    ├── OreCascade/
    ├── AutoFishing/
    └── FishingLinePlus/
```

---

# 32. NÃO DUPLICAR INFRAESTRUTURA

Depois da migração:

### OreCascade NÃO deve possuir:

- loader próprio;
- injector próprio;
- Harmony global próprio;
- logger próprio;
- config manager próprio.

### AutoFishing NÃO deve possuir:

- loader;
- Harmony global;
- config system paralelo.

### FishingLinePlus NÃO deve possuir:

- loader;
- Harmony global;
- configuração global.

Tudo isso pertence ao TMC.

---

# 33. DOCUMENTAÇÃO

Crie:

```text
README.md
ARCHITECTURE.md
MODDING.md
COMPATIBILITY.md
```

### README

Usuário comum.

### ARCHITECTURE

Desenvolvedores do Core.

### MODDING

Como criar novos plugins.

### COMPATIBILITY

Terraria versions, dependências e limitações.

Inclua um tutorial mínimo:

```text
Criando um quarto mod:

1. criar diretório
2. criar manifest.json
3. criar projeto
4. referenciar TMC API
5. implementar IMod
6. compilar
7. colocar DLL em /mods
8. iniciar Terraria
```

---

# 34. DOCUMENTAÇÃO DA API

Documente interfaces públicas.

No mínimo:

```text
IMod
IModContext
ILogger
IConfig
IPatchManager
IModRegistry
IEventBus
IGameServices
```

Use XML documentation quando apropriado.

---

# 35. COMPATIBILIDADE FUTURA

O Core deve possuir seu próprio versionamento:

```text
1.0.0
1.1.0
2.0.0
```

Use Semantic Versioning quando apropriado.

Um mod deve declarar:

```text
coreVersion
```

ou:

```text
minCoreVersion
maxCoreVersion
```

Evite quebrar todos os mods sempre que uma pequena funcionalidade for adicionada.

---

# 36. ATUALIZAÇÕES DO TERRARIA

Como o framework depende de implementação interna do Terraria, uma atualização do jogo pode quebrar patches.

Crie uma camada de:

```text
Compatibility
```

capaz de centralizar:

- identificação de versão;
- resolução de métodos;
- resolução de tipos;
- signatures;
- diferenças entre builds.

Evite espalhar:

```text
1.4.5.7-specific code
```

por todos os mods.

---

# 37. PRINCÍPIO CENTRAL

A arquitetura deve seguir:

```text
Core = infraestrutura
Mods = comportamento
```

Não:

```text
Core = gameplay
Mods = wrappers
```

O Core não deve saber o que é minério ou pesca.

Ele deve apenas fornecer infraestrutura.

Somente:

```text
OreCascade
AutoFishing
FishingLinePlus
```

conhecem suas respectivas regras.

---

# 38. PROCESSO DE IMPLEMENTAÇÃO

Execute nesta ordem:

```text
1. AUDITAR Terraria
2. AUDITAR OreCascade
3. ENTENDER o bootstrap atual
4. DEFINIR arquitetura TMC
5. CRIAR Core
6. CRIAR Plugin API
7. CRIAR PatchManager
8. CRIAR ModRegistry
9. CRIAR Configuration System
10. CRIAR Logging System
11. CRIAR Dependency Resolver
12. MIGRAR OreCascade
13. VALIDAR OreCascade isoladamente
14. IMPLEMENTAR AutoFishing
15. IMPLEMENTAR FishingLinePlus
16. VALIDAR cada um isoladamente
17. VALIDAR combinações
18. TESTAR falhas
19. TESTAR incompatibilidades
20. CRIAR documentação
21. GERAR distribuição final
```

Não pule diretamente para escrever três mods sem estabelecer a infraestrutura.

---

# 39. CRITÉRIO DE CONCLUSÃO

Considere o trabalho concluído somente quando:

- TMC inicializa no Terraria 1.4.5.7;
- `/mods` é descoberto automaticamente;
- manifests são validados;
- dependências funcionam;
- mods podem ser habilitados/desabilitados individualmente;
- OreCascade foi migrado;
- AutoFishing funciona como plugin;
- FishingLinePlus funciona como plugin;
- Harmony é administrado centralmente;
- erros de um mod não derrubam os outros;
- conflitos de patches são detectados;
- configurações são independentes;
- logs identificam o mod responsável;
- todos os três mods funcionam individualmente;
- todos os três funcionam simultaneamente;
- nenhum deles exige tModLoader;
- Terraria vanilla continua preservado;
- o sistema é suficientemente genérico para receber um quarto mod sem alterar o Core.

---

# 40. RELATÓRIO FINAL OBRIGATÓRIO

Ao concluir, apresente:

## Arquitetura

Explique:

```text
Injector
   ↓
TMC
   ↓
Plugin System
   ↓
Mods
```

## Estrutura

Mostre a árvore final de diretórios.

## OreCascade

Explique o que foi migrado e modificado.

## AutoFishing

Explique os hooks utilizados e como a automação funciona.

## FishingLinePlus

Explique como o Terraria limita a quantidade de hooks/linhas ativos e como o patch transforma essa limitação em uma quantidade configurável de hooks/linhas simultâneos.

## Compatibilidade

Mostre quais métodos cada mod intercepta.

## Conflitos

Mostre quaisquer métodos compartilhados e como os conflitos foram resolvidos.

## Testes

Mostre:

```text
OreCascade                         PASS
AutoFishing                        PASS
FishingLinePlus                    PASS

OreCascade + AutoFishing           PASS
OreCascade + FishingLinePlus       PASS
AutoFishing + FishingLinePlus      PASS
All three                          PASS
```

Não marque `PASS` sem realmente executar o teste.

---

# REGRA FINAL

Você não está construindo simplesmente três mods.

Você está construindo:

> **uma pequena plataforma de modding para Terraria Vanilla 1.4.5.7.**

Portanto, privilegie:

- arquitetura;
- modularidade;
- isolamento;
- extensibilidade;
- observabilidade;
- compatibilidade;
- reversibilidade;
- manutenção;
- baixo acoplamento.

O `OreCascade` existente deve ser considerado o **primeiro caso de produção e também o primeiro teste de que a arquitetura realmente funciona**.

Não jogue fora uma implementação funcional sem justificativa.

Inspecione, compreenda, abstraia e migre.

Depois utilize os dois mods de pesca para validar se o framework realmente consegue suportar múltiplos plugins que alteram partes relacionadas do mesmo jogo sem interferência destrutiva.