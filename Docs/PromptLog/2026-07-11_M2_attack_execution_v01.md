---
title: 프롬프트 기록 — M2 공격 실행별 단일 타격 v01
date: 2026-07-11
milestone: M2
requirements:
  - FR-COMBAT-002
status: complete
tags:
  - prompt-log
  - attack-execution
---

# 프롬프트 기록 — M2 공격 실행별 단일 타격 v01

## 사용자 원문 요청

```text
계속 진행
```

## AI 실행 범위

```text
OpenSpec 3.5와 3.6을 함께 구현한다.
공격 실행마다 피해 대상 집합을 만들고, Collider 부모의 ActorHealth를
정체성으로 사용해 다중 Collider 적에게 한 번만 피해를 적용한다.
새 공격 실행은 같은 적을 다시 타격할 수 있어야 한다.
```

## 채택

- AttackExecution 인스턴스별 HashSet
- ActorHealth 부모 기준 대상 통합
- 실제 피해 전 대상 등록
- 다중 Collider 집중 PlayMode 테스트

## 기각·보류

- Collider 비활성화와 Collider 인스턴스 ID 기준 중복 방지는 기각
- 이미 타격한 대상의 ScriptableObject 저장은 기각
- 피격·사망·데미지 숫자 출력은 3.7로 보류

## 검증 결과

- EditMode: 47/47 통과
- PlayMode: 14/14 통과
- 다중 Collider 피해: 실행당 정확히 1회

## 다음

OpenSpec 3.7에서 실제 체력 변화와 사망을 피격 반응·Death 전이·데미지 숫자 이벤트에 연결한다.

## 연결

- PRD: [[../01_PRD]]
- 계약: [[../17_ATTACK_EXECUTION]]
- 개발일지: [[../DevLog/2026-07-11_M2-attack-execution]]
