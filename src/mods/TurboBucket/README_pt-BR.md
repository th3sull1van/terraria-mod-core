<div align="center">

# 🪣 TurboBucket — Aceleração de Baldes de Líquido para Terraria Vanilla

**Acelera a velocidade de despejamento e posicionamento de baldes de líquidos e baldes sem fundo no Terraria Vanilla em até 60 TPS com fluxo contínuo e zero modificação de arquivos.**

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

## 🚀 Funcionalidades

- **⚡ Multiplicador de Velocidade Configurável**: Acelera o despejamento de 10 ticks para 2 ticks (30 despejos/seg a 5x) ou 1 tick (60 despejos/seg a 10x).
- **🍯 Suporte a Baldes de Mel**: Esvazie e despeje mel instantaneamente sem lentidão.
- **🌋 Suporte a Baldes de Lava**: Crie pontes de obsidiana e preencha hellevators rapidamente com lava.
- **💧 Suporte a Baldes de Água**: Restauração rápida de oceanos e criação de lagos.
- **✨ Suporte a Baldes Sem Fundo**: Totalmente compatível com Baldes Sem Fundo de Água, Lava, Mel e Shimmer.
- **🧹 Aceleração Opcional de Baldes Vazios e Esponjas**: Boost de velocidade opcional para coletar líquidos com baldes vazios e secar com esponjas.

---

## ⚙️ Configuração (`mods/TurboBucket/config.json`)

```json
{
  "Enabled": true,
  "SpeedMultiplier": 5,
  "AffectsWater": true,
  "AffectsLava": true,
  "AffectsHoney": true,
  "AffectsBottomlessBuckets": true,
  "AffectsEmptyBuckets": false,
  "AffectsSponges": false
}
```
