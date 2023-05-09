
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

        public Wordscollection(List<Doc> docs)
        {
            foreach (Doc item in docs)
            {
                foreach (var pair in item.wordcoll)
                {
                    if (word_count.ContainsKey(pair.Key))
                    {
                        word_count[pair.Key] += 1;
                    }
                    else
                    {
                        word_count.Add(pair.Key, 1);
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
            SpanishStemmer stm = SpanishStemmer.Instance;
            foreach (var pair in word_count)
            {
                string aux = stm.Stem(pair.Key);
                if (!root_words.ContainsKey(aux))
                {
                    root_words.Add(aux, 1);
                }
                else
                {
                    root_words[aux] += 1;
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
        public Dictionary<string, double> word_count = new Dictionary<string, double>();
        /// <summary>
        /// Diccionario que contiene todas las palabras raices de la coleccion de documentos y la cantidad de documentos donde estas aparecen
        ///Sobre este se construyen los vectores de los pesos TFIDF de las palabras raices
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <typeparam name="double"></typeparam>
        /// <returns></returns>
        public Dictionary<string, double> root_words = new Dictionary<string, double>();
    }
}