using System;
using DefaultNamespace.VREM.Model;

namespace DefaultNamespace.VREM
{
    [Serializable]
    public class ListExhibitionsResponse
    {
        public ExhibitionSummary[] exhibitions;
    }
}