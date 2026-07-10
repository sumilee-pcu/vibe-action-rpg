---
title: 프롬프트 기록 — M2 공격 타이밍 측정 v01
date: 2026-07-11
milestone: M2
requirements:
  - FR-COMBAT-001
  - FR-COMBAT-003
  - AC-003
status: complete
tags:
  - prompt-log
  - attack-timing
---

# 프롬프트 기록 — M2 공격 타이밍 측정 v01

## 사용자 원문 요청

```text
계속 진행
```

## AI 실행 범위

```text
OpenSpec 3.8을 구현한다.
OpenHitWindow Animation Event에서 실제 물리 범위 탐지와 피해를 적용한다.
CombatSandbox 훈련 타깃으로 공격 시작부터 체력 감소까지의 시간을 측정한다.
측정값과 설정값의 차이, 적용 대상 수와 전체 회귀 결과를 문서화한다.
```

## 채택

- Animation Event와 실제 피해 적용의 단일 실행 지점
- 고정 Collider 버퍼 기반 비할당 범위 탐지
- 자기 자신 제외와 실행별 대상 등록 유지
- 테스트 결과 XML에 실제 지연값 출력

## 실패·수정

- 일반 Collider 훈련 타깃이 회피 이동을 막는 결과를 기각
- Trigger 타깃과 Trigger 포함 공격 쿼리로 교정
- 편집 모드에서 비직렬화 HealthState를 검증하던 조건을 직렬화 참조 검증으로 교정

## 검증 결과

- 측정 0.1500초 / 설정 0.1500초 / 오차 0.0000초
- EditMode 47/47, PlayMode 15/15 통과

## 다음

M2를 완료하고 OpenSpec 4.1의 적 정의 데이터로 이동한다.

## 연결

- PRD: [[../01_PRD]]
- 측정 기록: [[../19_ATTACK_TIMING_MEASUREMENT]]
- 개발일지: [[../DevLog/2026-07-11_M2-attack-timing-measurement]]
