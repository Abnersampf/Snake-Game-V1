class Program
{
    // Game size (including edges);
    // 10 x 20 minimum recommended and 20 x 40 ideal recommended (max size deppends of your computer).
    static int lines = 15;
    static int columns = 30;

    // Tracking variables, used to store and track some body parts positions.
    static int[] tracker = new int[2];
    static int[] trackLastMove = new int[2];
    static int[] trackFirstMove = new int[2];

    // Number of body parts (it will be incremented automatically).
    static int bodyParts = 1;

    // exit: true = new game starts | false = the game ends;
    static bool exit = true;
    static bool status = true; //-> true = the match keep running | false = the match ends (even if you win).

    // This variable represents the number of the 2nd body part;
    static int n = 2;

    // The name speaks for itself;
    static int record = 0;
    static int points = 0;
    // This variable represents the maximum number of points that is possible to make during the match.
    static int maxNumberPoints = (lines - 2) * (columns - 2) - 1;

    // lower number means more speed (0 is lightspeed lol).
    static int speed;

    // Used to store the current pressed key.
    static char key;

    // Initialize the 2d array (background) with edge, snake head and apple.
    // Note:
    // -2 = edge;
    // -1 = apple;
    //  0 = playable area;
    //  1 = snake head;
    //  2 to maxNumberPoints = other body parts.
    static void fill2dArray(int[,] _2dArray, int[] apple)
    {
        Random r = new Random();
        for (int i = 0; i < lines; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Delimits the edges;
                if (i == 0 || i == lines - 1 || j == 0 || j == columns - 1)
                {
                    _2dArray[i, j] = -2;
                }
                // Fill the rest of the space with zeros (the playable area);
                else
                {
                    _2dArray[i, j] = 0;
                }
            }
        }
        // Put the number 1 (head) in the center of the board.
        int l = ((lines - 2) / 2) + 1;
        int c = ((columns - 2) / 2) + 1;
        _2dArray[l, c] = 1;
        // Choose a direction for the first head movement (number 1).
        l = r.Next(0, 4);
        if (l == 0)
        {
            key = 'w';
        }
        else if (l == 1)
        {
            key = 'a';
        }
        else if (l == 2)
        {
            key = 's';
        }
        else if (l == 3)
        {
            key = 'd';
        }
        // Find a random position to place the apple.
        do
        {
            apple[0] = r.Next(1, lines - 1);
            apple[1] = r.Next(1, columns - 1);
        } while (_2dArray[apple[0], apple[1]] != 0);
        _2dArray[apple[0], apple[1]] = -1;
    }
    static void Main()
    {
        // Variable used just to one iteration
        int c = 0;
        // This array store two numbers (one in each position). These numbers...
        // ...are used to indicate where the apple (number -1) is in the 2d array (background).
        int[] apple = new int[2];
        // 2 dimensional array that store all numbers
        int[,] background = new int[lines, columns];

        // as long as the user wants to continue playing.
        while (exit)
        {
            points = 0;
            bodyParts = 1;

            fill2dArray(background, apple);

            // Get the position of the head.
            findNumberOne(background);
            trackFirstMove[0] = tracker[0];
            trackFirstMove[1] = tracker[1];

            // Loop that is responsible for every screen update and where practically all functions take place.
            while (status)
            {
                // Counts down 3 seconds.
                if (c == 0)
                {
                    Console.WriteLine("The game will start in 3");
                    drawGame(background);
                    Thread.Sleep(1000);
                    Console.Clear();
                    Console.WriteLine("The game will start in 2");
                    drawGame(background);
                    Thread.Sleep(1000);
                    Console.Clear();
                    Console.WriteLine("The game will start in 1");
                    drawGame(background);
                    Thread.Sleep(1000);
                    Console.Clear();
                    c = 1;
                }
                Console.WriteLine("Points: " + points);
                // The name is self explanatory.
                drawGame(background);
                // Checks to see if a key has been pressed, if so, it reads and stores it.
                if (Console.KeyAvailable)
                {
                    key = Console.ReadKey(true).KeyChar;
                }
                // Find the previous position where the last body part was.
                findLastNumber(background);
                trackLastMove[0] = tracker[0];
                trackLastMove[1] = tracker[1];
                // If the player tries to go back he blocks, if not, he allows the player to move.
                if (allowMove(background))
                {
                    moveNumberOne(background);
                }
                // Checks for collision with edges.
                gameOver(background, key);
                // Checks for collision with the apple.
                appleColision(background, apple);
                if (points == maxNumberPoints)
                {
                    status = false;
                }
                // Pause for 100ms to let the image stay on the screen before clearing the screen and drawing the game again.
                Thread.Sleep(speed);
                Console.Clear();
            }
            // When the match ends:
            // if you beat your record
            if (points > record)
            {
                record = points;
            }
            Console.WriteLine("Your Record: " + record);
            Console.WriteLine("Points: " + points);
            // Checks if it was a win or a loss (there is only one way to win, which is when all the points are reached)
            if (points == maxNumberPoints)
            {
                endScreen("You Win");
            }
            else
            {
                endScreen("You Lose");
            }
            // Asks the user if he wants to keep playing
            Console.Write("\nWant to try one more time ?\n[y = YES / n = NO]: ");
            Thread.Sleep(1000);
            key = Console.ReadKey().KeyChar;
            while (key != 'y' && key != 'n')
            {
                Console.Write("\nInvalid option, try agin: ");
                key = Console.ReadKey().KeyChar;
            }
            if (key == 'y')
            {
                exit = true;
                status = true;
                Console.Clear();
            }
            else
            {
                exit = false;
                Console.WriteLine();
            }
        }
    }
    //This function is for drawing the game, basically it represents each...
    //...number in the 2d vector(background) in a different way.
    static void drawGame(int[,] _2dArray)
    {
        for (int l = 0; l < lines; l++)
        {
            for (int c = 0; c < columns; c++)
            {
                // If this position of the array is occupied by the number -1 (apple),...
                //...the 'smile' character will be printed on the screen.
                if (_2dArray[l, c] == -1)
                {
                    Console.Write('☺');
                }
                // If this position of the array has any number between 1 (head) and...
                // ...the maximum number of points (which coincidentally is the maximum number of parts the body can have),...
                // ...the 'black square' character will be shown on the screen.
                else if (_2dArray[l, c] >= 1 && _2dArray[l, c] <= maxNumberPoints)
                {
                    Console.Write('■');
                }
                // If this position of the array is occupied by the number -2 (edge),...
                //...the 'x' character will be printed on the screen.
                else if (_2dArray[l, c] == -2)
                {
                    Console.Write('x');
                }
                // Any other position will be filled by dots
                else
                {
                    Console.Write('.');
                }
            }
            Console.WriteLine();
        }
    }
    // Find the position (line and columns) of the head (number 1) and assign these values to...
    //... the 'tracker' array. tracker[0] recives the position of the line and tracker[1] recives...
    //... the position of the column.
    static void findNumberOne(int[,] _2dArray)
    {
        for (int l = 0; l < lines; l++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (_2dArray[l, c] == 1)
                {
                    tracker[0] = l;
                    tracker[1] = c;
                }
            }
        }
    }
    // The name is self explanatory
    static void findLastNumber(int[,] _2dArray)
    {
        for (int l = 0; l < lines; l++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (_2dArray[l, c] == bodyParts)
                {
                    tracker[0] = l;
                    tracker[1] = c;
                }
            }
        }
    }
    // Add the 2nd body part (head is number 1) to the background array (background and _2dArray are the same array);
    static void addFirstPart(int[,] _2dArray)
    {
        findNumberOne(_2dArray);
        if (bodyParts == 2)
        {
            //Adds the number 2 (second part of the body) just behind the head (number 1), depending on...
            //...the last key pressed. This control is used, so that the second part of the body is not added...
            //...to the incorrect side of the head.
            if (key == 'w')
            {
                _2dArray[tracker[0] + 1, tracker[1]] = 2;
            }
            else if (key == 'a')
            {
                _2dArray[tracker[0], tracker[1] + 1] = 2;
            }
            else if (key == 's')
            {
                _2dArray[tracker[0] - 1, tracker[1]] = 2;
            }
            else if (key == 'd')
            {
                _2dArray[tracker[0], tracker[1] - 1] = 2;
            }
        }
        else
        {
            // The rest of the body is added.
            addRest(_2dArray);
        }
    }
    // Adds another body part, based on the previous position the last body part was in.
    static void addRest(int[,] _2dArray)
    {
        _2dArray[trackLastMove[0], trackLastMove[1]] = bodyParts;
    }
    // Move the head based on the current direction
    // Note:
    //    'w' = up;
    //    'a' = left;
    //    's' = down;
    //    'd' = right;
    static void moveNumberOne(int[,] _2dArray)
    {
        findNumberOne(_2dArray);
        if (key == 'w')
        {
            // tracker[0] - 1 and tracker[1] means one line above the current head position;
            // REMEMBER: tracker[0] = line, and tracker[1] = column;
            _2dArray[tracker[0] - 1, tracker[1]] = 1;
            // The head moves faster in 'up' and 'down' directions, so it is necessary to slow down.
            // Note: higher speed number = slower snake;
            //       lower speed number = faster snake;
            //       So, basically 160 is slower than 100.
            speed = 160;
        }
        else if (key == 'a')
        {
            // tracker[0] and tracker[1] - 1 means one column ahead the current head position;
            // The number 1(head) is placed in its new position.
            _2dArray[tracker[0], tracker[1] - 1] = 1;
            speed = 100;
        }
        else if (key == 's')
        {
            // tracker[0] + 1 and tracker[1] means one line below the current head position;
            // The number 1(head) is placed in its new position.
            _2dArray[tracker[0] + 1, tracker[1]] = 1;
            speed = 160;
        }
        else if (key == 'd')
        {
            // tracker[0] and tracker[1] - 1 means one column behind the current head position;
            // The number 1(head) is placed in its new position.
            _2dArray[tracker[0], tracker[1] + 1] = 1;
            speed = 100;
        }
        // The old head position is filled with 0.
        _2dArray[tracker[0], tracker[1]] = 0;
        // If the body has more than 1 part:
        if (bodyParts > 1)
        {
            moveRest(_2dArray);
        }
    }
    static void moveRest(int[,] _2dArray)
    {
        for (int l = 0; l < lines; l++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (_2dArray[l, c] == n)
                {
                    // Move/place the number 2(2nd body position) to the old head position.
                    _2dArray[tracker[0], tracker[1]] = n;
                    // Now that the number 2 has/is in a new position, we fill its old position with a 0.
                    _2dArray[l, c] = 0;
                    // And now, we put the values of the position where the current number was found into the 'trcker' array.
                    tracker[0] = l;
                    tracker[1] = c;
                    // The value of n is increased, because now we will be looking for the next body part.
                    n++;
                    // We reset the row and column values to 0, so we can search for the next number;
                    // IMPORTANT: It is necessary to reset the values, because if the next body part were...
                    // ...in a position that the for loop has already gone through, it would not be able to find it.
                    l = c = 0;
                }
            }
        }
        // Set the n value with the number of the 2nd body part, because all the 'rest' of the body...
        //...starts in position 2
        n = 2;
    }
    static void applePosition(int[,] _2dArray, int[] apple)
    {
        int avaliableSpace = 0;
        Random r = new Random();
        // Checks if there is an available position (number 0) for the apple (number -1) to be placed.
        // REMEMBER: 
        // -2 = edge;
        // -1 = apple;
        //  0 = playable area;
        //  1 = snake head;
        //  2 to maxNumberPoints = other body parts.
        for (int l = 0; l < lines - 1; l++)
        {
            for (int c = 0; c < columns - 1; c++)
            {
                if (_2dArray[l, c] == 0)
                {
                    avaliableSpace++;
                }
            }
        }
        // If there is a free space.
        if (avaliableSpace != 0)
        {
            do
            {
                apple[0] = r.Next(1, lines - 1);
                apple[1] = r.Next(1, columns - 1);
            } while (_2dArray[apple[0], apple[1]] != 0);
            _2dArray[apple[0], apple[1]] = -1;
        }
        // If there is no more room for the apple to be placed, it is because the snake's body...
        // ...is taking up all the space, and this means that the game is over.
        else
        {
            status = false;
            points = maxNumberPoints;
        }
    }
    static void appleColision(int[,] _2dArray, int[] apple)
    {
        // Find the head position and assign these values to tracker[0] and tracker[1];
        // REMEMBER: tracker[0] = line, and tracker[1] = column;
        findNumberOne(_2dArray);
        // If the head is in the same position as the apple, then there was a collision (basically the snake collected the apple)
        if (apple[0] == tracker[0] && apple[1] == tracker[1])
        {
            points++;
            bodyParts++;
            addFirstPart(_2dArray);
            applePosition(_2dArray, apple);
        }
    }
    // Checks if the user is trying to go back or if there was a collision with your own body.
    static bool allowMove(int[,] _2dArray)
    {
        findNumberOne(_2dArray);
        // Checks if the user is trying to go back, analyzing if the position he would go to is...
        // ...the same as the one he was in the previous movement.
        // Note: the trackFirstMove vector is responsible for storing the previous position where the head was;
        //       trackFirstMove[0] = line and trackFirstMove[1] = column.
        if (key == 'w' && tracker[0] - 1 == trackFirstMove[0] && tracker[1] == trackFirstMove[1])
        {
            // If he is trying to go back, the new direction will be opposite to the one he just tried to go in,...
            // ...so he will keep going forward as if nothing happened.
            key = 's';
            // return false indicates that it is not possible to move in the desired direction.
            return false;
        }
        // The same thing happens here
        else if (key == 'a' && tracker[0] == trackFirstMove[0] && tracker[1] - 1 == trackFirstMove[1])
        {
            key = 'd';
            return false;
        }
        // The same thing happens here
        else if (key == 's' && tracker[0] + 1 == trackFirstMove[0] && tracker[1] == trackFirstMove[1])
        {
            key = 'w';
            return false;
        }
        // The same thing happens here
        else if (key == 'd' && tracker[0] == trackFirstMove[0] && tracker[1] + 1 == trackFirstMove[1])
        {
            key = 'a';
            return false;
        }
        // If he is not trying to turn back:
        else
        {
            // Checks for a collision with your own body;
            // Basically it checks if the position you are trying to go to already belongs to a part of your own body.
            // REMEMBER:
            //  number 1 = snake head;
            //  number 2 to maxNumberPoints = other body parts.
            if (key == 'w' && _2dArray[tracker[0] - 1, tracker[1]] >= 2 && _2dArray[tracker[0] - 1, tracker[1]] <= maxNumberPoints)
            {
                // If there was a collision the match ends
                status = false;
                return false;
            }
            // The same thing happens here
            else if (key == 'a' && _2dArray[tracker[0], tracker[1] - 1] >= 2 && _2dArray[tracker[0], tracker[1] - 1] <= maxNumberPoints)
            {
                status = false;
                return false;
            }
            // The same thing happens here
            else if (key == 's' && _2dArray[tracker[0] + 1, tracker[1]] >= 2 && _2dArray[tracker[0] + 1, tracker[1]] <= maxNumberPoints)
            {
                status = false;
                return false;
            }
            // The same thing happens here
            else if (key == 'd' && _2dArray[tracker[0], tracker[1] + 1] >= 2 && _2dArray[tracker[0], tracker[1] + 1] <= maxNumberPoints)
            {
                status = false;
                return false;
            }
            // If he is not trying to turn back and there was no collision:
            else
            {
                // trackFirstMove[0] and trackFirstMove[1] store the new head psosition
                trackFirstMove[0] = tracker[0];
                trackFirstMove[1] = tracker[1];
                // return true because it's possible to move
                return true;
            }
        }
    }
    // Checks for collision with edges.
    static void gameOver(int[,] _2dArray, char key)
    {
        findNumberOne(_2dArray);
        if (tracker[0] == 0 || tracker[0] == lines - 1 || tracker[1] == 0 || tracker[1] == columns - 1)
        {
            status = false;
        }
    }
    // Presents a personalized message for the endgame (both if you win and if you lose)
    static void endScreen(string text)
    {
        int num = (columns - text.Length) / 2;
        for (int l = 0; l < lines; l++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (l == 0 || l == lines - 1 || c == 0 || c == columns - 1)
                {
                    Console.Write('x');
                }
                else if (lines / 2 == l && c == num)
                {
                    Console.Write(text);
                    c += text.Length - 1;
                }
                else
                {
                    Console.Write('.');
                }
            }
            Console.WriteLine();
        }
    }
}
