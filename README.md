# LogAnalyzer

다양한 형식의 로그 파일을 자동으로 분석하고 분류하는  
WPF 기반 로그 분석 도구입니다.

## ✨ 주요 기능
- JSON / Key-Value / CSV / 자유형식 로그 자동 파싱
- 시간(Time), 레벨(Level), 메시지(Message) 구조화
- 인식 불가능한 로그는 UNKNOWN으로 분류
- WPF UI를 통한 로그 시각화

## 🛠 기술 스택
- C# (.NET)
- WPF
- Newtonsoft.Json
- Regex 기반 파싱 로직

## 📂 프로젝트 구조
LogAnalyzer
├─ Models // LogEntry 모델
├─ Services // LogParser 핵심 로직
├─ Views // WPF UI

## 🔍 지원 로그 예시

### JSON
```json
{"time":"2025-12-28T08:00:00","level":"ERROR","message":"Database connection failed"}
```

### Key-Value
```
time=2025-12-28 08:00:00 level=ERROR msg="Database connection failed"
```

### Loose Format
```
ERROR at 08:00 Database connection failed
WARN 08:05 Low disk space
```

## ⚙️ 동작 원리

### 처리 흐름
1. `FileService`가 `OpenFileDialog`로 `.log` / `.txt` 파일을 선택받아 전체 텍스트를 읽습니다.
2. `LogParser.Parse()`가 줄바꿈 기준으로 내용을 분리한 뒤, 한 줄씩 파싱합니다.
3. 파싱 결과(`List<LogEntry>`)를 `MainWindow`의 `DataGrid`에 바인딩해 표시합니다.

### 파싱 엔진 (Chain of Responsibility)
한 줄을 아래 순서로 시도하며, 앞 파서가 실패(null 반환)하면 다음 파서로 자동으로 넘어가는
체인 구조로 동작합니다.

```csharp
LogEntry log =
       ParseJson(line)
    ?? ParseKeyValue(line)
    ?? ParseBracket(line)
    ?? ParseCsv(line)
    ?? ParseLoose(line)
    ?? CreateUnknown(line);
```

| 순서 | 파서 | 인식 조건 |
|---|---|---|
| 1 | ParseJson | 줄이 `{`로 시작하고 유효한 JSON일 것 |
| 2 | ParseKeyValue | `time=`, `level=` 키워드를 모두 포함 |
| 3 | ParseBracket | `[LEVEL]` 형태의 대괄호 구간 존재 |
| 4 | ParseCsv | 콤마로 구분된 필드가 3개 이상 |
| 5 | ParseLoose | ERROR / WARN / INFO / DEBUG 키워드 포함 |
| 6 | CreateUnknown | 위 조건에 모두 해당하지 않을 경우, 원문을 그대로 보존 |

### 시간 정규화
모든 파서에서 추출한 시간 값은 공통으로 `NormalizeTime()`을 거쳐 `yyyy-MM-dd HH:mm:ss`
형식으로 통일됩니다. 변환할 수 없는 값은 원본 문자열을 그대로 유지하고, 시간 정보 자체가
없으면 `UNKNOWN`으로 표시됩니다.

### UI 동작
- **필터**: ALL / ERROR / WARN / INFO 버튼과 검색어(대소문자 구분 옵션 포함)로
  전체 로그(`_allLogs`)를 걸러 현재 화면(`_currentView`)에 반영합니다.
- **정렬**: 시간순 / 레벨순 / 메시지순 정렬을 지원합니다.
- **통계**: 현재 필터링된 결과를 기준으로 레벨별 개수와 총 로그 수를 하단에 실시간으로 표시합니다.

### 데이터 무결성 정책
파싱에 실패한 로그도 누락 없이 `UNKNOWN`으로 분류되며, 원본 텍스트(Original Raw Message)를
그대로 보존하여 분석 과정에서 데이터 손실이 발생하지 않도록 합니다.
