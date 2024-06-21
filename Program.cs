static void TreasureHunt()
{
    var (gridRows, gridColumns) = GetGridSize();
    var grid = InitializeGrid(gridRows, gridColumns);
    var treasureSymbol = '$';
    var obstacleSymbol = '|';
    var playerSymbol = 'P';
    var monsterSymbol = 'M';
    var numberOfTreasures = (int)(gridRows * gridColumns * 0.1);
    var numberOfObstacles = (int)(gridRows * gridColumns * 0.2);
    var (playerRow, playerColumn) = PlacePlayer(treasureSymbol, playerSymbol, monsterSymbol, grid);
    var (monsterRow, monsterColumn) = PlaceMonster(treasureSymbol, playerSymbol, monsterSymbol, grid);
    var treasureCounter = 0;

    Console.Clear();
    PlaceTreasures(treasureSymbol, playerSymbol, numberOfTreasures, grid);
    PlaceObstacles(obstacleSymbol, treasureSymbol, playerSymbol, numberOfObstacles, grid);
    DisplayGrid(grid);

    ConsoleKeyInfo keyInfo;
    while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Escape)
        {
        (playerRow, playerColumn, treasureCounter) = MovePlayer(treasureSymbol, obstacleSymbol, playerSymbol, grid,
            playerRow, playerColumn, keyInfo, treasureCounter);
        (monsterRow, monsterColumn) = MoveMonster(treasureSymbol, obstacleSymbol, monsterSymbol, grid, playerRow,
            playerColumn, monsterRow, monsterColumn);

        Console.Clear();
        DisplayGrid(grid);
        Console.WriteLine("Number of Treasures: " + numberOfTreasures);
        Console.WriteLine("Treasure Counter: " + treasureCounter);

        if (treasureCounter == numberOfTreasures)
            {
            Console.Clear();
            Console.WriteLine($"You found all {numberOfTreasures} treasures and win the game!");
            break;
            }

        if (monsterRow == playerRow && monsterColumn == playerColumn)
            {
            Console.Clear();
            Console.WriteLine("The monster caught you! Game over.");
            TreasureHunt();
            }
        }
}

static (int, int) GetGridSize()
{
    while (true)
        {
        Console.WriteLine("Enter row and column (for example 5x5):");
        var input = Console.ReadLine();
        var parts = input.Split('x');
        
        if (parts.Length != 2)
            {
            Console.WriteLine("Invalid format. Please enter in the format 'rowxcolumn' (e.g., 5x5).");
            continue;
            }
        
        if (!int.TryParse(parts[0], out int gridRows) || !int.TryParse(parts[1], out int gridColumns))
            {
            Console.WriteLine("Invalid numbers. Please enter valid integers for rows and columns.");
            continue;
            }
        
        if (gridRows <= 0 || gridColumns <= 0)
            {
            Console.WriteLine("Rows and columns must be positive integers. Please try again.");
            continue;
            }
        
        return (gridRows, gridColumns);
        }
}

static char[,] InitializeGrid(int gridRows, int gridColumns)
{
    var grid = new char[gridRows, gridColumns];
    for (var r = 0; r < gridRows; r++)
    for (var c = 0; c < gridColumns; c++)
        {
        grid[r, c] = '.';
        }

    return grid;
}

static int PlaceTreasures(char treasureSymbol, char playerSymbol, int numberOfTreasures, char[,] grid)
{
    var random = Random.Shared;
    var gridRows = grid.GetLength(0);
    var gridColumns = grid.GetLength(1);
    for (var i = 0; i < numberOfTreasures; i++)
        {
        int row;
        int column;
        do
            {
            row = random.Next(gridRows);
            column = random.Next(gridColumns);
            } while (grid[row, column] == treasureSymbol || grid[row, column] == playerSymbol);

        grid[row, column] = treasureSymbol;
        }

    return numberOfTreasures;
}

static int PlaceObstacles(
    char obstacleSymbol, char treasureSymbol, char playerSymbol, int numberOfObstacles, char[,] grid)
{
    var random = Random.Shared;
    var gridRows = grid.GetLength(0);
    var gridColumns = grid.GetLength(1);
    for (var i = 0; i < numberOfObstacles; i++)
        {
        int row;
        int column;
        do
            {
            row = random.Next(gridRows);
            column = random.Next(gridColumns);
            } while (grid[row, column] == treasureSymbol || grid[row, column] == playerSymbol);

        grid[row, column] = obstacleSymbol;
        }

    return numberOfObstacles;
}

static (int, int) PlacePlayer(char treasureSymbol, char playerSymbol, char monsterSymbol, char[,] grid)
{
    var random = Random.Shared;
    var gridRows = grid.GetLength(0);
    var gridColumns = grid.GetLength(1);
    int playerRow, playerColumn;
    do
        {
        playerRow = random.Next(gridRows);
        playerColumn = random.Next(gridColumns);
        } while (grid[playerRow, playerColumn] == treasureSymbol || grid[playerRow, playerColumn] == monsterSymbol);

    grid[playerRow, playerColumn] = playerSymbol;
    return (playerRow, playerColumn);
}

static (int, int) PlaceMonster(char treasureSymbol, char playerSymbol, char monsterSymbol, char[,] grid)
{
    var random = Random.Shared;
    var gridRows = grid.GetLength(0);
    var gridColumns = grid.GetLength(1);
    int monsterRow, monsterColumn;
    do
        {
        monsterRow = random.Next(gridRows);
        monsterColumn = random.Next(gridColumns);
        } while (grid[monsterRow, monsterColumn] == treasureSymbol || grid[monsterRow, monsterColumn] == playerSymbol);

    grid[monsterRow, monsterColumn] = monsterSymbol;
    return (monsterRow, monsterColumn);
}

static (int, int, int) MovePlayer(
    char treasureSymbol, char obstacleSymbol, char playerSymbol, char[,] grid, int playerRow, int playerColumn,
    ConsoleKeyInfo keyInfo, int treasureCounter)
{
    var newRow = playerRow;
    var newColumn = playerColumn;

    switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:
                newRow = playerRow > 0 ? playerRow - 1 : playerRow;
                break;
            case ConsoleKey.DownArrow:
                newRow = playerRow < grid.GetLength(0) - 1 ? playerRow + 1 : playerRow;
                break;
            case ConsoleKey.LeftArrow:
                newColumn = playerColumn > 0 ? playerColumn - 1 : playerColumn;
                break;
            case ConsoleKey.RightArrow:
                newColumn = playerColumn < grid.GetLength(1) - 1 ? playerColumn + 1 : playerColumn;
                break;
        }

    if (grid[newRow, newColumn] != obstacleSymbol)
        {
        if (grid[newRow, newColumn] == treasureSymbol)
            {
            treasureCounter++;
            }

        grid[playerRow, playerColumn] = '.';
        grid[newRow, newColumn] = playerSymbol;
        playerRow = newRow;
        playerColumn = newColumn;
        }

    return (playerRow, playerColumn, treasureCounter);
}

static (int, int) MoveMonster(
    char treasureSymbol, char obstacleSymbol, char monsterSymbol, char[,] grid, int playerRow, int playerColumn,
    int monsterRow, int monsterColumn)
{
    var random = Random.Shared;
    var monsterNewRow = monsterRow;
    var monsterNewColumn = monsterColumn;

    if (random.Next(2) == 0)
        {
        if (playerRow < monsterRow)
            {
            monsterNewRow--;
            }
        else if (playerRow > monsterRow)
            {
            monsterNewRow++;
            }
        }
    else
        {
        if (playerColumn < monsterColumn)
            {
            monsterNewColumn--;
            }
        else if (playerColumn > monsterColumn)
            {
            monsterNewColumn++;
            }
        }

    if (monsterNewRow >= 0 && monsterNewRow < grid.GetLength(0) && monsterNewColumn >= 0 &&
        monsterNewColumn < grid.GetLength(1))
        {
        if (grid[monsterNewRow, monsterNewColumn] != obstacleSymbol &&
            grid[monsterNewRow, monsterNewColumn] != treasureSymbol)
            {
            grid[monsterRow, monsterColumn] = '.';
            grid[monsterNewRow, monsterNewColumn] = monsterSymbol;
            monsterRow = monsterNewRow;
            monsterColumn = monsterNewColumn;
            }
        }

    return (monsterRow, monsterColumn);
}

static void DisplayGrid(char[,] grid)
{
    var rows = grid.GetLength(0);
    var columns = grid.GetLength(1);

    for (var r = 0; r < rows; r++)
        {
        for (var c = 0; c < columns; c++)
            {
            Console.Write(grid[r, c] + " ");
            }

        Console.WriteLine();
        }
}

TreasureHunt();