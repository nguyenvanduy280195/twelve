# twelve


## 1. Design skills
- slvl: skill level
- atk: unit's attack
- lvl: unit's level

| Skill name    | Formula                             | Havest      |    Turns     | Mana Consumed | Gold to lvlup   |
| ------------- | ----------------------------------- | ----------- | :----------: | ------------- | --------------- |
| Fireball 1    | `dmg = atk * (1 + 0.1 * slvl)`      | 1x 3x3      |              | `10 * slvl`   | 50x (slvl + 1)  |
| Meteor 2      | `dmg = atk * slvl`                  | 1x 6x6      |              | `100 * slvl`  | 100x (slvl + 1) |
| Mana break 1  | `dmg = atk * slvl`                  | 1x 3x3      |              | `10 * slvl`   | 50x (slvl + 1)  |
|               | `target.mana = dmg * 0.5 * slvl`    |             |              |               |                 |
| Leech mana 2  | `dmg = atk * slvl`                  |             |              | `10 * slvl`   | 50x (slvl + 1)  |
|               | `target.mana = dmg * 0.5 * slvl`    |             |              |               |                 |
|               | `mana = target.mana * 0.1 * slvl`   |             |              |               |                 |
| Poison 1      | `dmg = atk * (1 + 0.05 * slvl)`     |             | `ceil(slvl)` | `10 * slvl`   | 50x (slvl + 1)  |
|               | `dmg per turn = dmg * 0.1 * slvl`   |             |              |               |                 |
| Toxic 2       | `dmg = atk * (1 + 0.1 * slvl)`      |             | `ceil(slvl)` | `15 * slvl`   | 100x (slvl + 1) |
|               | `dmg per turn = dmg * 0.1 * slvl`   |             |              |               |                 |
|               |                                     |             |              |               |                 |
| Chain Attack  | `dmg = atk * 0.01 * slvl * nSwords` | all swords  |              |               |                 |
| Chain Mana    | `dmg = atk * slvl`                  | all mana    |              |               |                 |
| Chain HP      | `dmg = atk * slvl`                  | all hp      |              |               |                 |
| Chain Stamina | `dmg = atk * slvl`                  | all stamina |              |               |                 |
| Chain Gold    | `dmg = atk * slvl`                  | all gold    |              |               |                 |
| Chain Exp     | `dmg = atk * slvl`                  | all exp     |              |               |                 |
| Thunderstorm  | `dmg = atk * slvl`                  | 5x 3x3      |              |               |                 |
| Aura slash    | `dmg = atk * slvl`                  | 1x row      |              | `10 * slvl`   | 50x (slvl + 1)  |
| Leech Life    | `dmg = atk * slvl`                  | 1x 3x3      |              | `10 * slvl`   | 50x (slvl + 1)  |
|               | `hp = dmg * 0.5 * slvl`             |             |              |               |                 |
| Aura shield   | `hp = -(target.dmg * 0.1 * slvl)`   |             |      5       | `10 * slvl`   |                 |
|               |                                     |             |              |               |                 |
|               |                                     |             |              |               |                 |
|               |                                     |             |              |               |                 |
|               |                                     |             |              |               |                 |
|               |                                     |             |              |               |                 |
|               |                                     |             |              |               |                 |

## 2. Design map
