## ADDED Requirements

### Requirement: Player status HUD
시스템은(SHALL) 플레이어의 현재 체력, 최대 체력, 레벨과 경험치 진행도를 HUD에 표시하고 값이 바뀌면 갱신해야 한다.

#### Scenario: Player takes damage
- **WHEN** 플레이어 현재 체력이 감소한다
- **THEN** HUD 체력 표시가 변경된 현재 체력과 최대 체력을 반영한다

#### Scenario: Player gains experience
- **WHEN** 플레이어 경험치 또는 레벨이 변경된다
- **THEN** HUD의 경험치 진행도와 레벨 표시가 새 상태를 반영한다

### Requirement: Skill cooldown HUD
시스템은(SHALL) 각 액티브 스킬 슬롯에 사용 가능 여부와 남은 쿨다운을 식별 가능한 방식으로 표시해야 한다.

#### Scenario: Skill enters cooldown
- **WHEN** 플레이어가 사용 가능한 스킬을 실행한다
- **THEN** 해당 슬롯은 사용할 수 없는 시각 상태와 남은 시간을 표시한다

### Requirement: Enemy world-space information
시스템은(SHALL) 설정된 표시 조건을 만족하는 적의 이름, 레벨과 체력을 월드 공간 UI로 표시해야 한다.

#### Scenario: Enemy health changes
- **WHEN** 표시 중인 적이 피해를 받는다
- **THEN** 해당 적의 월드 체력 표시가 새 체력을 반영한다

### Requirement: Enemy awareness feedback
시스템은(SHALL) 적이 플레이어를 처음 인식할 때 짧고 명확한 인식 표시를 제공해야 한다.

#### Scenario: Enemy acquires player
- **WHEN** 적이 Idle 상태에서 플레이어를 처음 탐지한다
- **THEN** 해당 적 주변에 인식 표시가 정해진 시간 동안 나타난 뒤 사라진다

### Requirement: Damage number feedback
시스템은(SHALL) 피해가 적용된 위치 근처에 피해량을 나타내는 월드 공간 숫자를 표시하고 정해진 시간 후 제거해야 한다.

#### Scenario: Successful hit
- **WHEN** 공격이 유효 대상에게 피해를 적용한다
- **THEN** 적용된 피해량을 나타내는 숫자가 대상 근처에 한 번 나타났다가 자동으로 사라진다
