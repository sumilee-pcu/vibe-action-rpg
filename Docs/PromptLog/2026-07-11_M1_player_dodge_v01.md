---
title: 프롬프트 기록 — M1 플레이어 회피·무적 v01
date: 2026-07-11
milestone: M1
requirements:
  - FR-PLAYER-005
  - AC-004
status: complete
tags:
  - prompt-log
  - dodge
---

# 프롬프트 기록 — M1 플레이어 회피·무적 v01

## 목표

- OpenSpec 2.6을 구현한다.
- 회피 거리와 무적 구간을 Inspector에서 조정 가능하게 한다.
- 체력 시스템 없이도 무적 경계와 이동 거리를 자동 검증한다.

## 사용자 원문 요청

```text
ㅇㅋ vibe-action-rpg 바로 이어서 진행. prd 진척도 체크 잊지마
```

## AI 실행 범위

```text
2.5 완료 후 다음 미완료 작업 2.6을 진행한다.
순수 회피 상태와 Unity 이동 어댑터를 분리하고 Space 실제 입력,
프레임 독립 거리, 무적 경계, Pause 취소를 검증한다.
피해 시스템 연결은 3.1 범위로 남긴다.
```

## 채택·수정·기각

### 채택

- 4m / 0.35초 / 무적 0.05~0.25초 기본값
- 카메라 기준 입력 방향, 무입력 시 캐릭터 전방
- 중복 회피 거부와 Pause 취소
- `IsInvulnerable` 공개 상태

### 수정

- 기존 카메라 회귀 테스트가 Player Transform 직접 이동과 CharacterController 이동을 경쟁시키던 문제를 격리했다.

### 기각

- 체력 시스템보다 먼저 임시 피해 클래스를 만드는 접근
- 스태미나·연속 구르기·애니메이션 상태 머신을 이번 작업에 추가하는 범위 확대
- 프레임 수로 회피 시간을 고정하는 구현

## 검증 결과

- EditMode: 32/32 통과
- PlayMode: 11/11 통과
- 컴파일 오류·런타임 예외: 0개

## 다음

- OpenSpec 2.7에서 M1 수동 시나리오와 학습 기록을 통합한다.
- OpenSpec 3.1에서 `IsInvulnerable`을 실제 피해 거부 규칙에 연결한다.

## 연결

- PRD: [[../01_PRD]]
- 회피 계약: [[../12_PLAYER_DODGE]]
- 개발일지: [[../DevLog/2026-07-11_M1-player-dodge]]
