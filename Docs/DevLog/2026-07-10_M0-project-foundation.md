---
title: M0 개발일지 — Unity 프로젝트 기반
date: 2026-07-10
milestone: M0
requirements:
  - NFR-COMPAT-001
  - NFR-TEST-001
  - NFR-DOC-001
status: complete
tags:
  - devlog
  - unity
  - foundation
---

# M0 개발일지 — Unity 프로젝트 기반

## 목표

Unity 6.3 LTS Universal 3D 프로젝트를 재현 가능한 개발·테스트·빌드 기준선으로 구성한다.

## 시작 상태

- Unity Hub에서 새 `UnityProject` 생성
- Editor: 6000.3.11f1 Apple Silicon
- Template: Universal 3D
- Scene: 템플릿 `SampleScene`
- Console: 컴파일 오류 0개, ADB 인스턴스 정리 알림 1개

## 수행 내용

1. Unity 에디터와 URP 버전을 확인했다.
2. Input System, Cinemachine, AI Navigation, Test Framework를 정확한 버전으로 고정했다.
3. `_Project` 아래 Runtime, Editor, EditMode, PlayMode 어셈블리와 Data, Prefabs, Scenes 구조를 만들었다.
4. `CombatSandbox`와 `VerticalSlice` 씬을 만들고 빌드 순서에 등록했다.
5. Force Text, Visible Meta Files, Input System 사용 설정을 확인했다.
6. 저장소 루트에 Unity용 `.gitignore`, `.gitattributes`, UnityYAMLMerge 설정과 Git LFS 정책을 추가했다.
7. 프로젝트 기반 자동 검증과 macOS ARM64 빌드 메뉴를 추가했다.
8. EditMode·PlayMode 스모크 테스트와 실제 앱 실행을 검증했다.

## 변경된 자산

- 코드: `ProjectIdentity.cs`, `ProjectFoundationTools.cs`
- 테스트: `ProjectFoundationTests.cs`, `ProjectFoundationPlayModeTests.cs`
- 씬: `CombatSandbox.unity`, `VerticalSlice.unity`
- 패키지: Cinemachine 3.1.5 추가, 기존 필수 패키지 버전 고정
- 설정: EditorBuildSettings, EditorSettings root namespace, Git merge driver
- 문서: 기술 기준선, Git·LFS 정책

## 검증 결과

| 항목 | 절차 | 기대 결과 | 실제 결과 | 판정 |
|---|---|---|---|---|
| 기반 검증 | `ValidateProjectFoundation` 배치 실행 | 버전·URP·씬·빌드 순서 유효 | validation passed | 통과 |
| EditMode | Unity Test Framework | 스모크 테스트 1개 통과 | 1/1 passed | 통과 |
| PlayMode | Unity Test Framework | 런타임 어셈블리 로드 | 1/1 passed | 통과 |
| 빌드 | macOS Standalone, ARM64 | BuildResult.Succeeded | Success | 통과 |
| 아키텍처 | `file`로 앱 실행 파일 확인 | Mach-O arm64 | Mach-O 64-bit executable arm64 | 통과 |
| 코드 서명 | `codesign --verify --deep --strict` | 오류 없이 종료 | exit 0 | 통과 |
| 실제 실행 | 640×360 창 모드 실행 | 엔진·Metal 초기화 | Unity 6000.3.11f1 초기화 확인 | 통과 |

## 문제와 해결

### 외부 파일 변경을 즉시 인식하지 않음

- 증상: `manifest.json` 변경 후 `packages-lock.json`이 바로 갱신되지 않았다.
- 원인: Unity 에디터가 백그라운드에 있어 Asset Database 새로고침이 아직 실행되지 않았다.
- 해결: 에디터를 활성화해 새로고침을 유도하고 lock 파일과 PackageCache를 확인했다.
- 회귀 방지: 패키지 변경 뒤 `manifest.json`, `packages-lock.json`, Editor.log 세 곳을 확인한다.

### 첫 macOS 빌드가 Universal로 생성됨

- 증상: Apple Silicon 빌드 의도와 달리 실행 파일에 x86_64와 arm64가 모두 포함되었다.
- 원인: PlayerSettings만 바꾸는 방식으로는 Unity 6.3의 macOS UserBuildSettings가 갱신되지 않았다.
- 해결: macOS 플랫폼 확장 설정의 `architecture`를 `OSArchitecture.ARM64`로 명시하고 다시 빌드했다.
- 회귀 방지: 빌드 성공 로그뿐 아니라 `file`로 실제 Mach-O 아키텍처를 확인한다.

### 에디터 재실행 후 Cloud Project 404 오류 4건

- 증상: 프로젝트를 다시 연 뒤 Project ID 404와 Unity Connect 400 오류가 두 번씩 표시되었다.
- 원인: 일반 `open -a` 재실행이 Unity Hub의 최초 `-createproject -cloudProject` 인수를 복원했고 Unity 인스턴스도 두 개가 남았다.
- 해결: 두 인스턴스를 종료하고 Cloud 생성 인수 없이 `-projectPath`만 전달해 새 인스턴스로 열었다.
- 회귀 방지: 재실행 후 Editor.log의 명령행과 최신 세션 오류를 확인한다.
- 상세 기록: [[2026-07-10-unity-cloud-project-id-404]]

## 바이브코딩 회고

- AI가 잘한 점: PRD와 OpenSpec 작업을 폴더·패키지·테스트·빌드 검증으로 구체화했다.
- AI가 놓치기 쉬운 점: “빌드 성공”만으로 Apple Silicon 전용이라고 단정할 수 없었다.
- AI가 잘못한 점: 에디터를 다시 여는 명령이 이전 Hub 생성 인수를 복원할 가능성을 확인하지 않았다.
- 사람이 판단한 점: 빈 어셈블리 경고를 방치하지 않고 최소 스모크 테스트로 구조 자체를 검증했다.
- 프롬프트를 다시 쓴다면: 결과물 이름뿐 아니라 실행 파일 아키텍처 확인을 처음부터 인수 기준에 포함한다.

## 교육 포인트

- Unity 프로젝트에서 버전 고정은 에디터 버전뿐 아니라 `manifest.json`과 lock 파일까지 포함한다.
- Console 오류 0개와 테스트 통과는 서로 다른 검증이다.
- GUI에 표시된 빌드 옵션과 실제 바이너리 형식이 일치하는지 독립적으로 확인해야 한다.
- AI 생성 코드도 asmdef 경계와 플랫폼별 API 의존성을 검토해야 한다.

## 연결

- PRD: [[01_PRD]]
- 기술 기준선: [[05_TECHNICAL_BASELINE]]
- Git 정책: [[06_ASSET_AND_GIT_POLICY]]
- 프롬프트: [[2026-07-10_M0_project_foundation_v01]]
- OpenSpec: [build-action-rpg-vertical-slice](../../openspec/changes/build-action-rpg-vertical-slice/tasks.md)
