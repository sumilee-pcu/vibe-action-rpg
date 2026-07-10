## Why

참고 영상의 전투 감각을 학습 가능한 범위로 축소한 3인칭 액션 RPG 버티컬 슬라이스가 필요하다. 결과물뿐 아니라 요구사항, AI 프롬프트, 코드 검토, 테스트와 회고를 함께 남겨 재사용 가능한 바이브코딩 교육 사례로 만든다.

## What Changes

- Unity 6.3 LTS와 URP 기반의 새 게임 프로젝트를 구성한다.
- 3인칭 이동, 카메라, 기본 공격, 회피와 액티브 스킬을 제공한다.
- 탐지, 추적, 공격, 복귀, 피격과 사망 상태를 가진 적 AI를 제공한다.
- 체력, 경험치, 레벨업, 일반 전투와 보스전의 진행 흐름을 제공한다.
- HUD, 월드 공간 적 정보, 데미지 숫자와 승리·패배 화면을 제공한다.
- macOS Apple Silicon 빌드를 우선 검증하고 Windows에서 교차 검증한다.
- 기능별 프롬프트, 구현 판단, 테스트 증거, 오류와 회고를 Obsidian 호환 Markdown으로 기록한다.

## Capabilities

### New Capabilities

- `player-control`: 카메라 기준 이동, 3인칭 카메라, 입력 차단과 회피 동작을 다룬다.
- `combat-system`: 공격 판정, 피해, 피격, 사망, 스킬과 쿨다운 규칙을 다룬다.
- `enemy-ai`: 적 탐지, 추적, 공격, 복귀, 다수 적 공간 분리와 보스 패턴을 다룬다.
- `progression-game-flow`: 경험치, 레벨업, 게임 세션, 승리, 패배와 재시작 흐름을 다룬다.
- `game-ui-feedback`: HUD, 적 머리 위 정보, 인식 표시, 데미지 숫자와 전투 피드백을 다룬다.
- `educational-traceability`: 요구사항에서 프롬프트, 코드, 검증, 회고까지 이어지는 교육 기록을 다룬다.

### Modified Capabilities

없음. 이 프로젝트에는 기존 OpenSpec 기능 명세가 없다.

## Impact

- 새 Unity 프로젝트와 `Assets/_Project` 런타임·테스트 구조가 추가된다.
- Input System, Cinemachine, AI Navigation, Unity Test Framework와 URP를 사용한다.
- 캐릭터, 공격, 스킬과 적 설정을 위한 ScriptableObject 데이터가 추가된다.
- macOS 및 Windows 빌드 설정과 검증 절차가 추가된다.
- `Docs`의 PRD, 프롬프트 기록, 개발일지, 오류 기록과 교육과정 문서가 OpenSpec 요구사항에 연결된다.
