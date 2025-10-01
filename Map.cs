using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

//using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    internal static class Map
    {
        //플레이어와 몬스터의 위치를 추적하여 충돌을 확인하기 위해 사용
        static bool[,] playerMap;
        static bool[,] monsterMap;

        //1001 플레이어의 공격 범위를 추적하여 충돌을 확인하기 위해 사용
        static bool[,] attackMap;

        public static void InitMap(int width, int height)
        {
            playerMap = new bool[width, height];
            monsterMap = new bool[width, height];
            attackMap = new bool[width, height];
        }

        public static void SetMonsterMap(int x, int y)
        {
            //참고로 몬스터는 여러개 있는 경우 해당 몬스터 위치로 이동하지 않도록 몬스터 클래스에 구현
            //true인 경우 false <- 이동 시작점이라 지나쳤으므로 false로 변환
            if (monsterMap[x, y])
            {
                monsterMap[x, y] = false;
            }
            //false인 경우 true <- 도착점이므로 true로 변환
            else
            {
                monsterMap[x, y] = true;
            }

            //몬스터와 플레이어가 충돌 시
            if (monsterMap[x, y] && playerMap[x, y])
            {
                //발생할 코드 작성 예정
                //몬스터가 들이 박은거니 플레이어한테 부정적 영향
                Console.Write("충돌 발생:Monster");
            }
        }
        public static void SetPlayerMap(int x, int y)
        {
            //true인 경우 false <- 이동 시작점이라 지나쳤으므로 false로 변환
            if (playerMap[x, y])
            {
                playerMap[x, y] = false;
            }
            //false인 경우 true <- 도착점이므로 true로 변환
            else
            {
                playerMap[x, y] = true;

                //몬스터와 플레이어가 충돌 시
                if (monsterMap[x, y] && playerMap[x, y])
                {
                    //발생할 코드 작성 예정
                    //플레이어가 들이 박은거니 플레이어한테 부정적 영향

                    //해당 문자열 출력 확인 -> 충돌 기능 정상 작동
                    Console.Write("충돌 발생:Player");
                }
            }
        }

        static void AttackProcess(int x, int y, int duration)
        {
            attackMap[x, y] = true;
            Console.SetCursorPosition(x, y);
            Console.Write("c");

            if (attackMap[x, y] && monsterMap[x, y])
            {
                monsterMap[x, y] = false;
                Console.WriteLine("몬스터 공격");
            }

            Thread.Sleep(duration * 1000);
            //Console.WriteLine("지속시간 경과");
            attackMap[x, y] = false;
            Console.SetCursorPosition(x, y);
            Console.Write(" ");
        }

        //이거 호출하면 해당 좌표가 true로 바뀌고 지속시간 끝나면 다시 false로 바뀐다
        public static void SetAttackMap(MyPos pos, int forwardRangeX, int rearRangeX, int topRangeY, int bottomRangeY, int duration)
        {


            BackgroundWorker attack = new BackgroundWorker();
            attack.DoWork += (sender, e) =>
            {
                List<int> attackRangeX = new List<int>();
                List<int> attackRangeY = new List<int>();
                for (int i = 1; i <= forwardRangeX; i++)
                {
                    if (pos.x + i < 200)
                    {
                        attackMap[pos.x + i, pos.y] = true;
                        attackRangeX.Add(pos.x + i);
                        attackRangeY.Add(pos.y);
                        Console.SetCursorPosition(pos.x + i, pos.y);
                        Console.Write("c");

                        if (attackMap[pos.x + i, pos.y] && monsterMap[pos.x + i, pos.y])
                        {
                            monsterMap[pos.x + i, pos.y] = false;
                            Console.WriteLine("몬스터 공격");
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = 1; i <= rearRangeX; i++)
                {
                    if (pos.x - i >= 0)
                    {
                        attackMap[pos.x - i, pos.y] = true;
                        attackRangeX.Add(pos.x - i);
                        attackRangeY.Add(pos.y);

                        Console.SetCursorPosition(pos.x - i, pos.y);
                        Console.Write("c");

                        if (attackMap[pos.x - i, pos.y] && monsterMap[pos.x - i, pos.y])
                        {
                            monsterMap[pos.x - i, pos.y] = false;
                            Console.WriteLine("몬스터 공격");
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = 1; i <= topRangeY; i++)
                {
                    if (pos.y - i >= 0)
                    {
                        attackMap[pos.x, pos.y - i] = true;
                        attackRangeX.Add(pos.x);
                        attackRangeY.Add(pos.y - i);

                        Console.SetCursorPosition(pos.x, pos.y - i);
                        Console.Write("c");

                        if (attackMap[pos.x, pos.y-i] && monsterMap[pos.x, pos.y-i])
                        {
                            monsterMap[pos.x, pos.y-i] = false;
                            Console.WriteLine("몬스터 공격");
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = 1; i <= bottomRangeY; i++)
                {
                    if (pos.y + i <= 60)
                    {
                        attackMap[pos.x, pos.y+i] = true;
                        attackRangeX.Add(pos.x);
                        attackRangeY.Add(pos.y+i);
                        Console.SetCursorPosition(pos.x, pos.y+1);
                        Console.Write("c");
                        if (attackMap[pos.x, pos.y+i] && monsterMap[pos.x, pos.y+i])
                        {
                            monsterMap[pos.x, pos.y+i] = false;
                            Console.WriteLine("몬스터 공격");
                        }
                    }
                    else
                    {
                        break;
                    }
                }


                Thread.Sleep(duration * 1000);
                //Console.WriteLine("지속시간 경과");

                for (int i = 0; i < attackRangeX.Count; i++)
                {
                    Console.SetCursorPosition(attackRangeX[i], attackRangeY[i]);
                    Console.Write(" ");
                    attackMap[attackRangeX[i],attackRangeY[i]] = false;

                }

                attackRangeX = null;
                attackRangeY = null;
            };
            attack.RunWorkerCompleted += (sender, e) =>
            {
                attack = null;
            };

            attack.RunWorkerAsync();


            //Thread attack = new Thread(new ThreadStart(()=>AttackProcess(x,y,duration)));
            //attack.Start();
            //attack.Join();

            ////스탑워치를 이용해 공격 지속시간 설정

            //Stopwatch watch = new Stopwatch();
            //watch.Start();

            //attackMap[x, y] = true;
            //Console.SetCursorPosition(x, y);
            //Console.Write("c");
            ////일정 시간 지나면 발동하려고 했는데 일단 다른 방식 사용
            ////Timer timer = new Timer(AttackMap, null, duration * 1000, Timeout.Infinite);

            //if (attackMap[x, y] && monsterMap[x, y])
            //{
            //    monsterMap[x, y] = false;
            //    Console.WriteLine("몬스터 공격");
            //    Console.WriteLine(watch.ElapsedMilliseconds);
            //}


            //if (watch.ElapsedMilliseconds >= duration)
            //{
            //    //Console.SetCursorPosition(0, 0);
            //    Console.WriteLine("지속시간 경과");
            //    attackMap[x, y] = false;
            //    Console.SetCursorPosition(x, y);
            //    Console.Write(" ");
            //    watch.Stop();
            //    watch = null;
            //}



        }
        ////이거 호출하면 해당 좌표가 true로 바뀌고 지속시간 끝나면 다시 false로 바뀐다
        //public static void SetAttackMap(int x, int y, int duration)
        //{


        //    BackgroundWorker attack = new BackgroundWorker();
        //    attack.DoWork += (sender, e) =>
        //    {
        //        int ux, uy;
        //        ux=x; uy = y;
        //        attackMap[ux, uy] = true;
        //        Console.SetCursorPosition(ux, uy);
        //        Console.Write("c");

        //        if (attackMap[ux, uy] && monsterMap[ux, uy])
        //        {
        //            monsterMap[ux, uy] = false;
        //            Console.WriteLine("몬스터 공격");
        //        }

        //        Thread.Sleep(duration * 1000);
        //        //Console.WriteLine("지속시간 경과");
        //        attackMap[ux, uy] = false;
        //        Console.SetCursorPosition(ux, uy);
        //        Console.Write(" ");
        //    };
        //    attack.RunWorkerCompleted += (sender, e) =>
        //    {
        //        attack = null;
        //    };

        //    attack.RunWorkerAsync();


        //    //Thread attack = new Thread(new ThreadStart(()=>AttackProcess(x,y,duration)));
        //    //attack.Start();
        //    //attack.Join();

        //    ////스탑워치를 이용해 공격 지속시간 설정

        //    //Stopwatch watch = new Stopwatch();
        //    //watch.Start();

        //    //attackMap[x, y] = true;
        //    //Console.SetCursorPosition(x, y);
        //    //Console.Write("c");
        //    ////일정 시간 지나면 발동하려고 했는데 일단 다른 방식 사용
        //    ////Timer timer = new Timer(AttackMap, null, duration * 1000, Timeout.Infinite);

        //    //if (attackMap[x, y] && monsterMap[x, y])
        //    //{
        //    //    monsterMap[x, y] = false;
        //    //    Console.WriteLine("몬스터 공격");
        //    //    Console.WriteLine(watch.ElapsedMilliseconds);
        //    //}


        //    //if (watch.ElapsedMilliseconds >= duration)
        //    //{
        //    //    //Console.SetCursorPosition(0, 0);
        //    //    Console.WriteLine("지속시간 경과");
        //    //    attackMap[x, y] = false;
        //    //    Console.SetCursorPosition(x, y);
        //    //    Console.Write(" ");
        //    //    watch.Stop();
        //    //    watch = null;
        //    //}



        //}
        public static void AttackMap(object obj)
        {
            //attackMap[x, y] = false;
            AttackProcess(1, 1, 1);
        }

    }
}
