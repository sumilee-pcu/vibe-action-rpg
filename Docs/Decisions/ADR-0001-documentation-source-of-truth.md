---
title: ADR-0001 문서 원본은 Obsidian 호환 Markdown으로 관리
date: 2026-07-10
status: accepted
tags:
  - adr
  - documentation
---

# ADR-0001 — 문서 원본은 Obsidian 호환 Markdown으로 관리

## 상황

개발 문서는 코드·커밋·Unity 자산과 긴밀하게 연결되어야 하며, 이후 바이브코딩 교육자료로 재구성할 수 있어야 한다. Notion과 Obsidian이 후보였다.

## 결정

제품 PRD와 교육 문서는 프로젝트 저장소의 `Docs` 폴더에 Obsidian 호환 Markdown 원본으로 둔다. 구현 변화의 규범 명세와 작업 상태는 `openspec/changes`를 원본으로 두며 요구사항 ID로 두 영역을 연결한다. 필요 시 완성된 교육자료를 Notion에 게시한다.

## 이유

- 코드와 같은 Git 이력으로 변경을 추적할 수 있다.
- 프롬프트, 테스트, 오류 기록을 실제 파일 경로와 연결하기 쉽다.
- 오프라인에서도 편집할 수 있다.
- 일반 Markdown이므로 Notion·정적 사이트·문서 도구로 이관하기 쉽다.

## 고려한 대안

### Notion을 원본으로 사용

공유와 데이터베이스 보기는 편하지만 코드 커밋과 원자적으로 변경하기 어렵고, 실습 중 네트워크·인증에 의존한다.

### 두 곳에서 동시에 편집

동기화 충돌과 최신 원본 혼란이 발생하므로 채택하지 않는다.

## 결과

- 개발 중에는 Markdown만 편집한다.
- 구현을 시작하거나 변경할 때는 OpenSpec proposal, specs, design, tasks를 먼저 갱신한다.
- Notion에 게시한 문서는 파생본으로 취급하고 원본 파일 링크 또는 기준 커밋을 표시한다.
- 교육과정이 확정되면 Notion 허브·회차별 페이지로 별도 게시할 수 있다.
