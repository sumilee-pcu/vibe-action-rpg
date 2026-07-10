# Project Tiny Vanguard

Threads 참고 영상의 전투 장면을 출발점으로 만드는 3인칭 로우폴리 액션 RPG 버티컬 슬라이스 프로젝트다. 프로젝트의 두 목표는 다음과 같다.

1. 10~15분 동안 플레이할 수 있는 완결된 전투 데모 제작
2. PRD부터 구현·검증·회고까지의 전 과정을 재사용 가능한 바이브코딩 교육자료로 축적

## 문서 시작점

- [Obsidian 홈](Docs/00_HOME.md)
- [제품 요구사항 정의서](Docs/01_PRD.md)
- [개발 로드맵](Docs/02_ROADMAP.md)
- [바이브코딩 기록 절차](Docs/03_VIBE_CODING_WORKFLOW.md)
- [OpenSpec 제안](openspec/changes/build-action-rpg-vertical-slice/proposal.md)
- [OpenSpec 기술 설계](openspec/changes/build-action-rpg-vertical-slice/design.md)
- [OpenSpec 구현 작업](openspec/changes/build-action-rpg-vertical-slice/tasks.md)
- [OpenSpec 기능 명세](openspec/changes/build-action-rpg-vertical-slice/specs)

## 문서 운영 원칙

- `Docs` 폴더를 Obsidian에서 보관소로 열 수 있다.
- 제품 요구사항에는 `FR`, `NFR`, `AC` 식별자를 사용한다.
- AI에게 보낸 프롬프트와 결과는 `Docs/PromptLog`에 남긴다.
- 하루의 개발 과정과 검증 결과는 `Docs/DevLog`에 남긴다.
- 중요한 기술·범위 결정은 `Docs/Decisions`에 ADR 형식으로 남긴다.
- 해결한 오류는 `Docs/Troubleshooting`에 재현 조건과 함께 남긴다.
- 코드 변경은 관련 요구사항·프롬프트·개발일지를 커밋 메시지에서 추적할 수 있게 한다.
- 구현 상태와 기능별 규범 요구사항은 `openspec/changes`를 기준으로 관리한다.

## 현재 상태

- 단계: 기획
- PRD: v0.1 초안
- OpenSpec: CLI 1.5.0 초기화 및 첫 change 검증 완료
- Unity 프로젝트: 6000.3.11f1 Universal 3D 생성, M0 기반 검증 완료
- 구현 기준: Unity 6, URP, Apple Silicon 우선 개발
