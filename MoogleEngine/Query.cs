using System.Text.RegularExpressions;

namespace MoogleEngine
{
    public class Query : Document
    {
        
        /// <summary>
        /// Representa una matriz donde se guarda verdadero o falso si alguna palabra del query posee un caracer especial
        /// la primera fila representa "^" y la segunda fila "!"
        /// </summary>
        public bool[,] BaseSpecialChar { get; private set; }

        /// <summary>
        /// Representa una matriz con la cantidad de acumulaciones de una  palabra si se ha introducido el caracter "*"
        /// </summary>
        /// <value></value>
        public double[] AcumulativeSpecialChar { get; private set; }
        /// <summary>
        /// Representa los pares de palabras cuya proximidad en un documento presenta relevancia por la introduccion del caracter "~"
        /// </summary>
        /// <value></value>
        public Dictionary<string, string> ProximitySpecialChar = new Dictionary<string, string>();

        public Query() : base("QUERY")
        {

        }

        /// <summary>
        /// Inicializa las propiedades relacionadas con los carecteres especiales del query introducidos por el usuario 
        /// </summary>
        /// <param name="query"></param>
        public void Initialice(string query)
        {
            InicialiceSpecialContent(query.ToLowerInvariant().Trim());
        }
        /// <summary>
        /// Se procesa el query para determinar los caracteres especiales que aparecen y si son válidos
        /// </summary>
        /// <param name="query"></param>
        private void InicialiceSpecialContent(string query)
        {
            BaseSpecialChar = new bool[2, content.Length];
            AcumulativeSpecialChar = new double[content.Length];
            Dictionary<string, List<char>> sp_content = new Dictionary<string, List<char>>();

            string pattern = @"[^a-zA-Z0-9á-úÁ-Úä-üÄ-Ü^!*]+";
            string[] splitedQuery = Regex.Split(query, pattern);
            splitedQuery = splitedQuery.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            splitedQuery = splitedQuery.Where((d) => !DocumentUtils.removableWords.Contains(d.RemoveSpecial())).ToArray();

            for (int i = 0; i < splitedQuery.Length; i++)
            {
                char c = splitedQuery[i].ElementAt(0);
                if (splitedQuery[i].Length > 1)
                {

                    if (c == '^')
                    {
                        BaseSpecialChar[0, i] = true;
                    }
                    else if (c == '!')
                    {

                        BaseSpecialChar[1, i] = true;
                    }
                    else if (c == '*')
                    {
                        double mult = GetMultiply(splitedQuery[i]);
                        AcumulativeSpecialChar[i] = mult;
                    }

                }
                try
                {
                    if (c == '~' && i != 0 && i != splitedQuery.Length - 1)
                    {
                        ProximitySpecialChar.Add(content[i], content[i - 1]);
                    }
                }
                catch(Exception)
                {
                    continue;
                }

            }
            string pattern2 = @"[^a-zA-Z0-9~]+";
            string[] splitedQuery2 = Regex.Split(query, pattern2);
            splitedQuery2 = splitedQuery2.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            splitedQuery2 = splitedQuery2.Where((d) => !DocumentUtils.removableWords.Contains(d)).ToArray();
            for (int i = 0; i < splitedQuery.Length; i++)
            {
                char c = splitedQuery2[i].ElementAt(0);
                if (splitedQuery[i].Length > 1)
                {
                    if (c == '~' && i != 0)
                    {
                        ProximitySpecialChar.Add(content[i], content[i - 1]);
                    }
                }
                else if (c == '~' && i != 0 && i != splitedQuery2.Length - 1)
                {
                    ProximitySpecialChar.Add(content[i], content[i - 1]);
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
        /// <param name="document"></param>
        /// <returns></returns>
        public double ScoreMod(Document document)
        {
            double aux = 1;

            for (int i = 0; i < content.Length; i++)
            {

                if (BaseSpecialChar[0, i] && !document.wordcoll.ContainsKey(content[i]))
                    return 0;

                else if (document.wordcoll.ContainsKey(content[i]))
                {
                    if (BaseSpecialChar[1, i])
                    {
                        return 0;
                    }
                    if (AcumulativeSpecialChar[i] > 0)
                    {
                        aux += AcumulativeSpecialChar[i];
                    }
                }
            }
            double mult = 1;
            foreach (var pair in ProximitySpecialChar)
            {
                try{
                if (document.wordcoll.ContainsKey(pair.Key) && document.wordcoll.ContainsKey(pair.Value))
                {
                    mult += (document.content.Length + 1) / (document.GetDistance(pair.Key, pair.Value) + 1);
                }
                }
                catch(Exception)
                {
                    continue;
                }
            }
            return aux + DocumentUtils.AcumulativeFunction(mult);
        }

    }
}