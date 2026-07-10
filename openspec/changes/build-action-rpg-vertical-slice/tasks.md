## 1. Project Foundation

- [x] 1.1 Create the Unity 6000.3.11f1 Universal 3D project in `UnityProject`
- [x] 1.2 Pin and record URP, Input System, Cinemachine, AI Navigation, and Test Framework package versions
- [x] 1.3 Add the `_Project` runtime, data, prefab, scene, editor, and test folder structure with assembly definitions
- [x] 1.4 Configure Unity `.gitignore`, text serialization, visible meta files, and the Git LFS asset policy
- [x] 1.5 Create `CombatSandbox` and `VerticalSlice` scenes and verify a clean Console
- [x] 1.6 Produce and record a runnable empty macOS Apple Silicon build

## 2. Player Control and Camera

- [x] 2.1 Define movement, look, attack, dodge, skill, pause, and UI Input Actions
- [x] 2.2 Implement frame-independent camera-relative player movement and facing
- [x] 2.3 Configure a Cinemachine third-person follow camera with sensitivity and vertical limits
- [x] 2.4 Add camera obstacle handling and verify representative map occlusion cases
- [x] 2.5 Implement gameplay input gating for pause, victory, and defeat states
- [x] 2.6 Implement dodge movement and configurable invulnerability timing
- [x] 2.7 Record player-control prompts, manual scenarios, and learning notes

## 3. Health and Combat Rules

- [x] 3.1 Implement pure C# health, damage, and single-fire death rules
- [x] 3.2 Add EditMode tests for health bounds, lethal damage, and repeated damage after death
- [x] 3.3 Create actor and attack definition ScriptableObjects without runtime state
- [x] 3.4 Connect player attack animation to a configurable active hit window
- [x] 3.5 Implement one-hit-per-target-per-attack execution semantics
- [x] 3.6 Add EditMode or focused PlayMode coverage for enemies with multiple colliders
- [ ] 3.7 Add hit reaction, death transition, and damage-number event output
- [ ] 3.8 Measure and document animation-to-hit timing in the combat sandbox

## 4. Enemy AI

- [ ] 4.1 Create enemy definition data for movement, detection, attack, disengage, and rewards
- [ ] 4.2 Implement Idle, Chase, Attack, Hit, Return, and Dead state transitions
- [ ] 4.3 Configure NavMesh baking and agent movement in the combat sandbox
- [ ] 4.4 Implement player detection, pursuit, attack-range stopping, and cooldown behavior
- [ ] 4.5 Implement disengage and return-to-home behavior
- [ ] 4.6 Prevent dead enemies from moving, attacking, or issuing repeated rewards
- [ ] 4.7 Add the minimum spatial-separation rule for five simultaneous melee enemies
- [ ] 4.8 Record the enemy state-transition test matrix and a NavMesh troubleshooting case

## 5. Skills and Progression

- [ ] 5.1 Create skill definition data and per-instance runtime cooldown state
- [ ] 5.2 Implement and verify two distinct active skills
- [ ] 5.3 Add cooldown boundary tests and reject skill input during cooldown
- [ ] 5.4 Implement experience rewards with exactly-once enemy death semantics
- [ ] 5.5 Implement level thresholds, carry-over experience, and stat growth
- [ ] 5.6 Add EditMode tests for exact thresholds and multi-level rewards
- [ ] 5.7 Connect player health, level, and experience changes to UI events

## 6. UI and Combat Feedback

- [ ] 6.1 Build the player HUD for health, level, experience, and skill slots
- [ ] 6.2 Display cooldown availability and remaining time for each active skill
- [ ] 6.3 Build enemy world-space name, level, and health UI with display conditions
- [ ] 6.4 Add a timed awareness indicator when an enemy first detects the player
- [ ] 6.5 Add world-space damage numbers that appear once per applied hit and expire
- [ ] 6.6 Verify event subscription cleanup after disable, death, and restart

## 7. Boss and Session Flow

- [ ] 7.1 Create boss data and implement two visually distinguishable attack patterns
- [ ] 7.2 Implement the Playing, Paused, Victory, and Defeat game-session states
- [ ] 7.3 Transition to Victory exactly once after boss death and display the result UI
- [ ] 7.4 Transition to Defeat after player death and display restart UI
- [ ] 7.5 Reset player, enemies, progression, UI, and session state on restart
- [ ] 7.6 Verify the complete new-game-to-victory manual scenario
- [ ] 7.7 Verify the complete new-game-to-defeat-to-restart manual scenario
- [ ] 7.8 Measure and tune the vertical slice toward a 10–15 minute play session

## 8. Polish, Performance, and Compatibility

- [ ] 8.1 Integrate licensed URP-compatible low-poly character, environment, and animation assets
- [ ] 8.2 Add and tune hit VFX, sound effects, and camera feedback
- [ ] 8.3 Profile a representative fight with ten active regular enemies at 1920×1080
- [ ] 8.4 Apply measured pooling or bottleneck fixes and record before-and-after evidence
- [ ] 8.5 Complete the external asset source, license, version, and usage register
- [ ] 8.6 Verify a complete macOS Apple Silicon build outside the Unity Editor
- [ ] 8.7 Verify input, shaders, resolution, and complete play flow in a Windows build

## 9. Educational Packaging

- [ ] 9.1 Link each completed capability to its prompt log, changed files, tests, and development log
- [ ] 9.2 Curate at least five failed-or-corrected AI output cases with human decision rationale
- [ ] 9.3 Convert each milestone into introduction, concept, prompt critique, lab, validation, and reflection sections
- [ ] 9.4 Prepare learner exercises separately from instructor explanations and expected results
- [ ] 9.5 Mark reproducible start and finish commits for each instructional unit
- [ ] 9.6 Run the feature-done checklist across all vertical-slice acceptance criteria
- [ ] 9.7 Publish the final demo, build instructions, troubleshooting index, and course retrospective
