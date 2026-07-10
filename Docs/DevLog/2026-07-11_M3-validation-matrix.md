---
title: M3 개발일지 — 적 AI 검증 통합
date: 2026-07-11
milestone: M3
requirements:
  - FR-AI-001
  - FR-AI-002
  - FR-AI-003
  - FR-AI-004
  - FR-AI-005
status: complete
tags:
  - devlog
  - enemy-ai
  - validation
---

# M3 개발일지 — 적 AI 검증 통합

## 목표

OpenSpec 4.8에 따라 M3 상태 전이 테스트 매트릭스와 재현 가능한 NavMesh 문제 해결 사례를 정리한다.

## 시작 상태

- 기준 커밋: `45076c0` — Separate multiple enemy approaches
- OpenSpec: 28/64
- M3 런타임 4.1~4.7 완료
- 기능별 테스트는 있으나 한눈에 보는 상태 전이 증거표가 없음

## 수행 내용

1. 여섯 상태의 조건·기대 행동·자동 테스트를 한 표로 연결했다.
2. 정의·NavMesh·다섯 적 분리의 비상태 검증을 별도 표로 정리했다.
3. fake-null, ResetPath 정지 순서와 Trigger Collider 문제 해결 문서를 연결했다.
4. M3 기능 완료 체크리스트를 적용했다.
5. 최종 테스트 수와 OpenSpec 진척도를 PRD·로드맵에 동기화했다.

## M3 최종 결과

| 항목 | 결과 |
|---|---|
| OpenSpec M3 | 8/8 complete |
| 전체 OpenSpec | 29/64, 45.3% |
| EditMode | 54/54 passed |
| PlayMode | 20/20 passed |
| 상태 전이 | Idle·Chase·Attack·Hit·Return·Dead PASS |
| 다중 적 | 다섯 PathComplete·쌍간 0.5m 초과 |

## 다음 단계

M4의 첫 작업은 OpenSpec 5.1 스킬 정의 데이터와 인스턴스별 쿨다운 상태다. 기존 AttackDefinition·HealthState와 같은 정의/런타임 분리 원칙을 유지한다.

## 연결

- 검증 매트릭스: [[../27_M3_ENEMY_AI_VALIDATION]]
- 프롬프트: [[../PromptLog/2026-07-11_M3_validation_matrix_v01]]
- 문제 해결: [[../Troubleshooting/2026-07-11-navmesh-reset-path-stop-order]]
