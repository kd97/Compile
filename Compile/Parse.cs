using System;


using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace Compile
{

    class Information
    {
        

        public int id;
        public String name; 
        public String   parentName;
        public Information(int id,String name,String parentName)
        {
            this.id = id;
            this.name = name;
            this.parentName = parentName;
        }

    }


    class Hopelast
    {
        public ArrayList father = new ArrayList ();
        public ArrayList children = new ArrayList();
    }

    
    

    struct WordStruct
    {
        String value;    
        String name;      
        int id;           
        String descri;    
        public int line; 
        ArrayList first;  //first集
        ArrayList follow; //follow集
        public WordStruct(String name, int id, String descri)
        {
            this.name = name;
            this.id = id;
            this.descri = descri;
            this.value = "";
            first = new ArrayList();
            follow = new ArrayList();
            line = -1;
        }

        public WordStruct(String name, int id, String descri, String value)
        {
            this.name = name;
            this.id = id;
            this.descri = descri;
            this.value = value;
            first = new ArrayList();
            follow = new ArrayList();
            line = -1;
        }
     
        public WordStruct(String name, int id, String descri, String value, int line)
        {
            this.name = name;
            this.id = id;
            this.descri = descri;
            this.value = value;
            first = new ArrayList();
            follow = new ArrayList();
            this.line = line;
        }
      
        public void setName(String name)
        {

            this.name = name;

        }
        public String getName()
        {
            return name;
        }
        public int getId()
        {
            return id;
        }
        public String getDecri()
        {
            return descri;
        }
        public String getValue()
        {
            return value;
        }
       
        public void addFirst(String f)
        {
            first.Add(f);
        }
        public void addFirst(ArrayList f)
        {
            foreach (Object o in f)
            {
                if (!this.first.Contains(o))
                {
                    this.first.Add(o);
                }
            }
        }
        public void clearFirst(String f)
        {
            this.first = new ArrayList();
        }
        public ArrayList getFirst()
        {
            return first;
        }
    
        public void addFollow(String f)
        {
            follow.Add(f);
        }
        public void addFollow(ArrayList f)
        {
            foreach (Object o in f)
            {
                if (!this.follow.Contains(o))
                {
                    this.follow.Add(o);
                }
            }
        }
        public void clearFollow(String f)
        {
            this.follow = new ArrayList();
        }
        public ArrayList getFollow()
        {
            return follow;
        }
    }
 
    class Parse
    {
        private Stack<WordStruct> input;     
        private Stack<WordStruct> derivating;
        public  Hashtable nTerminals;       //非终结符集
        private Hashtable terminals;        
        public Hashtable products;         
        public Hashtable products3;        
        private Hashtable deriTable;        
        List <Information> informationlist = new List<Information>(400);
        public ArrayList treelist = new ArrayList();
        public Hopelast hopelast = new Hopelast();
        TreeNode [] tree = new TreeNode[150];
        public ArrayList m = new ArrayList();
        public ArrayList h = new ArrayList();
        public Parse()
        {
            input = new Stack<WordStruct>();
            derivating = new Stack<WordStruct>();           
        
            nTerminals = new Hashtable();
            readNTerminal();
          
            terminals = new Hashtable();
            readTerminal();
          
            products = new Hashtable();
            initProducts();
            products3 = new Hashtable();
            initProducts3();
          
            getFirstX();
           
            getFollowX();
          
            getSelect();
           
            deriTable = new Hashtable();         
            productTable();
        }
        
        public List<Information> parsexe(String phrase, out String sen, out String errIn)
        {
            hopelast.father.Add("程序");
            ArrayList pro = new ArrayList();
            pro.Add("开始");
            hopelast.children.Add(pro);
            sen = "";
            errIn = "";
            
            readIn(phrase);
            Meaning.setInit();
            int k = 0;
            //int line = 1;
            while (input.Count > 0)
            {
                if (derivating.Count < 1)
                {
                    errIn += "错误" + "\r\n";
                    break;
                }
                WordStruct x = derivating.Pop();
                derivating.Push(x);
                WordStruct a = input.Pop();    
                input.Push(a);
            
               
                if (x.getId() > 490)        
                {
                    WordStruct y=derivating.Pop();
                    Meaning.run(int.Parse(x.getName()), a.getValue(), "" + a.line);
                }
                else if (x.getId() < 290)       //终结符
                {

                    if (x.getName().Equals(a.getName())) 
                    {
                        WordStruct y = derivating.Pop();           
                        treelist.Add(y.getName());
                        input.Pop();                    
                    }
                    else
                    {
                        
                        errIn += ("Error at line " + a.line + "\t输入 " + WForm.name(a) + "输入与推导符号串栈中的不匹配" + "\r\n");
                       
                        WordStruct y = derivating.Pop();           
                        treelist.Add(y.getName());
                      
                    }
                }
                else                        
                {
                    Product p = (Product)(((Hashtable)deriTable[x.getName()])[a.getName()]);
                    if (p == null)
                    {
                        errIn += "系统错误" + "\r\n";
                        input.Pop();
                    }
                    else if (p.getLeft().Equals("synch"))
                    {
                        errIn += "错误：分析栈顶为 <" + x.getName() + "> 进入错误处理 " + a.getName() + "\r\n";
                        WordStruct y = derivating.Pop();           
                        treelist.Add(y.getName());
                    }
                    else if (p.getLeft().Equals("error"))
                    {
                        errIn += "错误：输入栈顶为 " + a.getName() + " 不可用分析栈顶文法错误 <" + x.getName() + "> 规约\r\n";
                        input.Pop();
                    }
                    else//输出语法分析结果
                    {
                        


                        sen += "line "+a.line+"\t<" + x.getName() + "> -> ";
                        hopelast.father.Add(x.getName());
                        WordStruct y = derivating.Pop();           //将该符号从推导符号串栈中弹出
                        treelist.Add(y.getName());
                        ArrayList right = p.getRight();
                        //把产生式右部压入栈
                        for (int i = 0; i < right.Count; i++)
                        {
                            Object o = nTerminals[right[right.Count - i - 1]];
                            if (o != null)
                            {

                                WordStruct w = (WordStruct)o;
                                if (!w.getName().Equals("空"))
                                {
                                    derivating.Push(w);
                                }
                            }
                            else
                            {
                                o = terminals[right[right.Count - i - 1]];
                                if (o != null)
                                {
                                    derivating.Push((WordStruct)o);
                                }
                                else
                                {
                                    derivating.Push(new WordStruct((String)(right[right.Count - i - 1]),500,"代码"));
                                }
                            }
                        }
                        //输出产生式
                        
                        ArrayList tmp = new ArrayList();
                        for (int i = 0; i < right.Count; i++)
                        {
                            Object o = nTerminals[right[i]];
                            if (o != null)
                            {
                                sen += "<" + right[i] + ">";

                                k++;
                                Information one;
                                one = new Information(k, right[i] + "",x.getName());
                                informationlist.Add(one);
                                tmp.Add(right[i] + "");
                            }
                            else
                            {
                                o = terminals[right[i]];

                                if (o != null)
                                {
                                    
                                    sen += "" + right[i] + "";
                                    k++;
                                    Information one = new Information(k, right[i] + "", x.getName());
                                    

                                    informationlist.Add(one);
                                    tmp.Add(right[i]+ "");
                                
                                   
                                }
                                
                            }

                            
                        }
                        hopelast.children.Add(tmp);
                       
                        sen += "\r\n";
                        //treeMore.Add(oneTree);
                    }
                }
              
            }
            errIn += "\r\n语义分析错误:\r\n";
            errIn += Meaning.errIn;
            return informationlist;
        }

       
        void readNTerminal()
        {
           
            StreamReader r = new StreamReader(@"..\\..\\tool\\nTerminals.txt");
            String temp;
            for (int i = 0; i < 100; i++)     
            {
                temp = r.ReadLine();
                if (temp == null || temp.Equals(""))
                {
                    break;
                }
                nTerminals[temp] = new WordStruct(temp, 300 + i, temp);
            }
            r.Close();
            
        }


       
        void readTerminal()
        {
            terminals["结束"] = new WordStruct("结束", -1, "结束");
            
            StreamReader r = new StreamReader(@"..\\..\\tool\\terminals.txt");
            String temp;
            for (int i = 0; i < 100; i++)
            {
                temp = r.ReadLine();
                if (temp == null || temp.Equals(""))
                {
                    break;
                }
                terminals[temp] = new WordStruct(temp, i, temp);
            }
            r.Close();
        }
       
        void initProducts()
        {
            
          
            String left = "";
            ArrayList cans = new ArrayList();
            ArrayList right = new ArrayList();
            StreamReader r = new StreamReader(@"..\\..\\tool\\products.txt");
            string last = "开始";
            string temp = "";
            for (int i = 0; i <70 ; i++)
            {

                temp = r.ReadLine();
                if (temp == null || temp.Equals(""))
                {
                    break;

                }
                string[] tmp = temp.Split((new char[2] { 'y', 'y' }));
                if (last == tmp[0])
                {

                    right = new ArrayList();
                    int u = 1;
                    for (; u < tmp.Length; u++)
                    {
                        if (tmp[u] != "")
                            right.Add(tmp[u]);

                    }


                    cans.Add(new Product(tmp[0], right));

                }
                if (last != tmp[0])
                {
                    left = last;
                    products[left] = cans;
                    cans = new ArrayList();
                    right = new ArrayList();
                    int u = 1;
                    for (; u < tmp.Length; u++)
                    {
                        if (tmp[u] != "")
                            right.Add(tmp[u]);

                    }

                    cans.Add(new Product(tmp[0], right));


                   
                    last = tmp[0];
                }



            }

            r.Close();
           

        }

    
        void initProducts3()
        {
            
            String left = "";
            ArrayList cans = new ArrayList();
            ArrayList right = new ArrayList();
            StreamReader r = new StreamReader(@"..\\..\\tool\\products3.txt");
            string last = "开始";
            string temp = "";
            for (int i = 0; i < 70; i++)
            {

                temp = r.ReadLine();
                if (temp == null || temp.Equals(""))
                {
                    break;

                }
                string[] tmp = temp.Split((new char[2] { 'y', 'y' }));

                if (last == tmp[0])
                {

                    right = new ArrayList();
                    int u = 1;
                    for (; u < tmp.Length; u++)
                    {
                        if (tmp[u] != "")
                            right.Add(tmp[u]);

                    }


                    cans.Add(new Product(tmp[0], right));

                }
                if (last != tmp[0])
                {
                    left = last;
                    products3[left] = cans;
                    cans = new ArrayList();
                    right = new ArrayList();
                    int u = 1;
                    for (; u < tmp.Length; u++)
                    {
                        if (tmp[u] != "")
                            right.Add(tmp[u]);

                    }

                    cans.Add(new Product(tmp[0], right));


                    
                    last = tmp[0];
                }



            }

            r.Close();

        }
      
        void getFirstX()
        {
            
           
            foreach (DictionaryEntry d in terminals)
            {
                WordStruct w = (WordStruct)d.Value;
                w.addFirst(w.getName());
                m.Add(w.getName() + "\t" + w.getName()+"\t");
            }
        
            foreach (DictionaryEntry d in products)
            {
                ArrayList a = (ArrayList)d.Value;
                WordStruct n = (WordStruct)nTerminals[(String)d.Key];
                foreach (Product p in a)
                {
                    String f = (String)((ArrayList)p.getRight())[0];
                    if (terminals[f] != null)
                    {
                        n.addFirst(f);
                        m.Add(n.getName() + "\t" + f+"\t");
                    }
                }
            }
            Boolean end = false;
            while (!end)
            {
                end = true;
                foreach (DictionaryEntry d in products)
                {
                    ArrayList a = (ArrayList)d.Value;
                    WordStruct n = (WordStruct)nTerminals[(String)d.Key];
                    foreach (Product p in a)
                    {
                        ArrayList right = (ArrayList)p.getRight();
                        String f = (String)(right)[0];
                        //X->Y..
                        if (nTerminals[f] != null)
                        {
                            WordStruct w = (WordStruct)nTerminals[f];
                            foreach (String s in w.getFirst())
                            {
                                if (!n.getFirst().Contains(s))
                                {
                                    end = false;
                                    n.addFirst(s);
                                    m.Add(n.getName() + "\t" + s+"\t");
                                }
                            }
                        }
                   
                        for (int i = 0; i < right.Count && !((String)(right)[i]).Equals("空") && nTerminals[(String)(right)[i]] != null; i++)
                        {
                            //判断是否能否退出空
                            Boolean isNull = false;
                            foreach (Product ap in (ArrayList)products[(String)(right)[i]])
                            {
                                if (ap.getRight().Count == 1 && ap.getRight()[0].Equals("空"))
                                {
                                    isNull = true;
                                    break;
                                }
                            }
                            if (isNull)
                            {
                                WordStruct w = (WordStruct)nTerminals[(String)(right)[i]];
                                foreach (String s in w.getFirst())
                                {
                                    if (!n.getFirst().Contains(s))
                                    {
                                        end = false;
                                        n.addFirst(s);
                                        m.Add(n.getName() + "\t" + s+"\t");
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }

                        }

                    }
                }
            }

        }
       
        ArrayList getFirstXs(ArrayList a)
        {
            ArrayList b = new ArrayList();
            if (a.Count < 1)
            {
                return b;
            }

            Object o = nTerminals[(String)a[0]];
            if (o != null)
            {
                WordStruct w = (WordStruct)o;
                foreach (String s in w.getFirst())
                {
                    if (!b.Contains(s))
                    {
                        b.Add(s);
                    }
                }
            }
            else
            {
                o = terminals[(String)a[0]];
                if (o != null)
                {
                    WordStruct w = (WordStruct)o;
                    foreach (String s in w.getFirst())
                    {
                        if (!b.Contains(s))
                        {
                            b.Add(s);
                        }
                    }
                }
            }
            //for
            for (int i = 0; i < a.Count-1 && !((String)(a)[i]).Equals("空") && nTerminals[(String)(a)[i]] != null; i++)
            {
                Boolean isNull = false;
                foreach (Product ap in (ArrayList)products[(String)(a)[i]])
                {
                    if (ap.getRight().Count == 1 && ap.getRight()[0].Equals("空"))
                    {
                        isNull = true;
                        break;
                    }
                }
                if (isNull)
                {
                    Object oo = nTerminals[(String)(a)[i+1]];
                    if (oo != null)
                    {
                        WordStruct w = (WordStruct)oo;
                        foreach (String s in w.getFirst())
                        {
                            if (!b.Contains(s))
                            {
                                b.Add(s);
                            }
                        }
                    }
                    else
                    {
                        oo = terminals[(String)(a)[i + 1]];
                        if (oo != null)
                        {
                            WordStruct w = (WordStruct)oo;
                            foreach (String s in w.getFirst())
                            {
                                if (!b.Contains(s))
                                {
                                    b.Add(s);
                                }
                            }
                        }
                    }
                }
                else
                {
                    break;
                }

            }
            return b;
        }
      
        void getFollowX()
        {
            
            
            WordStruct w = (WordStruct)nTerminals["开始"];
            w.addFollow("结束");
        
            foreach (DictionaryEntry d in products)
            {
                ArrayList a = (ArrayList)d.Value;
                WordStruct n = (WordStruct)nTerminals[(String)d.Key];
                foreach (Product p in a)//遍历产生式
                {
                    ArrayList right = p.getRight();
                    foreach (String b in right)
                    {
                        if (right.IndexOf(b) < right.Count - 1)
                        {
                            ArrayList aa = new ArrayList();
                            for (int i = right.IndexOf(b) + 1; i < right.Count; i++)
                            {
                                aa.Add(right[i]);
                            }
                            ArrayList fs = getFirstXs(aa);
                            Object oo = nTerminals[b];
                            if (oo != null)
                            {
                                WordStruct ww = (WordStruct)nTerminals[b];
                                foreach (String c in fs)
                                {
                                    if (!ww.getFollow().Contains(c))
                                    {
                                        ww.addFollow(c);
                                        //tmp += c+" ";
                                        h.Add(ww.getName() + "\t" +c+"\t");
                                    }
                                }
                            }
                        }

                    }
                }
            }
        
            Boolean end = false;
            while (!end)
            {
                end = true;
                foreach (DictionaryEntry d in products)
                {
                    ArrayList a = (ArrayList)d.Value;
                    WordStruct n = (WordStruct)nTerminals[(String)d.Key];
                    foreach (Product p in a)
                    {
                        ArrayList right = p.getRight();
                        String b = (String)right[right.Count - 1];
                        Object oo = nTerminals[b];
                        if (oo != null)
                        {
                            WordStruct ww = (WordStruct)nTerminals[b];
                            foreach (String c in n.getFollow())
                            {
                                if (!ww.getFollow().Contains(c))
                                {
                                    end = false;
                                    ww.addFollow(c);
                                    h.Add(ww.getName() + "\t" + c+"\t");
                                   
                                }
                            }
                          
                            for (int i = right.Count - 1; i > 0 && !((String)(right)[i]).Equals("空") && nTerminals[(String)(right)[i]] != null; i--)
                            {
                               
                                Boolean isNull = false;
                                foreach (Product ap in (ArrayList)products[(String)(right)[i]])
                                {
                                    if (ap.getRight().Count == 1 && ap.getRight()[0].Equals("空"))
                                    {
                                        isNull = true;
                                        break;
                                    }
                                }
                                if (isNull)
                                {
                                    String bb = (String)right[i - 1];
                                    Object ooo = nTerminals[bb];
                                    if (ooo != null)
                                    {
                                        WordStruct www = (WordStruct)ooo;
                                        foreach (String c in n.getFollow())
                                        {
                                            if (!www.getFollow().Contains(c))
                                            {
                                                end = false;
                                                www.addFollow(c);
                                                //tmp += c + " ";
                                                h.Add(www.getName() + "\t" + c+"\t");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }

                            }
                        }
                    }
                }
            }

        }
      



        void getSelect()
        {
            foreach (DictionaryEntry d in products)
            {
                ArrayList a = (ArrayList)d.Value;
                WordStruct n = (WordStruct)nTerminals[(String)d.Key];
                foreach (Product p in a)
                {
                   
                    ArrayList right = p.getRight();
                    if (right.Count == 1 && right[0].Equals("空"))
                    {
                        p.addSelect(n.getFollow());
                    }
                    else
                    {
                        p.addSelect(getFirstXs(right));
                    }
                }
            }
        }
     



        void productTable()
        {
            foreach (DictionaryEntry d1 in nTerminals)
            {
                if (d1.Key.Equals("空"))
                    continue;
                ArrayList a = (ArrayList)products[(String)d1.Key];
                ArrayList b = (ArrayList)products3[(String)d1.Key];
                for (int i = 0; i < a.Count;i++ )
                {
                    ((Product)b[i]).addSelect(((Product)a[i]).getSelect());
                }
            }

            foreach (DictionaryEntry d1 in nTerminals)
            {
                if (d1.Key.Equals("空"))
                    continue;
                deriTable[d1.Key] = new Hashtable();
                foreach (DictionaryEntry d2 in terminals)
                {
                    ArrayList a = (ArrayList)products3[(String)d1.Key];
                    if (a == null)
                        continue;
                    Boolean isSelect = false;
                    foreach (Product p in a)
                    {
                        if (p.getSelect().Contains(d2.Key))
                        {
                            ((Hashtable)deriTable[d1.Key])[d2.Key] = p;
                            isSelect = true;
                            break;
                        }
                    }
                    if (!isSelect)
                    {
                        ArrayList a2 = ((WordStruct)nTerminals[d1.Key]).getFollow();
                        foreach (String s1 in a2)
                        {
                            if (s1.Equals(d2.Key))
                            {
                                ((Hashtable)deriTable[d1.Key])[d2.Key] = new Product("synch", null);
                                isSelect = true;
                                break;
                            }
                        }
                    }
                    if (!isSelect)
                    {
                        ((Hashtable)deriTable[d1.Key])[d2.Key] = new Product("error", null);
                    }
                }
            }
        }
      


        private void readIn(String phrase)
        {
            Stack<WordStruct> sw = new Stack<WordStruct>();
           
            MatchCollection mc = Regex.Matches(phrase, "line [0-9]+\t(.+)\t[(] (.+), [0-9]+, (.+)");
            foreach (Match m in mc)
            {
                String a = m.Value;
                a = a.Substring(5, a.Length - 5);
                String[] s = a.Split(new char[] { '\t' });
                s[2] = s[2].Substring(0, s[2].Length - 2);
                s[2] = s[2].Substring(2, s[2].Length - 2);
                String[] t = s[2].Split(new char[] { ',', ' ' });

                
                WordStruct w;
                try
                {
                    w = new WordStruct(t[0], int.Parse(t[2]), t[4], s[1], int.Parse(s[0]));
                }
                catch
                {
                    w = new WordStruct(",", int.Parse(t[3]), t[5], s[1], int.Parse(s[0]));
                }
                sw.Push(w);
            }
        
            input.Push((WordStruct)terminals["结束"]);
            while (sw.Count > 0)
            {
                input.Push(sw.Pop());
            }
            derivating.Push((WordStruct)terminals["结束"]);
            derivating.Push((WordStruct)nTerminals["开始"]);
        }
    }
}
