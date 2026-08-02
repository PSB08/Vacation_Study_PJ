# Vacation_BraveGroupPJ_Study
버전 : 6000.3.8f1, Universal 3D


구조

Button / Keyboard / OSC
        ↓
ShowCueRouter
        ↓
Camera / Light / Expression / VFX / Timeline / OBS / Log


OSC 명령 목록

| OSC Address | 동작 |
|---|---|
| /show/start | Timeline 공연 시작 |
| /show/stop | Timeline 공연 정지 |
| /camera/1 | Wide 카메라 전환 |
| /camera/2 | CloseUp 카메라 전환 |
| /camera/3 | Side 카메라 전환 |
| /expression/smile | 웃는 표정 적용 |
| /light/normal | Normal 조명 |
| /light/live | Live 조명 |
| /light/emergency | Emergency 조명 |
| /vfx/accent | 강조 VFX 실행 |
| /fallback | fallback 상태 복구 |


OBS WebSocket 연동

OBS WebSocket 5.x를 사용하여 Unity에서 OBS 씬을 전환할 수 있도록 구현했습니다.

| 버튼 | OBS 씬 |
|---|---|
| Standby | Standby 씬으로 전환 |
| Live | Live 씬으로 전환 |
| Emergency | Emergency 씬으로 전환 |

연결 실패 시 운영자 패널에 상태를 표시하고, Reconnect 버튼으로 다시 연결할 수 있도록 했습니다.


성능 측정 / 최적화 요약

Unity Profiler, Frame Debugger, Memory Profiler를 사용하여 성능을 확인했습니다.

| 항목 | 확인 내용 |
|---|---|
| Profiler | FPS, CPU Main Thread, Render Thread 확인 |
| Frame Debugger | Bloom, SSAO, UI, Shadow 렌더링 흐름 확인 |
| Memory Profiler | RenderTexture, Texture2D, Graphics Memory 확인 |
| Low Quality Mode | Bloom 약화, Shadow 감소, VFX 축소 |
| Emergency Mode | Post Processing 최소화, 불필요한 조명 비활성화 |


장애 대응 시나리오

| 상황 | 대응 |
|---|---|
| OSC에 알 수 없는 명령이 들어옴 | Warning Log 출력 |
| OBS 연결 실패 | UI에 Disconnected 표시, Reconnect 버튼 제공 |
| Timeline 중 캐릭터 상태가 깨짐 | Fallback 버튼으로 idle 상태 복구 |
| FPS가 낮아짐 | Low Quality Mode 적용 |
| 긴급 상황 발생 | Emergency 상태로 전환, 조명/VFX/Post Processing 축소 |


실행 방법

1. Unity 6에서 프로젝트를 엽니다.
2. 메인 씬을 실행합니다.
3. Play Mode에서 운영자 패널을 사용합니다.
4. 필요 시 OSC 송신 프로그램으로 명령을 보냅니다.
5. OBS WebSocket을 켠 뒤 Connect 버튼을 누릅니다.


