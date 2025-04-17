using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace TextRPG
{
   
    internal class Program
    {
        static Inventory inventory = new Inventory();
        static List<Item> storeItems = new List<Item>();

        static void Main(string[] args)
        {
            InitializeStore();
            Player player = CreatePlayer();
            EnterVillage(player);
        }

        // 게임 오버
        static void GameOver(Player player)
        {
            Console.WriteLine("체력이 0이 되었습니다...");
            Console.WriteLine("<게임 오버>");
            Console.WriteLine();
            Console.WriteLine("Enter를 누르면 마을로 돌아갑니다.");
            Console.ReadLine();
            EnterVillage(player);

        }

        // 상점 초기화
        static void InitializeStore()
        {
            storeItems.Add(new Potion("작은 체력 포션", 50, 30));
            storeItems.Add(new Potion("중간 체력 포션", 100, 60));
            storeItems.Add(new Equipment("다이아 검", 200, 10, 0, EquipmentType.Weapon));
            storeItems.Add(new Equipment("다이아 방패", 150, 0, 10, EquipmentType.Armor));
            storeItems.Add(new Equipment("절대 못 사는 비싼 물건", 1000, 0, 10, EquipmentType.Armor));
        }


        // 여러 선택이 가능한 마을로 진입
        static void EnterVillage(Player player)
        {
            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전입장");
            Console.WriteLine("5. 휴식하기");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.Write(">> ");

            // 올바른 입력인지 확인
            while (true)
            {
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        ShowStatus(player);
                        break;
                    case "2":
                        inventory.ShowInventory(player);
                        break;
                    case "3":
                        ShowStore(player);
                        break;
                    case "4":
                        EnterDungeonGate(player);
                        break;
                    case "5":
                         Rest(player);
                        break;
                    default:
                        Console.WriteLine("잘못된 입력입니다.");
                        continue;
                }
                break;
            }
        }

        // 직업 유형
        enum JobType
        {
            None = 0,
            전사 = 1,
            궁수 = 2,
            마법사 = 3
        }

        // 장비 유형
        enum EquipmentType
        {
            Weapon,
            Armor
        }

        // 던전 유형
        enum DungeonType
        {
            쉬운,
            일반,
            어려운
        }

        // 플레이어 생성
        static Player CreatePlayer()
        {
            Console.WriteLine("캐릭터의 이름을 입력해주세요.");
            while(true)
            {
                Console.Write(">> ");
                string input = Console.ReadLine();
                if(String.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("캐릭터의 이름은 한 글자 이상이어야 합니다.");
                }
                else
                {
                    Console.Clear();
                    Player player = new Player(input, ChooseJob());
                    return player;
                }
            }
            
        }

        // JobType을 반환하는 직업선택 함수
        static JobType ChooseJob()
        {
            Console.WriteLine("캐릭터의 직업을 선택하세요.");
            Console.WriteLine("[1] 전사");
            Console.WriteLine("[2] 궁수");
            Console.WriteLine("[3] 마법사");

            JobType choiceJob = JobType.None;

            // 올바른 입력인지 확인
            while (true)
            {
                Console.Write(">> ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        choiceJob = JobType.전사;
                        break;
                    case "2":
                        choiceJob = JobType.궁수;
                        break;
                    case "3":
                        choiceJob = JobType.마법사;
                        break;
                    default:
                        Console.WriteLine("잘못된 입력입니다.");
                        continue;
                }
                break;
            }
            Console.Clear();
            return choiceJob;
        }

        // 상태 보기
        static void ShowStatus(Player player)
        {
            Console.Clear();
            Console.WriteLine("현재 플레이어의 정보입니다.");
            Console.WriteLine();
            Console.WriteLine($"Lv. {player.level}");
            Console.WriteLine($"{player.name} ({player.job})");
            Console.WriteLine($"공격력          : {player.attack}");
            Console.WriteLine($"방어력          : {player.defense}");
            Console.WriteLine($"체력            : {player.health}");
            Console.WriteLine($"Gold            : {player.gold}");
            Console.WriteLine($"장착한 무기     : {(player.weapon != null ? player.weapon.Name : "없음")}");
            Console.WriteLine($"장착한 방어구   : {(player.armor != null ? player.armor.Name : "없음")}");
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            while(true)
            {
                string input = Console.ReadLine();
                if (input == "0")
                {
                    Console.Clear();
                    EnterVillage(player);
                    break;
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    continue;
                }
            }
        }

        // 상점
        static void ShowStore(Player player)
        {
            Console.Clear();
            Console.WriteLine("상점에 도착했습니다. 어서오세요!");
            Console.WriteLine($"[보유 골드] {player.gold}");
            Console.WriteLine();
            for (int i = 0; i < storeItems.Count; i++)
            {
                Console.WriteLine(storeItems[i].Name);
            }
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("1. 아이템 구매");
                Console.WriteLine("2. 아이템 판매");
                Console.WriteLine("3. 마을로 돌아가기");

                Console.Write(">> ");
                string input = Console.ReadLine();

                switch (input)
                {
                    // 아이템 구매
                    case "1":
                        Console.Clear();
                        Console.WriteLine("구매할 물건을 선택해주세요.");
                        Console.WriteLine();

                        // 상점 물품 출력
                        for (int i = 0; i < storeItems.Count; i++)
                        {
                            bool isSold = inventory.HasItem(storeItems[i]);
                            // 만약 팔렸다면 판매완료, 팔리지 않았다면 가격을 띄우게끔 한다.
                            string soldStatus = isSold ? "[판매완료]" : $"{storeItems[i].Price} 골드";

                            // 공격력, 방어력, 회복량 등의 아이템 효과
                            string description = "";

                            // 장비면 공격력, 방어력 표시
                            if (storeItems[i] is Equipment)
                            {
                                Equipment equip = storeItems[i] as Equipment;
                                if(equip.Type == EquipmentType.Weapon)
                                {
                                    description = "공격력 +" + equip.AttackAmount.ToString();
                                }
                                else
                                {
                                    description = "방어력 +" + equip.DefenseAmount.ToString();
                                }
                            }
                            // 포션이면 회복량 표시
                            else
                            {
                                Potion potion = storeItems[i] as Potion;
                                description = "체력 " + potion.HealAmount.ToString();
                            }
                            Console.WriteLine($"({i}). {storeItems[i].Name} | {description} | {soldStatus}");
                        }

                        Console.Write(">>");
                        string selectItem = Console.ReadLine();

                        if (int.TryParse(selectItem, out int selectNum) == false ||
                            selectNum < 0 || selectNum > storeItems.Count)
                        {
                            Console.Clear();
                            Console.WriteLine("잘못된 입력입니다.");
                            break;
                        }
                        else
                        {
                            Console.Clear();
                            // Item 형식의 selectedItem에 선택한 아이템을 담는다.
                            Item selectedItem = storeItems[selectNum];

                            if (inventory.HasItem(selectedItem))
                            {
                                Console.WriteLine("이미 구매한 아이템입니다.");
                                break;
                            }

                            else if (player.gold >= selectedItem.Price)
                            {
                                player.gold -= selectedItem.Price;
                                inventory.AddItem(selectedItem);
                                Console.WriteLine("구매가 완료되었습니다! 감사합니다.");
                                break;
                            }
                            else
                            {
                                Console.WriteLine("골드가 부족합니다...");
                                break;
                            }
                        }
                    // 아이템 판매
                    case "2":
                        Console.Clear();
                        Console.WriteLine("판매할 아이템을 선택해주세요.");
                        Console.WriteLine();

                        // 플레이어가 가지고 있는 아이템 출력
                        List<Item> items = inventory.GetItems();

                        if(items.Count == 0)
                        {
                            Console.WriteLine("가지고 있는 아이템이 없습니다.");
                            continue;
                        }
                        for(int i = 0; i< items.Count; i++)
                        {
                            Console.WriteLine($"{i}. {items[i].Name} | {(int)(items[i].Price * 0.85f)}G");
                        }

                        Console.Write(">> ");
                        string input2 = Console.ReadLine();

                        // 오입력 방지
                        if (int.TryParse(input2, out int sellNum) == false ||
                            sellNum < 0 || sellNum >= items.Count)
                        {
                            Console.WriteLine("잘못된 입력입니다.");
                            continue;
                        }

                        if (items[sellNum] is Equipment playerEquip)
                        {
                            if(player.weapon == playerEquip || player.armor == playerEquip)
                            {
                                // 장착하고 있는 장비라면 해제
                                playerEquip.Equip(player);
                            }
                        }
                        Console.WriteLine($"{items[sellNum].Name}을 판매했습니다! {(int)(items[sellNum].Price * 0.85f)}G를 획득했습니다.");
                        Console.WriteLine();
                        player.gold += (int)(items[sellNum].Price * 0.85f);
                        inventory.RemoveItem(items[sellNum]);
                        break;

                    case "3":
                        Console.Clear();
                        EnterVillage(player);
                        break;

                    default:
                        Console.Clear();
                        Console.WriteLine("잘못된 입력입니다.");
                        continue;
                }
            }
        }

        // 휴식
        static void Rest(Player player)
        {
            Console.Clear();
            Console.WriteLine("휴식을 선택하셨습니다.");
            Console.WriteLine("500G를 지불하면 체력을 회복할 수 있습니다.");
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("1. 휴식하기");
                Console.WriteLine("2. 마을로 돌아가기");
                Console.WriteLine();

                string input = Console.ReadLine();
                if (input == "1")
                {
                    if (player.gold >= 500)
                    {
                        Console.Clear();
                        player.health = 100;
                        player.gold -= 500;
                        Console.WriteLine("체력을 100까지 회복했습니다!");
                        continue;

                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("골드가 부족합니다.");
                        continue;
                    }
                }
                else if (input == "2")
                {
                    Console.Clear();
                    EnterVillage(player);
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("잘못된 입력입니다.");
                    continue;
                }
            }

            
        }

        // 던전 입구 입장
        static void EnterDungeonGate(Player player)
        {
            // 던전 배열
            Dungeon[] dungeons = new Dungeon[]
            {
            new Dungeon(DungeonType.쉬운, 5, 1000),
            new Dungeon(DungeonType.일반, 11, 1700),
            new Dungeon(DungeonType.어려운, 17, 2500)
            };

            while (true)
            {
                Console.Clear();
                Console.WriteLine("던전입장");
                Console.WriteLine("이 곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.");
                Console.WriteLine();
                Console.WriteLine("1. 쉬운 던전     | 방어력 5 이상 권장");
                Console.WriteLine("2. 일반 던전     | 방어력 11 이상 권장");
                Console.WriteLine("3. 어려운 던전   | 방어력 17 이상 권장");
                Console.WriteLine("0. 마을로 돌아가기");
                Console.WriteLine();
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">> ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        dungeons[0].EnterDungeon(player);
                        Console.WriteLine();
                        Console.WriteLine("Enter를 눌러주세요.");
                        Console.ReadLine();
                        break;
                    case "2":
                        dungeons[1].EnterDungeon(player);
                        Console.WriteLine();
                        Console.WriteLine("Enter를 눌러주세요.");
                        Console.ReadLine();
                        break;
                    case "3":
                        dungeons[2].EnterDungeon(player);
                        Console.WriteLine();
                        Console.WriteLine("Enter를 눌러주세요.");
                        Console.ReadLine();
                        break;
                    case "0":
                        Console.Clear();
                        EnterVillage(player);
                        break;
                    default:
                        Console.WriteLine();
                        Console.WriteLine("잘못된 입력입니다.");
                        continue;
                }
            }

        }

        // 던전 클래스
        class Dungeon
        {
            // 던전 난이도, 추천 방어력, 기본 보상
            DungeonType Type { get; }
            public float RecDefense { get; }
            public int Reward { get;  }

            // 초기값 생성을 위한 생성자
            public Dungeon(DungeonType type, float recDefense, int reward)
            {
                Type = type;
                RecDefense = recDefense;
                Reward = reward;
            }

            // 던전 입장
            public void EnterDungeon(Player player)
            {
                Console.Clear();
                Console.WriteLine($"{Type} 던전에 입장합니다.");

                if(player.defense < RecDefense)
                {
                    DungeonFailed(player);
                }
                else
                {
                    DungeonClear(player);
                }
            }

            // 던전 클리어
            void DungeonClear(Player player)
            {
                // 플레이어 이전 체력과 소지금 저장
                float preHealth = player.health;
                float preGold = player.gold;

                // 플레이어 방어력과 권장 방어력에 따른 보정값
                float correction = RecDefense - player.defense;
                Random random = new Random();
                float damage = random.Next(20, 36);
                float adjustedDamage = damage + correction;

                // 0보다 작아지지 않게 조정
                adjustedDamage = Math.Max(0, adjustedDamage);
                player.health -= adjustedDamage;

                // 게임 오버 체크
                if (player.health <= 0)
                {
                    player.health = 1;
                    GameOver(player);
                }

                // 보상
                Random random2 = new Random();
                float attackBonus = random2.Next((int)player.attack, (int)player.attack * 2 +1);
                float bonusPercent = attackBonus / 100;

                int reward = Reward + (int)(Reward * bonusPercent);
                player.gold += reward;


                Console.WriteLine("축하합니다!");
                Console.WriteLine($"{Type}던전을 클리어했습니다.");
                Console.WriteLine();
                Console.WriteLine("[탐험 결과]");
                Console.WriteLine($"체력 {preHealth} - > {player.health}");
                Console.WriteLine($"Gold {preGold} - > {player.gold}");


                player.ExpUp();
            }

            // 던전 실패
            void DungeonFailed(Player player)
            {
                Random random = new Random();

                if(random.Next(0,100) < 40)
                {
                    player.health /= 2;
                    Console.WriteLine("던전 클리어에 실패했습니다...");

                    if (player.health <= 0)
                    {
                        player.health = 1;
                        GameOver(player);
                    }

                    Console.WriteLine($"체력이 {player.health}로 감소했습니다.");
                }
                else
                {
                    DungeonClear(player);
                }
            }
        }




        

        // 플레이어 상태 및 능력치, 장비 클래스
        class Player
        {
            public string name;
            public int level;
            public int exp;
            int clearCount = 0;
            public JobType job;
            public float attack;
            public float defense;
            public float health;
            public int gold;
            public Equipment weapon;
            public Equipment armor;

            // 초기 이름, 직업 설정을 위한 생성자
            public Player(string name, JobType job)
            {
                this.name = name;
                this.level = 1;
                this.job = job;
                this.gold = 500;
                this.exp = 0;

                // 직업별 능력치
                switch(job)
                {
                    case JobType.전사:
                        attack = 10;
                        defense = 15;
                        health = 150;
                        break;
                    case JobType.궁수:
                        attack = 20;
                        defense = 10;
                        health = 100;
                        break;
                    case JobType.마법사:
                        attack = 30;
                        defense = 10;
                        health = 80;
                        break;
                    default:
                        attack = defense = health = gold = 0;
                        break;

                }

            }

           // 레벨업 기능
           public void LevelUp()
            {
                this.level++;
                this.attack += 0.5f;
                Console.WriteLine($"레벨업 했습니다! 레벨 {level}이 되었습니다.");
            }

           public  void ExpUp()
            {
                clearCount++;

                if (this.level == clearCount)
                {
                    LevelUp();
                    clearCount = 0;
                }
            }
            
        }

        // 인벤토리 클래스 리스트 구현
        class Inventory
        {
            // 플레이어가 지닌 아이템 리스트
            List<Item> playerItems = new List<Item>();

            public void AddItem(Item item)
            {
                playerItems.Add(item);
                Console.WriteLine($"{item.Name}을 획득했습니다.");
            }

            // 플레이어가 지닌 아이템 리스트 출력
            public List<Item> GetItems()
            {
                return playerItems;
            }

            // 플레이어가 지닌 아이템 삭제
            public void RemoveItem(Item item)
            {
                playerItems.Remove(item);
            }

            // 플레이어가 아이템을 지녔는지 판단하는 함수
            public bool HasItem(Item item)
            {
                return playerItems.Contains(item);
            }

            // 인벤토리 여는 함수
            public void ShowInventory(Player player)
            {
                Console.Clear();
                Console.WriteLine("인벤토리");
                Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
                Console.WriteLine();
                Console.WriteLine("[아이템 목록]");
                Console.WriteLine("=============");

                if(playerItems.Count == 0)
                {
                    Console.WriteLine("현재 가지고 있는 아이템이 없습니다!");
                    Console.WriteLine("=============");
                    Console.WriteLine();
                    Console.WriteLine("마을로 돌아가려면 enter를 입력하세요.");
                    Console.ReadLine();
                    Console.Clear();
                    EnterVillage(player);
                }
                else
                {
                    while (true)
                    {
                        for (int i = 0; i < playerItems.Count; i++)
                        {
                            Console.WriteLine($"<{playerItems[i].Name}> ");
                        }
                        Console.WriteLine("=============");
                        Console.WriteLine();
                        Console.WriteLine("1. 아이템 사용하기");
                        Console.WriteLine("2. 마을로 돌아가기");

                        Console.Write(">> ");
                        string input = Console.ReadLine();

                        Console.Clear();

                        switch (input)
                        {
                            case "1":
                                Console.WriteLine("사용할 아이템을 선택하세요");
                                for (int i = 0; i < playerItems.Count; i++)
                                {
                                    string equipStatus = "";
                                    
                                    // 지금 착용하고 있는 장비 확인
                                    if (playerItems[i] == player.weapon || playerItems[i] == player.armor)
                                    {
                                        equipStatus = "[E]";
                                    }

                                    Console.WriteLine($"({i}). {equipStatus}{playerItems[i].Name}");
                                }
                                Console.Write(">> ");
                                string selectItem = Console.ReadLine();

                                if(int.TryParse(selectItem, out int selectNum) == false ||
                                    selectNum < 0 || selectNum >= playerItems.Count)
                                {
                                    Console.Clear() ;
                                    Console.WriteLine("잘못된 입력입니다.");
                                    break;
                                }

                                Item item = playerItems[selectNum];

                                if (item is IUseable useable)
                                {
                                    // 아이템을 사용하고 삭제
                                    useable.Use(player);
                                    playerItems.RemoveAt(selectNum);
                                    continue;
                                }
                                else if (item is IEquipable equipable)
                                {
                                    // 장비를 장착 혹은 해제
                                    equipable.Equip(player);
                                }
                                break;
                            case "2":
                                Console.Clear();
                                EnterVillage(player);
                                break;
                            default:
                                Console.WriteLine("잘못된 입력입니다.");
                                continue;
                        }

                    }

                }
                //Console.WriteLine("=============");
            }

        }

        // 아이템 추상 클래스 
        abstract class Item
        {
            // 이름과 가격은 공통으로 가져야 한다
            public string Name { get; set; }
            public int Price { get; set; }
            
            public Item(string name, int price)
            {
                Name = name;
                Price = price;
            }
        }

        // 포션 클래스
        class Potion : Item, IUseable
        {
            public int HealAmount { get; set; }
            public Potion(string name, int price, int healAmount) : base(name, price)
            {
                HealAmount = healAmount;
            }

            public void Use(Player player)
            {
                Console.Clear();
                // 플레이어 health 회복 구현
                player.health += HealAmount;
                Console.WriteLine($"{Name}을 사용했다. 체력이 {HealAmount}만큼 회복됐다!");
            }
        }

        // 무기 클래스
        class Equipment : Item, IEquipable
        {
            public EquipmentType Type { get; set; }
            public int AttackAmount { get; set; }
            public int DefenseAmount { get; set; }
            public Equipment(string name, int price, int attackAmount, int defenseAmount, EquipmentType type) : base(name, price)
            {
                AttackAmount = attackAmount;
                DefenseAmount = defenseAmount;
                Type = type;
            }

            public void Equip(Player player)
            {
                Console.Clear();
                if(Type == EquipmentType.Weapon)
                {
                    // 이미 이 무기를 장착하고 있을 때
                    if(player.weapon == this)
                    {
                        player.attack -= AttackAmount;
                        player.weapon = null;
                        Console.WriteLine($"{Name}을 해제했습니다.");
                    }
                    // 다른 무기를 장착하고 있을 때
                    else if (player.weapon != null)
                    {
                        player.attack -= player.weapon.AttackAmount;
                        Console.WriteLine($"{player.weapon.Name}을 해제했습니다.");
                    }
                    // 새로 장착
                    player.attack += AttackAmount;
                    player.weapon = this;
                    Console.WriteLine($"{Name}을(를) 장착했습니다!");
                }
                else if (Type == EquipmentType.Armor)
                {
                    // 이미 이 방어구를 장착하고 있을 때
                    if (player.armor == this)
                    {
                        player.defense -= DefenseAmount;
                        player.armor = null;
                        Console.WriteLine($"{Name}을 해제했습니다.");
                    }
                    // 다른 방어구를 장착하고 있을 때
                    else if (player.armor != null)
                    {
                        player.defense -= player.armor.DefenseAmount;
                        Console.WriteLine($"{player.armor.Name}을 해제했습니다.");
                    }
                    // 새로 장착
                    player.defense += DefenseAmount;
                    player.armor = this;
                    Console.WriteLine($"{Name}을(를) 장착했습니다!");
                }
            }
        }


        // 사용 가능한 아이템 인터페이스
        interface IUseable
        {
            // 사용 메서드
            void Use(Player player);
        }


        // 장착 가능한 장비 인터페이스
        interface IEquipable
        {
            // 장착 메서드
            void Equip(Player player);
        }
    }
}
