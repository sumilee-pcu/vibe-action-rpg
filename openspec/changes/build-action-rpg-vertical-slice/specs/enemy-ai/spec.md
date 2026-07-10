## ADDED Requirements

### Requirement: Enemy combat state flow
시스템은(SHALL) 일반 적에게 Idle, Chase, Attack, Hit, Return, Dead 상태를 제공하고 현재 조건에 맞는 상태의 행동만 실행해야 한다.

#### Scenario: Player enters detection range
- **WHEN** 살아 있는 플레이어가 Idle 상태 적의 탐지 조건을 만족한다
- **THEN** 적은 Chase 상태로 전환하고 플레이어를 향해 이동한다

#### Scenario: Player enters attack range
- **WHEN** Chase 상태의 적이 플레이어 공격 거리와 공격 가능 조건을 만족한다
- **THEN** 적은 Attack 상태로 전환하고 이동을 멈춘 뒤 공격을 실행한다

#### Scenario: Dead enemy receives state update
- **WHEN** Dead 상태 적의 일반 AI 갱신 시점이 도달한다
- **THEN** 추적, 공격과 복귀 행동을 실행하지 않는다

### Requirement: Enemy disengage and return
시스템은(SHALL) 플레이어가 이탈 조건을 만족하면 적을 생성 또는 초기 대기 위치로 복귀시켜야 한다.

#### Scenario: Player exceeds disengage distance
- **WHEN** 전투 중 플레이어와 적의 기준 위치 사이 거리가 이탈 거리보다 커진다
- **THEN** 적은 Return 상태로 전환하고 초기 대기 위치로 이동한다

#### Scenario: Enemy reaches home position
- **WHEN** Return 상태 적이 초기 대기 위치의 허용 반경에 도달한다
- **THEN** 적은 Idle 상태로 전환하고 다음 탐지를 기다린다

### Requirement: Multiple enemy spatial readability
시스템은(SHALL) 여러 근접 적이 플레이어를 추적하거나 공격할 때 같은 위치에 완전히 겹치는 현상을 완화해야 한다.

#### Scenario: Five enemies approach one player
- **WHEN** 다섯 근접 적이 동시에 한 플레이어를 추적한다
- **THEN** 적들은 설정된 정지 거리, 회피 반경 또는 공격 위치 규칙에 따라 구분 가능한 위치를 유지한다

### Requirement: Boss attack variety
시스템은(SHALL) 보스에게 플레이어가 구분할 수 있는 최소 두 가지 공격 패턴을 제공하고 조건에 따라 패턴을 선택해야 한다.

#### Scenario: Boss combat sample
- **WHEN** 살아 있는 보스와 플레이어의 전투가 충분한 시간 동안 지속된다
- **THEN** 보스는 서로 다른 예고와 판정을 가진 공격 패턴을 최소 두 종류 사용한다
