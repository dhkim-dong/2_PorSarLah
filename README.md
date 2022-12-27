# PorSarLah

<img src="https://capsule-render.vercel.app/api?type=wave&color=auto&height=300&section=header&text=PorSarLah&fontSize=90" />

> PorSarLah는 Unity 2022.3.11f1버전으로 작업한 팀 프로젝트입니다.
<br/> **프로젝트 기간** : 2022년 9월 23일 ~ 2022년 10월 17일
<br/> **개발 인원**    : 3명
<br/> **게임 장르**    : 슈팅, ADV
<br/> **노션 주소**    : [노션](https://www.notion.so/Unity-C-117ac0c996df4ac284e2e8bdd6b7a7f2?p=a79261e78b964848b5b0d3b542dd25f2&pm=c)

:bulb:   Git-구성
------------------------
* 1_Assets 부터 9_PostProcess로 공통된 폴더 명으로 작업하였습니다.
  * 구역을 분담하여 팀원들은 각자 컨셉에 맞는 지역을 구현하였습니다. 
    * git 용량 제한으로 용량이 큰 Asset파일은 제외되었습니다.
<img src="img/one.png">

💾 김동훈 Git 작업 내역
------------------------
* Assets > 6_Scripts 폴더 안에서 제가 작업한 코드 Script를 확인할 수 있습니다.
  * Billboard : NPC의 UI Canvas가 항상 Player의 카메라를 바라보게 합니다.
  * Dialogue System : Quest와 연동하여 대사를 출력하는 Class입니다.
  * Enemy : Enemy의 FSM에 따른 행동을 정의하고 있는 Class입니다.
  * Manager : 오디오, 시간, 게임 매니저 등 공통으로 사용하는 Class들을 모아두었습니다.
  * Player : 플레이어의 행동을 정의하고 있는 Class입니다.
  * Quest System : Quest를 구현하고 있습니다.


:nail_care: README.md Version ManageMent
------------------------

색인|버전|날짜|월|일|작업 내용
---|---|---|---|---|---|
1|V1.0|2022년|12월|27일|최초 README.md Commit
