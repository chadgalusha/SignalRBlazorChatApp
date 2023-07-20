namespace JWTGenerator
{
    public class Program
    {
        static void Main(string[] args)
        {
            PrintMenu();
            Console.ReadLine();
        }

        private static void PrintMenu()
        {
            int userSelection = 1;

            while (userSelection != 0)
            {
                Menu();
                userSelection = UserSelection();

                switch (userSelection)
                {
                    case 1:
                        string newJwt = JwtProcessor.GetJwtToken();
                        Console.WriteLine(newJwt);
                        break;
                    case 0:
                        break;
                    case -1:
                        Console.WriteLine("Invalid selection. Please try again.");
                        break;
                    default: 
                        Console.WriteLine("Invalid input. Please try again.");
                        break;
                }
            }
        }

        private static void Menu()
        {
            Console.WriteLine("\nEnter 0 to exit the program");
            Console.WriteLine("Enter 1 to receive a JWT good for 15 minutes.\n");
        }

        private static int UserSelection()
        {
            try
            {
                Console.Write("Please make a selection: ");
                return Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}