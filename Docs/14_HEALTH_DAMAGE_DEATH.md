---
title: 체력·피해·사망 불변조건 계약
status: active
updated: 2026-07-11
requirements:
  - FR-COMBAT-004
  - FR-COMBAT-005
  - AC-004
  - NFR-MAINT-002
  - NFR-TEST-001
tags:
  - combat
  - health
  - damage
  - death
---

# 체력·피해·사망 불변조건 계약

OpenSpec 3.1·3.2에서 구현한 순수 C# 체력, 피해, 무적 거부와 사망 단일 발생 규칙을 정의한다. Unity 오브젝트와 ScriptableObject 연결은 후속 작업에서 수행한다.

## 불변조건

| 규칙 | 보장 |
|---|---|
| 최대 체력 | 1 이상의 정수 |
| 현재 체력 | 항상 0~최대 체력 |
| 치명 피해 | 현재 체력까지만 적용하고 0으로 고정 |
| 사망 | 처음 0에 도달할 때 정확히 한 번 발생 |
| 사망 후 피해 | 거부되며 체력·이벤트·보상에 영향 없음 |
| 무적 피해 | 피해 0, 체력 변경 이벤트 없음 |
| 치유 | 최대 체력을 넘지 않으며 사망자를 부활시키지 않음 |

## API

- `HealthState`: 최대·현재 체력과 사망 여부를 소유한다.
- `ApplyDamage(int, bool isInvulnerable)`: 피해 결과를 반환한다.
- `Heal(int)`: 실제 회복된 양을 반환한다.
- `HealthChanged`: 실제 체력 변화 때만 발생한다.
- `Died`: 살아 있는 상태에서 체력이 처음 0이 될 때만 발생한다.
- `DamageResult`: 요청·적용 피해, 이전·현재 체력, 거부 이유, 사망 발생 여부를 제공한다.

## 피해 거부 이유

| 값 | 조건 |
|---|---|
| `NonPositiveDamage` | 0 이하 피해 |
| `Invulnerable` | 회피 등 무적 상태 |
| `AlreadyDead` | 이미 사망한 대상 |

거부된 피해는 `AppliedDamage=0`, `CausedDeath=false`이며 이벤트를 발생시키지 않는다.

## 회피 연결

현재 회피 시스템은 `PlayerMovementController.IsInvulnerable`을 제공한다. 이후 피해 어댑터는 다음처럼 현재 상태를 전달한다.

```csharp
health.ApplyDamage(incomingDamage, playerMovement.IsInvulnerable);
```

이 호출 계약으로 무적 구간 안의 피해는 거부되고 구간 밖의 피해는 정상 적용된다. 실제 Player 컴포넌트 연결은 Actor 정의와 런타임 어댑터를 만드는 후속 작업에서 완료한다.

## 자동 검증

- 최대 체력 0·음수 거부
- 비치명 피해와 실제 적용량
- 현재 체력과 같은 정확 치명 피해
- 현재 체력을 넘는 초과 치명 피해
- 사망 이벤트 정확히 1회
- 사망 후 추가 피해 거부
- 무적 피해 거부와 구간 밖 피해 적용
- 0·음수 피해 거부
- 치유 최대값 제한과 사망 후 부활 금지
- 전체 EditMode **42/42 passed**
- 전체 PlayMode 회귀 **11/11 passed**

## 연결

- PRD: [[01_PRD]]
- 회피·무적: [[12_PLAYER_DODGE]]
- 개발일지: [[DevLog/2026-07-11_M2-health-damage-death]]
- 프롬프트: [[PromptLog/2026-07-11_M2_health_damage_death_v01]]
- OpenSpec: [combat-system spec](../openspec/changes/build-action-rpg-vertical-slice/specs/combat-system/spec.md)
