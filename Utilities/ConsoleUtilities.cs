namespace AmmoTracker.Utilities
{
    public static class ConsoleUtilities
    {
        /// <summary>
        /// Creates a typewriter effect for the given message
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="delay">Delay between characters in milliseconds (default: 50)</param>
        /// <param name="newLine">Whether to add a new line at the end (default: true)</param>
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
