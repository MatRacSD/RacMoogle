using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoogleEngine
{
    public class Words
    {
        public Words(string word, int length)  /// Método para iniciar objetos del tipo documento
        {
            this.word = word;
            LocalTF = new double[length];
            this.Total_IDF = 0;
            special_char = '0';
            multiplicity = 1;

        }

        public void Special(string query, Words words)      /// Método para guardar y separar los caracteres especiales
        {
            int indx = -1;

            indx = query.IndexOf('!');

            if (indx == 0)
            {
                words.special_char = '!';
            }

            else if ((indx = query.IndexOf('^')) == 0)
            {
                words.special_char = '^';
            }

            else { words.special_char = '0'; }
           
        }
        public void Multiplicity(string query)
        {
            double indx = 1;
            
            for (int i = 0; i < query.Length;)
            {
                if (query.ElementAt(i) == '*')
                {
                    indx = indx * 2;

                    query = query.Remove(i, i + 1);                   
                }

                else if (query.ElementAt(i) != '*')
                {                   
                    break;
                }
            }
            multiplicity = indx;
           
        }   ///metodo q determina la importancia multiplicativa de una palabra

        public void SpecialsSetter(string query_special, Words word_obj)
        {

            for (int i = 0; i < query_special.Length; i++)
            {
                word_obj.Special(query_special, word_obj);

                word_obj.Multiplicity(query_special);
            }

        }

        public string word;  /// String  que representa cada palabra

        public double[] LocalTF; /// Array de double que contiene la frecuencia de la palabra en cada documento

        public double Total_IDF; /// Valor double que representa el total de apariciones de la palabra en la coleccion de documentos

        public char special_char; /// Variable que contiene el caracter especial de la palabra(en caso de que la introduzca el usuario)

        public double multiplicity; /// Variable que determina la importancia de una palabra según la cantidad de caracteres '*' que el usuario haya colocado a la palabra
    }
}
