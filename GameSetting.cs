using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    static class GameSetting
    {
        //게임 화면 콘솔창 세팅
        //작동하지 않을 경우 [윈도우 -> 설정 -> 시스템 -> 개발자용 -> 터미널을 "콘솔 호스트"로 변경]
        public static void WindowSetting(int w, int h)
        {
            int maxWidth = Console.LargestWindowWidth;
            int maxHeight = Console.LargestWindowHeight;

            int width = Math.Min(w, maxWidth);
            int height = Math.Min(h, maxHeight);

            //윈도우 콘솔창 크기 설정
            Console.SetWindowSize(width, height);

            //맵 크기 = 윈도우 콘솔창 크기
            Map.InitMap(width, height);
        }
    }
}
