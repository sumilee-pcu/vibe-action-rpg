---
title: 프롬프트 기록 — M2 기본 공격 판정 창 v01
date: 2026-07-11
milestone: M2
requirements:
  - FR-COMBAT-001
  - FR-COMBAT-003
status: complete
tags:
  - prompt-log
  - animation-event
---

# 프롬프트 기록 — M2 기본 공격 판정 창 v01

## 사용자 원문 요청

```text
계속 진행
```

## AI 실행 범위

```text
3.3 완료 후 3.4를 단일 Basic Attack으로 구현한다.
외부 애니메이션 에셋 없이 임시 클립과 Animator를 생성하고,
AttackDefinition의 0.15~0.30초 판정 구간을 Animation Event로 연결한다.
실제 대상 탐지와 피해는 3.5로 남긴다.
```

## 채택

- 단일 공격 MVP
- 데이터 기반 Animation Event 생성
- 임시 Visual 스케일 애니메이션
- 공격 중 재입력 거부와 Pause 취소

## 수정

- UnityEngine.Object에 `??` 대신 명시적 null 검사 사용
- 새 자산을 강제 동기 임포트한 뒤 씬 참조 설정
- SerializedObject 대신 직접 Configure API로 정의 참조 직렬화

## 기각·보류

- Update 경과시간에 판정 시점을 하드코딩하는 방식은 기각
- 실제 피해와 중복 타격 차단은 3.5로 보류
- 최종 외부 애니메이션과 3연속 콤보는 후속 변경으로 보류

## 검증 결과

- EditMode: 45/45 통과
- PlayMode: 13/13 통과
- 원본 동일 복사본 최종 회귀: 통과

## 다음

OpenSpec 3.5에서 활성 판정 창 동안 공격 실행별 타격 대상 집합을 사용해 동일 대상을 최대 한 번만 피해 처리한다.

## 연결

- PRD: [[../01_PRD]]
- 계약: [[../16_ATTACK_ANIMATION_WINDOW]]
- 개발일지: [[../DevLog/2026-07-11_M2-attack-animation-window]]
