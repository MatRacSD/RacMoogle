



namespace MoogleEngine
{
    /// <summary>
    /// Representa el espacio vectorial de documentos
    /// </summary>
    public class DocumentSpace
    {
        /// <summary>
        /// Inicializa el espacio y todos sus documentos
        /// </summary>
        public DocumentSpace()
        {
            docs = DocumentUtils.DocLoader();
            words = new Wordscollection(docs);
            double length = docs.Count;
            Parallel.ForEach(docs, (Doc doc) =>
            {
                doc.CalculateTF(words, length);
                doc.CalculateTFR(words, length);
                doc.Normalize();
                doc.NormalizeR();
            });
        }

        /// <summary>
        /// Realiza la busqueda para el query introducido
        /// </summary>
        /// <param name="query">El query que se va a buscar</param>
        /// <returns>Devuelve un SearchResult que contiene informacion sobre los elementos encontrados durante la busqueda</returns>
        public SearchResult DoSearch(string query)
        {
            ///Se inicializa el vector query
            Query query_vector = DocumentUtils.QueryLoader(query, words, docs.Count);

            if (query_vector.content.Length == 0) ///Si el query no tiene palabras válidas se termina la búsqueda
                return new SearchResult(new SearchItem[] { new SearchItem("Introduzca una búsqueda válida", "...", 0) }, "...");
            foreach (Doc doc in docs) ///Se calcula el score de los documentos
            {
                doc.SimilarityCosine(query_vector);
                doc.SimilarityCosineR(query_vector);
            }
            ///Se organizan los documentos
            docs.Sort();
            docs.Reverse();

            List<SearchItem> si = new List<SearchItem>();
            for (int i = 0; i < docs.Count; i++)
            {
                if (docs[i].score > 0) ///Se devuelven los documentos con score mayor que 0
                    si.Add(new SearchItem(docs[i].ToString(), docs[i].GetSnippet(query_vector.wordcoll.Keys.ToArray()), (float)docs[i].score));
                if (i == 5)
                    break;
            }
            if (si.Count() < 4) ////si se ecuentran pocos archivos se realiza nuevamente la búsqueda con palabras similares
            {
                query_vector.FindSimilarWords(words);
                if (query_vector.FindSimilar)
                {
                    query_vector.Stemmer();
                    query_vector.CalculateTF(words: words, docs.Count());
                    query_vector.CalculateTFR(words, docs.Count());
                    foreach (Doc doc in docs)
                    {
                        doc.SimilarityCosine(query_vector);
                        doc.SimilarityCosineR(query_vector);
                    }
                    docs.Sort();
                    docs.Reverse();
                    for (int i = 0; i < docs.Count; i++)
                    {
                        if (docs[i].score > 0)
                            si.Add(new SearchItem(docs[i].ToString(), docs[i].GetSnippet(query_vector.wordcoll.Keys.ToArray()), (float)docs[i].score));
                        if (i == 5)
                            break;
                    }
                }
                else ////Si despues de realizar la busqueda con elementos similares no se encuentra nada, se informa que no se enecontró ningun elemento
                {
                    SearchItem si1 = new SearchItem("No se encontró ningún elemento", "...", 0);
                    return new SearchResult(new SearchItem[] { si1 }, "...");
                }
            }


            string suggestion = " ";
            foreach (var item in query_vector.wordcoll)
            {
                suggestion += " " + item.Key;
            }
            foreach (var item in si)
            {
                Console.WriteLine(item.Title + "_______" + item.Score);
            }

            return new SearchResult(si.ToArray(), suggestion);

        }
        /// <summary>
        /// Contiene todos los documentos de la coleccion vectorizados
        /// </summary>
        private List<Doc> docs;
        /// <summary>
        /// Contiene la coleccion de palabras de la coleccion de documentos
        /// </summary>
        private Wordscollection words;
    }
}