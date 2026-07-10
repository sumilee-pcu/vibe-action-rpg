---
title: Git 및 대형 에셋 정책
status: active
updated: 2026-07-10
tags:
  - git
  - git-lfs
  - unity
  - assets
---

# Git 및 대형 에셋 정책

## 저장소 범위

Git 저장소의 루트는 `vibe-action-rpg`이며 PRD, OpenSpec, 교육자료와 `UnityProject`를 하나의 변경 이력으로 관리한다.

### 버전 관리 대상

- `Docs`, `openspec`, `.codex`
- `UnityProject/Assets`
- `UnityProject/Packages/manifest.json`
- `UnityProject/Packages/packages-lock.json`
- `UnityProject/ProjectSettings`
- 모든 Unity `.meta` 파일

### 제외 대상

- `Library`, `Temp`, `Obj`, `Logs`, `UserSettings`
- IDE 생성 파일과 개인 설정
- 빌드 산출물과 캡처 파일

## Unity 직렬화 규칙

- Asset Serialization: `Force Text`
- Version Control Mode: `Visible Meta Files`
- `.meta` 파일은 대응하는 에셋과 같은 커밋에 포함한다.
- Scene·Prefab 충돌은 UnityYAMLMerge 사용을 우선한다.
- 같은 Scene이나 Prefab을 두 사람이 동시에 편집하지 않는 것을 기본으로 한다.

## Git LFS 정책

현재 로컬에는 Git LFS CLI가 설치되어 있지 않고 대형 외부 에셋도 아직 없다. 따라서 텍스트와 작은 기본 에셋으로 M0를 완료하고, 첫 대형 바이너리 에셋을 가져오기 전에 Git LFS를 설치·활성화한다.

### LFS 적용 후보

```text
*.fbx
*.blend
*.psd
*.tga
*.wav
*.mp3
*.ogg
*.mp4
*.mov
*.ttf
*.otf
```

PNG·JPG는 파일 크기와 변경 빈도를 확인한 뒤 적용한다. 작은 UI 아이콘까지 무조건 LFS로 보내지 않는다.

### 활성화 절차

```bash
brew install git-lfs
git lfs install --local
git lfs track "*.fbx" "*.blend" "*.psd" "*.wav" "*.mp3" "*.ogg" "*.mp4" "*.mov"
```

LFS 활성화는 첫 외부 아트 에셋 도입 작업에서 별도로 검증한다. 이미 일반 Git으로 커밋된 대형 파일은 임의로 히스토리를 재작성하지 않는다.

## 커밋 추적

기능 커밋은 OpenSpec task와 요구사항 식별자를 포함한다.

```text
chore(project): establish Unity foundation [OpenSpec 1.1-1.5]
```

프롬프트, 테스트와 개발일지는 커밋 본문 또는 관련 Markdown에서 연결한다.
