---
title: M2 개발일지 — Actor·Attack 정의 데이터
date: 2026-07-11
milestone: M2
requirements:
  - FR-DATA-001
  - FR-DATA-002
  - FR-COMBAT-003
status: complete
tags:
  - devlog
  - scriptable-object
  - combat-data
---

# M2 개발일지 — Actor·Attack 정의 데이터

## 목표

OpenSpec 3.3에 따라 Actor와 Attack의 조정값을 ScriptableObject로 만들고 현재 체력·쿨다운·타격 대상 같은 런타임 상태를 자산에서 배제한다.

## 시작 상태

- 기준 커밋: `c74dfd4` — Implement health and death rules
- OpenSpec: 15/64
- `HealthState`가 런타임 체력을 인스턴스로 소유
- Data/Actors와 Data/Attacks 폴더만 존재

## 수행 내용

1. `ActorDefinition`에 레벨, 최대 체력, 공격력, 이동 속도와 경험치 보상을 정의했다.
2. `AttackDefinition`에 피해, 범위, 판정 구간, 쿨다운, 경직과 이동 배율을 정의했다.
3. `OnValidate`에서 음수와 잘못된 시간 순서를 보정했다.
4. Editor 도구로 Player와 BasicAttack 자산을 반복 생성·검증하게 했다.
5. 리플렉션 테스트로 정의 자산의 런타임 상태 필드를 금지했다.

## 검증 결과

| 항목 | 결과 |
|---|---|
| Player 자산 | 로드·기본값 통과 |
| BasicAttack 자산 | 판정 시간·수치 통과 |
| 런타임 상태 금지 | Actor·Attack 모두 통과 |
| EditMode 전체 | 44/44 passed |
| PlayMode 전체 | 11/11 passed |

## 사람의 판단

- 채택: 공격 중 이동 허용량을 0~1 배율로 데이터화
- 채택: 판정 시작·종료와 쿨다운의 시간 순서 보정
- 채택: 기본 자산을 코드로 반복 생성해 수업 환경 편차 감소
- 기각: ScriptableObject에 현재 체력이나 남은 쿨다운 저장
- 보류: 애니메이션 클립·Animator 연결 — OpenSpec 3.4

## 교육 포인트

- ScriptableObject는 여러 인스턴스가 공유하는 정의값에 적합하다.
- 현재 체력처럼 인스턴스마다 달라지는 값은 순수 런타임 객체가 소유해야 한다.
- 데이터 시간 순서는 Inspector 제약만으로 충분하지 않아 저장 검증이 필요하다.

## 연결

- 계약: [[../15_COMBAT_DEFINITIONS]]
- 체력 규칙: [[../14_HEALTH_DAMAGE_DEATH]]
- 프롬프트: [[../PromptLog/2026-07-11_M2_combat_definitions_v01]]
