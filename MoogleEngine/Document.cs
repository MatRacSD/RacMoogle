using SharpNL.Stemmer.Snowball;

namespace MoogleEngine
{



    /// <summary>
    /// Representa la clase de los documentos epxresados en forma vectorial
    /// </summary>
    public class Doc : IComparable<Doc>
    {


        /// <summary>
        /// Contructor del tipo Doc
        /// </summary>
        /// <param name="titl">El nombre del documento al que hace referencia este vector</param>

        public Doc(string titl)
        {
            Title = titl;

        }

        public int CompareTo(Doc document)
        {
            if (this.score == document.score)
                return 0;
            else if (this.score < document.score)
                return -1;
            return 1;
        }

        /// <summary>
        /// Inicializa la magnitud del vector
        /// </summary>
        public void Normalize()
        {
            module = 0;
            for (int i = 0; i < TFIDF.Length; i++)
            {
                module += Math.Pow(TFIDF[i], 2);
                module = Math.Sqrt(module);
            }

            
        }
        /// <summary>
        /// Inicializa la magnitud del vector de las raíces
        /// </summary>
        public void NormalizeR()
        {
            moduleR = 0;
            for (int i = 0; i < TF_root.Length; i++)
            {
                moduleR += Math.Pow(TF_root[i], 2);
                moduleR = Math.Sqrt(moduleR);
            }

            
        }

        /// <summary>
        /// Inicializa la coleccion de palabras del vector documento
        /// </summary>
        public void InitialiceWordsColl()
        {

            foreach (string word in content)
            {
                if (wordcoll.ContainsKey(word))
                {
                    wordcoll[word] += 1;
                    continue;
                }
                else
                {
                    wordcoll.Add(word, 1);
                }
            }
        }
        /// <summary>
        /// Calcula la distancia minima entre dos palabras en un documentos
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>un double que representa la distancia mínima</returns>
        public double GetDistance(string a, string b)
        {
            double distance = content.Length;
            if (wordcoll.ContainsKey(a) && wordcoll.ContainsKey(b))
            {
                for (int i = 0; i < content.Length - 1; i++)
                {

                    if (content[i] == a)
                        for (int k = 0; k < content.Length; k++)
                        {
                            if (content[k] == b && k != i)
                            {
                                distance = Math.Min(distance, Math.Abs(i - k));
                            }
                        }
                }
            }

            return distance;
        }
        /// <summary>
        /// Calcula el coseno de similitud entre un vector documento y el query
        /// </summary>
        /// <param name="vector"></param>
        public void SimilarityCosine(Query vector)
        {
            score = DocumentUtils.ScalarProduct(TFIDF, vector.TFIDF) / (module * vector.module);
            score *= vector.ScoreMod(this);

        }
        /// <summary>
        /// Igual que el coseno de similitud, pero con el vector de las palabras raíces
        /// </summary>
        /// <param name="vector"></param>
        public void SimilarityCosineR(Query vector)
        {
            double a = (0.4f) * DocumentUtils.ScalarProduct(TF_root, vector.TF_root)  ;
            double b = (moduleR * vector.moduleR);
            score += a / b;

        }

        /// <summary>
        /// Calcula e inicializa el vector TF-IDF del documento
        /// </summary>
        /// <param name="words">vector palabras</param>
        /// <param name="total">total de documentos de la coleccion</param>
        public void CalculateTF(Wordscollection words, double total)
        {
            TFIDF = new double[words.word_count.Count];
            int counter = 0;
            foreach (var item in words.word_count)
            {
                if (wordcoll.ContainsKey(item.Key))
                {
                    TFIDF[counter] = wordcoll[item.Key] /( length * (Math.Log(total / item.Value ) )+ 1.1f);

                }
                else
                {
                    TFIDF[counter] = 0;
                }
                counter += 1;
            }

            for (int i = 0; i < TFIDF.Length; i++)
            {
                module += Math.Pow(TFIDF[i], 2);
                module = Math.Sqrt(module);
            }


        }

        /// <summary>
        /// Calcula los pesos TFIDF de los vectores documentos utilizando las raíces de las palabras
        /// </summary>
        /// <param name="words">Coleccion de palabras del documento</param>
        /// <param name="total">total de documentos de la coleccion</param>
        public void CalculateTFR(Wordscollection words, double total)
        {
            TF_root = new double[words.root_words.Count()];
            int counter = 0;
            foreach (var item in words.root_words)
            {
                if (root_words.ContainsKey(item.Key))
                {
                    TF_root[counter] = root_words[item.Key] / ((double)root_words.Count() * Math.Log(total / item.Value  ) + 1.1f);

                }
                else
                {
                    TF_root[counter] = 0;
                }
                counter += 1;
            }
        }

        /// <summary>
        /// Devuelve un string que representa un fragmento del documento en el que aparece al menos una de las palabras introducidas 
        /// </summary>
        /// <param name="words">La coleccion de palabras sobre las cuales se quiere obtener un fragmento que las contenga</param>
        /// <returns>Un fragmento del documento que contenga a la primera palabra que se encuentre en el documento</returns>
        public string GetSnippet(string[] words)
        {

            foreach (string word in words)
            {
                if (wordcoll.ContainsKey(word))
                {
                    return GetFragment(word);
                }
            }
            return "";

        }
        /// <summary>
        /// Metodo que retorna un fragmento del texto donde esta la palabra
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private string GetFragment(string word)
        {

            int index = Array.IndexOf(content_unnormalized, word);
            int startIndex = Math.Max(0, index - 5);
            int endIndex = Math.Min(content_unnormalized.Length - 1, index + 5);

            if (index == -1)
            {
                return "";
            }


            string fragment = string.Join(" ", content_unnormalized[startIndex..(endIndex + 1)]);

            return fragment;
        }


        /// <summary>
        /// Inicializa un diccionario con las raices de todas las palabras del docuemnto
        /// </summary>
        public void Stemmer()
        {
            SpanishStemmer stm2 = SpanishStemmer.Instance;
            foreach (var pair in this.wordcoll)
            {
                ///El motivo de la implementacion del  try-catch se debe a que el Stemmer tendía a provocar exepciones sobre todo
                /// al ejecutarse en paralelo, lo que al final se utilizó la la iteración convencional
                try
                {

                    string aux2 = stm2.Stem(pair.Key);

                    if (!root_words.ContainsKey(aux2))
                    {
                        root_words.Add(aux2, 1);
                    }
                    else
                    {
                        root_words[aux2] += 1;
                    }
                }
                catch (Exception)
                {

                    continue;
                }

            }
        }

        public override string ToString()
        {
            return Title;
        }

        /// <summary>
        /// Cantidad de palabras del documento
        /// </summary>
        /// <value></value>
        private int length { get => content.Length; }

        /// <summary>
        /// Diccionario que contiene todas las palabras del documento y su cantidad de repeticiones
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <typeparam name="double"></typeparam>
        /// <returns></returns>
        public Dictionary<string, double> wordcoll = new Dictionary<string, double>();
        /// <summary>
        /// Diccionario que contiene todas las raíces de las palabras del documento y sus repeticiones
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <typeparam name="double"></typeparam>
        /// <returns></returns>

        public Dictionary<string, double> root_words = new Dictionary<string, double>();

        /// <summary>
        /// Longitud del vector del TFIDF
        /// </summary>
        public double module = 0;
        public double moduleR = 0;
        /// <summary>
        /// Valor booleano que es verdadero si se han encontrado palabras similares con la distancia de Levenstein
        /// </summary>
        public bool FindSimilar = false;
        /// <summary>
        /// Contenido del texto separado sin normalizar, utilizado para el snippet
        /// </summary>
        /// <value></value>
        public string[] content_unnormalized { get; set; }
        /// <summary>
        /// Direccion del documento
        /// </summary>
        /// <value></value>
        public string doc_path { get; set; }
        /// <summary>
        /// Titulo del documento
        /// </summary>
        /// <value></value>
        public string Title { get; protected set; }
        /// <summary>
        /// Representa la importancia del documento segun la query introducida
        /// </summary>
        /// <value></value>
        public double score { get; private set; }
        /// <summary>
        /// Representa el vector de los pesos TFIDF de las palabras del documento
        /// </summary>
        /// <value></value>
        public double[] TFIDF { get; protected set; }
        /// <summary>
        /// Representa el vector de los pesos TFIDF de las palabras raíces del documento
        /// </summary>
        /// <value></value>
        public double[] TF_root { get; protected set; }
        /// <summary>
        /// Array de string que contiene las palabras importantes del documento
        /// </summary>
        /// <value></value>
        public string[] content { get; set; }
    }





}