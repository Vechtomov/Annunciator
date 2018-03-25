namespace Annunciator
{
    /// <summary>
    /// Класс для установки правильного окончания у слов, которые стоят рядом с числительными
    /// </summary>
    public class Word
    {
        private string one;
        private string two;
        private string five;
        /// <summary>
        /// Устанавливает правильное окончание словам в зависимости от числительного, которое стоит перед словом
        /// Например: 1 день, 2 дня, 5 дней
        /// </summary>
        /// <param name="one">Как слово произносится с числительным "один"</param>
        /// <param name="two">Как слово произносится с числительным "два"</param>
        /// <param name="five">Как слово произносится с числительным "пять"</param>
        public Word(string one, string two, string five)
        {
            this.one = one;
            this.two = two;
            this.five = five;
        }

        /// <summary>
        /// Возвращает слово с правильным окончанием в зависимости от числительного, которое стоит перед словом
        /// </summary>
        /// <param name="number">Числительное</param>
        /// <returns></returns>
        public string GetWord(int number)
        {
            string result = string.Empty;

            if (number % 10 == 1 && number % 100 != 11) {
                result = one;
            } else if ((number % 10) > 1 && (number % 10) < 5 && !((number % 100) > 10 && (number % 100) < 15)) {
                result = two;
            }
            else result = five;

            return result;
        }

    }
}
