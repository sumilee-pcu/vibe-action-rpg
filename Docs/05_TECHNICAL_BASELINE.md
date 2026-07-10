---
title: 기술 환경 및 패키지 기준선
status: active
updated: 2026-07-10
tags:
  - unity
  - packages
  - baseline
---

# 기술 환경 및 패키지 기준선

## Unity 환경

| 항목 | 고정값 |
|---|---|
| Unity Editor | 6000.3.11f1 |
| Unity 릴리스 | Unity 6.3 LTS |
| 에디터 아키텍처 | Apple Silicon |
| 렌더링 API | Metal |
| 프로젝트 템플릿 | Universal 3D |
| 프로젝트 경로 | `UnityProject` |
| 1차 빌드 플랫폼 | macOS Apple Silicon |
| 교차 검증 플랫폼 | Windows 11 x86-64 |

## 직접 참조 패키지

`UnityProject/Packages/manifest.json`을 패키지 버전의 기준 원본으로 사용한다.

| 패키지 | ID | 고정 버전 | 용도 |
|---|---|---:|---|
| Universal Render Pipeline | `com.unity.render-pipelines.universal` | 17.3.0 | 렌더링 |
| Input System | `com.unity.inputsystem` | 1.19.0 | 플레이어·UI 입력 |
| Cinemachine | `com.unity.cinemachine` | 3.1.5 | 3인칭 카메라 |
| AI Navigation | `com.unity.ai.navigation` | 2.0.11 | NavMesh 적 이동 |
| Unity Test Framework | `com.unity.test-framework` | 1.6.0 | EditMode·PlayMode 테스트 |
| uGUI | `com.unity.ugui` | 2.0.0 | HUD와 월드 공간 UI |

## 버전 변경 규칙

1. 구현 change가 진행 중일 때 에디터 주·부 버전을 올리지 않는다.
2. 패키지는 Package Manager의 자동 업데이트 제안을 바로 적용하지 않는다.
3. 버전 변경이 필요하면 별도 OpenSpec change에서 호환성과 회귀 범위를 정의한다.
4. `manifest.json`과 `packages-lock.json`을 함께 버전 관리한다.
5. 버전 변경 후 최소한 컴파일, EditMode 테스트, 대표 PlayMode 시나리오, macOS 빌드를 검증한다.

## 확인 증거

- `ProjectSettings/ProjectVersion.txt`: `6000.3.11f1 (3000ef702840)`
- Universal 3D 템플릿의 URP: `17.3.0`
- 에디터 상단 표시: `Unity 6.3 LTS (6000.3.11f1) <Metal>`
- Cinemachine 3.1.5는 Unity 6000 계열용 릴리스 버전이다.

## 관련 문서

- [[01_PRD]]
- [[02_ROADMAP]]
- [OpenSpec 기술 설계](../openspec/changes/build-action-rpg-vertical-slice/design.md)
- [OpenSpec 구현 작업](../openspec/changes/build-action-rpg-vertical-slice/tasks.md)
