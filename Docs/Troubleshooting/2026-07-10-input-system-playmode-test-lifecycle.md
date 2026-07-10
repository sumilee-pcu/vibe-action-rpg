---
title: Input System PlayMode 테스트의 시간·장치·수명주기 문제
date: 2026-07-10
status: resolved
tags:
  - troubleshooting
  - unity
  - input-system
  - playmode
---

# Input System PlayMode 테스트의 시간·장치·수명주기 문제

## 증상

카메라 기준 이동 PlayMode 테스트를 구현하는 동안 서로 연결된 세 증상이 발생했다.

1. 이동은 됐지만 20프레임 뒤 캐릭터 방향 내적이 기대 0.95보다 낮은 0.8926이었다.
2. PlayerInput을 사용하는 씬이 종료될 때 'listenForUnpairedDeviceActivity' 값이 음수가 되는 예외가 발생했다.
3. 가상 W 키를 누른 직후 상태와 Move 값을 검사하면 모두 0이었다.

## 발생 조건

- Unity 6000.3.11f1
- Input System 1.19.0
- Test Framework 1.6.0
- 배치 모드 PlayMode UnityTest
- InputTestFixture의 가상 Keyboard 사용
- PlayerInput과 InputActionAsset 런타임 복제·장치 페어링 사용

## 실패한 접근 1 — 고정 프레임 수

~~~text
Press(W)
20 frames 대기
방향 정렬 검사
~~~

배치 모드는 매우 높은 프레임레이트로 실행될 수 있다. 20프레임이 지나도 Time.deltaTime 누적값이 작아 초당 회전량이 충분히 적용되지 않았다.

### 수정

게임 시간 0.25초가 흐르도록 'WaitForSeconds'를 사용했다. 프레임 독립성 자체는 별도의 순수 EditMode 테스트에서 30fps와 120fps로 검증했다.

## 실패한 접근 2 — PlayerInput과 Fixture 복원 순서

PlayerInput은 장치 자동 페어링과 미연결 장치 감시를 관리한다. InputTestFixture도 테스트 전후로 Input System 전체 상태를 저장·복원한다. 두 수명주기가 겹치자 씬 종료 시 PlayerInput이 이미 복원된 장치 감시 카운터를 다시 감소시키며 예외가 발생했다.

### 수정

현재 게임은 로컬 싱글플레이 한 명만 필요하다. PlayerInput의 사용자 관리 기능을 제거하고 PlayerMovementController가 직렬화된 InputActionAsset의 Gameplay 맵을 직접 활성화하도록 단순화했다.

이 결정은 단지 테스트를 통과시키기 위한 우회가 아니다. 현재 요구사항에 필요하지 않은 런타임 액션 복제와 사용자 페어링을 제거해 실제 게임 수명주기도 단순해졌다.

## 실패한 접근 3 — Press 직후 상태 검사

InputTestFixture 구현을 확인한 결과 UnityTest에서 Press는 즉시 InputSystem.Update를 호출하지 않는다. 이벤트를 큐에 넣고 다음 Player Loop가 처리하도록 한다.

~~~text
Press(W)
즉시 isPressed 검사  -> false
즉시 Move 검사       -> 0
~~~

### 수정

~~~text
Press(W)
한 프레임 진행
isPressed와 Move 검사 -> 정상
~~~

## 최종 구조

- InputTestFixture가 테스트용 Keyboard를 생성한다.
- CombatSandbox를 로드하면 PlayerMovementController가 Gameplay 맵을 활성화한다.
- Press로 W 이벤트를 큐에 넣는다.
- 한 프레임 뒤 키와 Move 상태를 확인한다.
- 0.25초 동안 이동시킨 뒤 방향과 회전을 검증한다.
- Fixture가 Input System 상태를 복원한다.

## 회귀 검증

- 이동 수학 EditMode: 6/6 passed
- 전체 EditMode: 13/13 passed
- PlayerMovement PlayMode: passed
- 전체 PlayMode: 2/2 passed
- 종료 시 ArgumentOutOfRangeException: 0개

## 예방 규칙

1. 시간 기반 동작을 고정 프레임 수만으로 검증하지 않는다.
2. UnityTest의 입력 이벤트가 적용되는 Player Loop 시점을 확인한다.
3. PlayerInput은 다중 사용자·장치 페어링이 실제 요구사항일 때 도입한다.
4. 테스트 실패를 임계값 완화로 숨기지 말고 시간·입력·수명주기를 분리해 확인한다.
5. 최종 테스트는 임시 수정 프로젝트가 아니라 현재 원본 파일의 새 복사본에서 다시 실행한다.

## 연결

- 이동 계약: [[08_PLAYER_MOVEMENT]]
- 개발일지: [[../DevLog/2026-07-10_M1-camera-relative-movement]]
- 프롬프트: [[../PromptLog/2026-07-10_M1_camera_relative_movement_v01]]
- Input System CLI 문제: [[2026-07-10-unity-run-tests-quit]]
