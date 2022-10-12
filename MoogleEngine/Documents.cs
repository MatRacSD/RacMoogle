using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoogleEngine
{
    public class Documents
    {
        public Documents(string title, string content, int index) ///Método para iniciar los objetos de tipo Documento
        {
            this.title = title;

            content = content.ToLower();

            content_splited = content.Split(new char[] { ',', ' ', ';', ':', '"', '-', '+', '=', ')', '(', '|', '.', '#', '@', '%', '&', '/', '/', '{', '}', '[', ']', '*', '~', '!','?', '¡', '¿', '^' }, StringSplitOptions.RemoveEmptyEntries);

            this.index = index;

            total_score = 0.000000000001;

            proximity = 1;

            snipet = "";


        }
        public static Documents[] FirstDocumentsGetters(string[] path, int Length) /////Método para iniciar un array de objetos del tipo Documentos
        {
            Documents[] algo = new Documents[Length];                               

            for (int i = 0; i < Length; i++)                                      
            {
                StreamReader srt = new StreamReader(path[i]);

                string lerop = srt.ReadToEnd();

                string filename = Path.GetFileName(path[i]);

                algo[i] = new Documents(filename, lerop, i);
            }

            return algo;
        }

        public static double Proximity(string s1, string s2, string[] content, Documents docs) ////establece la proximidad de dos palabras en el documento dividiendo a 30 (mientras más cerca, mayor el valor que devuelve)
        {
            int s1_indx = Array.IndexOf(content, s1);

            int s2_indx = Array.IndexOf(content, s2);

            return  30/(Math.Abs(s2_indx - s1_indx) + 1);
        }

        public string title;  /// Representa el título del documento

        public string[] content_splited; /// Representa un array separado de palabras del documento

        public int index;  ///representa el índice del documento

        public double total_score; /// Representa la importancia del documento

        public double proximity; /// Valor que modifica al Score del documento, según que tan cerca estén las palabras adyacentes al caracter '~'

        public string snipet;  /// Representa una cadena donde aparece una palabra introducida por el usuario

        public static Documents[] TotalScoreSetter(Words[] asd, Documents[] rts)  /// Determina el score de cada documento de un Array de documentos aplicando TF -IDF
        {
            for (int i = 0; i < asd.Length; i++)
            {
                for (int j = 0; j < rts.Length; j++)
                {
                    rts[j].total_score = rts[j].total_score + asd[i].LocalTF[j] * ((rts.Length - 0.1) / (asd[i].Total_IDF - asd[i].LocalTF[j] + 1));
                }
            }
            return rts;
        }
        public static Documents[] DocumentsSort(Documents[] docset_3) /// organiza los documentos segun  su score
        {
            List<Documents> list = docset_3.ToList();

            list.Sort((x, y) => y.total_score.CompareTo(x.total_score));

            Documents[] arrList = list.ToArray();

            return arrList;
        }
        public static string SnipetGetter(string[] content, string word) /// Determina el Snipet del Documento introducido
        {
            int index = Array.IndexOf(content, word);

            if (index > 0 && index < content.Length - 2)
            {
                return string.Join(" ", content[index - 1], " ", word, content[index + 1], " ", content[index + 2], " ");
            }

            else if (index > 0 && index < content.Length - 1)
            {
                return string.Join(" ", content[index - 1], " ", word, content[index + 1], " ");
            }

            else if (index > 0)
            {
                return string.Join(" ", content[index - 1], word, " ");
            }

            else if (index < content.Length - 1)
            {
                return string.Join(" ",word, " ", content[index + 1], " ");
            }
            return word; 
        } 

        public static double ProximitySetter(int w1, int w2, Documents docs, Words[] words, int Length) ///Modifica el proximidad en los Documentos en caso de ser necesario
        {
            double prox = 1;

            for (int i = 0; i < Length; i++)
            {
                if (words[w1].LocalTF[i] != 0 && words[w2].LocalTF[i] != 0)
                {
                    prox = prox + Documents.Proximity(words[w1].word, words[w2].word, docs.content_splited, docs);
                }
            }

            return prox;
        }
        public static void FinalScoreSetter(Documents doc, Words[] words)  /// establece el score final del documento aplicando la modificacion por caracteres especiales
        {
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].special_char == '!' && words[i].LocalTF[doc.index] != 0)
                {
                    doc.total_score = 0;
                }

                else if (words[i].special_char == '^' && words[i].LocalTF[doc.index] == 0)
                {
                    doc.total_score = 0;
                }

                

                if (words[i].multiplicity > 1)
                {
                    doc.total_score = doc.total_score + words[i].multiplicity * words[i].LocalTF[doc.index];
                }

                

                

                if (doc.proximity != 1)
                {
                    doc.total_score = doc.total_score *  doc.proximity;
                }
            }
        }
    }
}
