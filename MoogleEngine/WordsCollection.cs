
using SharpNL.Stemmer.Snowball;

namespace MoogleEngine
{
    /// <summary>
    /// Clase que contiene a todas las palabras de la coleccion de documentos
    /// </summary>
    public class Wordscollection
    {
        /// <summary>
        /// Inicializa el array counts con la respectiva cantidad de apariciones de 
        /// cada palabra en la coleccion de documentos
        /// </summary>
        /// <param name="docs">Representa la coleccion de documentos 
        /// sobre la que se esta trabajando</param>

        public Wordscollection(List<Document> docs)
        {
            foreach (Document item in docs)
            {
                foreach (var pair in item.wordcoll)
                {
                    if (wordCount.ContainsKey(pair.Key))
                    {
                        wordCount[pair.Key] += 1;
                    }
                    else
                    {
                        wordCount.Add(pair.Key, 1);
                    }
                }
            }
            this.Stemmer();
        }

        /// <summary>
        /// Inicializa un diccionario con todas las raices de las palabras de la coleccion
        /// </summary>
        public void Stemmer()
        {
            SpanishStemmer stemmer = SpanishStemmer.Instance;
            foreach (var pair in wordCount)
            {
                string wordStemed = stemmer.Stem(pair.Key);
                if (!rootWords.ContainsKey(wordStemed))
                {
                    rootWords.Add(wordStemed, 1);
                }
                else
                {
                    rootWords[wordStemed] += 1;
                }
            }

        }
        /// <summary>
        /// Diccionario que contiene todas las palabras de la coleccion de documentos y la cantidad de documentos donde esta aparece
        /// Sobre este se construyen los vectores de los pesos TFIDF
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <typeparam name="double"></typeparam>
        /// <returns></returns>
        public Dictionary<string, double> wordCount = new Dictionary<string, double>();
        /// <summary>
        /// Diccionario que contiene todas las palabras raices de la coleccion de documentos y la cantidad de documentos donde estas aparecen
        ///Sobre este se construyen los vectores de los pesos TFIDF de las palabras raices
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <typeparam name="double"></typeparam>
        /// <returns></returns>
        public Dictionary<string, double> rootWords = new Dictionary<string, double>();
    }
}