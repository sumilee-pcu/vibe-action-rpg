---
title: M2 개발일지 — 기본 공격 애니메이션·판정 창
date: 2026-07-11
milestone: M2
requirements:
  - FR-COMBAT-001
  - FR-COMBAT-003
status: complete
tags:
  - devlog
  - attack
  - animation-event
---

# M2 개발일지 — 기본 공격 애니메이션·판정 창

## 목표

OpenSpec 3.4에 따라 BasicAttack의 데이터화된 판정 시작·종료 시점을 실제 공격 애니메이션에 연결한다.

## 시작 상태

- 기준 커밋: `c8d2c70` — Add combat definition assets
- OpenSpec: 16/64
- BasicAttack 정의: 시작 0.15초, 종료 0.30초, 쿨다운 0.50초
- Player에는 Animator와 공격 Controller가 없음

## 수행 내용

1. 단일 Basic Attack을 MVP 기본 공격으로 결정했다.
2. PlayerAttackController가 왼쪽 클릭, Animator Trigger와 공격 상태를 관리하게 했다.
3. Editor 도구가 정의값으로 Animation Event 세 개를 생성하게 했다.
4. 임시 Visual 스케일 애니메이션과 Animator Controller를 생성했다.
5. CombatSandbox Player에 Animator와 공격 Controller를 연결했다.
6. 이벤트 시점 EditMode와 실제 입력·Pause PlayMode를 검증했다.

## 검증 결과

| 항목 | 결과 |
|---|---|
| 판정 시작 | 0.15초 이벤트 일치 |
| 판정 종료 | 0.30초 이벤트 일치 |
| 공격 완료 | 0.50초 이벤트 일치 |
| 왼쪽 클릭 | 애니메이션·판정 상태 통과 |
| Pause | 공격·판정 즉시 취소 |
| EditMode 전체 | 45/45 passed |
| PlayMode 전체 | 13/13 passed |

## 문제와 해결

`GetComponent<Animator>() ?? AddComponent<Animator>()`가 Unity 가짜 null을 제대로 처리하지 못해 MissingComponentException이 발생했다. UnityEngine.Object에는 명시적 `if (component == null)` 검사를 사용하도록 수정했다.

새 공격 정의 자산을 포함한 임시 프로젝트의 오래된 Library 캐시 때문에 씬의 AttackDefinition 참조가 0으로 저장되는 문제도 있었다. 자산을 강제 동기 임포트하고 직접 `Configure` API로 참조를 설정해 해결했다.

## 사람의 판단

- 채택: 단일 공격으로 판정 파이프라인을 먼저 완성
- 채택: AttackDefinition이 Animation Event 시점의 원본
- 채택: 최종 에셋 전에도 관찰 가능한 임시 애니메이션 생성
- 기각: 판정 시간을 Update에 하드코딩하는 방식
- 보류: 3연속 콤보와 최종 캐릭터 공격 모션

## 연결

- 계약: [[../16_ATTACK_ANIMATION_WINDOW]]
- 전투 정의: [[../15_COMBAT_DEFINITIONS]]
- 프롬프트: [[../PromptLog/2026-07-11_M2_attack_animation_window_v01]]
