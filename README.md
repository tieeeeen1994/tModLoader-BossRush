# Summary

Boss Rush API mod is a mod that allows the creation of Boss Rush events in the game. It does not any content into the game. It is a framework for other mods to use to create Boss Rush events.

This repository includes the source code for Boss Rush API mod, as well as a **playable** example mod which implements the Boss Rush API.

This mod is not strictly necessary to be implemented by a content mod; it is possible to create a completely separate mod that implements the Boss Rush API alongside other content mods to mash together the bosses from different mods in the Boss Rush event.

If interested, the [repository wiki](https://github.com/tieeeeen1994/tModLoader-BossRush/wiki) contains guides on how to implement the API.

# Boss Rush API Changelog

1.0.6
- Add periodic syncing feature. This will sync server boss rush details periodically in case of situational desyncs.

1.0.5
- Rerelease the mod due to a compilation quirk. It made bosses have chungus amount of health.

1.0.4
- Add safety checks and assignments in code in case other mods make detours.
- Fix most other QoL mod incompatibilities.
- The fixes may not cover all incompatibilities, so please report when issues arise!

1.0.3
- Remove the multiplayer compatibility config option.
- Implement a fool-proof teleporting system to allow seamless multiplayer experience.

1.0.2
- Optimizations, and more features for the API.
- Not documented for now, but they will be in the wiki for sure.
- Add config options for multiplayer compatibility when teleporting players.
- Add config options for bending the rules of boss rush events regarding player respawns.

1.0.0
- Initial release.

# Example Boss Rush Changelog

1.0.8
- Few balance tweaks again for bosses.

1.0.7
- Few tweaks for bosses in boss rush mode.
- Eater of worlds and its corruptors are less tanky now.
- Added a different adjustment for Golem in For The Worthy. He's a different level of beast in For The Worthy.

1.0.6
- Fixed Eater of Worlds where Vile Spits are travelling too fast due to velocity updates running in all clients.
- Decreased the Vile Spit's time to live to 3 seconds only to avoid bad bugs.

1.0.5
- Rerelease the mod due to a compilation quirk. It made bosses have chungus amount of health.

1.0.4
- Remove Seletron Prime's arms as Sub Types.

1.0.3
- Quick hotfix that fixes the boss rush starting with Wall of Flesh.
- Also fix bugs regarding OriginalDamage not being fetched. Bad code.

1.0.2
- Rebalanced everything.
- Weaknened everything as I realized balancing around Solar Armor and Zenith is a huge mistake.
- They are stills tronge than their non-boss rush counterparts, however.

1.0.0
- Initial release.
