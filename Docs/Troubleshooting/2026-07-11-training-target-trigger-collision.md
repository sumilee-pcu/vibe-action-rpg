---
title: CombatSandbox 훈련 타깃이 회피 이동을 막는 문제
date: 2026-07-11
status: resolved
tags:
  - troubleshooting
  - collider
  - trigger
  - playmode-test
---

# CombatSandbox 훈련 타깃이 회피 이동을 막는 문제

## 증상

공격 피해·타이밍 측정을 위해 Player 전방 1.5m에 Capsule 훈련 타깃을 추가한 뒤 PlayMode 전체 결과가 15/15에서 14/15로 감소했다. 실패한 기존 시나리오는 회피 입력 후 약 4m 전진을 검증하는 테스트였다.

## 원인

훈련 타깃의 일반 CapsuleCollider가 CharacterController의 전진 경로를 물리적으로 차단했다. 새 전투 측정 도구가 기존 이동 샌드박스의 불변조건을 깨뜨린 경우다.

## 수정

1. 훈련 타깃 Collider를 `isTrigger=true`로 변경했다.
2. 공격 범위 쿼리는 `QueryTriggerInteraction.Collide`로 Trigger를 포함했다.
3. 이동과 공격 전체 PlayMode를 다시 실행했다.

## 결과

- 회피 거리 테스트 복구
- 훈련 타깃 피해 100→85 유지
- 공격 지연 0.1500초 유지
- PlayMode 15/15 통과

## 재발 방지

샌드박스 측정용 객체를 추가할 때는 대상 기능만 확인하지 않고 기존 이동·카메라·입력 회귀도 함께 실행한다. 전투 표적이 이동 장애물 역할까지 가져야 하는지는 요구사항으로 별도 결정한다.

## 연결

- 측정 기록: [[../19_ATTACK_TIMING_MEASUREMENT]]
- 개발일지: [[../DevLog/2026-07-11_M2-attack-timing-measurement]]
