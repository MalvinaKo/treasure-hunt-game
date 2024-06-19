TreasureHunt();

static void TreasureHunt()
    {
    var gameRunning = true;

    while (gameRunning)
        {
        var (gridRows, gridColumns) = GetGridSize();
        var grid = InitializeGrid(gridRows, gridColumns);
        var numberOfTreasures = (int)(gridRows * gridColumns * 0.1);
        var numberOfObstacles = (int)(gridRows * gridColumns * 0.2);
        var (playerRow, playerColumn) = PlacePlayer(grid);
        var (monsterRow, monsterColumn) = PlaceMonster(grid);

        var treasureCounter = 0;
        Console.Clear();
        PlaceItems(numberOfTreasures, '$', grid);
        PlaceItems(numberOfObstacles, '|', grid);
        DisplayGrid(grid);

        RunTheGame(grid, numberOfTreasures, ref playerRow, ref playerColumn, ref monsterRow, ref monsterColumn,
            ref treasureCounter);

        gameRunning = RestartTheGame();
        }
    }

static void RunTheGame
(
    char[,] grid, int numberOfTreasures, ref int playerRow, ref int playerColumn, ref int monsterRow,
    ref int monsterColumn, ref int treasureCounter
)
    {
    ConsoleKeyInfo keyInfo;
    var gameActive = true;

    while (gameActive && (keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Escape)
        {
        (playerRow, playerColumn, treasureCounter) =
            MovePlayer(grid, playerRow, playerColumn, keyInfo, treasureCounter);
        (monsterRow, monsterColumn) = MoveMonster(grid, playerRow, playerColumn, monsterRow, monsterColumn);

        Console.Clear();
        DisplayGrid(grid);
        Console.WriteLine("Number of Treasures: " + numberOfTreasures);
        Console.WriteLine("Treasure Counter: " + treasureCounter);

        if (treasureCounter == numberOfTreasures)
            {
            Console.Clear();
            Console.WriteLine($"You found all {numberOfTreasures} treasures and win the game!");
            gameActive = false;
            }
        else if (monsterRow == playerRow && monsterColumn == playerColumn)
            {
            Console.Clear();
            Console.WriteLine("The monster caught you! Game over.");
            gameActive = false;
            }
        }
    }

static bool RestartTheGame()
    {
    Console.WriteLine("Press 'R' to restart the game or any other key to exit.");
    var restartKey = Console.ReadKey(true).Key;
    return restartKey == ConsoleKey.R;
    }

static (int height, int width) GetGridSize()
    {
    while (true)
        {
        Console.WriteLine("Enter row and column (for example 5x5):");
        var input = Console.ReadLine();
        var parts = input.Split('x');

        if (parts.Length == 2 && int.TryParse(parts[0], out var gridRows) &&
            int.TryParse(parts[1], out var gridColumns) && gridRows > 0 && gridColumns > 0)
            {
            return (height: gridRows, width: gridColumns);
            }

        Console.WriteLine(
            "Invalid input. Please enter the grid size in the format RxC, where R and C are positive integers.");
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

static int PlaceItems(int numberOfItems, char itemSymbol, char[,] grid)
    {
    var random = new Random();
    var gridRows = grid.GetLength(0);
    var gridColumns = grid.GetLength(1);
    for (var i = 0; i < numberOfItems; i++)
        {
        int row;
        int column;
        do
            {
            row = random.Next(gridRows);
            column = random.Next(gridColumns);
            } while (grid[row, column] == '$' || grid[row, column] == 'P');

        grid[row, column] = itemSymbol;
        }

    return numberOfItems;
    }

static (int row, int column) PlacePlayer(char[,] grid)
    {
    var random = new Random();
    var gridRows = grid.GetLength(0);
    var gridColumns = grid.GetLength(1);
    int playerRow, playerColumn;
    do
        {
        playerRow = random.Next(gridRows);
        playerColumn = random.Next(gridColumns);
        } while (grid[playerRow, playerColumn] == '$' || grid[playerRow, playerColumn] == 'M');

    grid[playerRow, playerColumn] = 'P';
    return (row: playerRow, column: playerColumn);
    }

static (int row, int column) PlaceMonster(char[,] grid)
    {
    var random = new Random();
    var gridRows = grid.GetLength(0);
    var gridColumns = grid.GetLength(1);
    int monsterRow;
    int monsterColumn;
    do
        {
        monsterRow = random.Next(gridRows);
        monsterColumn = random.Next(gridColumns);
        } while (grid[monsterRow, monsterColumn] == '$' || grid[monsterRow, monsterColumn] == 'P');

    grid[monsterRow, monsterColumn] = 'M';
    return (row: monsterRow, column: monsterColumn);
    }

static (int row, int column, int treasureCounter) MovePlayer
    (char[,] grid, int playerRow, int playerColumn, ConsoleKeyInfo keyInfo, int treasureCounter)
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

    if (grid[newRow, newColumn] != '|')
        {
        if (grid[newRow, newColumn] == '$')
            {
            treasureCounter++;
            }

        grid[playerRow, playerColumn] = '.';
        grid[newRow, newColumn] = 'P';
        playerRow = newRow;
        playerColumn = newColumn;
        }

    return (row: playerRow, column: playerColumn, treasureCounter);
    }

static (int row, int column) MoveMonster
    (char[,] grid, int playerRow, int playerColumn, int monsterRow, int monsterColumn)
    {
    var random = new Random();
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
        if (grid[monsterNewRow, monsterNewColumn] != '|' && grid[monsterNewRow, monsterNewColumn] != '$')
            {
            grid[monsterRow, monsterColumn] = '.';
            grid[monsterNewRow, monsterNewColumn] = 'M';
            monsterRow = monsterNewRow;
            monsterColumn = monsterNewColumn;
            }
        }

    return (row: monsterRow, column: monsterColumn);
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