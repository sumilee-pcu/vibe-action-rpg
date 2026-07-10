## ADDED Requirements

### Requirement: Attack timing and damage
시스템은(SHALL) 공격 정의에 지정된 판정 구간에만 유효 대상을 탐지하고 계산된 피해를 적용해야 한다.

#### Scenario: Target inside the active hit window
- **WHEN** 유효한 적이 공격 판정 구간과 범위 안에 있다
- **THEN** 시스템은 해당 적에게 공격 정의와 공격자 능력치로 계산한 피해를 적용한다

#### Scenario: Target outside the active hit window
- **WHEN** 적이 공격 범위에 있지만 판정 구간이 시작되지 않았거나 종료되었다
- **THEN** 해당 공격으로 피해를 적용하지 않는다

### Requirement: One hit per target per attack execution
시스템은(SHALL) 하나의 공격 실행에서 동일한 피해 수신 대상에게 최대 한 번만 피해를 적용해야 한다.

#### Scenario: Enemy has multiple colliders
- **WHEN** 한 번의 공격 판정이 동일 적의 둘 이상의 콜라이더와 겹친다
- **THEN** 해당 적의 체력은 한 번의 피해량만큼만 감소한다

#### Scenario: New attack execution hits again
- **WHEN** 이전 공격이 종료된 후 새로운 공격 실행이 동일 적을 적중한다
- **THEN** 새 공격의 피해는 정상적으로 한 번 적용된다

### Requirement: Health and death invariants
시스템은(SHALL) 체력을 0과 최대 체력 사이로 제한하고, 체력이 처음 0에 도달할 때 사망 전이를 정확히 한 번 실행해야 한다.

#### Scenario: Lethal damage
- **WHEN** 현재 체력 이상의 피해가 살아 있는 액터에게 적용된다
- **THEN** 체력은 0이 되고 사망 이벤트는 한 번 발생하며 이후 전투 행동이 차단된다

#### Scenario: Additional damage after death
- **WHEN** 사망한 액터에게 추가 피해가 전달된다
- **THEN** 사망 이벤트와 사망 보상은 다시 발생하지 않는다

### Requirement: Skill cooldown enforcement
시스템은(SHALL) 스킬 사용 직후 정의된 쿨다운을 시작하고 완료되기 전 동일 스킬의 재사용을 거부해야 한다.

#### Scenario: Reuse skill during cooldown
- **WHEN** 남은 쿨다운이 0보다 큰 스킬의 입력이 들어온다
- **THEN** 스킬 효과를 실행하지 않고 기존 쿨다운을 유지한다

#### Scenario: Use skill after cooldown
- **WHEN** 스킬 쿨다운이 완료된 후 사용 입력이 들어온다
- **THEN** 스킬 효과를 실행하고 새 쿨다운을 시작한다
