---
title: Unity 시작 시 Multiple ADB server instances 알림
date: 2026-07-10
unity-version: 6000.3.11f1
platform: macOS Apple Silicon
status: monitoring
tags:
  - troubleshooting
  - adb
  - android
---

# Unity 시작 시 Multiple ADB server instances 알림

## 증상

새 Universal 3D 프로젝트를 연 직후 Console에 다음 취지의 메시지가 한 번 표시되었다.

```text
Multiple ADB server instances found. The duplicate ADB server instance was terminated.
```

## 현재 판단

- Console 분류는 오류와 경고가 아니라 일반 로그였다.
- 현재 목표인 macOS 에디터·Standalone 빌드, 컴파일, EditMode·PlayMode 테스트에는 영향을 주지 않았다.
- Unity가 다른 Android SDK 또는 ADB 프로세스와 중복된 인스턴스를 발견하고 하나를 종료한 상태로 보인다.
- Android가 MVP 대상이 아니므로 지금은 프로젝트 범위를 확장해 SDK 설정을 변경하지 않는다.

## 검증

- [x] 프로젝트 기반 검증 통과
- [x] EditMode 테스트 통과
- [x] PlayMode 테스트 통과
- [x] macOS ARM64 빌드·실행 통과

## Android 빌드가 필요할 때의 후속 확인

1. Unity Preferences의 Android SDK 경로를 확인한다.
2. Android Studio와 Unity가 서로 다른 SDK의 ADB를 동시에 실행하는지 확인한다.
3. `adb kill-server`를 실행한 뒤 Unity가 사용하는 ADB 하나만 시작한다.
4. 실제 Android 디바이스 연결과 빌드로 회귀 검증한다.

현재는 기능 장애가 아니므로 강제 종료 명령이나 SDK 재설치를 수행하지 않는다.

## 교육 포인트

Console 메시지를 발견했다고 즉시 설정을 바꾸지 않는다. 로그·경고·오류의 심각도를 구분하고, 현재 목표 플랫폼의 인수 기준에 실제 영향이 있는지 먼저 검증한다.
