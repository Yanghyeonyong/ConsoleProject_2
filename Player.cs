using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    enum Direction
    { left, right, up, down }

    struct MyPos
    {
        public int x;
        public int y;
    }

    // 캐릭터 스테이터스, hp, mp, 방어력, 공격력, 치명타 확률, 치명타 데미지
    struct Status
    {
        public int hp;
        //public int mp;
        public int defence;
        public int attack;
    }

    //플레이어가 착용한 아이템
    struct Equipment
    {
        public List<PlayerItem> myItem;
        public Potion myPotion;
    }


    internal class Player
    {
        string name;
        Direction dir;

        static string[] playerImage;
        static string[] eraserplayerImage;

        bool onAdventure;
        bool onVillage;
        //더블점프 포기하고 그냥 점프중엔 다시 점프 조작 안되게 만들기 위함
        bool onJump;

        //전방을 바라보고 있는지 확인
        bool onFront;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        //해당 좌표는 플레이어의 좌측 상단 좌표이다
        public MyPos pos;


        //캐릭터 자체의 스탯
        public Status myBaseStatus;

        //캐릭터 자체의 스탯 + 아이템 스탯<-향후 아이템 추가 후 메서드 추가
        public Status myTotalStatus;

        //인벤토리
        public Inventory inventory;

        //착용 장비
        Equipment equipment;
        const int MaxEquipment = 5;

        //현재 체력, 위에는 맥스 체력임
        int currentHp;
        //경험치 및 레벨
        int level;
        int maxExp;
        public int currentExp;
        int statPoint;

        int currentStage;

        bool onStart = false;
        public Player(bool test)
        {
        }

        public Player()
        {
            playerImage = new string[2];
            playerImage[0] = " ∧ ∧";
            playerImage[1] = "( '▽' )";//<-이거 특수문자 때문에 크기 8 먹는다 [2]*8 배열인듯
            Name = "Nothing";
            eraserplayerImage = new string[2];
            eraserplayerImage[0] = "        ";
            eraserplayerImage[1] = "        ";
            pos.x = 100;
            pos.y = 30;

        }



        #region 조작
        public void ControlPlayer()
        {
            BackgroundWorker control = new BackgroundWorker();
            ConsoleKeyInfo key;
            control.DoWork += (sender, e) =>
            {
                Stopwatch gravity= new Stopwatch();
                Stopwatch attackDelay= new Stopwatch();

                gravity.Start();
                attackDelay.Start();

                List<int> attackPosX;
                List<int> attackPosY;
                int attackX;
                int attackY;
                attackPosX = new List<int>();
                attackPosY = new List<int>();
                bool onAttack=false;
                while (true)
                {
                    if (Console.KeyAvailable)
                    {
                        if (onVillage || onAdventure)
                        {
                            key = Console.ReadKey(true);
                            switch (key.Key)
                            {
                                case ConsoleKey.UpArrow:
                                    if (!onAdventure && onVillage)
                                    {
                                        MoveUp();
                                    }
                                    else if (onAdventure && !onVillage)
                                    {
                                        //Console.WriteLine("점프!!");
                                        MoveUp();
                                        MoveUp();
                                        MoveUp();
                                        MoveUp();
                                        MoveUp();
                                        MoveUp();
                                        MoveUp();
                                        MoveUp();
                                        MoveUp();
                                        MoveUp();
                                        //Console.WriteLine("점프 성공");
                                    }
                                    break;
                                case ConsoleKey.DownArrow:
                                    MoveDown();
                                    break;
                                case ConsoleKey.LeftArrow:
                                    onFront = false;
                                    MoveLeft();
                                    break;
                                case ConsoleKey.RightArrow:
                                    onFront = true;
                                    MoveRight();
                                    break;
                                case ConsoleKey.A:
                                    if (attackDelay.ElapsedMilliseconds >= 1500)
                                    {
                                        if (!onAttack)
                                        {
                                            onAttack = true;
                                        }

                                        if (onFront)
                                        {
                                            Attack(3, 0, 0, 0, attackPosX, attackPosY);

                                        }
                                        else
                                        {
                                            Attack(0, 3, 0, 0, attackPosX, attackPosY);
                                        }
                                        attackDelay.Restart();
                                    }
                                    
                                    break;
                                case ConsoleKey.S:
                                    currentHp += equipment.myPotion.healingHp;
                                    if (currentHp > myTotalStatus.hp)
                                    {
                                        currentHp = myTotalStatus.hp;
                                    }
                                    break;
                                case ConsoleKey.Spacebar:
                                    if (onVillage)
                                    {
                                        if (Map.shopPortal[pos.x, pos.y])
                                        {
                                            onVillage = false;
                                            onAdventure = false;
                                            OnShop();
                                        }
                                        if (Map.adventurePortal_Stage1[pos.x, pos.y]&&currentStage==0)
                                        {
                                            OnAdventure();
                                            //currentStage = 1;
                                            //Console.WriteLine("이동했다ㅏㅏㅏㅏㅏ");
                                            Console.WriteLine("dkkkkkkkkkkkkkkkkkk현재 curentStage = " + currentStage);
                                        }
                                        if (Map.homePortal[pos.x, pos.y])
                                        {
                                            onVillage = false;
                                            onAdventure = false;
                                            ShowMyCharacter();
                                        }
                                    }
                                    if (onAdventure)
                                    {
                                        if (Map.villagePortal[pos.x, pos.y])
                                        {
                                            OnVillage();
                                        }
                                        if (Map.adventurePortal_Stage2[pos.x, pos.y] && currentStage == 1)
                                        {
                                            //currentStage = 2;
                                            OnAdventure2();
                                            //Console.WriteLine("이동했다ㅏㅏㅏㅏㅏ");
                                        }
                                        if (Map.adventurePortal_Stage3[pos.x, pos.y] && currentStage == 2)
                                        {
                                            //currentStage = 3;
                                            OnAdventure3();
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    if (onAdventure && !onVillage)
                    {
                        if (!Map.BaseMap[pos.x, pos.y + playerImage.Length]&&gravity.ElapsedTicks%5000==0)
                        {
                            MoveDown();
                            gravity.Restart();
                            //여기다가 몬스터 일정 시간마다 움직이는 코드 넣어도 될 듯?
                        }
                    }

                    if (attackDelay.ElapsedMilliseconds == 1000&&onAttack)
                    {
                        //Console.WriteLine("지우개 시작ㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱ");
                        for (int i = 0; i < attackPosX.Count; i++)
                        {
                            attackX = attackPosX[i];
                            attackY = attackPosY[i];
                            Console.SetCursorPosition(attackX,attackY);
                            Map.AttackMap[attackX, attackY] =false;
                            Console.Write(Map.adventureMap[attackX,attackY]);
                        }

                        Console.SetCursorPosition(pos.x, pos.y);
                        Console.Write(playerImage[0]);
                        Console.SetCursorPosition(pos.x, pos.y + 1);
                        Console.Write(playerImage[1]);
                        attackPosX.RemoveRange(0, attackPosX.Count-1);
                        attackPosY.RemoveRange(0, attackPosY.Count-1);
                    }
                }
            };
            control.RunWorkerCompleted += (sender, e) =>
            {
                control = null;
            };
            control.RunWorkerAsync();
        }
        public void SetCharacterPos(string[] s)
        {
            Map.SetPlayerMap(pos.x, pos.y);
            Console.SetCursorPosition(pos.x, pos.y);
            Console.Write(s[0]);
            Console.SetCursorPosition(pos.x, pos.y + 1);
            Console.Write(s[1]);
        }

        public void Move(Direction dir)
        {
            Map.SetPlayerMap(pos.x,pos.y);
            if (onVillage)
            {
                Console.SetCursorPosition(pos.x, pos.y);
                for (int i = 0; i < 8; i++)
                {
                    Console.Write(Map.villageMap[pos.x + i, pos.y]);
                }
                Console.SetCursorPosition(pos.x, pos.y+1);
                for (int i = 0; i < 8; i++)
                {
                    Console.Write(Map.villageMap[pos.x + i, pos.y+1]);
                }
            }
            else if (onAdventure)
            {
                Console.SetCursorPosition(pos.x, pos.y);
                for (int i = 0; i < 8; i++)
                {
                    Console.Write(Map.adventureMap[pos.x + i, pos.y]);
                }
                Console.SetCursorPosition(pos.x, pos.y+1);
                for (int i = 0; i < 8; i++)
                {
                    Console.Write(Map.adventureMap[pos.x + i, pos.y+1]);
                }
                if (currentStage == 1)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Console.SetCursorPosition(Map.BaseMap.GetLength(0) - 30, 9 + i);
                        Console.Write(Map.NextStage[i]);
                    }
                }
                if (currentStage == 2)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Console.SetCursorPosition(170, 54 + i);
                        Console.Write(Map.NextStage[i]);
                    }
                }
            }

            switch (dir)
            {
                case Direction.left:
                    pos.x--;
                    break;
                case Direction.right:
                    pos.x++;
                    break;
                case Direction.up:
                    pos.y--;
                    break;
                case Direction.down:
                    pos.y++;
                    break;
            }

            SetCharacterPos(playerImage);
        }
        public void MoveLeft()
        {
            //특수문자라 -2다 일반이면 -1로 바꿔야 한다
            if (!Map.BaseMap[pos.x - 2, pos.y] 
                && !Map.MonsterMap[Math.Max(0,pos.x - 8), pos.y+1] 
                && !Map.MonsterMap[Math.Max(0, pos.x - 8), pos.y] 
                && !Map.MonsterMap[Math.Max(0, pos.x - 8), pos.y-1])
            {
                Move(Direction.left);
            }
        }
        public void MoveRight()
        {
            if (!Map.BaseMap[pos.x + playerImage[1].Length + 1, pos.y] 
                && !Map.MonsterMap[Math.Max(0, pos.x) + playerImage[1].Length+1, pos.y + 1] 
                && !Map.MonsterMap[Math.Max(0, pos.x) + playerImage[1].Length + 1, pos.y] 
                && !Map.MonsterMap[Math.Max(0, pos.x) + playerImage[1].Length + 1, pos.y - 1])
            {
                Move(Direction.right);
            }
        }
        public void MoveUp()
        {
            if (!Map.BaseMap[pos.x, pos.y - playerImage.Length + 1]
                && !Map.MonsterMap[Math.Max(0, pos.x-7), pos.y - playerImage.Length + 1]
                && !Map.MonsterMap[Math.Max(0, pos.x-6), pos.y - playerImage.Length + 1]
                && !Map.MonsterMap[Math.Max(0, pos.x-5), pos.y - playerImage.Length + 1]
                && !Map.MonsterMap[Math.Max(0, pos.x-4), pos.y - playerImage.Length + 1]
                && !Map.MonsterMap[Math.Max(0, pos.x-3), pos.y - playerImage.Length + 1]
                && !Map.MonsterMap[Math.Max(0, pos.x-2), pos.y - playerImage.Length + 1]
                && !Map.MonsterMap[Math.Max(0, pos.x-1), pos.y - playerImage.Length + 1]
                && !Map.MonsterMap[Math.Max(0, pos.x), pos.y - playerImage.Length + 1]
                && !Map.MonsterMap[Math.Max(0, pos.x+1), pos.y - playerImage.Length + 1]
                && !Map.MonsterMap[Math.Max(0, pos.x+2), pos.y - playerImage.Length + 1]
                && !Map.MonsterMap[Math.Max(0, pos.x+3), pos.y - playerImage.Length + 1]
                && !Map.MonsterMap[Math.Max(0, pos.x+4), pos.y - playerImage.Length + 1]
                && !Map.MonsterMap[Math.Max(0, pos.x+5), pos.y - playerImage.Length + 1]
                && !Map.MonsterMap[Math.Max(0, pos.x+6), pos.y - playerImage.Length + 1]
                && !Map.MonsterMap[Math.Max(0, pos.x+7), pos.y - playerImage.Length + 1])
            {
                Move(Direction.up);
            }

        }
        public void MoveDown()
        {
            if (!Map.BaseMap[pos.x, pos.y + playerImage.Length]
                && !Map.MonsterMap[Math.Max(0, pos.x - 7), pos.y + playerImage.Length]
                && !Map.MonsterMap[Math.Max(0, pos.x - 6), pos.y + playerImage.Length]
                && !Map.MonsterMap[Math.Max(0, pos.x - 5), pos.y + playerImage.Length]
                && !Map.MonsterMap[Math.Max(0, pos.x - 4), pos.y + playerImage.Length]
                && !Map.MonsterMap[Math.Max(0, pos.x - 3), pos.y + playerImage.Length]
                && !Map.MonsterMap[Math.Max(0, pos.x - 2), pos.y + playerImage.Length]
                && !Map.MonsterMap[Math.Max(0, pos.x - 1), pos.y + playerImage.Length]
                && !Map.MonsterMap[Math.Max(0, pos.x), pos.y + playerImage.Length]
                && !Map.MonsterMap[Math.Max(0, pos.x + 1), pos.y + playerImage.Length]
                && !Map.MonsterMap[Math.Max(0, pos.x + 2), pos.y + playerImage.Length]
                && !Map.MonsterMap[Math.Max(0, pos.x + 3), pos.y + playerImage.Length]
                && !Map.MonsterMap[Math.Max(0, pos.x + 4), pos.y + playerImage.Length]
                && !Map.MonsterMap[Math.Max(0, pos.x + 5), pos.y + playerImage.Length]
                && !Map.MonsterMap[Math.Max(0, pos.x + 6), pos.y + playerImage.Length]
                && !Map.MonsterMap[Math.Max(0, pos.x + 7), pos.y + playerImage.Length]
                )
            {
                Move(Direction.down);
            }

        }
        public void Attack(int forwardRangeX, int rearRangeX, int topRangeY, int bottomRangeY, List<int> attackX, List<int> attackY)
        {
            int forward = 0;
            bool check = false;
            //전방 공격
            for (int i = 1; i <= forwardRangeX; i++)
            {
                forward = pos.x + playerImage[1].Length + i;
                if (forward >= 198)
                {
                    break;
                }

                for (int j = 0; j < playerImage.Length; j++)
                {
                    Map.AttackMap[forward, pos.y + j] = true;
                    if ((Map.AttackMap[forward, pos.y + j] && Map.MonsterMap[forward, pos.y + j]))
                    {
                        GameSystem.AttackMonster(forward, pos.y + j);
                        check=true;
                        break;
                    }
                    Console.SetCursorPosition(forward, pos.y + j);
                    Console.Write("c");
                    attackX.Add(forward);
                    attackY.Add(pos.y + j);

                }
                if (check)
                {
                    break;
                }    
            }

            check = false;

            int rear = 0;
            //후방 공격
            for (int i = 1; i <= rearRangeX; i++)
            {
                rear = pos.x - i;
                if (rear <= 2)
                {
                    break;
                }

                for (int j = 0; j < playerImage.Length; j++)
                {
                    Map.AttackMap[rear, pos.y + j] = true;

                    if ((Map.AttackMap[rear, pos.y + j] && Map.MonsterMap[rear-7, pos.y + j]))
                    {
                        GameSystem.AttackMonster(rear - 7, pos.y + j);
                        check = true;
                        break;
                    }
                    Console.SetCursorPosition(rear, pos.y + j);
                    Console.Write("c");

                    attackX.Add(rear);
                    attackY.Add(pos.y + j);
                }
                if (check)
                {
                    break;
                }
            }
        }

        #endregion

        #region 캐릭터 초기 설정
        static Status InitBaseStatus()
        {
            Status baseStatus;
            baseStatus.defence = 0;
            baseStatus.attack = 1;
            baseStatus.hp = 10;

            return baseStatus;
        }

        public void InitBaseCharacter()
        {
            name = "모험가";

            //초기 스탯 설정
            myBaseStatus = InitBaseStatus();
            myTotalStatus = InitBaseStatus();

            //초기 인벤토리와 장비 설정
            inventory = new Inventory();

            InitBaseEquipment();
        } 

        //캐릭터 이름 설정
        public void SetMyName()
        {
            ConsoleKeyInfo key;
            bool setName = true;
            bool retry = false;
            string name;

            while (true)
            {
                setName = true;
                Console.WriteLine("▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷   당 신 은     누 구 신 가 요 ?   ◁\n△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△\n");
                Console.Write("이름 : ");
                name = Console.ReadLine();
                Console.WriteLine($"\n당신의 이름은 [{name}] 맞습니까?\n\n다음으로 넘어가고자 한다면 Enter\n재입력을 원하시면 A 키를 눌러주세요\n");
                while (setName)
                {
                    key = Console.ReadKey(true);

                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:
                            setName = false;
                            retry = false;
                            break;
                        case ConsoleKey.A:
                            Console.Clear();
                            setName = false;
                            retry = true;
                            break;
                    }
                }
                if (!retry)
                {
                    break;
                }
            }
            Name = name;
            Console.Clear();
        }
        //초기 시작시 혹은 레벨업 시(레벨업 시는 마을에서) 스테이터스 설정
        public void SetMyStatusOnStart()
        {
            bool setStatus = true;
            bool retry = false;
            ConsoleKeyInfo key;

            int originalStatPoint = statPoint;
            int originalHp = myBaseStatus.hp;
            int originalAttack = myBaseStatus.attack;
            int originalDefence = myBaseStatus.defence;

            //사용자 입력 기반 스탯 배분(hp는 최소 1 이상 필요)
            //입력 순서 hp mp 공격력, 방어력 크확 ,크뎀
            while (true)
            {
                Console.WriteLine($"▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷ 스탯을 분배해주세요(총 {statPoint} point) ◁\n△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△");

                setStatus = true;
                statPoint = originalStatPoint;
                myBaseStatus.hp = originalHp;
                myBaseStatus.attack = originalAttack;
                myBaseStatus.defence = originalDefence;

                Console.WriteLine($"\n▶ 잔여스탯 {statPoint: 000} point ◀");
                Console.Write("HP(기본 10) : ");
                    myBaseStatus.hp += SetMyStatusDetail(0);
                    Console.WriteLine($"\n▶ 잔여스탯 {statPoint: 000} point ◀");
                Console.Write("공격력(기본 1) : ");
                myBaseStatus.attack += SetMyStatusDetail(0);

                Console.WriteLine($"\n▶ 잔여스탯 {statPoint: 000} point ◀");
                Console.Write("방어력(기본 0) : ");
                myBaseStatus.defence += SetMyStatusDetail(0);

                Console.WriteLine("\n스탯 분배에 만족한다면 Enter\n재설정을 원한다면 A 키를 눌러주세요");
                while (setStatus)
                {
                    key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:
                            setStatus = false;
                            retry = false;
                            break;
                        case ConsoleKey.A:
                            Console.Clear();
                            setStatus = false;
                            retry = true;
                            break;
                    }
                }
                if (!retry)
                {
                    break;
                }
            }
            SetTotalStatus();
            Console.WriteLine();
        }
        public void InitBaseEquipment()
        {
            equipment.myItem = new List<PlayerItem>();
            //for (int i = 0; i < equipment.myItem.Length; i++)
            //{
            //    equipment.myItem[i] = new PlayerItem();
            //}
            equipment.myPotion = new Potion("체력 포션", 50);
        }
        public void SetMyCharacterOnStart()
        {
            SetMyName();
            SetMyStatusOnStart();
        }

        #endregion

        #region 캐릭터 설정
        public void ShowMyCharacter()
        {
            Console.Clear();
            int myInt;
            Console.WriteLine($"▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷     캐 릭 터           정 보      ◁\n△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△\n");
            Console.WriteLine("무엇을 확인하시겠습니까?");
            Console.WriteLine("0. 내 캐릭터의 스테이터스");
            Console.WriteLine("1. 내 캐릭터의 장비");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 레벨업");
            Console.WriteLine("그 이외의 숫자 입력시 마을로 복귀합니다.");
            Console.WriteLine("\n원하는 번호를 입력하세요\n");
            Console.Write("▶ ");
            myInt = Read.ReadUInt(0);
            if (myInt > 3)
            {
                OnVillage();
            }
            else if (myInt == 0)
            {
                ShowMyCharacterInformation();
            }
            else if (myInt == 1)
            {
                ShowMyEquipment();
            }
            else if (myInt == 2)
            {
                ShowMyInventory();
            }
            else if (myInt == 3)
            {
                LevelUp();
            }
        }
        public void ShowMyCharacterInformation()
        {
            Console.Clear();
            ConsoleKeyInfo key;
            bool returnPage = true;

            Console.WriteLine($"이름 : {name}");
            Console.WriteLine($"\n▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷        B a s e S t a t u s        ◁\n△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△\n");
            Console.WriteLine($"HP : {myBaseStatus.hp}");
            Console.WriteLine($"공격력 : {myBaseStatus.attack}");
            Console.WriteLine($"방어력 : {myBaseStatus.defence}");

            Console.WriteLine($"\n▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷       T o t a l S t a t u s       ◁\n△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△\n");
            Console.WriteLine($"HP : {currentHp}/{myTotalStatus.hp}");
            Console.WriteLine($"공격력 : {myTotalStatus.attack}");
            Console.WriteLine($"방어력 : {myTotalStatus.defence}");

            Console.WriteLine("\nEnter키 입력시 캐릭터 정보 화면으로 복귀합니다.");
            while (returnPage)
            {
                key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        returnPage = false;
                        break;
                }
            }
            ShowMyCharacter();
        }
        //캐릭터 스탯 설정
        public int SetMyStatusDetail(int minStatPoint)
        {
            int usingStatPoint = 0;
            while (true)
            {
                usingStatPoint = Read.ReadUInt(0);
                if (usingStatPoint > statPoint)
                {
                    Console.WriteLine("잔여스탯을 넘어섰습니다. 다시 입력해주세요");
                    Console.Write("▶ ");
                    continue;
                }
                if (minStatPoint > usingStatPoint)
                {
                    Console.WriteLine($"최소 {minStatPoint} point가 필요합니다.");
                    Console.Write("▶ ");
                    continue;
                }
                break;
            }
            statPoint -= usingStatPoint;
            return usingStatPoint;
        }

        //캐릭터 Total 스테이터스(초기 스탯+장비 스탯_
        public void SetTotalStatus()
        {
            //향후 해당 값들에 아이템 스탯 추가 예정
            int hp = 0;
            int defence = 0;
            int attack = 0;



            //아이템 수치만큼 추가
            for (int i = 0; i < equipment.myItem.Count; i++)
            {
                hp += equipment.myItem[i].hp;
                defence += equipment.myItem[i].defence;
                attack += equipment.myItem[i].attack;

            }

            myTotalStatus.hp = myBaseStatus.hp + hp;
            currentHp = myTotalStatus.hp;
            myTotalStatus.defence = myBaseStatus.defence + defence;
            myTotalStatus.attack = myBaseStatus.attack + attack;
        }
        #endregion
        
        //마을로 이동하는 메서드
        public void OnVillage()
        {
            currentStage = 0;
            onVillage = true;
            Map.InitBaseMap();

            Map.MakePortal(0, 65, 25, 31, Map.adventurePortal_Stage1);
            Map.MakePortal(147, 182, 28, 34, Map.shopPortal);
            Map.MakePortal(81, 116, 52, 58, Map.homePortal);
            Map.MakePortal(0, 32, 54, 59,Map.villagePortal);

            GameSystem.NewMapMonster();

            //Console.Clear();
            Map.DrawVillageMap();
            pos.x = 100;
            pos.y = 30;
            SetCharacterPos(playerImage);

            //일단 테스트용이라 현재는 true로 둔거고
            //나중엔 사냥터 갔을 때만 true로 바꿀 예정
            onAdventure = false;
            //이거도 테스트용이라 지금 여기서 실행하는 거임
            //Gravity();

            //일단 지금 true로 설정해도 나중에 벽에 닿으면 false로 바뀜
            //onJump = true;
        }

        //모험으로 이동
        public void OnAdventure()
        {
            onVillage=false;
            onAdventure=true;
            Map.DrawAdventureMap();
            Map.InitBaseMapStage1();
            Map.DrawBaseMap();
            currentStage = 1;

            Map.SetPlayerMap(pos.x, pos.y);
            pos.x = 40;
            pos.y = 57;
            SetCharacterPos(playerImage);
        }
        public void OnAdventure2()
        {
            Map.DrawAdventureMap();
            Map.InitBaseMapStage2();
            Map.DrawBaseMap();
            currentStage = 2;

            Map.SetPlayerMap(pos.x, pos.y);
            pos.x = 40;
            pos.y = 57;
            SetCharacterPos(playerImage);
        }
        public void OnAdventure3()
        {
            onVillage=false;
            onAdventure=true;
            Map.DrawAdventureMap();
            Map.InitBaseMapStage3();
            Map.DrawBaseMap();
            currentStage = 3;

            Map.SetPlayerMap(pos.x, pos.y);
            pos.x = 40;
            pos.y = 57;
            SetCharacterPos(playerImage);
        }

        //상점으로 이동
        public void OnShop()
        {
            onVillage=false;
            onAdventure=false;
            Console.Clear();
            bool onShop = true;
            int myInt;
            Console.WriteLine(" ____  _   _  ___  ____  \r\n/ ___|| | | |/ _ \\|  _ \\ \r\n\\___ \\| |_| | | | | |_) |\r\n ___) |  _  | |_| |  __/ \r\n|____/|_| |_|\\___/|_|  ");
            Console.WriteLine("\n0. 마을로 귀환");
            Console.WriteLine("\n1. 목검 : 500 G");
            Console.WriteLine("\n2. 철검 : 800 G");
            Console.WriteLine("\n3. 바람의 검 : 2,000 G");
            Console.WriteLine("\n4. 용사의 검 : 10,000 G");
            Console.WriteLine("\n5. 중급 체력 포션 : 50 G");
            Console.WriteLine("\n현재 자본 : " + inventory.money);


            while (onShop)
            {
                Console.Write("▶ ");
                myInt = Read.ReadUInt(0);
                switch (myInt)
                {
                    case 0:
                        onShop = false;
                        break;
                    case 1:
                        if (inventory.money >= 500)
                        {
                            inventory.inventoryItem.Add(new PlayerItem("목검", 3, 0, 0, ItemType.Weapon));
                            inventory.money -= 500;
                            Console.WriteLine("[무기 : 목검]을 구매하였습니다.");
                            Console.WriteLine("현재 자본 : " + inventory.money + "\n");
                        }
                        break;
                    case 2:
                        if (inventory.money >= 800)
                        {
                            inventory.inventoryItem.Add(new PlayerItem("철검", 8, 0, 0, ItemType.Weapon));
                            inventory.money -= 800;
                            Console.WriteLine("[무기 : 철검]을 구매하였습니다.");
                            Console.WriteLine("현재 자본 : " + inventory.money + "\n");
                        }
                        break;
                    case 3:
                        if (inventory.money >= 2000)
                        {
                            inventory.inventoryItem.Add(new PlayerItem("바람의 검", 20, 10, 5, ItemType.Weapon));
                            inventory.money -= 2000;
                            Console.WriteLine("[무기 : 바람의 검]을 구매하였습니다.");
                            Console.WriteLine("현재 자본 : " + inventory.money + "\n");
                        }
                        break;
                    case 4:
                        if (inventory.money >= 10000)
                        {
                            inventory.inventoryItem.Add(new PlayerItem("용사의 검", 100, 50, 30, ItemType.Weapon));
                            inventory.money -= 10000;
                            Console.WriteLine("[무기 : 용사의 검]을 구매하였습니다.");
                            Console.WriteLine("현재 자본 : " + inventory.money + "\n");
                        }
                        break;
                    case 5:
                        if (inventory.money >= 50)
                        {
                            inventory.inventoryPotion.Add(new Potion("중급 체력 포션", 100));
                            inventory.money -= 50;
                            Console.WriteLine("[포션 : 중급 체력 포션]을 구매하였습니다.");
                            Console.WriteLine("현재 자본 : " + inventory.money + "\n");
                        }
                        break;
                }
            }

            OnVillage();
        }


        //인벤토리 내용물을 보여주는 메서드
        public void ShowMyInventory()
        {
            Console.Clear();
            ConsoleKeyInfo key;
            bool returnPage = true;

            Console.WriteLine($"\n▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷         I n v e n t o r y         ◁\n△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△\n");
            for (int i = 0; i < inventory.inventoryItem.Count; i++)
            {
                Console.WriteLine($"[{i + 1}]번 장비 인벤토리 ");
                if (inventory.inventoryItem[i].name == "")
                {
                    Console.WriteLine($"비어있습니다\n");
                }
                else
                {
                    Console.WriteLine($"이름 : {inventory.inventoryItem[i].name}\n");
                }

            }

            for (int i = 0; i < inventory.inventoryPotion.Count; i++)
            {
                Console.WriteLine($"[{i + 1}]번 포션 인벤토리");
                if (inventory.inventoryPotion[i].name == "")
                {
                    Console.WriteLine("비어있습니다\n");
                }
                else
                {
                    Console.WriteLine($"이름 : {inventory.inventoryPotion[i].name}\n");
                }

            }
            while (returnPage)
            {
                Console.WriteLine("0. 캐릭터 정보 화면으로 복귀");
                Console.WriteLine("1. 장비 장착");
                Console.WriteLine("2. 포션 장착");
                Console.WriteLine("원하는 동작을 선택하세요.");
                switch (Read.ReadInt(0, 2))
                {
                    case 0:
                        returnPage = false;
                        break;
                    case 1:
                        Console.WriteLine("착용을 원하는 장비 번호를 입력하세요");
                        EquipItem(Read.ReadUInt(0));
                        break;
                    case 2:
                        Console.WriteLine("착용을 원하는 포션 번호를 입력하세요");
                        EquipPotion(Read.ReadUInt(0));
                        break;
                }
            }
            ShowMyCharacter();
        }

        //아이템을 장착하는 메서드
        public void EquipItem(int itemNum)
        {
            if (equipment.myItem.Count < MaxEquipment)
            {
                if (itemNum<inventory.inventoryItem.Count)
                {
                    equipment.myItem.Add(inventory.inventoryItem[itemNum]);
                    inventory.inventoryItem.RemoveAt(itemNum);
                    Console.WriteLine($"[{equipment.myItem[equipment.myItem.Count - 1].name}]을 장비하였습니다.");
                    SetTotalStatus();
                }
                else
                {
                    Console.WriteLine("해당 인벤토리에 장비가 없습니다.");
                }
            }
            else
            {
                Console.WriteLine("더 이상 장비를 착용할 수 없습니다.");
            }
        }
        //포션을 장착하는 메서드
        public void EquipPotion(int itemNum)
        {
            if (inventory.inventoryPotion[itemNum] != null)
            {
                if (equipment.myPotion == null)
                {
                    equipment.myPotion = inventory.inventoryPotion[itemNum];
                    Console.WriteLine($"[{equipment.myPotion.name}]을 장비하였습니다.");
                }
                else
                {
                    inventory.inventoryPotion.Add(equipment.myPotion);
                    equipment.myPotion= inventory.inventoryPotion[itemNum];
                    inventory.inventoryPotion.RemoveAt(itemNum);
                }            
            }
        }
        //착용중인 장비를 보여주는 메서드
        public void ShowMyEquipment()
        {

            Console.Clear();
            ConsoleKeyInfo key;
            bool returnPage = true;

            Console.WriteLine($"\n▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷         E q u i p m e n t         ◁\n△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△\n");
            for (int i = 0; i < equipment.myItem.Count; i++)
            {
                Console.WriteLine($"[{i + 1}]번 장비 ");
                if (equipment.myItem[i]== null)
                {
                    Console.WriteLine($"비어있습니다\n");
                }
                else
                {
                    Console.WriteLine($"이름 : {equipment.myItem[i].name}\n");
                }
            }


            Console.WriteLine("[포션 주머니]");
            if (equipment.myPotion== null)
            {
                Console.WriteLine($"비어있습니다\n");
            }
            else
            {
                Console.WriteLine($"이름 : {equipment.myPotion.name}\n");
            }


            while (returnPage)
            {
            Console.WriteLine("0. 캐릭터 정보 화면으로 복귀");
            Console.WriteLine("1. 장비 해제");
            Console.WriteLine("2. 포션 해제");
                Console.WriteLine("원하는 동작을 선택하세요.");
                switch (Read.ReadInt(0,2))
                {
                    case 0:
                        returnPage = false;
                        break;
                    case 1:
                        Console.WriteLine("착용을 해제를 원하는 장비 번호를 입력하세요");
                        UnEquipItem(Read.ReadUInt(0));
                        break;
                    case 2:
                        UnEquipPotion();
                        break;
                }
            }

            //Console.WriteLine("\nEnter키 입력시 캐릭터 정보 화면으로 복귀합니다.");
            //while (returnPage)
            //{
            //    key = Console.ReadKey(true);

            //    switch (key.Key)
            //    {
            //        case ConsoleKey.Enter:
            //            returnPage = false;
            //            break;
            //    }
            //}
            ShowMyCharacter();
        }

        //아이템을 장착하는 메서드
        public void UnEquipItem(int itemNum)
        {
            if (itemNum< inventory.inventoryItem.Count)
            {
                inventory.inventoryItem.Add(equipment.myItem[itemNum]);
                Console.WriteLine($"[{equipment.myItem[itemNum].name}]을 장비 해제하였습니다.");
                equipment.myItem.RemoveAt(itemNum);
                SetTotalStatus();
            }
            else
            {
                Console.WriteLine("해당 위치에 장비를 착용하고있지 않습니다.");
            }
        }
        public void UnEquipPotion()
        {
            if (equipment.myPotion!=null)
            {
                inventory.inventoryPotion.Add(equipment.myPotion);
                Console.WriteLine($"[{equipment.myPotion.name}]을 장비 해제하였습니다.");
                equipment.myPotion = null;
            }
            else
            {
                Console.WriteLine("해당 위치에 포션을 착용하고있지 않습니다.");
            }
        }

        public void onGame()
        {
            Console.Clear();
            Map.SetMap();
            statPoint = 50;
            level = 1;
            SetMaxExp();
            //캐릭터 초기 설정
            InitBaseCharacter();

            SetMyCharacterOnStart();

            Console.Clear();

            ControlPlayer();
            onFront = true;
            OnVillage();
        }
        public void SetMaxExp()
        {
            maxExp = level * 10;
        }
        public void LevelUp()
        {
            bool returnPage=true;
            ConsoleKeyInfo key;
            int levelUpCount = 0;
            if (currentExp >= maxExp)
            {
                while (currentExp >= maxExp)
                {
                    level++;
                    currentExp -= maxExp;
                    SetMaxExp();
                    statPoint += 5;
                    levelUpCount++;
                }

                Console.WriteLine($"레벨이 총 [{levelUpCount}] 상승하였습니다.");

            }
            else
            {
                Console.WriteLine("경험치가 부족합니다.");
                Console.WriteLine($"{currentExp}/{maxExp}");
            }

            while (returnPage)
            {
                key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        returnPage = false;
                        break;
                }
            }
            ShowMyCharacter();
        }
    }
}
