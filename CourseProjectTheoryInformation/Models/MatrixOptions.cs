using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSender.Models
{
    internal static class MatrixOptions
    {
        public static List<List<int>> TransposeMatrix(List<List<int>> Matr)
        {
            var rows = Matr.Count;
            var cols = Matr[0].Count;

            var result = new List<List<int>>();

            for (var i = 0; i < cols; i++)
            {
                result.Add(new List<int>());

                for (var j = 0; j < rows; j++) result[i].Add(Matr[j][i]);
            }

            return result;
        }
    }
}
