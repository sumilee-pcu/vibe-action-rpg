---
title: UnityEngine.Object와 null 병합 연산자의 MissingComponentException
date: 2026-07-11
status: resolved
tags:
  - troubleshooting
  - unity
  - null
  - editor-script
---

# UnityEngine.Object와 null 병합 연산자의 MissingComponentException

## 증상

Player에 Animator가 없을 때 다음 코드가 새 컴포넌트를 추가할 것으로 예상했지만 `runtimeAnimatorController` 설정에서 MissingComponentException이 발생했다.

```csharp
var animator = player.GetComponent<Animator>() ?? player.AddComponent<Animator>();
```

## 근본 원인

UnityEngine.Object는 네이티브 오브젝트가 없거나 파괴된 상태를 `== null` 연산자 오버로드로 표현한다. C# null 병합 연산자는 이 Unity 전용 판정과 다르게 동작할 수 있어 가짜 null 래퍼를 그대로 선택했다.

## 해결

Unity 컴포넌트는 명시적 Unity null 검사를 사용한다.

```csharp
var animator = player.GetComponent<Animator>();
if (animator == null)
{
    animator = player.AddComponent<Animator>();
}
```

PlayerAttackController에도 같은 규칙을 적용했다.

## 회귀 검증

- 공격 샌드박스 반복 생성 성공
- Player Animator 단일 인스턴스 확인
- EditMode 45/45 통과
- PlayMode 13/13 통과

## 예방 규칙

- UnityEngine.Object 파생 타입에는 `??`와 null 조건부 접근을 일반 객체처럼 가정하지 않는다.
- Editor 자동 구성 도구는 반복 실행 후 컴포넌트 중복 여부를 검증한다.
