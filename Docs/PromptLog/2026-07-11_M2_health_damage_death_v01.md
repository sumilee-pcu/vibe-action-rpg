---
title: 프롬프트 기록 — M2 체력·피해·사망 v01
date: 2026-07-11
milestone: M2
requirements:
  - FR-COMBAT-004
  - FR-COMBAT-005
  - AC-004
status: complete
tags:
  - prompt-log
  - health
  - damage
---

# 프롬프트 기록 — M2 체력·피해·사망 v01

## 사용자 원문 요청

```text
계속 진행
```

## AI 실행 범위

```text
OpenSpec 3.1 순수 C# 체력·피해·사망 규칙과 3.2 EditMode 경계 테스트를
하나의 기능 단위로 구현한다. ScriptableObject와 공격 판정은 후속 작업으로
남기고, 회피의 IsInvulnerable을 받을 피해 API 계약까지 만든다.
```

## 채택

- `HealthState` 인스턴스가 현재 체력을 소유
- `DamageResult`로 적용·거부 결과 명시
- 정확·초과 치명 피해 모두 사망 1회
- 사망 후 피해와 치유 거부
- 무적 여부를 명시적 인자로 전달

## 기각·보류

- 음수 피해를 치유로 처리하는 암묵적 API는 기각
- ScriptableObject 런타임 체력 저장은 기각
- Actor MonoBehaviour와 HUD 이벤트 연결은 3.3·3.7 이후로 보류
- 부활과 세션 리셋은 7.5로 보류

## 검증 결과

- EditMode: 42/42 통과
- PlayMode: 11/11 통과
- OpenSpec strict validation: 통과
- 새 컴파일 오류·런타임 예외: 0개

## 다음

OpenSpec 3.3에서 `ActorDefinition`, `AttackDefinition` ScriptableObject를 만들되 현재 체력과 공격 실행 상태는 자산에 저장하지 않는다.

## 연결

- PRD: [[../01_PRD]]
- 계약: [[../14_HEALTH_DAMAGE_DEATH]]
- 개발일지: [[../DevLog/2026-07-11_M2-health-damage-death]]
