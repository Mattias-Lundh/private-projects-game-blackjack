using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{

    class Program
    {
        static EmbededConsoleGraphics Display = new EmbededConsoleGraphics { };
        static void Main(string[] args)
        {

            Display.Heading();
            Console.WriteLine("Press any Key to start");
            Console.ReadKey();
            PlayGame();

        }

        public static void PlayGame()
        {
            Game blackJackSession = new Game { };
            Setup(blackJackSession);

            while (true)
            {
                Display.GetReady(blackJackSession);
                bool validBet = false;
                while (!validBet)
                {
                    try
                    {
                        Display.PlaceBets(blackJackSession);
                        validBet = true;
                    }
                    catch (InsufficientFundsException)
                    {
                        Console.WriteLine("You dont have enough credits");
                    }
                }
                validBet = true;                
                foreach(Player player in blackJackSession.Participants)
                {
                    player.ClearCards();
                }
                blackJackSession.DealCards();
                Display.ShowBoardState(blackJackSession);
                if (Display.InsuranceEvent(blackJackSession))
                {
                    //new round
                }
                Display.PlayerActions(blackJackSession);




            }
        }


        public static void Setup(Game game)
        {
            string input = "";
            int playerCount = 0;
            while (!int.TryParse(input, out playerCount))
            {
                Console.WriteLine("how many players?");
                input = Console.ReadLine();
            }

            for (int i = 0; i < playerCount; i++)
            {

                Console.WriteLine("Enter Player" + (i + 1) + "`s Name");
                input = Console.ReadLine();
                if (input == "")
                {
                    input = "Plarer " + i;
                }
                game.AddCustomer(input);
            }
        }

    }
    class Player
    {
        public string Name { get; set; }
        public List<List<Card>> Hands { get; set; }
        public int Position { get; set; }
        public int Insurance { get; set; }
        public double Bet { get; set; }
        public bool[] HandsAreSet { get; set; }
        public int ActiveHands
        {
            get
            {
                int handsCount = 0;
                foreach (List<Card> list in Hands)
                {
                    if (list.Count > 0)
                    {
                        handsCount++;
                    }
                }
                return handsCount;
            }
        }
        private double currency;
        public double Currency
        {
            get { return currency; }
            set
            {
                if (value + currency < 0)
                {
                    throw new InsufficientFundsException();
                }
                else
                {
                    currency = value;
                }
            }
        }
        public string Label
        {
            get
            {
                string result = "||" + Name.ToUpper() + "||";

                if (Hands[0].Count == 0)
                {
                    return result + " Surrendered";
                }

                for (int i = 0; i < ActiveHands; i++)
                {
                    if (EvaluateHand(i) > 21.5)
                    {
                        result += " Hand" + (i + 1) + " Value: Bust";
                    }
                    else if (EvaluateHand(i) == 21.5)
                    {
                        result += " Hand" + (i + 1) + " Value: BLACKJACK!!";
                    }
                    else
                    {
                        result += " Hand" + (i + 1) + " Value: " + EvaluateHand(i);
                    }

                }
                result += " Bet: " + Bet;
                return result;
            }
        }
        public Player()
        {
            Hands = new List<List<Card>> { };
            Hands.Add(new List<Card> { });
            Hands.Add(new List<Card> { });
            Hands.Add(new List<Card> { });
            Hands.Add(new List<Card> { });
            HandsAreSet = new bool[] { true, true, true, true };
        }
        public int NextAvaliableHand()
        {
            int result = -1;

            for (int i = 0; i < 4; i++)
            {
                if (!HandsAreSet[i])
                {
                    return i;
                }
            }

            return result;
        }
        public void Hit(int hand, Card card)
        {
            Hands[hand].Add(card);
        }

        public void ClearCards()
        {
            foreach (List<Card> hand in Hands)
            {
                hand.Clear();
            }
        }

        public double EvaluateHand(int hand)
        {
            double value = 0;
            int ace = 0;

            foreach (Card card in Hands[hand])
            {
                if (card.Value == 1)
                {
                    ace += 1;
                    value += card.Points + 10;
                }
                else
                {
                    value += card.Points;
                }
            }

            if (value == 21 && Hands[hand].Count == 2)
            {
                value += 0.5;
            }

            for (int i = ace; i > 0; i--)
            {
                if (value > 21.5)
                {
                    double newValue = 0;
                    int aceConverted = 0;
                    foreach (Card card in Hands[hand])
                    {

                        if (card.Points == 1 && aceConverted <= ace - i)
                        {
                            aceConverted += 1;
                            newValue += card.Points;
                        }
                        else
                        {
                            newValue += card.Points + 10;

                        }
                    }
                    if (newValue < 22)
                    {
                        value = newValue;
                    }
                }
            }

            return value;
        }

    }

    class Dealer : Player
    {
        public Dealer()
        {
            Position = 0;
            Name = "Dealer";
        }
    }

    class Customer : Player
    {
        public Customer()
        {
            Currency = 1000;
        }
    }

    class EmbededConsoleGraphics
    {
        private static string[] Welcome =
        {
            "█║█║█#█▀#█║#█▀#█▀█#█▀█▀█#█▀",
            "█║█║█#█▀#█║#█║#█║█#█║█║█#█▀",
            "█▄█▄█#█▄#█▄#█▄#█▄█#█║█║█#█▄"
        };

        private static string[] BlackJack =
        {
            @"__________.__                 __          ____.              __    ",
            @"\______   \  | _____    ____ |  | __     |    |____    ____ |  | __",
            @" |    |  _/  | \__  \ _/ ___\|  |/ /     |    \__  \ _/ ___\|  |/ /",
            @" |    |   \  |__/ __ \\  \___|    <  /\__|    |/ __ \\  \___|    < ",
            @" |______  /____(____  /\___  >__|_ \ \________(____  /\___  >__|_ \",

        };

        private static string Line
        {
            get
            {
                return "####################################################################################################";
            }
        }

        private string[] CardString(Card card)
        {
            string[] cardGraphic =         {
                    "┌─────────┐",
                    "│        │",
                    "│         │",
                    "│         │",
                    "│        │",
                    "│         │",
                    "│         │",
                    "│        │",
                    "└─────────┘"
        };

            if (!card.FaceUp)
            {
                cardGraphic[1] = cardGraphic[1].Insert(1, " ");
                cardGraphic[4] = cardGraphic[4].Insert(1, " ");
                cardGraphic[7] = cardGraphic[7].Insert(1, " ");
                return cardGraphic;
            }

            for (int i = 0; i < 8; i++)
            {

                switch (i)
                {
                    case 1:
                        if (card.Value > 9 || card.Value == 1)
                        {
                            if (card.Value == 10)
                            {
                                cardGraphic[i] = cardGraphic[i].Insert(1, card.Value.ToString()).Remove(5, 1);
                            }
                            else
                            {
                                cardGraphic[i] = cardGraphic[i].Insert(1, card.Name.Substring(0, 1));
                            }
                        }
                        else
                        {
                            cardGraphic[i] = cardGraphic[i].Insert(1, card.Value.ToString());
                        }
                        break;

                    case 4:
                        cardGraphic[i] = cardGraphic[i].Insert(5, card.Suit.Substring(0, 1).ToUpper());
                        break;

                    case 7:
                        if (card.Value > 9 || card.Value == 1)
                        {
                            if (card.Value == 10)
                            {
                                cardGraphic[i] = cardGraphic[i].Insert(8, card.Value.ToString()).Remove(5, 1);
                            }
                            else
                            {
                                cardGraphic[i] = cardGraphic[i].Insert(8, card.Name.Substring(0, 1));
                            }
                        }
                        else
                        {
                            cardGraphic[i] = cardGraphic[i].Insert(8, card.Value.ToString());
                        }
                        break;
                }
            }
            return cardGraphic;
        }
        public void GetReady(Game game)
        {
            string output = "";
            foreach (Player player in game.Participants)
            {
                if (player.Position != 0)
                {
                    output += player.Name + ", ";
                }
            }
            output = output.Substring(0, output.Length - 2);
            Console.WriteLine(Line);
            PrintCentered("Players: " + output);
            PrintCentered("Get ready for the Next hand");
            Console.WriteLine(Line);
            Console.WriteLine("Press any key to Continue");
            Console.ReadKey();
        }

        public void PlaceBets(Game game)
        {
            for (int i = 1; i < game.Participants.Count; i++)
            {
                Console.WriteLine(game.Participants[i].Name + ", How much do you want to bet?");
                int bet = 0;

                while (!int.TryParse(Console.ReadLine(), out bet))
                {
                    Console.WriteLine(game.Participants[i].Name + ", How much do you want to bet?");
                }
                game.Participants[i].Currency -= bet;
                game.Participants[i].Bet = bet;

            }
        }

        private void ShowPlayerHands(Game game, int position)
        {
            for (int i = 0; i < game.Participants[position].ActiveHands; i++)
            {
                string[] hand = new string[9];
                foreach (Card card in game.Participants[position].Hands[i])
                {
                    hand = MergeArrays(hand, CardString(card));
                }
                PrintArray(hand);
            }
        }

        public void ShowBoardState(Game game)
        {
            Console.WriteLine(Line);
            PrintCentered("DEALERS HAND");
            ShowPlayerHands(game, 0);
            PrintCentered("");
            PrintCentered("");
            ShowPlayerHands(game, game.CurrentPlayer);
            PrintCentered(game.Participants[game.CurrentPlayer].Label);
            PrintCentered("Remaining Credits: " + game.Participants[game.CurrentPlayer].Currency);
            PrintArray(OtherPlayersInfo(game).Split('#'));
            Console.WriteLine(Line);
            Console.ReadKey();
        }

        public bool InsuranceEvent(Game game)
        {
            if (game.Participants[0].Hands[0][1].Value == 1)
            {
                Console.WriteLine("O oh, the dealer might have a Black Jack");

                foreach (Player player in game.Participants)
                {
                    try
                    {
                        player.Insurance = OfferInsurance(player);
                    }
                    catch (InsufficientFundsException)
                    {
                        Console.WriteLine("You dont have enough credits");
                    }
                }
                if (game.Participants[0].EvaluateHand(0) == 21.5)
                {
                    return true;
                }
            }
            return false;
        }

        public int OfferInsurance(Player player)
        {
            int insurance = 0;
            Console.WriteLine(player.Name + " how many credits worth of insurance would you like to buy?");
            while (!(int.TryParse(Console.ReadLine(), out insurance)))
            {
                Console.WriteLine("you must enter a valid amount, try 0 if you dont want insurance");
                Console.WriteLine(player.Name + " how many credits worth of insurance would you like to buy?");
            }
            player.Currency -= insurance;
            return insurance;
        }

        public string OtherPlayersInfo(Game game)
        {
            string result = "  ";
            foreach (Player player in game.Participants)
            {
                if (!(player.Position == 0 || player.Position == game.CurrentPlayer))
                {
                    result += "   #" + player.Name.ToUpper() + ":";
                    for (int i = 0; i < player.ActiveHands; i++)
                    {
                        foreach (Card card in player.Hands[i])
                        {
                            result += card.Name + ", ";
                        }
                    }
                }
            }

            result = result.Substring(0, result.Length - 2);
            return result;
        }

        public void PlayerActions(Game game)
        {
            while (game.CurrentPlayer != 0)
            {
                TakeAction(game);
                if (game.CurrentPlayer == game.Participants.Count - 1)
                {
                    game.CurrentPlayer = 0;
                }
                else
                {
                    game.CurrentPlayer += 1;
                }
            }
        }

        public void TakeAction(Game game)
        {
            Player selectedPlayer = game.Participants[game.CurrentPlayer];
            string input = "";
            selectedPlayer.HandsAreSet[0] = false;
            int hand = selectedPlayer.NextAvaliableHand();
            while (hand >= 0)
            {

                string actions = AvaliableActions(selectedPlayer, hand);
                bool validateInput = false;

                Console.WriteLine("you are on " + selectedPlayer.EvaluateHand(hand) + " Points");
                while (!validateInput)
                {
                    Console.WriteLine("||" + selectedPlayer.Name + "||" + " what would you like to do?");
                    Console.WriteLine(actions);

                    input = Console.ReadLine();
                    foreach (string s in actions.Split(','))
                    {
                        if (input.ToLower().Contains(s.Trim()))
                        {
                            validateInput = true;
                        }
                    }
                }

                switch (input.ToLower())
                {
                    case "stand":
                        selectedPlayer.HandsAreSet[hand] = true;                        
                        break;
                    case "hit":
                        selectedPlayer.Hands[hand].Add(game.DealersDeck.Draw());
                        break;
                    case "double":
                        selectedPlayer.Hands[hand].Add(game.DealersDeck.Draw());
                        selectedPlayer.HandsAreSet[hand] = true;                        
                        break;
                    case "split":
                        selectedPlayer.HandsAreSet[hand + 1] = false;
                        selectedPlayer.Hands[hand].Add(game.DealersDeck.Draw());
                        selectedPlayer.Hands[hand + 1].Add(game.DealersDeck.Draw());
                        break;
                    case "surender":
                        selectedPlayer.Hands[0].Clear();
                        selectedPlayer.Currency += selectedPlayer.Bet / 2;
                        selectedPlayer.HandsAreSet[hand] = true;                        
                        break;
                }

                if (selectedPlayer.EvaluateHand(hand) > 21.5)
                {
                    selectedPlayer.HandsAreSet[hand] = true;
                }


                ShowBoardState(game);
                hand = selectedPlayer.NextAvaliableHand();
            }
        }

        public string AvaliableActions(Player player, int hand)
        {
            string result = "stand, hit, double";

            if (player.Hands[hand][0].Points == player.Hands[hand][1].Points && player.ActiveHands != 4)
            {
                result += ", split";
            }

            if (player.ActiveHands == 1 && player.Hands[0].Count == 2)
            {
                result += ", surrender";
            }

            return result;
        }

        public void Heading()
        {
            Console.WriteLine(Line);
            PrintArray(Welcome);
            PrintCentered("TO");
            PrintArray(BlackJack);
            PrintCentered("BETA 1.0");
            Console.WriteLine(Line);
        }

        private void PrintCentered(string line)
        {
            int centerMargin = (100 - line.Length) / 2;

            Console.Write("#");
            for (int i = centerMargin - 1; i > 0; i--)
            {
                Console.Write(" ");
            }
            Console.Write(line);
            if (line.Length % 2 == 0)
            {
                centerMargin -= 1;
            }
            for (int i = centerMargin; i > 0; i--)
            {
                Console.Write(" ");
            }
            Console.Write("#");
            Console.WriteLine();
        }

        private void PrintArray(string[] content)
        {
            foreach (string line in content)
            {
                PrintCentered(line);
            }
        }

        private string[] MergeArrays(string[] a1, string[] a2)
        {
            string[] result = new string[a1.Length];
            for (int i = 0; i < a1.Length; i++)
            {
                result[i] = a1[i] + a2[i];
            }
            return result;
        }
    }

    class Game
    {
        public Deck DealersDeck;
        public List<Player> Participants;
        public int CurrentPlayer { get; set; }
        private static int nextPosition = 0;
        private static int NextPosition
        {
            get
            {
                nextPosition += 1;
                return nextPosition;
            }
        }

        public Game()
        {
            DealersDeck = new Deck { };
            Participants = new List<Player> { };
            Participants.Add(new Dealer { });
            CurrentPlayer = 1;
        }

        public void AddCustomer(string name)
        {
            Participants.Add(new Customer { Name = name, Position = NextPosition });
        }

        public void DealCards()
        {
            foreach (Player player in Participants)
            {
                player.Hands[0].Add(DealersDeck.Draw());
                player.Hands[0].Add(DealersDeck.Draw());
                player.Hands[0][0].FaceUp = true;
                player.Hands[0][1].FaceUp = true;
                if (player.Position == 0)
                {
                    player.Hands[0][0].FaceUp = false;
                }
            }
        }
    }

    class Deck
    {
        public List<Card> Library { get; set; }
        public List<Card> Drawn { get; set; } = new List<Card> { };

        public Deck()
        {
            Library = GenerateDeck();
            Shuffle();
        }

        private List<Card> GenerateDeck()
        {
            List<Card> deck = new List<Card>
            {
                new Card {Suit =  "spades", Value = 1},
                new Card {Suit =  "spades", Value = 2},
                new Card {Suit =  "spades", Value = 3},
                new Card {Suit =  "spades", Value = 4},
                new Card {Suit =  "spades", Value = 5},
                new Card {Suit =  "spades", Value = 6},
                new Card {Suit =  "spades", Value = 7},
                new Card {Suit =  "spades", Value = 8},
                new Card {Suit =  "spades", Value = 9},
                new Card {Suit =  "spades", Value = 10},
                new Card {Suit =  "spades", Value = 11},
                new Card {Suit =  "spades", Value = 12},
                new Card {Suit =  "spades", Value = 13},
                new Card {Suit =  "hearts", Value = 1},
                new Card {Suit =  "hearts", Value = 2},
                new Card {Suit =  "hearts", Value = 3},
                new Card {Suit =  "hearts", Value = 4},
                new Card {Suit =  "hearts", Value = 5},
                new Card {Suit =  "hearts", Value = 6},
                new Card {Suit =  "hearts", Value = 7},
                new Card {Suit =  "hearts", Value = 8},
                new Card {Suit =  "hearts", Value = 9},
                new Card {Suit =  "hearts", Value = 10},
                new Card {Suit =  "hearts", Value = 11},
                new Card {Suit =  "hearts", Value = 12},
                new Card {Suit =  "hearts", Value = 13},
                new Card {Suit =  "clubs", Value = 1},
                new Card {Suit =  "clubs", Value = 2},
                new Card {Suit =  "clubs", Value = 3},
                new Card {Suit =  "clubs", Value = 4},
                new Card {Suit =  "clubs", Value = 5},
                new Card {Suit =  "clubs", Value = 6},
                new Card {Suit =  "clubs", Value = 7},
                new Card {Suit =  "clubs", Value = 8},
                new Card {Suit =  "clubs", Value = 9},
                new Card {Suit =  "clubs", Value = 10},
                new Card {Suit =  "clubs", Value = 11},
                new Card {Suit =  "clubs", Value = 12},
                new Card {Suit =  "clubs", Value = 13},
                new Card {Suit =  "diamonds", Value = 1},
                new Card {Suit =  "diamonds", Value = 2},
                new Card {Suit =  "diamonds", Value = 3},
                new Card {Suit =  "diamonds", Value = 4},
                new Card {Suit =  "diamonds", Value = 5},
                new Card {Suit =  "diamonds", Value = 6},
                new Card {Suit =  "diamonds", Value = 7},
                new Card {Suit =  "diamonds", Value = 8},
                new Card {Suit =  "diamonds", Value = 9},
                new Card {Suit =  "diamonds", Value = 10},
                new Card {Suit =  "diamonds", Value = 11},
                new Card {Suit =  "diamonds", Value = 12},
                new Card {Suit =  "diamonds", Value = 13},
            };
            return deck;
        }
        public void AddDeck()
        {
            foreach (Card card in GenerateDeck())
            {
                Library.Add(card);
            }
            Shuffle();
        }

        public Card Draw()
        {
            Drawn.Add(Library[0]);
            Library.Remove(Library[0]);
            return Drawn[Drawn.Count - 1];
        }

        public void Shuffle()
        {
            List<Card> newDeck = new List<Card> { };

            for (int i = Library.Count; i > 0; i--)
            {
                int index = new Random().Next(1, i + 1);
                newDeck.Add(Library[index - 1]);
                Library.Remove(Library[index - 1]);
            }

            foreach (Card item in newDeck)
            {
                Library.Add(item);
            }
        }

        public void Reset()
        {
            foreach (Card card in Drawn)
            {
                Library.Add(card);
            }
            Drawn.Clear();
            Shuffle();
        }
    }

    class Card
    {
        public string Suit { get; set; }
        public int Value { get; set; }
        public bool FaceUp { get; set; }
        public int Points
        {
            get
            {
                if (Value > 10)
                {
                    return 10;
                }
                return Value;
            }
        }
        public string Name
        {
            get
            {
                if (Value == 1)
                {
                    return "A" + Suit.Substring(0, 1).ToUpper();
                }
                else if (Value < 11)
                {
                    return Value + Suit.Substring(0, 1).ToUpper();
                }
                else if (Value == 11)
                {
                    return "J" + Suit.Substring(0, 1).ToUpper();
                }
                else if (Value == 12)
                {
                    return "Q" + Suit.Substring(0, 1).ToUpper();
                }
                else
                {
                    return "K" + Suit.Substring(0, 1).ToUpper();
                }

            }
        }
        public Card()
        {
            FaceUp = true;
        }
    }

    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException()
        {

        }
    }
}
