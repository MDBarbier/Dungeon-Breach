# Dungeon-Breach
This is a turn based strategy game built in Unity engine in 3d. The theme is based on groups of journeymen who make it their profession to delve into lost dungeons and forgotten castle in search of riches and fame.

# Key features (planned, no order)

- Grid based squad tactics game
- Player controls a party of dungeoneers as they venture into different dungeons
- Narrow corridors and a variety of room sizes allow opportunity for suprise coordinated attacks as rooms are breached
- Turn based battles
- Randonly Generated dungeons
- Overworld which links dungeons together and has towns etc

# Mechanics

## Player party

- Character Race decides starting stats
- Type of weapon assigned to a character decides their class
- Once class is determined a subclass can be picked to give specialisation
- Actions cost TUs (Time units) both to activate and after activation i.e. a cooldown
- New characters can be hired from town or rescued from dungeons
- Death is permanent, unless the cleric gets there in time!
- Characters gain XP for defeated enemies, cleared rooms and completed missions 
- Characters levelling up get boosts to their stats and unlock new skills

### Classes

Class (melee/ranged/magic) is determined by weapon type used, then a subclass is chosen

#### Melee
- Fighter: melee combat + tanking
- Barbarian: dps melee combat 

#### Ranged
- Marksman: ranged dps combat, debuffs
- Scout: ranged combat, stealth, traps/locks

#### Mage
- Wizard: offensive + defensive magic
- Priest: healing + buffs
- Warlock: Summons + debuffs

### Specific class ideas

Mage shields – Wizards can deploy a shield spell covering one or more squares, protecting from all normal damage from one direction. The more squares the shield covers the more TU it costs to set up and cool down from. Parabolic ranged weapons such as grenades can get over the shield, and spells which appear directly on the target are unaffected.

Priest resurrection – If a character goes to zero hit points they enter a “wounded” stage. This lasts for a number of TU depending on their constitution and a random factor. If the priest can get to them before the end of that time they can stablize the character, which puts them to 1hp. If the TU timer runs out and they are not stablized, they die. They can be resurrected with the correct high level spell, or for a large amount of gold in a town.

Fighter taunt aura – attracts enemies to attack the fighter based on a skill check in the background

Barbarian rage – More likely to critically hit and a chance to shrug off part or all of damage received

## Stats

- Strength: Damage modifier, attack unlocks
- Dexterity: To hit modifier, initiative modifier, TU cost modifier
- Constitution: HP modifier, defensive ability unlocks
- Intelligence: TU modifier for spells, spell unlocks, spell damage
