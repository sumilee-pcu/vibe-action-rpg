# _Project

프로젝트가 직접 소유하는 Unity 자산만 이 폴더에 둔다. 외부 패키지와 임포트한 에셋은 공급자별 폴더에 유지하고 직접 수정하지 않는다.

- `Art`: 프로젝트가 직접 소유하거나 가공한 시각 자산
- `Audio`: 프로젝트가 직접 소유하거나 가공한 오디오
- `Data`: ScriptableObject 정의 데이터
- `Prefabs`: Characters, Combat, UI, VFX 프리팹
- `Scenes`: 기능 샌드박스와 버티컬 슬라이스 씬
- `Scripts/Runtime`: 빌드에 포함되는 런타임 코드
- `Scripts/Editor`: Unity Editor 전용 도구
- `Tests/EditMode`: 빠른 순수 규칙 테스트
- `Tests/PlayMode`: Unity 생명주기·씬 통합 테스트
