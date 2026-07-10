---
title: 프롬프트 기록 — M3 사망 행동·보상 차단 v01
date: 2026-07-11
milestone: M3
requirements:
  - FR-AI-001
  - FR-AI-005
  - AC-006
status: complete
tags:
  - prompt-log
  - enemy-death
---

# 프롬프트 기록 — M3 사망 행동·보상 차단 v01

## 사용자 원문 요청

```text
계속 진행
```

## AI 실행 범위

```text
OpenSpec 4.6을 구현한다.
ActorHealth 최초 사망을 EnemyBrain이 즉시 Dead와 Agent 정지로 연결한다.
이후 Tick과 추가 피해가 이동·공격·보상 신호를 만들지 않아야 한다.
실제 경험치 시스템이 소비할 RewardAvailable 신호는 정확히 한 번만 출력한다.
```

## 채택

- ActorHealth Died 이벤트 구독
- Dead 즉시 전이·경로 제거·Agent 정지
- EnemyBrain의 중복 사망 처리 방어
- 진행도와 분리된 1회성 보상 신호

## 기각·보류

- 사망 즉시 GameObject 제거는 사망 연출·검증을 막아 기각
- Player 경험치 직접 수정은 5.4 이전 결합이므로 보류
- ScriptableObject에 보상 지급 여부 저장은 기각

## 검증 결과

- EditMode 53/53, PlayMode 19/19 통과
- Dead·정지·공격 차단·사망 처리 1회·25 XP 신호 1회 통과

## 다음

OpenSpec 4.7에서 다섯 근접 적의 최소 공간 분리를 구현한다.

## 연결

- PRD: [[../01_PRD]]
- 사망 계약: [[../25_ENEMY_DEATH_GUARD]]
- 개발일지: [[../DevLog/2026-07-11_M3-enemy-death-guard]]
