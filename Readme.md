# Moogle!

> Proyecto de Programación I. Facultad de Matemática y Computación. Universidad de La Habana. Curso 2023.

Moogle! es una aplicación *totalmente original* cuyo propósito es buscar inteligentemente un texto en un conjunto de documentos.

Es una aplicación web, desarrollada con tecnología .NET Core 6.0, específicamente usando Blazor como *framework* web para la interfaz gráfica, y en el lenguaje C#.
La aplicación está dividida en dos componentes fundamentales:

- `MoogleServer` es un servidor web que renderiza la interfaz gráfica y sirve los resultados.
- `MoogleEngine` es una biblioteca de clases donde está implementada la lógica del algoritmo de búsqueda.



## Ejecutar

- Para ejecutar el programa puede simplemente ejecutar el script run.sh o desde una terminal dotnet run --project MoogleServer.
- En ambos casos deberá abrir el explorador en: localhost:5285/ .

## Sobre la búsqueda

El programa realiza la búsqueda de las palabras introducidas por el usuario en una colección de documentos utilizando el `modelo vectorial` 
del `TF-IDF` y utiliza la similitud coseno para determinar la relevancia de cada documento.
Para el idioma español filtra, tanto en el input del usuario como en los documentos, palabras irrelevantes como pronombres, preposiciones, etc... .Realiza también una búsqueda con las palabras raíces de los documentos y el input del usuario aunque con un menor impacto en la importnacia del documento

Para ofrecer mayor presición incorpora los siguientes caracteres especiales:
- *!* colocado antes de una palabra implicará que se descartarán todos los documentos donde aparezca la palabra.
- *^* colocado antes de una palabra implicará que se descartarán todos los documentos donde no aparezca la palabra.
- *** colocado antes de una palabra implicará que los documentos donde esta aparezca serán más importantes, este caracter es
acumulativo.
- *~* colocado entre dos palabras implica que mientras más cercanas estén estas en un documento, más importante éste es.

Nota: Si se colocan dos caracteres seguidos, salvo ***, solo se tomará en consideración el primero, además no serán reconocidos si se colocan entre dos palabras sin espacio ejemplo: "corría`!`asustado". 

Si la búsqueda resulta insatisfactoria, esta se realizará nuevamente sustituyendo las palabras del query que no aparecieron en ningún 
documento por otras similares que sí aparecen utilizando el algoritmo de Damerau - Levenshtein.





