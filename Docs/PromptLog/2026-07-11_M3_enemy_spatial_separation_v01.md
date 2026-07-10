---
title: 프롬프트 기록 — M3 다섯 적 공간 분리 v01
date: 2026-07-11
milestone: M3
requirements:
  - FR-AI-005
status: complete
tags:
  - prompt-log
  - enemy-separation
---

# 프롬프트 기록 — M3 다섯 적 공간 분리 v01

## 사용자 원문 요청

```text
계속 진행
```

## AI 실행 범위

```text
OpenSpec 4.7을 구현한다.
다섯 적에게 Player 둘레의 고유 원형 접근 슬롯을 배정한다.
NavMeshAgent 회피 반경과 슬롯 목적지를 함께 사용한다.
순수 슬롯 거리와 실제 다섯 Agent 동시 접근의 쌍간 거리를 검증한다.
```

## 채택

- 균등 원형 접근 슬롯
- EnemyBrain별 슬롯 인덱스
- 슬롯 목적지와 NavMesh 회피 반경 결합
- 적별 이동량·경로 상태 테스트 출력

## 기각·수정

- Player 중심 한 점 목적지는 겹침을 유발해 기각
- 단순 시작 위치만 다르게 하는 방식은 목적지 겹침을 해결하지 못해 기각
- 고정 1.5초 도착 가정은 장애물 우회를 반영하지 못해 최대 3초 조건 방식으로 수정

## 검증 결과

- EditMode 54/54, PlayMode 20/20 통과
- 다섯 PathComplete, 모든 쌍간 거리 0.5m 초과

## 다음

OpenSpec 4.8에서 M3 상태 전이 테스트 매트릭스와 NavMesh 문제 해결 사례를 통합한다.

## 연결

- PRD: [[../01_PRD]]
- 공간 분리 계약: [[../26_ENEMY_SPATIAL_SEPARATION]]
- 개발일지: [[../DevLog/2026-07-11_M3-enemy-spatial-separation]]
