---
title: 프롬프트 기록 — M3 이탈과 홈 복귀 v01
date: 2026-07-11
milestone: M3
requirements:
  - FR-AI-004
status: complete
tags:
  - prompt-log
  - enemy-return
---

# 프롬프트 기록 — M3 이탈과 홈 복귀 v01

## 사용자 원문 요청

```text
계속 진행
```

## AI 실행 범위

```text
OpenSpec 4.5를 구현한다.
이탈 거리는 현재 적이 아니라 초기 HomePosition과 Player의 거리로 판단한다.
Return에서는 Agent 목적지를 홈으로 설정하고 HomeTolerance 안에서 Idle로 복귀한다.
추격용 정지 거리와 복귀용 허용 반경을 구분한다.
```

## 채택

- 고정 HomePosition 기준 이탈
- Return 전용 0.25m 정지 거리
- 홈 도착 전 재탐지하지 않는 상태 머신 규칙 유지
- NavMesh 목적지 오차를 제품 HomeTolerance로 검증

## 기각·수정

- 적 현재 위치 기준 이탈은 전투 영역이 이동하므로 기각
- 고정 0.05m 테스트 임계값은 NavMesh 격자 특성을 무시해 정의값 기준으로 수정
- 순간 위치 복귀는 기각하고 Agent 경로 이동 사용

## 검증 결과

- EditMode 53/53, PlayMode 18/18 통과
- Return 목적지·정지 거리·Idle 복귀·공격 0회 통과

## 다음

OpenSpec 4.6에서 Dead 상태의 이동·공격과 반복 보상을 차단한다.

## 연결

- PRD: [[../01_PRD]]
- 복귀 계약: [[../24_ENEMY_RETURN_HOME]]
- 개발일지: [[../DevLog/2026-07-11_M3-enemy-return-home]]
