---
title: Cinemachine 추적 PlayMode 테스트의 순간 변위 불안정
date: 2026-07-11
status: resolved
tags:
  - troubleshooting
  - unity
  - cinemachine
  - playmode-test
---

# Cinemachine 추적 PlayMode 테스트의 순간 변위 불안정

## 증상

`OutputCameraFollowsPlayerTarget`가 동일 코드에서 통과와 실패를 반복했다. 플레이어를 3m 옮긴 뒤 0.3초 시점의 카메라 변위가 약 0.08~0.26m로 달라져 `0.1m 초과` 조건이 불안정했다.

## 발생 조건

- Cinemachine 위치 감쇠가 활성화되어 있다.
- 정면 벽과 Deoccluder가 함께 활성화되어 있다.
- 테스트가 추적과 가림 보정을 동시에 관찰한다.
- 배치 모드 프레임 진행량에 따라 감쇠의 중간 상태가 달라진다.

## 실패한 시도

- 한 번 통과한 0.1m 임계값을 고정된 기능 계약처럼 사용했다.
- 추적 테스트에서 가림 샘플의 영향을 분리하지 않았다.

## 근본 원인

테스트의 목적은 카메라가 플레이어 Target을 추적하는지 확인하는 것이지만, 장면의 Deoccluder와 장애물이 최종 카메라 위치에 영향을 주었다. 더 근본적으로는 활성 `PlayerMovementController`와 `CharacterController`가 있는 Player의 Transform을 테스트가 직접 순간 이동했다. 다음 프레임의 `CharacterController.Move`가 내부 위치와 다시 동기화하면서 Player가 원위치로 돌아가 카메라는 짧은 과도 상태만 보였다.

## 해결

1. 추적 테스트에서 `Camera Occlusion Cases`를 비활성화한다.
2. `PlayerMovementController`를 비활성화해 테스트의 직접 위치 변경과 경쟁하지 않게 한다.
3. 테스트에서 Orbital Follow의 위치 감쇠를 0으로 설정한다.
4. 가상 카메라의 이전 상태를 무효화해 파이프라인을 즉시 재계산한다.
5. 플레이어를 3m 이동한 뒤 두 프레임을 진행한다.
6. 출력 카메라가 시작점에서 2.5m 이상 이동했는지 확인한다.
5. 가림 동작은 별도의 정면 벽·측면 기둥·복귀 테스트가 계속 담당한다.

## 회귀 테스트

- 카메라 추적: 통과
- 정면 벽 가림: 통과
- 측면 기둥 가림: 통과
- 장애물 제거 후 복귀: 통과
- 전체 PlayMode: 9/9 통과

## 예방 규칙

- 하나의 테스트는 가능한 한 하나의 책임만 검증한다.
- CharacterController가 있는 오브젝트를 직접 이동할 때는 이동 Controller와의 경쟁을 차단한다.
- 감쇠 시스템은 명세에 없는 특정 프레임의 절대 위치를 완료 조건으로 만들지 않는다.
- 임계값을 낮추기 전에 다른 파이프라인이 관찰값에 개입하는지 먼저 분리한다.
