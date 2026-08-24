<div align="center">

# 🎣 FishingLinePlus — Múltiplas Linhas e Boias de Pesca para Terraria Vanilla

**Lance e gerencie múltiplas linhas de pesca funcionais simultaneamente com física de dispersão angular, sincronização de capturas em duas camadas e legitimidade total de drops vanilla.**

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

- **⚡ Múltiplas Linhas Simultâneas Configuráveis**:
  - Sobrescreve a restrição vanilla de 1 boia por jogador para permitir **4 (ou mais)** linhas funcionais simultâneas.

- **📐 Dispersão em Leque e Variação de Velocidade Realistas**:
  - Calcula automaticamente compensações angulares (`SpreadAngleDegrees`) e variações de velocidade (`VelocitySpread`) para que as boias se distribuam naturalmente na água em vez de se sobreporem em um único ponto.

- **🎣 Sincronização e Captura Dupla de Itens**:
  - Cada boia lançada realiza suas próprias rolagens de mordida independentes na água.
  - Ao recolher a linha, todas as boias ativas com mordidas confirmadas entregam seus respectivos itens capturados.

- **🤝 Coexistência Perfeita com o AutoFishing**:
  - Quando usado em conjunto com o `AutoFishing`, o bot detecta todas as boias ativas e realiza o recolhimento e relançamento de todas elas automaticamente.

---

## ⚙️ Configuração (`config.json`)

O arquivo de configuração está localizado em `mods/FishingLinePlus/config.json`:

```json
{
  "Enabled": true,
  "MaxActiveFishingLines": 4,
  "LinesPerCast": 4,
  "SpreadAngleDegrees": 7.0,
  "VelocitySpread": 0.08
}
```

### Referência de Opções

| Opção | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Ativa ou desativa o mod de múltiplas linhas. |
| `MaxActiveFishingLines` | `int` | `4` | Limite máximo de boias/linhas ativas no mundo simultaneamente. |
| `LinesPerCast` | `int` | `4` | Quantidade de boias disparadas em um único lançamento da vara. |
| `SpreadAngleDegrees` | `double` | `7.0` | Ângulo de abertura (em graus) para o leque de linhas. |
| `VelocitySpread` | `double` | `0.08` | Variação percentual aleatória de velocidade entre as boias. |

---

## 🔧 Métodos em Runtime Interceptados

| Classe Alvo | Método Alvo | Tipo de Hook | Função |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `ItemCheck_Shoot(...)` | `Postfix` | Dispara as boias adicionais com as variações de ângulo calculadas. |
| `Terraria.Player` | `ItemCheck_PullFishingBobbers(Item)` | `Prefix` | Avalia os drops de todas as boias ativas antes de iniciar a retração. |
| `Terraria.Projectile` | `AI_061_FishingBobber()` | `Postfix` | Sincroniza o estado de mordida e partículas de água em todas as boias. |

---

## 📁 Estrutura do Plugin

```text
mods/FishingLinePlus/
├── manifest.json            # Identidade, dependências e metadados
├── FishingLinePlus.dll      # Assembly compilado do plugin
├── FishingLinePlus.pdb      # Símbolos de depuração
├── README.md                # Documentação em inglês
├── README_pt-BR.md          # Documentação em português
└── config.json              # Configurações em tempo de execução
```
