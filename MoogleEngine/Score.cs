using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoogleEngine
{
    public static class Score
    {
        public static string[] QuerySpliter(string query, bool bl) /// Divide la búsqueda introducida por el usuario en un array de strings con las palabras de la búsqueda y en otro array de strings que incluye caracteres especiales 
        {
            string[] splited_query;

            if (bl) { splited_query = query.Split(new char[] { ',', ' ', ';', ':', '"', '-', '+', '=', ')', '(', '|', '.', '#', '@', '%', '&', '/', '/', '{', '}', '[', ']', '*', '~', '!', '?', '¡', '¿', '^' }, StringSplitOptions.RemoveEmptyEntries); }

            else { splited_query = query.Split(new char[] { ',', ' ', ';', ':', '"', '-', '+', '=', ')', '(', '|', '.', '#', '@', '%', '&', '/', '/', '{', '}', '[', ']', '~', '?', '¡', '¿' }, StringSplitOptions.RemoveEmptyEntries); }

            List<string> result = splited_query.ToList();

            result = result.Where(x => x != null && x.ElementAt(x.Length - 1) != '!' && x.ElementAt(x.Length - 1) != '*' && x.ElementAt(x.Length - 1) != '^'  && x.Length > 1).ToList();

            return result.ToArray();
        }  

        public static Words[] GetTFsArr(string[] query, int length_2, Documents[] DocSet) ///inicializa  los objetos del tipo words
        {
            Words[] algo = new Words[query.Length];

            for (int i = 0; i < query.Length; i++)
            {
                algo[i] = new Words(query[i], length_2);

                for (int j = 0; j < algo[i].LocalTF.Length; j++)
                {
                    algo[i].LocalTF[j] = LocalTFGetter(algo[i].word, DocSet[j].content_splited);
                }

                algo[i].Total_IDF = GetTotalIDF(algo[i].LocalTF);

            }

            return algo;
        } 

        public static double LocalTFGetter(string word_1, string[] contenido_splited)  ///metodo q retorna un string con el  TF de una palabra en cada documento
        {
            double local_suma = 0.0f;

            for (int i = 0; i < contenido_splited.Length; i++)
            {
                if (contenido_splited[i] == word_1 && contenido_splited[i] != null)
                {
                    local_suma = local_suma + 1.0f;
                }
            }

            return local_suma;
        }

        public static double GetTotalIDF(double[] test)  /// Devuelve todo el IDF de una palabra en una colección de documentos
        {
            double suma_2 = 0;

            for (int i = 0; i < test.Length; i++)
            {
                suma_2 = suma_2 + test[i];
            }

            return suma_2;
        }

        public static string[] SpecialSpliter(string query)  /// Divide la búsqueda en un array de strings que incluye el caracter '~'
        {
            string[] splited_query;

            splited_query = query.Split(new char[] { ',', ' ', ';', ':', '"', '-', '+', '=', ')', '(', '|', '.', '#', '@', '%', '&', '/', '/', '{', '}', '[', ']', '*', '!', '?', '¡', '¿', '^' }, StringSplitOptions.RemoveEmptyEntries);



            List<string> result = splited_query.ToList();

            result = result.Where(x => x != null && x.ElementAt(x.Length - 1) != '!' && x.ElementAt(x.Length - 1) != '*' && x.ElementAt(x.Length - 1) != '^').ToList();

            return result.ToArray();
        }

        public static int[] IndexGetter(string[] ss) ///devuelve los indices de las palabras cuya  proximidad se desea determinar
        {
            int[] arr = new int[2];

            for (int i = 0; i < ss.Length; i++)
            {
                if (ss[i] == "~" && i >= 1 && i < ss.Length )
                {
                    arr[0] = i - 1;
                    arr[1] = i;
                }
            }

            return arr;
        }

    }
}
