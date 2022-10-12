namespace MoogleEngine;

/// Raciel A. Simon Domenech C-121
public static class Moogle
{
    public static SearchResult Query(string query_first) {

        string path = Directory.GetCurrentDirectory();             ///Devuelve el directorio actual

        path = Path.Join(path, @"..\Content");                  

        string[] files = Directory.GetFiles(path);                   /// Se determina un array con los directorios de todos los .txt en ka carpeta

        string[] query_0 = Score.QuerySpliter(query_first, true);      /// Se divide la busqueda inicial en un array de solo palabras mayores que una letra

        string[] splited_query = Score.QuerySpliter(query_first, false); /// Se divide la busqueda inicial pero esta vez para obtener los caracteres especiales

        int[] proximity_index = Score.IndexGetter(Score.SpecialSpliter(query_first));  /// Se determina, en caso de ser posible, los índices de las palabras adyacentes a un caracter de proximidad "~"

        Documents[] Docset_1 = Documents.FirstDocumentsGetters(files, files.Length);  /// Se inicializa el primer array de Documentos

        Words[] words_1 = Score.GetTFsArr(query_0, files.Length, Docset_1);     /// se inicializa un array del tipo Words
       
        if (proximity_index[0] + proximity_index[1] >= 1 && proximity_index[1] <= words_1.Length -1  )   /// Se determina y establece, en caso de ser posible y necesario, la proximidad de dos palabras en los documentos en los que estén    
        {
            for (int i = 0; i < Docset_1.Length; i++)
            {
                Docset_1[i].proximity = Docset_1[i].proximity + (Documents.ProximitySetter(proximity_index[0], proximity_index[1], Docset_1[i], words_1, Docset_1.Length) + 0.0001);
            }
        }  

        for (int i = 0; i < words_1.Length; i++)      /// Se establecen los caracteres especiales de los objetos tipo Word
        {
            words_1[i].SpecialsSetter(splited_query[i], words_1[i]);
        }

        Documents[] Docset_2 = Documents.TotalScoreSetter(words_1, Docset_1);  /// Se establece el score de los documentos segun el TF - IDF 
        
        for (int i = 0; i < Docset_1.Length; i++)            /// Se modifica el score segun la presencia de caracteres especiales
        {
            Documents.FinalScoreSetter(Docset_2[i], words_1);
        }

        Documents[] Docset_03 = Documents.DocumentsSort(Docset_1);  ///Se organizan los Documentos segun el score

        string[] query_02 = new string[query_0.Length]; 

        string sugg = " ";      

        for (int i = 0; i < query_02.Length; i++)      /// Se determinan palabras semejantes a la búsqueda
        {
            query_02[i] = Sugestions.Comparer(Docset_2, query_0[i]);
        }

        for (int i = 0; i < query_02.Length; i++)       /// se determina una sugerencia
        {
            sugg = sugg + " " + query_02[i];
        }

        for (int i = 0; i < Docset_03.Length && i < 5; i++) ///Se establece un fragmento donde aparece una palabra de la búsqueda
        {
            for (int j = 0; j < words_1.Length; j++)
            {
                if (words_1[j].LocalTF[Docset_03[i].index] != 0)
                {
                    Docset_03[i].snipet = Documents.SnipetGetter(Docset_03[i].content_splited, words_1[j].word);
                }
            }
        }
        if (Docset_03[0].total_score > 0.00000001)               /// Si hay al menos un documento con score significativo, se termina la busqueda
        {
            for (int j = 4; j >= 0; j--)
            {
                if (Docset_03[j].total_score > 0.00000001)
                {
                    SearchItem[] items_03 = new SearchItem[j + 1];

                    for (int i = 0; i < Docset_03.Length && i <= j; i++)
                    {
                        items_03[i] = new SearchItem(Docset_03[i].title, Docset_03[i].snipet, Convert.ToSingle(Docset_03[i].total_score));
                    }
                    return new SearchResult(items_03, sugg);
                }
            }
        }
        
        /// En caso de que la búsqueda no haya sido satisfactoria, se realiza nuevamente la búsqueda  con la sugerencia

        Words[] words_2 = Score.GetTFsArr(query_02, files.Length, Docset_1);

        for (int i = 0; i < words_2.Length; i++)
        {
            words_2[i].SpecialsSetter(splited_query[i], words_2[i]);
        }

        Documents[] Docset_2_2 = Documents.TotalScoreSetter(words_2, Docset_1);

        for (int i = 0; i < Docset_2_2.Length; i++)
        {
            Documents.FinalScoreSetter(Docset_2_2[i], words_1);
        }
        
        Documents[] Docset_3_2 = Documents.DocumentsSort(Docset_2_2);

        for (int i = 0; i < Docset_3_2.Length && i < 5; i++)
        {
            for (int j = 0; j < words_2.Length; j++)
            {
                if (words_2[j].LocalTF[Docset_3_2[i].index] != 0)
                {
                    Docset_3_2[i].snipet = Documents.SnipetGetter(Docset_3_2[i].content_splited, words_2[j].word);
                }
            }          
        }

        for (int j = 4; j >= 0; j--)
        {
            if (Docset_3_2[j].total_score > 0.000000001)
            {
                SearchItem[] items_3 = new SearchItem[j + 1];
                
                for (int i = 0; i < Docset_3_2.Length && i <= j ; i++)
                {
                    items_3[i] = new SearchItem(Docset_3_2[i].title, Docset_3_2[i].snipet, Convert.ToSingle(Docset_3_2[i].total_score));
                }
                return new SearchResult(items_3, sugg);
            }         
        }

        SearchItem[] items = new SearchItem[1];
        items[0] = new SearchItem("No se encontró resultados ", "..." ,0f);
        
        return new SearchResult(items, sugg);       
    }   
}
