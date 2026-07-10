---
title: 프롬프트 기록 — M2 Actor·Attack 정의 v01
date: 2026-07-11
milestone: M2
requirements:
  - FR-DATA-001
  - FR-DATA-002
  - FR-COMBAT-003
status: complete
tags:
  - prompt-log
  - scriptable-object
---

# 프롬프트 기록 — M2 Actor·Attack 정의 v01

## 사용자 원문 요청

```text
계속 진행
```

## AI 실행 범위

```text
3.1·3.2를 커밋한 뒤 OpenSpec 3.3을 진행한다.
ActorDefinition과 AttackDefinition에는 조정 가능한 정의값만 두고,
기본 Player·BasicAttack 자산을 Editor 도구로 생성한다.
리플렉션 테스트로 런타임 상태 필드가 들어오지 못하게 한다.
```

## 채택

- Actor의 최대 체력·공격력·이동 속도 정의
- Attack의 피해·범위·판정 구간·경직·쿨다운·이동 배율 정의
- 기본 데이터 자산 자동 생성 도구
- 런타임 상태 금지 리플렉션 검사

## 기각·보류

- 현재 체력, 남은 쿨다운, 타격 대상 집합의 자산 저장은 기각
- 공격 애니메이션과 실제 판정 연결은 3.4~3.5로 보류
- 외부 캐릭터·애니메이션 에셋 선택은 폴리싱 단계로 보류

## 검증 결과

- EditMode: 44/44 통과
- PlayMode: 11/11 통과
- 기본 자산 생성·검증: 통과
- 새 컴파일 오류·런타임 예외: 0개

## 다음

OpenSpec 3.4에서 BasicAttack의 0.15~0.30초 판정 구간을 실제 공격 애니메이션 진행과 연결한다.

## 연결

- PRD: [[../01_PRD]]
- 계약: [[../15_COMBAT_DEFINITIONS]]
- 개발일지: [[../DevLog/2026-07-11_M2-combat-definitions]]
