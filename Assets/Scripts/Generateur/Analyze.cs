// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Xml.Linq;
// using UnityEngine;

// public enum Move
// {
//     Left = 1,
//     Right = 2,
//     Up = 3,
//     Down = 4
// }

// class Analyze
// {
//     public List<Move> Instance { get; private set; }
//     private readonly System.Random random = new();

//     private readonly Game game;

//     public Analyze(Game game)
//     {
//         this.game = game;
//         this.Instance = new List<Move>();
//     }

//     public void Run()
//     {   
//         bool SolutionFound = false;
//         FirstMove();
//         do
//         {
//             Expansion();
//             if (Solution())
//             {
//                 SolutionFound = true;
//                 break;
//             }
//             if (Instance.Count > 70 )
//             {
//                 // Console.WriteLine("Aucune solution trouvée.");
//                 break;
//             }
//         } while (!SolutionFound);

//         if(!SolutionFound) {
//             GameManager.FindSolution(game.nRows, game.nCols);
//             return;
//         }

//         Reduction();

//         SaveInFile();
//         Console.WriteLine("Fin de l'analyse.");
//     }

//     private void FirstMove()
//     {
//         Move move = Move.Left;
//         switch(random.Next(0,3)) {
//             case 0:
//                 move = Move.Right;
//                 break;
//             case 1:
//                 move = Move.Up;
//                 break;
//             case 2:
//                 move = Move.Down;
//                 break;
//         }
//         this.Instance.Add(move);
//     }

//     private void Reduction()
//     {
//         bool instanceReduced = false;
//         do {
//             instanceReduced = false;
//             for (int i = 0 ; i < Instance.Count ; i++)
//             {
//                 Move move = Instance[i];
//                 Instance.RemoveAt(i);
//                 if(Solution()) {
//                     instanceReduced = true;
//                     break;
//                 } else {
//                     Instance.Insert(i, move);
//                 }
//             }
//         } while(instanceReduced);

//         Console.WriteLine("Instance réduite.");
//         // AfficherInstance();
//     }

//     private void SaveInFile()
//     {
//         string pathToFile = "Game.txt";

//         string content = "";
//         content += $"{game.nRows} {game.nCols}\n";

//         for (int i = 0 ; i < game.nRows ; i++)
//         {
//             for (int j = 0 ; j < game.nCols ; j++)
//             {
//                 content += game.Board[i, j].ToString();
//                 if(j < game.nCols - 1)
//                     content += " ";
//             }
//             content += "\n";
//         }

//         foreach (Move move in Instance.Select(v => (int)v).Select(v => (Move)v))
//         {
//             content += $"{move.ToString()[..1]}";
//         }
//         content += "\n";
//         content += $"{Instance.Count}";

//         // Écrire le contenu dans le fichier
//         File.WriteAllText(pathToFile, content);

//         // Console.WriteLine("Fichier créé avec succès !");
//     }

//     private void Expansion()
//     {
//         Move move = Move.Left;
//         do {
//             move = Move.Left;
//             switch(random.Next(0,3)) {
//                 case 0:
//                     move = Move.Right;
//                     break;
//                 case 1:
//                     move = Move.Up;
//                     break;
//                 case 2:
//                     move = Move.Down;
//                     break;
//             }
//         } while (move == Instance.Last());
//         this.Instance.Add(move);
//     }

//     private bool Solution()
//     {
//         Game test = game.Clone();
//         // AfficherInstance();
//         foreach (var move in Instance)
//         {   
//             switch(move) {
//                 case Move.Left:
//                     test.UpdateGame(Game.Direction.Left);
//                     break;
//                 case Move.Right:
//                     test.UpdateGame(Game.Direction.Right);
//                     break;
//                 case Move.Up:
//                     test.UpdateGame(Game.Direction.Up);
//                     break;
//                 case Move.Down:
//                     test.UpdateGame(Game.Direction.Down);
//                     break;
//             }

//             if(test.Win(false)) {  
//                 // AfficherInstance();
//                 // test.Display();
//                 return true;
//             }

//         }
        
//         return false;
//     }
// }