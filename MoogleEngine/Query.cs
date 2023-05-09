using System.Text.RegularExpressions;

namespace MoogleEngine
{
    public class Query : Doc
    {
        public Query() : base("QUERY")
        {

        }

        /// <summary>
        /// Inicializa las propiedades relacionadas con los carecteres especiales del query introducidos por el usuario 
        /// </summary>
        /// <param name="qry"></param>
        public void Initialice(string qry)
        {
            InicialiceSpecialContent(qry.ToLowerInvariant().Trim());
        }
        /// <summary>
        /// Se procesa el query para determinar los caracteres especiales que aparecen y si son válidos
        /// </summary>
        /// <param name="qry"></param>
        private void InicialiceSpecialContent(string qry)
        {
            SpChar = new bool[2, content.Length];
            AcumSpChar = new double[content.Length];
            Dictionary<string, List<char>> sp_content = new Dictionary<string, List<char>>();

            string pattern = @"[^a-zA-Z0-9á-úÁ-Úä-üÄ-Ü^!*]+";
            string[] splited_qry = Regex.Split(qry, pattern);
            splited_qry = splited_qry.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            splited_qry = splited_qry.Where((d) => !DocumentUtils.removable_words.Contains(d.RemoveSpecial())).ToArray();

            for (int i = 0; i < splited_qry.Length; i++)
            {
                char c = splited_qry[i].ElementAt(0);
                if (splited_qry[i].Length > 1)
                {

                    if (c == '^')
                    {
                        SpChar[0, i] = true;
                    }
                    else if (c == '!')
                    {

                        SpChar[1, i] = true;
                    }
                    else if (c == '*')
                    {
                        double mult = GetMultiply(splited_qry[i]);
                        AcumSpChar[i] = mult;
                    }

                }
                try
                {
                    if (c == '~' && i != 0 && i != splited_qry.Length - 1)
                    {
                        ProxSpChar.Add(content[i], content[i - 1]);
                    }
                }
                catch(Exception)
                {
                    continue;
                }

            }
            string pattern2 = @"[^a-zA-Z0-9~]+";
            string[] splited_qry2 = Regex.Split(qry, pattern2);
            splited_qry2 = splited_qry2.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            splited_qry2 = splited_qry2.Where((d) => !DocumentUtils.removable_words.Contains(d)).ToArray();
            for (int i = 0; i < splited_qry.Length; i++)
            {
                char c = splited_qry2[i].ElementAt(0);
                if (splited_qry[i].Length > 1)
                {
                    if (c == '~' && i != 0)
                    {
                        ProxSpChar.Add(content[i], content[i - 1]);
                    }
                }
                else if (c == '~' && i != 0 && i != splited_qry2.Length - 1)
                {
                    ProxSpChar.Add(content[i], content[i - 1]);
                }
            }


        }
        /// <summary>
        /// Se utiliza cuando se introduce el caracter "*", y segun cuantas veces se haya introducido, aumenta la importancia de la palabra
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private static int GetMultiply(string word)
        {
            int multiply = 1;
            foreach (char letter in word)
            {
                if (letter == '*')
                    multiply += 2;
                else break;
            }
            return multiply;
        }
        /// <summary>
        /// Modifica el score de los docuemntos segun los parametros especiales introducidos
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public double ScoreMod(Doc doc)
        {
            double aux = 1;

            for (int i = 0; i < content.Length; i++)
            {

                if (SpChar[0, i] && !doc.wordcoll.ContainsKey(content[i]))
                    return 0;

                else if (doc.wordcoll.ContainsKey(content[i]))
                {
                    if (SpChar[1, i])
                    {
                        return 0;
                    }
                    if (AcumSpChar[i] > 0)
                    {
                        aux += AcumSpChar[i];
                    }
                }
            }
            double mult = 1;
            foreach (var pair in ProxSpChar)
            {
                try{
                if (doc.wordcoll.ContainsKey(pair.Key) && doc.wordcoll.ContainsKey(pair.Value))
                {
                    mult += (doc.content.Length + 1) / (doc.GetDistance(pair.Key, pair.Value) + 1);
                }
                }
                catch(Exception)
                {
                    continue;
                }
            }
            return aux + DocumentUtils.AcumulativeFunction(mult);
        }

        /// <summary>
        /// Representa una matriz donde se guarda verdadero o falso si alguna palabra del query posee un caracer especial
        /// la primera fila representa "^" y la segunda fila "!"
        /// </summary>
        public bool[,] SpChar { get; private set; }

        /// <summary>
        /// Representa una matriz con la cantidad de acumulaciones de una  palabra si se ha introducido el caracter "*"
        /// </summary>
        /// <value></value>
        public double[] AcumSpChar { get; private set; }
        /// <summary>
        /// Representa los pares de palabras cuya proximidad en un documento presenta relevancia por la introduccion del caracter "~"
        /// </summary>
        /// <value></value>
        public Dictionary<string, string> ProxSpChar = new Dictionary<string, string>();

    }
}