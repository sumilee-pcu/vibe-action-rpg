---
title: 프롬프트 기록 — M1 게임플레이 입력 차단 v01
date: 2026-07-11
milestone: M1
requirements:
  - FR-UI-005
  - FR-PROG-004
  - FR-PROG-005
  - NFR-MAINT-002
status: complete
tags:
  - prompt-log
  - input-system
  - session-state
---

# 프롬프트 기록 — M1 게임플레이 입력 차단 v01

## 목표

- OpenSpec 2.5의 중단된 구현을 복구하고 완료한다.
- Paused, Victory, Defeat에서 Gameplay 입력을 차단한다.
- PRD와 OpenSpec 진척도를 실제 구현 상태와 동기화한다.

## 사용자 원문 요청

```text
ㅇㅋ vibe-action-rpg 바로 이어서 진행. prd 진척도 체크 잊지마
```

## AI가 해석한 실행 범위

```text
build-action-rpg-vertical-slice의 2.5를 먼저 완료한다.
중단 당시 남은 작업 트리를 보존하고 원본 CombatSandbox에 입력 게이트를
유효한 GUID로 반영한다. 전체 EditMode·PlayMode 회귀를 통과시킨 뒤
기능 계약, 개발일지, 오류 기록, PRD와 로드맵을 동기화한다.
회피 구현은 다음 작업 2.6으로 남긴다.
```

## 실행 환경

- Unity: 6000.3.11f1
- Input System: 프로젝트 잠금 버전
- Cinemachine: 3.1.5
- Render Pipeline: URP 17.3.0
- 기준 커밋: `2ba8b42`
- OpenSpec change: `build-action-rpg-vertical-slice`

## AI에게 적용한 제약

- 기존 미완료 변경을 삭제하거나 덮어쓰지 않는다.
- 열려 있는 원본 Unity 에디터를 강제 종료하지 않는다.
- 입력 맵 활성화 소유자는 하나만 둔다.
- Victory·Defeat에서 Pause 입력으로 전투 상태에 복귀하지 못하게 한다.
- 임시 복사본에서 전체 테스트를 통과한 뒤 원본을 완료 처리한다.
- 테스트 임계값을 낮추기 전에 실패 원인과 책임 격리를 확인한다.
- PRD, 로드맵, OpenSpec, 교육 문서를 같은 완료 상태로 갱신한다.

## AI 결과 요약

- `GameSessionState`, `GameplayInputPolicy`, `GameplayInputGate` 구현
- 이동·카메라의 중복 맵 활성화와 커서 책임 제거
- CombatSandbox Game Session 구성과 자동 설정 도구
- Pause·Victory·Defeat 정책 및 실제 입력 테스트
- 씬 GUID 불일치 수정
- Cinemachine 추적 회귀 테스트 격리
- PRD·로드맵·기능 계약·개발일지 동기화

## 사람의 검토

### 채택

- 순수 정책과 Unity 어댑터 분리
- Gameplay/System/UI 상태표를 단일 원본으로 사용
- Pause 토글을 Playing과 Paused에만 허용
- 원본 프로젝트와 동일한 임시 복사본에서 전체 회귀 실행

### 수정

- 임시 프로젝트에서 생성된 스크립트 GUID를 원본 `.meta` GUID로 교정
- 전체 씬 재생성 대신 Game Session 루트만 추가하는 최소 직렬화 변경
- 카메라 추적 테스트에서 가림 오브젝트를 비활성화해 추적 책임만 검증

### 기각

- 이동·카메라·UI 컴포넌트가 각자 Action Map을 켜고 끄는 구조
- Victory·Defeat에서 Escape로 Playing에 복귀하는 동작
- 이전 임시 테스트 통과만 믿고 현재 원본 씬 검증을 생략하는 접근
- 불안정한 테스트를 통과시키기 위해 순간 이동 임계값만 낮추는 접근

## 검증 결과

- EditMode 전체: 27/27 통과
- PlayMode 전체: 9/9 통과
- Pause 중 이동 차단: 통과
- Victory·Defeat Gameplay 전체 차단: 통과
- 새 컴파일 오류와 런타임 예외: 0개

## 다음 프롬프트에서 개선할 점

- 2.6 회피는 이동 거리, 지속시간, 재사용 조건, 무적 시작·종료를 먼저 수치 계약으로 정한다.
- 체력 시스템이 아직 없으므로 무적 판정은 테스트 가능한 독립 정책으로 만들고 3.1에서 피해 규칙과 연결한다.
- M1 완료 전에 수동 시나리오와 통합 교육 노트를 2.7에서 정리한다.

## 연결

- PRD: [[../01_PRD]]
- 입력 차단 계약: [[../11_GAMEPLAY_INPUT_GATING]]
- 개발일지: [[../DevLog/2026-07-11_M1-gameplay-input-gating]]
- OpenSpec tasks: [tasks.md](../../openspec/changes/build-action-rpg-vertical-slice/tasks.md)
