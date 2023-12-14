using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class News
{ 
    public string Main { get; set; }
    public string Sub { get; set; }
    public News(string Main, string Sub) {
        this.Main = Main;
        this.Sub = Sub;
    }
}
