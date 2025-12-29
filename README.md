LogAnalyzer

다양한 형식의 로그 파일을 자동으로 분석하고 분류하는
WPF 기반 로그 분석 도구입니다.

주요 기능

JSON / Key-Value / CSV / 자유형식 로그 자동 파싱

시간(Time), 레벨(Level), 메시지(Message) 구조화

인식 불가능한 로그는 UNKNOWN으로 분류

WPF UI를 통한 로그 시각화

기술 스택

C# (.NET Framework)

WPF

Newtonsoft.Json

Regex 기반 파싱 로직

프로젝트 구조
LogAnalyzer
├─ Models (LogEntry 모델)
├─ Services (LogParser 핵심 로직)
├─ Views (WPF UI)

지원 로그 예시

JSON
{"time":"2025-12-28T08:00:00","level":"ERROR","message":"Database connection failed"}

Key-Value
time=2025-12-28 08:00:00 level=ERROR msg="Database connection failed"

Loose Format
ERROR at 08:00 Database connection failed
WARN 08:05 Low disk space

처리 정책

시간 또는 형식 인식 불가 시 UNKNOWN으로 표시

파싱 실패 로그도 원문을 유지하여 표시
