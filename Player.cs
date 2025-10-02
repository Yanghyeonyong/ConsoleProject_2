using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    enum Direction
    { left, right, up, down }
    //struct MyPos
    //{
    //    public int[] x;
    //    public int[] y;
    //}
    struct MyPos
    {
        public int x;
        public int y;
    }


    internal class Player
    {
        string name;
        Direction dir;
        string[] playerImage;
        string[] eraserplayerImage;

        bool onHuntingArea;
        //더블점프 포기하고 그냥 점프중엔 조작 안되게 만들기 위함
        bool onJump;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        //해당 좌표는 플레이어의 좌측 상단 좌표이다
        public MyPos pos;

        public Player()
        {
            //2*7의 배열로 캐릭터의 충돌 범위를 설정한다<-생각해보니 굳이 설정할 필요 없는데?
            //pos.x = new int[7];
            //pos.y = new int[2];
            playerImage = new string[2];
            playerImage[0] = " ∧ ∧";
            playerImage[1] = "( '▽' )";//<-이거 특수문자 때문에 크기 8 먹는다 [2]*8 배열인듯
            //playerImage = " ∧ ∧\n( '▽' )";
            Name = "Nothing";
            eraserplayerImage = new string[2];
            eraserplayerImage[0] = "        ";
            eraserplayerImage[1] = "        ";
            //pos.x[0] = 0;
            //pos.y[0] = 0;
            pos.x = 100;
            pos.y = 30;
            //MoveCharacter();
            SetCharacterPos(playerImage);

            //일단 테스트용이라 현재는 true로 둔거고
            //나중엔 사냥터 갔을 때만 true로 바꿀 예정
            onHuntingArea = true;
            //이거도 테스트용이라 지금 여기서 실행하는 거임
            Gravity();

            //일단 지금 true로 설정해도 나중에 벽에 닿으면 false로 바뀜
            onJump = true;
            
            //이거도 테스트용
            Jump();
        }

        public void SetCharacterPos(string[] s)
        {
            Map.SetPlayerMap(pos.x, pos.y);
            Console.SetCursorPosition(pos.x, pos.y);

            Console.Write(s[0]);
            Console.SetCursorPosition(pos.x, pos.y+1);
            Console.Write(s[1]);
        }

        public void Move(Direction dir)
        {
            SetCharacterPos(eraserplayerImage);
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
            if (!Map.BaseMap[pos.x - 2,pos.y])
            {
                Move(Direction.left);
            }
        }
        public void MoveRight()
        {
            if (!Map.BaseMap[pos.x + playerImage[1].Length+1,pos.y])
            {
                Move(Direction.right);
            }

        }
        public void MoveUp()
        {
            if (!Map.BaseMap[pos.x,pos.y-playerImage.Length+1])
            {
                Move(Direction.up);
            }

        }
        public void MoveDown()
        {
            //왜 이런 값이 나올까 생각해봤는제 pos는 플레이어의 좌측 상단 좌표
            // pos.y+playerImage+1이 아닌 이유는 애초에 내가 아랫줄 맵은 baseMap.GetLength(1)-1d으로 받아놔서 그런거 같음
            if (!Map.BaseMap[pos.x,pos.y+playerImage.Length])
            {
                Move(Direction.down);
            }

        }

        //전방 점프같은 경우 어떻게 구현하지
        //백그라운드로 구현
        //현재 점프를 반복할경우 일정 확률로 캐릭터의 이미지가 지워지지 않고 남아있는 것을 확인
        //반복문 처리 속도가 너무 빨라서 그럴 수도 있다고 생각함
        public void Jump()
        {
            BackgroundWorker jump = new BackgroundWorker();
            ConsoleKeyInfo key;
            jump.DoWork += (sender, e) =>
            {
                while (onHuntingArea)
                {
                    
                    //발 밑에 벽이 있을 경우 점프 가능
                    if (Map.BaseMap[pos.x, pos.y + playerImage.Length])
                    {
                        //Console.WriteLine("점프 가능");
                        onJump = false;
                    }
                    if (!onJump)
                    {
                        key = Console.ReadKey(true);
                        switch (key.Key)
                        {
                            case ConsoleKey.UpArrow:
                                //Console.WriteLine("점프!!");
                                MoveUp();
                                MoveUp();
                                //Console.WriteLine("점프 성공");
                                break;
                        }
                        onJump = true;
                    }
                }
            };
            jump.RunWorkerCompleted += (sender, e) =>
            {

            };
            jump.RunWorkerAsync();
        }

        //이건 백그라운드에서 일정시간마다 아래로 이동하도록 설정하면 될 것 같다.
        //해당 함수는 사냥터로 들어갈 경우 호출되며 사냥터 탈출 시 
        //onHuntingArea가 false로 전환되며 반복문도 끝난다
        public void Gravity()
        {
            BackgroundWorker gravity = new BackgroundWorker();
            gravity.DoWork += (sender, e) =>
            {
                while (onHuntingArea)
                {
                    if (!Map.BaseMap[pos.x, pos.y + playerImage.Length])
                    {
                        Move(Direction.down);
                    }
                    //해당 주기마다 아래로 내려간다
                    Thread.Sleep(250);
                }
            };
            gravity.RunWorkerCompleted += (sender, e) =>
            {

            };
            gravity.RunWorkerAsync();
        }

        public void Attack(int forwardRangeX, int rearRangeX, int topRangeY, int bottomRangeY)
        {
            //Map.SetAttackMap(pos,forwardRangeX, rearRangeX, topRangeY, bottomRangeY, 1);
            SetAttackMap(pos, forwardRangeX, rearRangeX, topRangeY, bottomRangeY, 1);
        }
        public static void SetAttack(int x, int y, List<int> attackRangeX, List<int> attackRangeY)
        {
            Map.AttackMap[x, y] = true;
            attackRangeX.Add(x);
            attackRangeY.Add(y);
            Console.SetCursorPosition(x, y);
            Console.Write("c");
            if (Map.AttackMap[x, y] && Map.MonsterMap[x, y])
            {
                Console.WriteLine("몬스터 공겨어어어억");
                Map.MonsterMap[x, y] = false;
            }
        }

        //이거 호출하면 해당 좌표가 true로 바뀌고 지속시간 끝나면 다시 false로 바뀐다
        public static void SetAttackMap(MyPos pos, int forwardRangeX, int rearRangeX, int topRangeY, int bottomRangeY, int duration)
        {
            BackgroundWorker attack = new BackgroundWorker();
            attack.DoWork += (sender, e) =>
            {
                int attackX;
                int attackY;
                List<int> attackRangeX = new List<int>();
                List<int> attackRangeY = new List<int>();
                for (int i = 1; i <= forwardRangeX; i++)
                {
                    attackX = pos.x + i;
                    attackY = pos.y;
                    if (attackX < 200)
                    {
                        SetAttack(attackX, attackY, attackRangeX, attackRangeY);
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = 1; i <= rearRangeX; i++)
                {
                    attackX = pos.x - i;
                    attackY = pos.y;
                    if (attackX >= 0)
                    {
                        SetAttack(attackX, attackY, attackRangeX, attackRangeY);
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = 1; i <= topRangeY; i++)
                {
                    attackX = pos.x;
                    attackY = pos.y - i;
                    if (attackY >= 0)
                    {
                        SetAttack(attackX, attackY, attackRangeX, attackRangeY);
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = 1; i <= bottomRangeY; i++)
                {
                    attackX = pos.x;
                    attackY = pos.y + i;
                    if (attackY <= 60)
                    {
                        SetAttack(attackX, attackY, attackRangeX, attackRangeY);
                    }
                    else
                    {
                        break;
                    }
                }

                Thread.Sleep(duration * 1000);

                for (int i = 0; i < attackRangeX.Count; i++)
                {
                    Console.SetCursorPosition(attackRangeX[i], attackRangeY[i]);
                    Console.Write(" ");
                    Map.AttackMap[attackRangeX[i], attackRangeY[i]] = false;
                }
                attackRangeX = null;
                attackRangeY = null;
            };
            attack.RunWorkerCompleted += (sender, e) =>
            {
                attack = null;
            };
            attack.RunWorkerAsync();
        }


    }
}
