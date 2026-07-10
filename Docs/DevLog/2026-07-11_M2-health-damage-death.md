---
title: M2 개발일지 — 체력·피해·사망 규칙
date: 2026-07-11
milestone: M2
requirements:
  - FR-COMBAT-004
  - FR-COMBAT-005
  - AC-004
  - NFR-TEST-001
status: complete
tags:
  - devlog
  - health
  - damage
  - death
---

# M2 개발일지 — 체력·피해·사망 규칙

## 목표

OpenSpec 3.1·3.2에 따라 Unity 생명주기와 분리된 체력·피해 규칙을 구현하고, 체력 범위·치명 피해·사망 후 추가 피해를 자동 검증한다.

## 시작 상태

- 기준 커밋: `db6a480` — Implement player dodge and complete M1
- OpenSpec: 13/64
- M1 플레이어 조작 작업 7/7 완료
- 회피 시스템이 `IsInvulnerable` 상태를 제공함
- 체력·피해·사망 규칙은 없음

## 수행 내용

1. `HealthState`가 최대·현재 체력과 사망 상태를 소유하도록 구현했다.
2. `DamageResult`에 요청 피해, 실제 피해, 전후 체력, 거부 이유와 사망 발생 여부를 담았다.
3. 무적, 사망, 0 이하 피해를 명시적으로 구분해 거부했다.
4. 치명 피해를 0으로 제한하고 사망 이벤트를 처음 한 번만 발생시켰다.
5. 최대 체력 상한과 사망 후 부활 금지를 포함한 치유 규칙을 추가했다.
6. 10개 신규 EditMode 사례와 전체 PlayMode 회귀를 검증했다.

## 검증 결과

| 항목 | 실제 결과 |
|---|---|
| 최대 체력 0·음수 | 예외로 거부 |
| 비치명 피해 | 적용량과 잔여 체력 일치 |
| 정확·초과 치명 피해 | 체력 0, 사망 1회 |
| 사망 후 피해 | `AlreadyDead`, 이벤트 0회 추가 |
| 무적 피해 | `Invulnerable`, 체력 유지 |
| 치유 상한 | 최대 체력에서 제한 |
| 사망 후 치유 | 0 반환, 사망 유지 |
| EditMode 전체 | 42/42 passed |
| PlayMode 전체 | 11/11 passed |

## 사람의 판단

- 채택: 체력은 정수로 관리해 피해·HUD 표시의 결정성을 유지
- 채택: 거부도 결과 객체로 반환해 호출자가 이유를 구분 가능
- 채택: 실제 변화가 있을 때만 `HealthChanged` 발생
- 채택: `Died` 이벤트를 최초 치명 피해에서만 발생
- 채택: 무적 여부는 피해 호출자가 명시적으로 전달
- 기각: ScriptableObject에 현재 체력을 저장하는 접근
- 기각: 음수 피해를 치유로 해석하는 암묵적 규칙
- 보류: 사망 후 부활·리셋 — 게임 재시작 작업에서 별도 정의

## 교육 포인트

- 불변조건은 MonoBehaviour보다 순수 객체에서 먼저 검증하기 쉽다.
- 요청 피해와 실제 적용 피해는 치명 초과 피해에서 서로 다르다.
- 사망 이벤트의 단일 발생은 경험치·승리 이벤트 중복 방지의 기반이다.
- 피해 거부 이유를 열거형으로 남기면 무적 피드백과 디버깅을 분리하기 쉽다.

## 연결

- 계약: [[../14_HEALTH_DAMAGE_DEATH]]
- 회피: [[../12_PLAYER_DODGE]]
- 프롬프트: [[../PromptLog/2026-07-11_M2_health_damage_death_v01]]
- OpenSpec tasks: [tasks.md](../../openspec/changes/build-action-rpg-vertical-slice/tasks.md)
