---
title: Actor·Attack 정의 데이터 계약
status: active
updated: 2026-07-11
requirements:
  - FR-DATA-001
  - FR-DATA-002
  - FR-COMBAT-003
  - FR-PLAYER-004
tags:
  - combat
  - scriptable-object
  - actor
  - attack
---

# Actor·Attack 정의 데이터 계약

OpenSpec 3.3에서 구현한 조정 가능한 Actor·Attack ScriptableObject와 런타임 상태 분리 규칙을 정의한다.

## ActorDefinition

| 필드 | Player 기본값 | 용도 |
|---|---:|---|
| Identifier | `player` | 안정적인 데이터 식별자 |
| Display Name | Tiny Vanguard | UI 표시 이름 |
| Level | 1 | 초기 레벨 |
| Maximum Health | 100 | HealthState 생성값 |
| Attack Power | 10 | 공격력 계산 입력 |
| Move Speed | 5 | 이동 속도 정의 |
| Experience Reward | 0 | 적 정의에서 사용할 보상값 |

## AttackDefinition

| 필드 | Basic Attack 기본값 | 용도 |
|---|---:|---|
| Base Damage | 15 | 공격 기본 피해 |
| Range | 1.5m | 공격 중심 거리 |
| Hit Radius | 0.6m | 판정 반경 |
| Active Start | 0.15초 | 판정 시작 |
| Active End | 0.30초 | 판정 종료 |
| Cooldown | 0.50초 | 다음 공격 허용 시점 |
| Stagger Duration | 0.10초 | 피격 경직 정의 |
| Movement Multiplier | 0.25 | 공격 중 이동 허용 비율 |

## 런타임 상태 금지

정의 자산에는 다음 상태를 저장하지 않는다.

- 현재 체력과 사망 여부
- 남은 쿨다운과 경과시간
- 현재 공격 단계와 애니메이션 진행도
- 한 공격에서 이미 타격한 대상 집합
- 현재 타깃과 피격 중 여부

이 상태는 씬의 각 액터·공격 실행 인스턴스가 소유한다. 같은 ScriptableObject를 여러 적이 공유해도 런타임 상태가 섞이지 않는다.

## 자동 생성

Unity 메뉴 `Tiny Vanguard > Create Default Combat Definitions`를 실행하면 다음 자산을 생성하거나 기준값으로 갱신한다.

- `Assets/_Project/Data/Actors/Player.asset`
- `Assets/_Project/Data/Attacks/BasicAttack.asset`

Editor 도구는 필수 값과 공격 판정 시간 순서를 저장 직후 검증한다.

## 자동 검증

- 기본 Player 정의 로드와 핵심 값 확인
- Basic Attack 판정 시작 < 종료 ≤ 쿨다운 확인
- Movement Multiplier 0~1 확인
- 리플렉션으로 런타임 상태를 나타내는 직렬화 필드 금지
- 전체 EditMode **44/44 passed**
- 전체 PlayMode **11/11 passed**

## 연결

- PRD: [[01_PRD]]
- 체력 규칙: [[14_HEALTH_DAMAGE_DEATH]]
- 개발일지: [[DevLog/2026-07-11_M2-combat-definitions]]
- 프롬프트: [[PromptLog/2026-07-11_M2_combat_definitions_v01]]
