

namespace MoogleEngine;


public static class Moogle
{
    public static SearchResult Query(string query,DocumentSpace space)
    {
        
         return space.DoSearch(query);

    }
    
}

        
