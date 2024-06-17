static void TreasureHunt()
{
    bool gameRunning = true;

    while (gameRunning)
    {
        var (gridRows, gridColumns) = GetGridSize();
        var grid = InitializeGrid(gridRows, gridColumns);
        var numberOfTreasures = PlaceTreasures(grid);
        var numberOfObstacles = PlaceObstacles(grid);
        var (playerRow, playerColumn) = PlacePlayer(grid);
        var (monsterRow, monsterColumn) = PlaceMonster(grid);

        var treasureCounter = 0;
        Console.Clear();
        DisplayGrid(grid);

        ConsoleKeyInfo keyInfo;
        bool gameActive = true;

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

        Console.WriteLine("Press 'R' to restart the game or any other key to exit.");
        var restartKey = Console.ReadKey(true).Key;
        if (restartKey != ConsoleKey.R)
        {
            gameRunning = false;
        }
    }
}

static (int, int) GetGridSize()
{
    Console.WriteLine("Enter row and column (for example 5x5):");
    var input = Console.ReadLine();
    var parts = input.Split('x');
    var gridRows = int.Parse(parts[0]);
    var gridColumns = int.Parse(parts[1]);
    return (gridRows, gridColumns);
}

static char[,] InitializeGrid(int gridRows, int gridColumns)
{
    var grid = new char[gridRows, gridColumns];
    for (var r = 0; r < gridRows; r++)
        for (var c = 0; c < gridColumns; c++)
            grid[r, c] = '.';
    return grid;
}

static int PlaceTreasures(char[,] grid)
{
    var random = new Random();
    var gridRows = grid.GetLength(0);
    var gridColumns = grid.GetLength(1);
    var numberOfTreasures = random.Next(2, gridRows * gridColumns / 10);
    for (var i = 0; i < numberOfTreasures; i++)
    {
        int row, column;
        do
        {
            row = random.Next(gridRows);
            column = random.Next(gridColumns);
        } while (grid[row, column] == '$' || grid[row, column] == 'P');

        grid[row, column] = '$';
    }

    return numberOfTreasures;
}

static int PlaceObstacles(char[,] grid)
{
    var random = new Random();
    var gridRows = grid.GetLength(0);
    var gridColumns = grid.GetLength(1);
    var numberOfObstacles = gridRows * gridColumns / 20;
    for (var i = 0; i < numberOfObstacles; i++)
    {
        int row, column;
        do
        {
            row = random.Next(gridRows);
            column = random.Next(gridColumns);
        } while (grid[row, column] == '$' || grid[row, column] == 'P');

        grid[row, column] = '|';
    }

    return numberOfObstacles;
}

static (int, int) PlacePlayer(char[,] grid)
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
    return (playerRow, playerColumn);
}

static (int, int) PlaceMonster(char[,] grid)
{
    var random = new Random();
    var gridRows = grid.GetLength(0);
    var gridColumns = grid.GetLength(1);
    int monsterRow, monsterColumn;
    do
    {
        monsterRow = random.Next(gridRows);
        monsterColumn = random.Next(gridColumns);
    } while (grid[monsterRow, monsterColumn] == '$' || grid[monsterRow, monsterColumn] == 'P');

    grid[monsterRow, monsterColumn] = 'M';
    return (monsterRow, monsterColumn);
}

static (int, int, int) MovePlayer(char[,] grid, int playerRow, int playerColumn, ConsoleKeyInfo keyInfo,
    int treasureCounter)
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
        if (grid[newRow, newColumn] == '$') treasureCounter++;

        grid[playerRow, playerColumn] = '.';
        grid[newRow, newColumn] = 'P';
        playerRow = newRow;
        playerColumn = newColumn;
    }

    return (playerRow, playerColumn, treasureCounter);
}

static (int, int) MoveMonster(char[,] grid, int playerRow, int playerColumn, int monsterRow, int monsterColumn)
{
    var random = new Random();
    var monsterNewRow = monsterRow;
    var monsterNewColumn = monsterColumn;

    if (random.Next(2) == 0)
    {
        if (playerRow < monsterRow) monsterNewRow--;
        else if (playerRow > monsterRow) monsterNewRow++;
    }
    else
    {
        if (playerColumn < monsterColumn) monsterNewColumn--;
        else if (playerColumn > monsterColumn) monsterNewColumn++;
    }

    if (monsterNewRow >= 0 && monsterNewRow < grid.GetLength(0) && monsterNewColumn >= 0 &&
        monsterNewColumn < grid.GetLength(1))
        if (grid[monsterNewRow, monsterNewColumn] != '|' && grid[monsterNewRow, monsterNewColumn] != '$')
        {
            grid[monsterRow, monsterColumn] = '.';
            grid[monsterNewRow, monsterNewColumn] = 'M';
            monsterRow = monsterNewRow;
            monsterColumn = monsterNewColumn;
        }

    return (monsterRow, monsterColumn);
}

static void DisplayGrid(char[,] grid)
{
    var rows = grid.GetLength(0);
    var columns = grid.GetLength(1);

    for (var r = 0; r < rows; r++)
    {
        for (var c = 0; c < columns; c++) Console.Write(grid[r, c] + " ");
        Console.WriteLine();
    }
}

TreasureHunt();
