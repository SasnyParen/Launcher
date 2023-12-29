using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class NewsJson: IEnumerable
{
    public List<News> News { get; set;} = new List<News>();

    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }
}

