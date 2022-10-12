using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoogleEngine
{
    public static class Sugestions
    {
        public static double DiferenciaStrings(String s1, String s2) /// Devuelve la semejanza de dos palabras 
        {
            double counter = 0;
            if (s1 != null && s2 != null)
            {
                for (int k = 0; k < s1.Length && k < s2.Length; k++)
                {
                    if (s1.ElementAt(k) == s2.ElementAt(k)) counter++;
                }
                    return counter * (10/(Math.Abs(s1.Length - s2.Length) + 0.1) );

            }

            return counter ;
        }
        public static string Comparer(Documents[] Docs, string query) /////Se determina la palabra más "parecida" a la palabra introducida
        {
            
            string word = null;

            for (int j = 0; j < Docs.Length; j++)
            {
                for (int i = 0; i < Docs[j].content_splited.Length; i++)
                {
                    if (DiferenciaStrings(query, Docs[j].content_splited[i]) > 0)
                    {
                        if (DiferenciaStrings(word, query) < DiferenciaStrings(query, Docs[j].content_splited[i]) || word == null && word != query)
                        {
                            word = Docs[j].content_splited[i];
                        }
                    }
                }
            }

            return word;
        } 
    }
}
