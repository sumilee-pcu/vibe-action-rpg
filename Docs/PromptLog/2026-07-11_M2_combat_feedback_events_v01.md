---
title: 프롬프트 기록 — M2 피격·사망·피해 출력 이벤트 v01
date: 2026-07-11
milestone: M2
requirements:
  - FR-COMBAT-004
  - FR-COMBAT-005
status: complete
tags:
  - prompt-log
  - combat-feedback
---

# 프롬프트 기록 — M2 피격·사망·피해 출력 이벤트 v01

## 사용자 원문 요청

```text
계속 진행
```

## AI 실행 범위

```text
OpenSpec 3.7을 구현한다.
실제 적용된 피해만 대상·피해량·월드 위치·치명 여부 이벤트를 1회 출력한다.
비치명 피해는 Hit 반응, 치명 피해는 Death 전환과 행동 차단으로 연결한다.
다중 Collider와 사망 후 추가 피해에서도 이벤트가 중복되지 않아야 한다.
```

## 채택

- ActorHealth의 적용 결과 기반 이벤트 출력
- 피격·사망 반응의 별도 구독 컴포넌트
- 치명 피해의 Hit 반응 생략
- 사망 시 진행 중 공격 취소와 새 공격 거부

## 기각·보류

- HealthState에서 Unity UI를 직접 생성하는 방식은 기각
- 실제 데미지 숫자 프리팹은 OpenSpec 6.5로 보류
- 실제 범위 탐지와 애니메이션 타이밍 측정은 3.8로 보류

## 검증 결과

- EditMode: 47/47 통과
- PlayMode: 15/15 통과
- 비치명·치명·사망 후·다중 Collider 이벤트 횟수 통과

## 다음

OpenSpec 3.8에서 활성 판정 창에 실제 공격 범위 탐지를 연결하고 시각 타격 시점과 피해 적용 시점의 차이를 측정한다.

## 연결

- PRD: [[../01_PRD]]
- 계약: [[../18_COMBAT_FEEDBACK_EVENTS]]
- 개발일지: [[../DevLog/2026-07-11_M2-combat-feedback-events]]
