---
title: Unity 재실행 후 Project ID 404·Unity Connect 400 오류
date: 2026-07-10
unity-version: 6000.3.11f1
platform: macOS Apple Silicon
status: resolved
tags:
  - troubleshooting
  - unity-cloud
  - launch-arguments
---

# Unity 재실행 후 Project ID 404·Unity Connect 400 오류

## 증상

M0 기반 작업 후 Unity 에디터를 다시 열었을 때 Console에 다음 오류가 두 세트, 총 네 건 표시됐다.

```text
Project ID request failed, Reason: The requested URL returned error: 404 (404).
Unknown Unity Connect error (400) ... legacy/v1/projects/ ... HTTP error code 404
```

## 영향

- C# 컴파일 오류는 아니었다.
- Unity Cloud 연결 요청만 실패했다.
- 로컬 씬, 테스트와 macOS 빌드는 정상 동작했다.
- 빨간 Console 항목이 남아 실제 게임 오류를 가리므로 수정이 필요했다.

## 조사

프로젝트 설정은 이미 로컬 프로젝트 상태였다.

- `UnityConnectSettings.m_Enabled: 0`
- `cloudProjectId`: 비어 있음
- `cloudEnabled: 0`

그러나 Editor.log의 명령행에는 새 프로젝트를 처음 만들 때 Unity Hub가 사용한 다음 인수가 다시 포함되어 있었다.

```text
-createproject
-cloneFromTemplate
-cloudProject
```

## 근본 원인

M0 배치 검증 후 에디터를 다시 열 때 macOS의 일반 `open -a` 방식을 사용했다. 이 방식이 새 `-projectPath` 인수 대신 Unity Hub의 최초 프로젝트 생성 실행 상태를 복원했다. 그 결과 이미 생성이 끝난 로컬 프로젝트에서 존재하지 않는 Cloud 프로젝트 생성을 다시 시도했다.

두 Unity 에디터 인스턴스가 동시에 남아 있어 일반 종료 요청이 한 인스턴스만 대상으로 한 것도 문제를 반복시켰다.

## 해결 방법

1. 실행 중인 두 Unity 에디터 인스턴스를 종료했다.
2. `-createproject`, `-cloneFromTemplate`, `-cloudProject` 없이 새 인스턴스를 실행했다.
3. 정확한 프로젝트 경로만 전달했다.

```text
Unity -projectPath /path/to/vibe-action-rpg/UnityProject
```

## 회귀 검증

- [x] Editor.log 명령행에 `-projectPath`만 존재
- [x] `-createproject` 없음
- [x] `-cloudProject` 없음
- [x] Asset Database 초기 임포트 완료
- [x] Project ID 404 없음
- [x] Unity Connect 400 없음
- [x] Import Worker 오류 없음
- [x] C# 컴파일 오류 없음

## 교육 포인트

- 프로젝트 설정이 정상이어도 프로세스 실행 인수 때문에 외부 서비스 요청이 발생할 수 있다.
- Console 메시지만 보고 Cloud 설정 파일을 임의로 수정하기 전에 Editor.log의 실제 실행 명령을 확인한다.
- macOS에서 Unity를 자동 재실행할 때 일반 앱 열기와 새 인스턴스·명시적 `-projectPath` 실행을 구분한다.
- “에디터가 열렸다”는 사실만으로 정상 실행을 판단하지 않고 최신 세션 로그에서 오류 재발 여부를 확인한다.
