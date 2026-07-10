## ADDED Requirements

### Requirement: Experience reward is granted once
시스템은(SHALL) 적이 처음 사망할 때 정의된 경험치 보상을 플레이어에게 정확히 한 번 지급해야 한다.

#### Scenario: Enemy death event repeats
- **WHEN** 동일 적에 대한 사망 또는 피해 알림이 중복 전달된다
- **THEN** 플레이어 경험치는 해당 적의 보상만큼 한 번만 증가한다

### Requirement: Level progression
시스템은(SHALL) 누적 경험치가 현재 레벨 기준값에 도달하면 레벨을 올리고 정의된 능력치 성장을 적용해야 한다.

#### Scenario: Reach exact level threshold
- **WHEN** 경험치 획득 후 누적 경험치가 다음 레벨 기준값과 정확히 같다
- **THEN** 플레이어 레벨이 1 증가하고 새 최대 체력 또는 공격 능력치가 적용된다

#### Scenario: Reward crosses multiple thresholds
- **WHEN** 한 번의 큰 경험치 보상으로 둘 이상의 레벨 기준값을 넘는다
- **THEN** 시스템은 충족한 모든 레벨업을 순서대로 적용하고 남은 경험치를 보존한다

### Requirement: Victory flow
시스템은(SHALL) 보스가 처음 사망하면 게임 세션을 Victory 상태로 전환하고 결과 화면을 표시해야 한다.

#### Scenario: Boss is defeated
- **WHEN** 보스 체력이 처음 0에 도달한다
- **THEN** 게임 세션은 Victory 상태가 되고 전투 입력이 차단되며 승리 화면이 표시된다

### Requirement: Defeat and restart flow
시스템은(SHALL) 플레이어가 사망하면 Defeat 상태와 패배 화면을 표시하고 새 게임 상태로 재시작할 수 있어야 한다.

#### Scenario: Restart after player death
- **WHEN** Defeat 화면에서 플레이어가 재시작을 선택한다
- **THEN** 플레이어, 적, 진행도와 게임 세션이 정의된 초기 상태로 복원되어 플레이할 수 있다
