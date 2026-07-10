## ADDED Requirements

### Requirement: Requirement-to-evidence traceability
프로젝트는(MUST) 완료된 핵심 기능마다 OpenSpec 또는 PRD 요구사항 ID에서 프롬프트, 변경 파일, 검증 결과와 개발 회고로 이어지는 추적 정보를 제공해야 한다.

#### Scenario: Review a completed feature
- **WHEN** 교육자가 완료된 기능의 요구사항 ID를 선택한다
- **THEN** 관련 프롬프트 기록, 구현 커밋 또는 파일, 테스트 증거와 개발일지를 찾을 수 있다

### Requirement: Prompt decision record
프로젝트는(MUST) 기능 구현에 사용한 핵심 AI 프롬프트와 AI 결과에 대한 사람의 채택, 수정 또는 기각 판단을 기록해야 한다.

#### Scenario: AI output requires correction
- **WHEN** AI가 생성한 접근이나 코드 일부를 사람이 수정 또는 기각한다
- **THEN** 프롬프트 기록에는 잘못된 가정, 판단 근거와 최종 검증 결과가 포함된다

### Requirement: Reproducible troubleshooting record
프로젝트는(MUST) 교육 가치가 있는 해결된 오류마다 증상, 발생 조건, 최소 재현 단계, 실패한 시도, 근본 원인, 해결과 회귀 테스트를 기록해야 한다.

#### Scenario: A recurring Unity error is solved
- **WHEN** 컴파일, 직렬화, NavMesh, 애니메이션 또는 빌드 오류가 해결된다
- **THEN** 다른 학습자가 같은 조건에서 문제와 해결을 재현할 수 있는 오류 문서가 존재한다

### Requirement: Feature quality gate
프로젝트는(MUST) 기능 완료 전에 요구사항, 구현 안전성, 자동·수동 검증, 교육 기록과 플랫폼 확인을 포함한 완료 체크리스트를 적용해야 한다.

#### Scenario: Feature is marked complete
- **WHEN** 개발자가 기능 작업을 완료 상태로 변경한다
- **THEN** 체크리스트의 적용 항목이 충족되고 미적용 항목에는 이유가 기록되어 있다
