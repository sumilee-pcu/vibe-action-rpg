---
title: Unity 배치 테스트가 결과 없이 종료되는 문제
date: 2026-07-10
status: resolved
tags:
  - troubleshooting
  - unity
  - test-framework
  - cli
---

# Unity 배치 테스트가 결과 없이 종료되는 문제

## 증상

- Unity 배치 프로세스의 종료 코드는 성공이다.
- 로그에는 'Exiting batchmode successfully'가 표시된다.
- 지정한 'editmode-results.xml' 파일은 생성되지 않는다.
- 테스트 성공·실패 개수도 로그에 나타나지 않는다.

## 발생 조건

Unity 6000.3.11f1에서 최초 임포트가 필요한 프로젝트에 다음 옵션을 함께 전달했다.

~~~text
-batchmode -nographics -runTests -testPlatform EditMode -quit
~~~

## 최소 재현

1. Library가 없는 Unity 프로젝트 복사본을 준비한다.
2. '-runTests'와 '-quit'을 동시에 전달한다.
3. 프로세스 종료 뒤 '-testResults'로 지정한 XML의 존재 여부를 확인한다.
4. 로그에서 테스트 러너 결과보다 'Batchmode quit successfully invoked'가 먼저 나타나는지 확인한다.

## 실패한 시도

성공 종료 코드만으로 테스트가 실행됐다고 판단하려 했다. 그러나 로그를 확인하니 Asset Database 초기 새로고침 직후 '-quit' 처리가 실행됐고 테스트 결과 XML이 없었다.

## 근본 원인

해당 실행에서는 명시적 '-quit' 요청이 테스트 러너의 실행과 결과 저장보다 먼저 처리됐다. Unity 프로세스 자체는 정상 종료했기 때문에 셸 종료 코드만으로는 이 문제를 잡을 수 없었다.

## 해결

'-quit'을 제거하고 테스트 러너가 실행 종료를 관리하도록 했다.

~~~text
-batchmode -nographics -runTests -testPlatform EditMode -testResults <result-path>
~~~

## 회귀 검증

- 'InputActionsAssetTests': 6/6 passed
- 전체 EditMode: 7/7 passed
- 결과 XML의 'result="Passed"', 'failed="0"' 확인

## 예방 규칙

1. Unity 테스트 명령의 성공 여부는 프로세스 종료 코드와 결과 XML을 모두 확인한다.
2. 결과 XML이 없으면 성공으로 기록하지 않는다.
3. 테스트 로그에서 실제 테스트 필터와 결과 저장 경로를 확인한다.
4. 열린 프로젝트를 검증할 때 lock 파일을 삭제하지 말고 별도 임시 복사본을 사용한다.

## 연결

- 개발일지: [[../DevLog/2026-07-10_M1-input-actions]]
- 프롬프트: [[../PromptLog/2026-07-10_M1_input_actions_v01]]
- 입력 계약: [[07_INPUT_ACTIONS]]
