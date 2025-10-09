using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    static class Read
    {
        public static int ReadInt(int min, int max)
        {
            int myInt = 0;
            bool isInt = false;

            //isInt 만족 못할 시 무한 반복
            while (isInt == false)
            {
                //Console.Write("\t▶ ");

                //정수 입력 시도
                isInt = int.TryParse(Console.ReadLine(), out myInt);

                //정수가 지정 범위를 만족 못할 경우 myInt false로 변환하여 다시 반복
                if (myInt < min || myInt > max)
                {
                    isInt = false;
                }
                if (isInt == false)
                {
                    Console.WriteLine("\t잘못된 입력입니다. 다시 입력해주세요");
                }
            }
            Console.WriteLine();
            return myInt;
        }

        //원하는 범위의 정수를 입력받는 함수(최솟값만 입력)
        public static int ReadUInt(int min)
        {
            int myInt = 0;
            bool isInt = false;

            //isInt 만족 못할 시 무한 반복
            while (isInt == false)
            {
                //Console.Write("\t▶ ");

                //정수 입력 시도
                isInt = int.TryParse(Console.ReadLine(), out myInt);

                //정수가 지정 범위를 만족 못할 경우 myInt false로 변환하여 다시 반복
                if (myInt < min)
                {
                    Console.WriteLine($"\t{min} 이상의 값을 설정해주세요");
                    isInt = false;
                }
                if (isInt == false)
                {
                    Console.WriteLine("\t잘못된 입력입니다. 다시 입력해주세요");
                }
            }
            Console.WriteLine();
            return myInt;
        }
    }
}
