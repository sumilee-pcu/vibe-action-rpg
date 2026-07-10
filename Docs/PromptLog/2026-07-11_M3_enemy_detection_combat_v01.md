---
title: 프롬프트 기록 — M3 Player 탐지·추적·공격 쿨다운 v01
date: 2026-07-11
milestone: M3
requirements:
  - FR-AI-001
  - FR-AI-002
  - FR-AI-003
  - AC-004
status: complete
tags:
  - prompt-log
  - enemy-combat
---

# 프롬프트 기록 — M3 Player 탐지·추적·공격 쿨다운 v01

## 사용자 원문 요청

```text
계속 진행
```

## AI 실행 범위

```text
OpenSpec 4.4를 구현한다.
EnemyBrain이 Player 거리·생존·쿨다운을 EnemyStateSignals로 변환한다.
Chase는 Agent 목적지를 갱신하고 Attack은 Agent를 멈춘 뒤 Player 체력에 피해를 적용한다.
쿨다운 전 재공격을 거부하고 실제 PlayMode 체력 변화로 검증한다.
```

## 채택

- 상태 결정과 Unity 행동 어댑터 분리
- 인스턴스별 다음 공격 시각
- Player ActorHealth와 회피 무적 어댑터 연결
- 실제 체력 100→84→68 PlayMode 검증

## 기각·보류

- Update에서 상태 머신 규칙을 중복 구현하는 방식은 기각
- EnemyDefinition에 남은 쿨다운 저장은 기각
- 공격 애니메이션·VFX는 후속 폴리싱으로 보류
- 이탈·홈 복귀는 4.5로 보류

## 검증 결과

- EditMode 53/53, PlayMode 17/17 통과
- 탐지·추적·정지·첫 공격·쿨다운 거부·재공격 통과

## 다음

OpenSpec 4.5에서 홈 기준 이탈과 Return·Idle 복귀를 구현한다.

## 연결

- PRD: [[../01_PRD]]
- 전투 계약: [[../23_ENEMY_DETECTION_COMBAT]]
- 개발일지: [[../DevLog/2026-07-11_M3-enemy-detection-combat]]
