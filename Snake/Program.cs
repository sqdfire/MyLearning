﻿using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Threading;

namespace Snake
{
    // функция ескейпа, левела но это не сильно имеет смысл так как я буду менять тут половину изза моргания, просто тренируюсь пока
    // сделать жизни, сделать в бут меню старт игры в хард режиме(без жизней) либо для слабочков в легком с жизнями, не знаю нужно ли это 
    // но можно сделать возврат в бут меню после проигрыша, разбить по ООП
    internal class Program
    {
        static void Main()
        {
            bool startGame = false;
            Console.CursorVisible = false;

            char[,] map = ReadMap("map.txt");
            ConsoleKeyInfo pressedKey = new ConsoleKeyInfo('w', ConsoleKey.W, false, false, false);

            Task.Run(() =>
            {
                while (true)
                {
                    pressedKey = Console.ReadKey();
                }
            });

            int snakeX = 1;
            int snakeY = 1;
            int score = 0;

            int intialDelayMilliseconds = 500;
            int delayMilliseconds = intialDelayMilliseconds;

            string snake = "_";

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(50, 1);
            Console.WriteLine("Snake");

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(50, 5);
            Console.WriteLine("Start");
            Console.SetCursorPosition(50, 6);
            Console.WriteLine("Exit");
            pressedKey = Console.ReadKey();

            Menu BootMenu = new Menu(ref pressedKey, ref startGame);


            while (startGame)
            {
                Console.Clear();

                HandleInput(pressedKey, ref snakeX, ref snakeY, map, ref score, ref snake, ref delayMilliseconds);

                Console.ForegroundColor = ConsoleColor.Blue;
                DrawMap(map);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.SetCursorPosition(snakeX, snakeY); //x, y
                Console.Write(snake);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(13, 0);
                Console.WriteLine($"Score: {score} ");

                Console.SetCursorPosition(36, 0);
                Console.WriteLine("$ - SpeedBoost");

                Console.SetCursorPosition(36, 1);
                Console.WriteLine("& - SlowBoost");

                Console.SetCursorPosition(36, 2);
                Console.WriteLine(". - Score Points");

                Console.SetCursorPosition(36, 3);
                Console.WriteLine("@ - Finish if u have all Points");

                Console.SetCursorPosition(36, 4);
                Console.WriteLine("Borders will kill you, be a carefull!");

                Console.SetCursorPosition(6, 10);
                Console.WriteLine("Just Beta, now thinking");

                Thread.Sleep(delayMilliseconds);
            }
        }

        private static char[,] ReadMap(string path)
        {
            string[] file = File.ReadAllLines("map.txt");

            char[,] map = new char[GetMaxLengthOfLine(file), file.Length];

            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++)
                    map[x, y] = file[y][x];

            return map;
        }

        private static void DrawMap(char[,] map)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    Console.Write(map[x, y]);
                }
                Console.Write("\n");
            }
        }

        private static void HandleInput(ConsoleKeyInfo pressedKey, ref int snakeX, ref int snakeY, char[,] map, ref int score, ref string snake, ref int delayMilliseconds)
        {
            int[] direction = GetDirection(pressedKey);
            int nextSnakePositionX = snakeX + direction[0];
            int nextSnakePositionY = snakeY + direction[1];

            char nextCell = map[nextSnakePositionX, nextSnakePositionY];

            if (nextCell == '$')
            {
                delayMilliseconds -= 100;
                map[nextSnakePositionX, nextSnakePositionY] = ' ';
            }
            if (nextCell == '&')
            {
                delayMilliseconds += 100;
                map[nextSnakePositionX, nextSnakePositionY] = ' ';
            }

            if (nextCell == ' ' || nextCell == '.')
            {
                snakeX = nextSnakePositionX;
                snakeY = nextSnakePositionY;
                ChangeSnakesRoad(ref snake, pressedKey);
                if (nextCell == '.')
                {
                    score++;
                    delayMilliseconds -= 10;
                    if (pressedKey.Key == ConsoleKey.UpArrow || pressedKey.Key == ConsoleKey.DownArrow)
                    {
                        snake = "|";
                    }

                    else if (pressedKey.Key == ConsoleKey.RightArrow || pressedKey.Key == ConsoleKey.LeftArrow)
                    {
                        snake = "_";
                    }
                    map[nextSnakePositionX, nextSnakePositionY] = ' ';
                }
            }

            else if (score >= 42 && nextCell == '@')
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(53, 5);
                Console.WriteLine("Wictory!");
                Thread.Sleep(5000);
                Environment.Exit(0);
            }
            else if (nextCell == '|' || nextCell == '_' || nextCell == '#')
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(53, 5);
                Console.WriteLine("You lose");
                Thread.Sleep(3000);
                Environment.Exit(0);
            }
        }

        private static int[] GetDirection(ConsoleKeyInfo pressedKey)
        {
            int[] direction = { 0, 0 };

            if (pressedKey.Key == ConsoleKey.UpArrow)
                direction[1] -= 1;
            else if (pressedKey.Key == ConsoleKey.DownArrow)
                direction[1] += 1;
            else if (pressedKey.Key == ConsoleKey.LeftArrow)
                direction[0] -= 1;
            else if (pressedKey.Key == ConsoleKey.RightArrow)
                direction[0] += 1;

            return direction;
        }

        public static void ChangeSnakesRoad(ref string snake, ConsoleKeyInfo pressedKey)
        {
            if (pressedKey.Key == ConsoleKey.UpArrow || pressedKey.Key == ConsoleKey.DownArrow)
            {
                snake = "|" + snake.Substring(1);
            }
            else if (pressedKey.Key == ConsoleKey.LeftArrow || pressedKey.Key == ConsoleKey.RightArrow)
            {
                snake = "_" + snake.Substring(1);
            }
        }

        private static int GetMaxLengthOfLine(string[] lines)
        {
            int maxLength = lines[0].Length;

            foreach (var line in lines)
                if (line.Length > maxLength)
                    maxLength = line.Length;

            return maxLength;
        }
    }

    class Menu
    {
        public Menu(ref ConsoleKeyInfo pressedKey, ref bool startGame)
        {
            while (true)
            {
                if (!startGame)
                {
                    if (pressedKey.Key == ConsoleKey.UpArrow)
                    {
                        Console.Clear();

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.SetCursorPosition(50, 1);
                        Console.WriteLine("Snake");

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.SetCursorPosition(50, 5);
                        Console.WriteLine("Start");
                        Console.SetCursorPosition(70, 5);
                        Console.WriteLine("Selected*");
                        Console.SetCursorPosition(50, 6);
                        Console.WriteLine("Exit");
                        Console.ReadKey();
                        if (pressedKey.Key == ConsoleKey.Enter)
                        {
                            startGame = true;
                            Console.Clear();
                        }
                    }

                    else if (pressedKey.Key == ConsoleKey.DownArrow)
                    {
                        Console.Clear();

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.SetCursorPosition(50, 1);
                        Console.WriteLine("Snake");

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.SetCursorPosition(50, 6);
                        Console.WriteLine("Exit");
                        Console.SetCursorPosition(70, 6);
                        Console.WriteLine("Selected*");
                        Console.SetCursorPosition(50, 5);
                        Console.WriteLine("Start");
                        Console.ReadKey();
                        if (pressedKey.Key == ConsoleKey.Enter)
                        {
                            startGame = false;
                            break;
                        }
                    }
                }

                else
                {
                    break;
                }
            }
        }
    }
}