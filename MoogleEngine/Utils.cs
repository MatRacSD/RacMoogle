
using SharpNL;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace MoogleEngine
{
    /// <summary>
    /// Clase con metodos útiles para trabajar sobre la coleccionde documentos
    /// </summary>
    public static class DocumentUtils
    {
        /// <summary>
        /// Inicializa una lista con todos los documentos
        /// </summary>
        /// <returns>Devuelve una lista de documentos con el titulo y el contenido</returns>
        public static List<Document> DocLoader()
        {



            List<Document> docs = new List<Document>();
            string apppath = Directory.GetCurrentDirectory();

            apppath = Path.Join(apppath, "..", "Content");
            string[] filepath = Directory.GetFiles(apppath, "*.txt");


            int total = filepath.Length;

            string pattern = @"[^a-zA-Z0-9á-úÁ-Úä-üÄ-Ü]+";

            /// <summary>
            /// Se inicializa cada documento usando de forma paralela si es posible
            /// </summary>
            /// <value></value>
            Parallel.ForEach(filepath, (path) =>
            {
                Document doc = new Document(Path.GetFileName(path));
                doc.docPath = path;

                ////Se le asocia a cadadocumento un array de palabras con las palabras que contiene
                doc.contentUnnormalized = Regex.Split(File.ReadAllText(path).ToLowerInvariant().Trim(), pattern);

                ////Se eliminan espacios vacios o nulos
                doc.content = doc.contentUnnormalized.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

                ////Se filtra la coleccion para eliminar palabras innecesarias como pronombres, preposiciones, estc
                doc.content = doc.content.Where((d) => !DocumentUtils.removableWords.Contains(d)).ToArray();
                doc.InitialiceWordsColl();

                docs.Add(doc);
            });
            foreach (Document doc in docs)
            {
                doc.Stemmer();
            }



            return docs;

        }
        /// <summary>
        /// Devuelve el string sin caracteres especiales
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>

        public static string RemoveSpecial(this string a)
        {
            List<char> b = new List<char>();

            foreach (char letter in a)
            {
                if (letter == '!' || letter == '~' || letter == '^' || letter == '*')
                { continue; }
                else b.Add(letter);
            }
            string c = "";
            foreach (char letter in b)
            {
                c += letter;
            }
            return c;
        }
        /// <summary>
        /// Inicializa un vector con la busqueda 
        /// </summary>
        /// <param name="queryString">Representa la cadena de string de la busqueda</param>
        /// <param name="words">La coleccion de palabras de la coleccion de documentos</param>
        /// <param name="total">Total de documentos de la coleccion</param>
        /// <returns></returns>
        public static Query QueryLoader(string queryString, Wordscollection words, double total)
        {


            Query queryVector = new Query();
            ///Se reliza el mismo procedimiento utilizado con los documentos
            string pattern = @"[^a-zA-Z0-9á-úÁ-Úä-üÄ-Ü]+";
            queryVector.content = Regex.Split(queryString.ToLowerInvariant().Trim(), pattern);
            queryVector.content = queryVector.content.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            queryVector.content = queryVector.content.Where((d) => !DocumentUtils.removableWords.Contains(d)).ToArray();
            if (queryVector.content.Length == 0)
            {
                return queryVector;
            }

            queryVector.InitialiceWordsColl();
            queryVector.Stemmer();
            queryVector.CalculateTF(words: words, total);
            queryVector.CalculateTFR(words, total);
            queryVector.Normalize();
            queryVector.NormalizeR();
            queryVector.Initialice(queryString);


            return queryVector;
        }
        /// <summary>
        /// Realiza el producto escalar de dos vectores
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double ScalarProduct(double[] x, double[] y)
        {
            if (x.Length != y.Length)
                throw new ArgumentException("ERROR AL INTENTAR EL PRODUCTO ESCALAR, LONGITUDES DE LOS VECTORES INVALIDAS");
            double z = 0;
            for (int i = 0; i < x.Length; i++)
            {
                z += x[i] * y[i];
            }
            return z;
        }

        /// <summary>
        /// Metodo para calcular la distancia de dos
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>

        public static int DamerauLevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 0; j <= m; j++)
            {
                d[0, j] = j;
            }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = s[i - 1] == t[j - 1] ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(
                            d[i - 1, j] + 1,
                            d[i, j - 1] + 1
                        ),
                        d[i - 1, j - 1] + cost
                    );

                    if (i > 1 && j > 1 && s[i - 1] == t[j - 2] && s[i - 2] == t[j - 1])
                    {
                        d[i, j] = Math.Min(
                            d[i, j],
                            d[i - 2, j - 2] + cost
                        );
                    }
                }
            }

            return d[n, m];
        }

        /// <summary>
        /// Metodo usado para filtrar elementos de una colección
        /// </summary>
        /// <param name="source">coleccion fuente</param>
        /// <param name="condition">condicion que deben cumplir los elementos para ser devueltos</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Un enumerable que contiene los elementos de una coleccion que cumplen una condicion</returns>
        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, bool> condition)
        {
            foreach (T item in source)
            {
                if (condition(item))
                    yield return item;
            }
        }

        /// <summary>
        /// Encuentra todas las palabras de una coleccion de documentos similares a las del query
        /// e inicializa un nuevo query con las palabras que tengan menor diferencia y si aparazcan en la coleccion
        /// No busca palabras similares si la palabra ya está en la coleccion
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="words"></param>
        public static void FindSimilarWords(this Document vector, Wordscollection words)
        {
            Dictionary<string, double> di = new Dictionary<string, double>();

            foreach (var item in vector.wordcoll)
            {
                Func<string, bool> comparer = (x) => Math.Abs(item.Key.Length - x.Length) < 3;

                if (!words.wordCount.ContainsKey(item.Key))
                {
                    string aux = item.Key;
                    int currentDiference = 3;
                    string[] similar_words = words.wordCount.Keys.Where(comparer).ToArray();
                    foreach (string w in similar_words)
                    {
                        int d = DamerauLevenshteinDistance(item.Key, w);
                        if (d < currentDiference)
                        {
                            aux = w;
                            currentDiference = d;
                            if (currentDiference == 1)
                            {
                                break;
                            }
                        }


                    }
                    if (aux != item.Key)

                        di.Add(aux, item.Value);

                }
            }
            if (di.Count > 0)
            {
                vector.wordcoll = di;
                vector.FindSimilar = true;
                foreach (var item in di)
                {
                    if(!vector.wordcoll.Contains(item))

                    vector.wordcoll.Add(item.Key, item.Value);
                }
            }
        }
        /// <summary>
        /// Funcion que retorna como valor proximo a 10 segun que tan grande sea el valor de entrada
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double AcumulativeFunction(double value)
        {
            if (value < 1)
                return 1;
            else return 10 - (9 / value);
        }
        /// <summary>
        /// Array que contiene Pronombres, preposiciones, articulos y conjunciones
        /// </summary>
        /// <value></value>
        public static readonly string[] removableWords = {"yo", "tú", "él", "ella", "usted", "nosotros", "nosotras", "vosotros", "vosotras", "ellos", "ellas", "ustedes",
"a", "ante", "bajo", "cabe", "con", "contra", "de", "desde", "en", "entre", "hacia", "hasta", "para", "por", "según", "sin", "so", "sobre", "tras",
"y", "e", "ni", "que", "o", "u", "bien", "ya", "pero", "aunque", "sino", "si", "cuando", "como", "donde", "porque", "pues", "ya que", "tal como", "como si", "a fin de que", "a menos que", "antes de que", "con tal que", "en caso de que", "para que", "sin que", "aunque", "como si", "después de que", "hasta que", "mientras", "tan pronto como",
"mío", "tuyo", "suyo", "nuestro", "vuestro", "suyos", "suyas", "mía", "tuya", "suya", "nuestra", "vuestra", "míos", "tuyos", "suyas", "nuestros", "vuestros", "mías", "tuyas", "nuestras", "vuestras",
"este", "ese", "aquel", "esta", "esa", "aquella", "esto", "eso", "aquello", "estos", "esos", "aquellos", "estas", "esas", "aquellas",
"el", "la", "los", "las", "un", "una", "unos", "unas","alguien", "nadie", "algo", "nada", "cualquiera", "todo", "ninguno","todos"};
    }
}
