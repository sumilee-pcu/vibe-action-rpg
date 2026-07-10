---
title: 프롬프트 기록 — M0 프로젝트 기반 v01
date: 2026-07-10
milestone: M0
requirements:
  - NFR-COMPAT-001
  - NFR-TEST-001
  - NFR-DOC-001
status: complete
tags:
  - prompt-log
  - unity
  - foundation
---

# 프롬프트 기록 — M0 프로젝트 기반 v01

## 목표

- Unity 6000.3.11f1 Universal 3D 프로젝트를 만든다.
- 패키지, 프로젝트 구조, Git 정책, 씬, 테스트와 macOS 빌드 기준을 확정한다.
- 구현 과정 자체를 바이브코딩 교육자료로 남긴다.

## 사용자 원문 요청

```text
새프로젝트를 urp 6버전으로 만들까?
OpenSpec CLI 이것도 만들어놔
생성완료
```

## 실행 환경

- Unity: 6000.3.11f1
- Render Pipeline: URP 17.3.0
- Platform: macOS Apple Silicon
- OpenSpec change: `build-action-rpg-vertical-slice`
- 기준 상태: 새 Universal 3D 템플릿

## AI에게 적용한 제약

- 사용자가 생성한 `UnityProject` 경로를 유지한다.
- Unity 생성 파일 중 Library, Temp, Logs는 버전 관리하지 않는다.
- ScriptableObject에는 이후 런타임 상태를 저장하지 않는다.
- 런타임, 에디터, 테스트 어셈블리를 분리한다.
- macOS ARM64 앱을 실제 실행해 확인한다.
- 모든 완료 작업은 OpenSpec checkbox와 교육 문서에 반영한다.

## AI 결과 요약

- 필수 패키지 버전 고정
- `_Project` 폴더와 asmdef 구성
- 프로젝트 기반 검증·빌드 자동화 코드
- EditMode·PlayMode 스모크 테스트
- Git, UnityYAMLMerge와 LFS 도입 정책
- CombatSandbox·VerticalSlice 씬
- ARM64 빌드와 실제 실행 검증

## 사람의 검토

### 채택

- 요구사항 → OpenSpec task → 파일 → 테스트 → 개발일지 추적 구조
- 테스트 가능한 런타임 어셈블리와 Editor 도구 분리
- 빌드 후 바이너리 아키텍처와 코드 서명 독립 검증

### 수정

- 첫 빌드가 Universal 바이너리로 생성되어 macOS 플랫폼별 UserBuildSettings를 추가했다.
- 빈 asmdef 경고를 없애고 어셈블리 로드를 검증하기 위해 최소 테스트 코드를 추가했다.
- 일반 앱 재실행 방식이 최초 Hub의 Cloud 생성 인수를 복원해 빨간 로그 4건을 만들었다. 두 Unity 인스턴스를 종료하고 명시적 `-projectPath`만 사용하는 새 인스턴스로 교체했다.

### 기각

- 모든 바이너리를 즉시 Git LFS로 보내는 구성은 기각했다. 실제 대형 에셋 도입 전에 LFS CLI를 설치하고 필요한 확장자만 적용한다.
- Unity Cloud Version Control은 사용하지 않고 로컬 Git을 기준으로 삼았다.

## 검증 결과

- 기반 검증: 통과
- EditMode: 1/1 통과
- PlayMode: 1/1 통과
- macOS ARM64 빌드: 성공
- 코드 서명: 통과
- 실제 앱 실행: Unity·Metal 초기화 확인

## 다음 프롬프트에서 개선할 점

- “macOS 빌드”와 “ARM64 전용 빌드”를 분리해 명시한다.
- Unity GUI 상태뿐 아니라 로그·lock 파일·바이너리 형식의 증거를 요구한다.
- 에디터 재실행은 프로세스 중복과 실제 명령행 인수를 검증 항목에 포함한다.
- 첫 플레이어 기능은 입력 액션 정의와 이동 로직을 별도 프롬프트로 나눈다.

## 연결

- PRD: [[01_PRD]]
- 개발일지: [[2026-07-10_M0-project-foundation]]
- 오류 기록: [[2026-07-10-unity-cloud-project-id-404]]
- OpenSpec tasks: [tasks.md](../../openspec/changes/build-action-rpg-vertical-slice/tasks.md)
