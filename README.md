# PCS_Assignment
박치수의 팀스파르타 TDS 몬스터 움직임 구현하기 과제입니다.

코드 구조
---
몬스터의 각 기능(이동, 점프, 공격 등)을 별도의 클래스로 분리하고, 몬스터 클래스의 각 이벤트에 등록하는 방식으로 구현하였습니다.

이를 통해 몬스터 별로 기능을 추가, 제거가 가능합니다.

> 예시 : 만약 점프하지 않는 몬스터의 경우 코드 수정 없이 JumpHandler 스크립트를 비활성화 or 제거하면 됩니다.

몬스터의 기능 뿐만 아니라 플레이어의 공격, 체력 처리 등 대부분의 기능을 별도의 클래스로 분리하고 이벤트에 등록하는 방식으로 구현하였습니다.

이를 통해 각 객체 간 결합도를 최대한 낮추고 유연성을 늘렸습니다.

필수 구현
---
TDS에서 사용하는 몬스터의 움직임 순환을 구현하였습니다.

TDS에서 몬스터가 트럭 앞에 쌓일 경우 맨 앞,맨 아래에 있는 몬스터를 뒤로 미는 기능을 구현하였습니다.

Rigidbody2D의 velocity를 이용하여 기본 움직임을 구현하고, 뒤로 밀리는 기능은 자연스럽게 뒤로 밀리게 하기 위해 DOTWeen의 rigidbody.DOMoveX를 이용하여 구현하였습니다.

추가구현
---
+ **몬스터 공격 기능**
  
몬스터의 공격 범위에 대상이 들어오면 공격 애니메이션을 실행하며, 대상의 체력을 공격력 만큼 줄입니다.

공격 범위는 Editor에서 Debug.DrawRay로 시각적으로 볼 수 있게 구현하였습니다.

+ **영웅 공격 기능**
  
게임 시작 시 자동으로 공격을 반복하며, 마우스 좌클릭 드래그를 할 경우 마우스의 드래그 지점으로 공격을 실행합니다.(모바일이 아니므로 마우스 클릭으로 대체)

마우스 좌클릭 입력이 없을 경우, 화면에 보이는 몬스터 중 가장 가까운 몬스터를 찾아 해당 몬스터로 공격을 실행합니다.

+ **박스 장애물 기능**
  
기본적으로 5개의 상자가 존재하며, 각 상자는 체력이 있고 몬스터에게 공격받을 시 체력이 감소합니다.

체력이 0이 될 경우 해당 상자는 파괴되며, 파괴된 상자 위에 존재하는 상자들이 자동으로 아래로 이동하여 빈 자리를 채웁니다.

+ **체력바 기능**
  
TDS를 본따 처음에는 체력바가 보이지 않지만, 공격으로 인해 체력의 변동이 생길 경우 체력바가 나타나게 되며, TDS와 마찬가지로 딜레이된 체력 감소(영상 속 흰색) 기능을 구현하였습니다.

TDS에서 체력바가 다른 몬스터에 의해 가려지므로, UGUI가 아닌 SpriteRenderer를 이용하여 체력바를 구현하였습니다.

+ **3 Lane 몬스터**
  
TDS에서는 한줄로 몬스터가 나오는 것이 아닌 3줄로 몬스터가 나옵니다. 이를 구현하기 위해 각 Lane과 Monster를 3개의 Layer로 분리하고,
Monster Spawner에서 설정된 Layer로 Monster의 Layer로 변환시켜 3줄로 몬스터가 이동하도록 구현하였습니다.

+ **반응형 UI**
  
개인 프레임워크인 UIAdjuster를 사용하여 화면의 크기 상관 없이 3040 x 1440의 화면이 보이도록 하였습니다.

Editor에서 Free Aspect로 변경 후 화면 크기를 변경하여도 3040 x 1440의 화면비가 유지되며, 빈 공간은 검은 Letter box로 채워지게 됩니다.


사용한 라이브러리
---

+ [UniTask](https://github.com/Cysharp/UniTask)

몬스터의 점프, 공격, 영웅의 공격 등 쿨타임 기능이 필요한 곳에 사용하였습니다.

+ [DOTween](https://dotween.demigiant.com/)

몬스터의 뒤로 밀리는 이동 기능, 하단의 박스가 부서져 상단의 박스가 아래로 내려오는 기능 등 자연스러운 움직임이 필요한 곳에 사용하였습니다.
