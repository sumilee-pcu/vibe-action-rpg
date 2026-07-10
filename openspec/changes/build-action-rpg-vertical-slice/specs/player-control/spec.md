## ADDED Requirements

### Requirement: Camera-relative player movement
시스템은(SHALL) 키보드 이동 입력을 현재 3인칭 카메라의 수평 방향을 기준으로 변환하고 프레임레이트와 무관한 속도로 플레이어를 이동해야 한다.

#### Scenario: Move forward relative to camera
- **WHEN** 카메라가 월드 전방과 다른 방향을 바라보는 상태에서 플레이어가 전진 입력을 유지한다
- **THEN** 플레이어는 카메라가 바라보는 수평 전방으로 이동하고 이동 방향을 향해 회전한다

#### Scenario: Stable movement across frame rates
- **WHEN** 동일한 이동 입력을 서로 다른 프레임레이트에서 같은 시간 동안 유지한다
- **THEN** 허용 오차 안에서 플레이어의 이동 거리가 동일하다

### Requirement: Third-person camera control
시스템은(SHALL) 마우스 입력으로 플레이어를 추적하는 3인칭 카메라를 회전시키고 일반적인 월드 장애물에 의한 가림을 최소화해야 한다.

#### Scenario: Orbit around the player
- **WHEN** 플레이어가 마우스를 수평 또는 수직으로 움직인다
- **THEN** 카메라는 설정된 감도와 수직 제한 안에서 플레이어 주위를 회전한다

### Requirement: Player dodge with invulnerability
시스템은(SHALL) 회피 입력 시 짧은 이동과 설정 가능한 무적 구간을 실행해야 하며 무적 구간 동안 들어온 피해를 적용하지 않아야 한다.

#### Scenario: Attack lands during invulnerability
- **WHEN** 적 공격의 피해 시점이 플레이어 회피의 무적 구간 안에 있다
- **THEN** 플레이어 체력은 감소하지 않고 회피는 계속 진행된다

#### Scenario: Attack lands outside invulnerability
- **WHEN** 적 공격의 피해 시점이 플레이어 회피의 무적 구간 밖에 있다
- **THEN** 정상 피해 규칙에 따라 플레이어 체력이 감소한다

### Requirement: Gameplay input gating
시스템은(SHALL) 일시정지, 승리 또는 패배 상태에서 게임플레이 입력을 차단하고 해당 UI 입력만 허용해야 한다.

#### Scenario: Input after game over
- **WHEN** 게임 세션이 Victory 또는 Defeat 상태다
- **THEN** 플레이어의 이동, 공격, 회피와 스킬 입력은 실행되지 않는다
