// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// public enum TileGame
// {
//     Void = 0,
//     Mur,
//     Red,
//     Blue,
//     Green,
//     Yellow,
//     Purple,
//     Pink,
//     Cyan,
//     Orange
// }

// public class Game
// {
//     public ulong Score { get; set; }
//     public TileGame[,] Board { get; set; }

//     public readonly int nRows;
//     public readonly int nCols;
//     private readonly System.Random random = new();

//     private static readonly TileGame[] ImmobileTiles = new TileGame[] { TileGame.Void, TileGame.Mur };
//     private readonly TileGame[] PossibleTiles = new TileGame[] { TileGame.Red, TileGame.Green, TileGame.Purple, TileGame.Pink, TileGame.Cyan, TileGame.Orange };

    
//     public Game(int nRows, int nCols)
//     {
//         this.nRows = nRows;
//         this.nCols = nCols;
//         this.Board = new TileGame[this.nRows, this.nCols];
//         InitializeBoard();
//         PutInitialTiles();
//         this.Score = 0;
//     }

//     private static string GetTileChar(TileGame tile)
//     {
//         return tile switch
//         {
//             TileGame.Void => ".",
//             TileGame.Mur => "#",
//             _ => tile.ToString()[..1],
//         };
//     }

//     private static bool UpdateGame(TileGame[,] board, Direction direction, out ulong score)
//     {
//         int nRows = board.GetLength(0);
//         int nCols = board.GetLength(1);

//         score = 0;
//         bool hasUpdated = false;

//         // You shouldn't be dead at this point. We always check if you're dead at the end of the Update()

//         // Drop along row or column? true: process inner along row; false: process inner along column
//         bool isAlongRow = direction == Direction.Left || direction == Direction.Right;

//         // Should we process inner dimension in increasing index order?
//         bool isIncreasing = direction == Direction.Left || direction == Direction.Up;

//         int outterCount = isAlongRow ? nRows : nCols;
//         int innerCount = isAlongRow ? nCols : nRows;
//         int innerStart = isIncreasing ? 0 : innerCount - 1;
//         int innerEnd = isIncreasing ? innerCount - 1 : 0;

//         Func<int, int> drop = isIncreasing 
//             ? new Func<int, int>(innerIndex => innerIndex - 1) 
//             : new Func<int, int>(innerIndex => innerIndex + 1);

//         Func<int, int> reverseDrop = isIncreasing 
//             ? new Func<int, int>(innerIndex => innerIndex + 1) 
//             : new Func<int, int>(innerIndex => innerIndex - 1);

//         Func<TileGame[,], int, int, TileGame> getValue = isAlongRow
//             ? new Func<TileGame[,], int, int, TileGame>((x, i, j) => x[i, j])
//             : new Func<TileGame[,], int, int, TileGame>((x, i, j) => x[j, i]);

//         Action<TileGame[,], int, int, TileGame> setValue = isAlongRow
//             ? new Action<TileGame[,], int, int, TileGame>((x, i, j, v) => x[i, j] = v)
//             : new Action<TileGame[,], int, int, TileGame>((x, i, j, v) => x[j, i] = v);

//         Func<int, bool> innerCondition = index => Math.Min(innerStart, innerEnd) <= index && index <= Math.Max(innerStart, innerEnd);

//         for (int i = 0; i < outterCount; i++)
//         {
//             for (int j = innerStart; innerCondition(j); j = reverseDrop(j))
//             {
//                 if (ImmobileTiles.Contains(getValue(board, i, j)))
//                 {
//                     continue;
//                 }

//                 int newJ = j;
//                 do
//                 {
//                     newJ = drop(newJ);
//                 }
//                 // Continue probing along as long as we haven't hit the boundary and the new position isn't occupied
//                 while (innerCondition(newJ) && getValue(board, i, newJ) == 0);

//                 if (innerCondition(newJ) && getValue(board, i, newJ) == getValue(board, i, j))
//                 {
//                     // We did not hit the canvas boundary (we hit a node) AND no previous merge occurred AND the nodes' values are the same
//                     // Let's merge
//                     TileGame newValue = getValue(board, i, newJ);
//                     setValue(board, i, newJ, newValue);
//                     setValue(board, i, j, 0);

//                     hasUpdated = true;
//                     score += 10;
//                 }
//                 else
//                 {
//                     // Reached the boundary OR...
//                     // we hit a node with different value OR...
//                     // we hit a node with same value BUT a prevous merge had occurred
//                     // 
//                     // Simply stack along
//                     newJ = reverseDrop(newJ); // reverse back to its valid position
//                     if (newJ != j)
//                     {
//                         // there's an update
//                         hasUpdated = true;
//                     }

//                     TileGame value = getValue(board, i, j);
//                     setValue(board, i, j, 0);
//                     setValue(board, i, newJ, value);
//                 }
//             }
//         }

//         return hasUpdated;
//     }

//     public bool UpdateGame(Direction dir)
//     {
//         ulong score;
//         bool isUpdated = Game.UpdateGame(this.Board, dir, out score);
//         this.Score += score;
//         return isUpdated;
//     }

//     public bool Win(bool afficher)
//     {

//         List<TileGame> TileListBoard = new();
//         for (int iRow = 0; iRow < nRows; iRow++)
//         {
//             for (int iCol = 0; iCol < nCols; iCol++)
//             {
//                 if (PossibleTiles.Contains(Board[iRow, iCol]))
//                 {
//                     TileListBoard.Add(Board[iRow, iCol]);
//                 }
//             }
//         }

//         int nbDistinctPossiblesTiles = TileListBoard.Distinct().Count();

//         if(afficher) {
//             Console.WriteLine("Number of tiles : {0}", TileListBoard.Count);
//             Console.WriteLine("Goal : {0}", nbDistinctPossiblesTiles);
//         }

//         return TileListBoard.Count == nbDistinctPossiblesTiles;
//     }

//     public Game Clone() {
//         Game newGame = new(this.nRows, this.nCols);
//         newGame.Score = 0;
//         newGame.Board = (TileGame[,])Board.Clone();

//         return newGame; 
//     }

//     private List<Tuple<int, int>> EmptySlots() {
//         List<Tuple<int, int>> emptySlots = new();
//         for (int iRow = 0; iRow < nRows; iRow++)
//         {
//             for (int iCol = 0; iCol < nCols; iCol++)
//             {
//                 if (Board[iRow, iCol] == TileGame.Void)
//                 {
//                     emptySlots.Add(new Tuple<int, int>(iRow, iCol));
//                 }
//             }
//         }
//         return emptySlots;
//     }

//     private void InitializeBoard()
//     {
//         int[,] grid;
//         int[] init;

//         do
//         {
//             (grid, init) = GenerateBoard();
//         } while (!Flooded(grid, init));

//         // Afficher(grid);

//         for (int iRow = 0; iRow < nRows; iRow++)
//         {
//             for (int iCol = 0; iCol < nCols; iCol++)
//             {
//                 Board[iRow, iCol] = grid[iRow, iCol] switch
//                 {
//                     -1 => TileGame.Mur,
//                     _ => TileGame.Void,
//                 };
//             }
//         }
//     }


//     private (int[,], int[]) GenerateBoard()
//     {
//         int[,] grid = new int[nRows, nCols];
//         int[] init = new int[] {-1, -1};

//         int nWalls = 0;

//         do {
//             // 1/5ème de chance d'avoir un mur
//             for (int iRow = 0; iRow < nRows; iRow++)
//             {
//                 for (int iCol = 0; iCol < nCols; iCol++)
//                 {
//                     if(random.Next(0, 100) > 80) {
//                         grid[iRow, iCol] = -1;
//                         nWalls++;
//                     } else {
//                         grid[iRow, iCol] = 0;
//                     }
//                     if(nWalls - 2 > nCols * nRows / 2) {
//                         return (grid, init);
//                     }
//                 }
//             }
//         } while(EmptySlots().Count < (nRows * nCols) + 2 
//                 && EmptySlots().Count < nCols * nRows / 2);

//         // Recherche de la première cellule vide
//         for (int iRow = 0; iRow < nRows; iRow++)
//         {
//             for (int iCol = 0; iCol < nCols; iCol++)
//             {
//                 if (grid[iRow, iCol] == 0)
//                 {
//                     grid[iRow, iCol] = 1;
//                     init = new int[] {iRow, iCol};
//                     return (grid, init);
//                 }
//             }
//         }

//         return (grid, init);
//     }

//     private bool Flooded(int[,] grid, int[] init) {
//         Flood(grid, init);

//         for (int iRow = 0; iRow < this.Board.GetLength(0); iRow++)
//         {
//             for (int iCol = 0; iCol < this.Board.GetLength(1); iCol++)
//             {
//                 if (grid[iRow, iCol] != -1 && grid[iRow, iCol] != 1)
//                 {
//                     return false;
//                 }
//             }
//         }
//         return true;
//     }

//     private int[,] Flood(int[,] grid, int[] current)
//     {
//         List<int[]> next = new();

//         void AddNext(int row, int col)
//         {
//             if (row >= 0 && row < nRows && col >= 0 && col < nCols && grid[row, col] == 0)
//             {
//                 next.Add(new int[] { row, col });
//             }
//         }

//         AddNext(current[0] - 1, current[1]);
//         AddNext(current[0] + 1, current[1]);
//         AddNext(current[0], current[1] - 1);
//         AddNext(current[0], current[1] + 1);

//         foreach (int[] n in next)
//         {
//             grid[n[0], n[1]] = 1;
//         }

//         foreach (int[] n in next)
//         {
//             Flood(grid, n);
//         }

//         return grid;
//     }

//     private void PutInitialTiles()
//     {
//         List<Tuple<int, int>> emptySlots = EmptySlots();
//         List<TileGame> randSortPossibleTiles = PossibleTiles.OrderBy(x => random.Next()).ToList();
//         List<TileGame> chosenTiles = new();

//         int proba = 200;

//         foreach (TileGame possibleTile in randSortPossibleTiles) {
//             if(random.Next(0, 100) < proba) {
//                 proba /= 2;
//                 chosenTiles.Add(possibleTile);
//             }
//         }

//         bool movePossible = false;
//         foreach (var slot in emptySlots)
//         {
//             TileGame randTile = chosenTiles[random.Next(0, chosenTiles.Count)];
//             Board[slot.Item1, slot.Item2] = randTile;

//             if(!movePossible) {    
//                 if (slot.Item1 != 0 && Board[slot.Item1 - 1, slot.Item2] == Board[slot.Item1, slot.Item2])
//                     movePossible = true;

//                 if (slot.Item2 != 0 && Board[slot.Item1, slot.Item2 - 1] == Board[slot.Item1, slot.Item2])
//                     movePossible = true;
//             }
//         }

//         // Si aucune tuiles voisines ne sont identiques, on modifie le Board pour en avoir au moins une 
//         if (!movePossible)
//         {
//             Tuple<int, int> slotToChange = emptySlots[random.Next(0, emptySlots.Count)];

//             List<TileGame> neighbours = new();

//             void AddNeighbour(int row, int col)
//             {
//                 if (row >= 0 && row < nRows && col >= 0 && col < nCols && PossibleTiles.Contains(Board[row, col]))
//                 {
//                     neighbours.Add(Board[row, col]);
//                 }
//             }

//             AddNeighbour(slotToChange.Item1 - 1, slotToChange.Item2);
//             AddNeighbour(slotToChange.Item1 + 1, slotToChange.Item2);
//             AddNeighbour(slotToChange.Item1, slotToChange.Item2 - 1);
//             AddNeighbour(slotToChange.Item1, slotToChange.Item2 + 1);

//             TileGame chosenNeighbour = neighbours[random.Next(0, neighbours.Count)];
//             Board[slotToChange.Item1, slotToChange.Item2] = chosenNeighbour;
//         }
//     }

//     public enum Direction
//     {
//         Up,
//         Down,
//         Right,
//         Left,
//     }
// }