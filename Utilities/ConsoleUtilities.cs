namespace AmmoTracker.Utilities
{
    public static class ConsoleUtilities
    {
        public static void TypeEffect(string message, int delay = 50, bool newLine = true)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));
            if (delay < 0)
                throw new ArgumentException("Delay must be non-negative", nameof(delay));

            foreach (char c in message)
            {
                Console.Write(c);
                if (delay > 0)
                {
                    Thread.Sleep(delay);
                }
            }
            if (newLine)
            {
                Console.WriteLine();
            }
        }
    }
}
