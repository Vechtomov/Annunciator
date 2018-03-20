namespace EgeAnnunciator
{
    /// <summary>
    /// Класс для установки правильного окончания у слов 
    /// </summary>
    public class Word
    {
        private string one;
        private string two;
        private string five;
        public Word(string one, string two, string five)
        {
            this.one = one;
            this.two = two;
            this.five = five;
        }

        public string GetWord(int number)
        {
            if (number % 10 == 1 && number % 100 != 11)
                return one;
            else if ((number % 10) > 1 && (number % 10) < 5 &&
                    !((number % 100) > 10 && (number % 100) < 15))
                return two;
            else return five;
        }

    }
}
