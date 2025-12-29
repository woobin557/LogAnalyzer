# LogAnalyzer 🔍

다양한 형식의 로그 파일을 자동으로 분석하고 분류하는 **WPF 기반 로그 분석 도구**입니다. 흩어져 있는 로그 데이터를 한눈에 확인하기 쉽게 구조화합니다.

---

## ✨ 주요 기능
* **자동 파싱 (Auto Parsing)**: JSON, Key-Value, CSV 및 자유 형식(Free-text) 로그를 자동으로 인식합니다.
* **데이터 구조화**: 모든 로그를 시간(Time), 레벨(Level), 메시지(Message)로 표준화하여 보여줍니다.
* **미분류 로그 처리**: 형식을 알 수 없는 로그는 UNKNOWN으로 자동 분류하여 누락 없이 관리합니다.
* **WPF 시각화**: 직관적인 UI를 통해 대량의 로그를 쾌적하게 탐색할 수 있습니다.

## 🛠 기술 스택
* **Language**: C# (.NET Framework)
* **Framework**: WPF (Windows Presentation Foundation)
* **Library**: Newtonsoft.Json (JSON 처리), Regex (패턴 매칭)

## 📂 프로젝트 구조
LogAnalyzer
├─ Models      # LogEntry 모델 (데이터 정의)
├─ Services    # LogParser 핵심 로직 (파싱 엔진)


## 📝 지원 로그 예시
LogAnalyzer는 아래와 같은 다양한 패턴을 분석할 수 있습니다.

| 형식 | 로그 데이터 예시 |
| :--- | :--- |
| **JSON** | {"time":"2025-12-28T08:00:00", "level":"ERROR", "message":"Database failed"} |
| **Key-Value** | time=2025-12-28 08:00:00 level=ERROR msg="Database failed" |
| **Loose Format** | ERROR at 08:00 Database failed / WARN 08:05 Low disk space |

## ⚙️ 처리 정책
* **UNKNOWN 분류**: 시간 정보를 찾을 수 없거나 정해진 패턴에 맞지 않을 경우 UNKNOWN 섹션으로 분류합니다.
* **데이터 무결성**: 파싱 실패 시에도 로그 원문(Original Raw Message)을 그대로 유지하여 분석 오류를 방지합니다.
