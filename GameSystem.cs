using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    static class GameSystem
    {
        static Player player;
        public static Dictionary<int, Monster> monsterPos;
        public static Action monsterMove=null;
        public static bool attackByMonster=false;

        static public void OnStart()
        {
            player = new Player();
            monsterPos = new Dictionary<int, Monster>();
            Control();
            player.onGame();
        }
        static public void NewMapMonster()
        {
            monsterMove = null;
            monsterPos = null;
            monsterPos = new Dictionary<int, Monster>();
        }
        static public void GenerateMonster()
        {
            Monster monster = new Monster();    
            //딕셔너리에 저장한 다음에 이후 검색
            monsterPos.Add(monster.pos.x * 200 + monster.pos.y * 60, monster);
            monsterMove += monster.RandomMove;
        }
        static public void GenerateMonster(int x, int y)
        {

            Monster monster = new Monster(x, y);    

            //딕셔너리에 저장한 다음에 이후 검색
            monsterPos.Add(monster.pos.x * 200 + monster.pos.y * 60, monster);

           monsterMove += monster.RandomMove;
        }
        static public void GenerateBossMonster(int x, int y)
        {
            Monster monster = new Monster(x, y);  
            monster.BossMonster(x, y);
            
            //딕셔너리에 저장한 다음에 이후 검색
            monsterPos.Add(monster.pos.x * 200 + monster.pos.y * 60, monster);
        }

        static public void ChangePos(int currentX, int currentY, int moveX, int moveY)
        {
            Monster valueMonster = monsterPos[currentX*200+currentY*60];
            monsterPos.Remove(currentX * 200 + currentY * 60);
            monsterPos[(currentX+moveX)*200 + (currentY+moveY)*60] = valueMonster;
        }

        public static void AttackMonster(int x, int y)
        {
            monsterPos[x * 200 + y * 60].myBaseStatus.hp -= player.myTotalStatus.attack;
            if (monsterPos[x * 200 + y * 60].myBaseStatus.hp <= 0)
            {
                player.IncreaseExp(monsterPos[x * 200 + y * 60]);
                monsterPos[x * 200 + y * 60].Die();
                monsterPos[x * 200 + y * 60] = null;
                Map.MonsterMap[x, y] = false;
            }
            else
            {
                Console.SetCursorPosition(x,y);
                Console.Write(monsterPos[x * 200 + y * 60].playerImage[0]);
                Console.SetCursorPosition(x, y+1);
                Console.Write(monsterPos[x * 200 + y * 60].playerImage[1]);
            }
        }

        public static void AttackPlayer(int attack)
        {
            player.AttackByMonster(attack);
        }
        public static void RemoveMonster()
        {
            for (int i = 0; i < Map.MonsterMap.GetLength(0); i++)
            {
                for (int j = 0; j < Map.MonsterMap.GetLength(1); j++)
                {
                    Map.MonsterMap[i, j] = false;
                    Map.monsterAttackMap[i, j] = true;
                }
            }
            monsterPos.Clear();
        }

        static void Control()
        {
            BackgroundWorker control = new BackgroundWorker();
            ConsoleKeyInfo key;
            control.DoWork += (sender, e) =>
            {
                Stopwatch gravity = new Stopwatch();
                Stopwatch attackDelay = new Stopwatch();
                Stopwatch monsterMovingDelay = new Stopwatch();

                gravity.Start();
                attackDelay.Start();
                monsterMovingDelay.Start();

                List<int> attackPosX;
                List<int> attackPosY;
                int attackX;
                int attackY;
                attackPosX = new List<int>();
                attackPosY = new List<int>();
                bool onAttack = false;
                while (true)
                {
                    if (Console.KeyAvailable)
                    {
                        if (player.onVillage || player.onAdventure)
                        {
                            key = Console.ReadKey(true);
                            switch (key.Key)
                            {
                                case ConsoleKey.UpArrow:
                                    if (player.onAdventure)
                                    {
                                        player.Jump();
                                    }
                                    else if (player.onVillage)
                                    {
                                        player.MoveUp();
                                    }
                                    break;
                                case ConsoleKey.DownArrow:
                                    player.MoveDown();
                                    break;
                                case ConsoleKey.LeftArrow:
                                    player.onFront = false;
                                    player.MoveLeft();
                                    break;
                                case ConsoleKey.RightArrow:
                                    player.onFront = true;
                                    player.MoveRight();
                                    break;
                                case ConsoleKey.A:
                                    if (attackDelay.ElapsedMilliseconds >= 1500)
                                    {
                                        if (!onAttack)
                                        {
                                            onAttack = true;
                                        }

                                        if (player.onFront)
                                        {
                                            player.Attack(3, 0, 0, 0, attackPosX, attackPosY);

                                        }
                                        else
                                        {
                                            player.Attack(0, 3, 0, 0, attackPosX, attackPosY);
                                        }
                                        attackDelay.Restart();
                                    }

                                    break;
                                case ConsoleKey.S:
                                    player.UsingPotion();
                                    break;
                                case ConsoleKey.Spacebar:
                                    if (player.onVillage)
                                    {
                                        if (Map.shopPortal[player.pos.x, player.pos.y])
                                        {
                                            player.onVillage = false;
                                            player.onAdventure = false;
                                            player.OnShop();
                                        }
                                        if (Map.adventurePortal_Stage1[player.pos.x, player.pos.y] && player.currentStage == 0)
                                        {
                                            monsterMove = null;
                                            player.OnAdventure();
                                        }
                                        if (Map.homePortal[player.pos.x, player.pos.y])
                                        {
                                            player.onVillage = false;
                                            player.onAdventure = false;
                                            player.ShowMyCharacter();
                                        }
                                    }
                                    if (player.onAdventure)
                                    {
                                        if (Map.villagePortal[player.pos.x, player.pos.y])
                                        {
                                            monsterMove = null;
                                            player.OnVillage();
                                        }
                                        if (Map.adventurePortal_Stage2[player.pos.x, player.pos.y] && player.currentStage == 1)
                                        {
                                            monsterMove = null;
                                            player.OnAdventure();
                                        }
                                        if (Map.adventurePortal_Stage3[player.pos.x, player.pos.y] && player.currentStage == 2)
                                        {
                                            monsterMove = null;
                                            player.OnAdventure();
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    if (player.onAdventure)
                    {
                        if (!Map.BaseMap[player.pos.x, player.pos.y + player.playerImage.Length] && gravity.ElapsedTicks % 10000 == 0)
                        {
                            player.MoveDown();
                            gravity.Restart();
                            //여기다가 몬스터 일정 시간마다 움직이는 코드 넣어도 될 듯?
                        }
                    }

                    if (attackDelay.ElapsedMilliseconds == 1000 && onAttack)
                    {
                        for (int i = 0; i < attackPosX.Count; i++)
                        {
                            attackX = attackPosX[i];
                            attackY = attackPosY[i];
                            Console.SetCursorPosition(attackX, attackY);
                            Map.playerAttackMap[attackX, attackY] = false;
                            if (player.onAdventure)
                            {
                                Console.Write(Map.adventureMap[attackX, attackY]);
                            }
                            if (player.onVillage)
                            {
                                Console.Write(Map.villageMap[attackX, attackY]);
                            }
                        }

                        player.DrawCharacter();

                        attackPosX.RemoveRange(0, attackPosX.Count - 1);
                        attackPosY.RemoveRange(0, attackPosY.Count - 1);
                    }
                    //시간관계상 AI 제작 실패
                    //if (monsterMovingDelay.ElapsedMilliseconds == 1000)
                    //{
                    //    monsterMove?.Invoke();
                    //    monsterMovingDelay.Restart();
                    //}
                }
            };
            control.RunWorkerCompleted += (sender, e) =>
            {
                control = null;
            };
            control.RunWorkerAsync();
        }
    }
}
