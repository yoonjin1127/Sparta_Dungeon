# Sparta_Dungeon

필수 기능부터 도전 기능까지 구현한 Text RPG입니다.

🌊흐름
 

Main()

=> 세이브 파일이 존재한다면 로드, 없다면 새로 생성

=> InitializeStore()에서 상점 아이템 초기화

=> CreatePlayer()로 플레이어 생성

=> EnterVillage()로 마을에 들어가 다양한 선택지를 제공

 

[핵심 기능]

플레이어 이름, 직업 선택 및 생성, 상태 보기
인벤토리 기능 - 아이템 사용 / 장착 / 해제
상점 기능 - 아이템 구매 / 아이템 판매 / 소지금 체크 / 품절 체크
저장 기능 - 데이터를 json파일로 저장 및 불러오기
던전 입장 기능 - 전투 판정 / 골드 획득 / 경험치 획득 및 레벨업 / 간단한 게임오버
 

[주요 클래스]

-Player

플레이어의 상태 정보 및 장비 장착 상태를 저장한다.
static으로 선언한 inventory 변수도 이 클래스에 넣을 수 있었지만, 더 간편히 코드를 작성하기 위해 생략했다.
 
-Inventory

플레이어의 아이템 목록을 관리한다.
AddItem(Item item)은 아이템을 획득하는 메서드이다.
ShowInventory(Player player)은 인벤토리를 보여주고, 아이템을 사용하는 기능을 처리한다.

-Item

추상 클래스이다. 포션, 장비 등의 모든 아이템의 공통 속성이 담겨 있다.
공통 속성으로는 Name, Price 등이 있다.

-Dungeon

던전 난이도 및 추천 방어력, 기본 보상 골드를 저장한다.
EnterDungeon(Player player)은 던전에 입장하는 메서드이다.
DungeonClear(Player player)은 던전을 클리어하는 메서드이다.
DungeonFailed(Player player)은 던전에 실패하는 메서드이다.
 

[파생 클래스]

-Potion

인터페이스 IUseable을 상속했다.
체력을 회복할 수 있는 아이템이다.
Use(Player player)은 체력을 회복하는 메서드이다.
 
-Equipment

인터페이스 IEquipable을 상속했다.
장착하거나 해제할 수 있는 장비이다.
EquipmentType Type은 무기와 방어구를 구분하는 enum형식이다.
Equip(Player player)은 장착과 해제를 담당하는 메서드이다.
 

[인터페이스]

-IUsable

Use() 메서드를 가진다.
사용할 수 있는 아이템에 상속시킨다.
 
-IEquipable

Equip() 메서드를 가진다.
장착할 수 있는 아이템에 상속시킨다.


[Enum]

-JobType

직업 유형이다. 이에 따라 초기 능력치를 다르게 설정했다.

-EquipmentType

장비 유형이다. 무기와 방어구로 나뉜다.

-DungeonType

던전 유형이다. 난이도에 따라 나뉜다.
