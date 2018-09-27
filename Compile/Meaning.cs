using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace Compile
{
    struct FuHao               
    {


        public string type;


        public int width;

        public string name;
        public int offset;
        
        public FuHao(string name, int offset, string type, int w)
        {
            this.name = name;
            this.offset = offset;
            this.type = type;
            this.width = w;

        }
    }



    class Meaning               
    {
        private static String zxin = "", name = "", fname = "", bxin = "", sxin = "", xin = "", Ez = "", Fz = "", Tz = "", Rz = "", Le = "", Lin = "", wb = "", we = "", p = "", pq = "", bj = "", wy = "", shuZuName = "", fhxin = "", canshuzu = "", hanshuname = "";
        public static int line = 0;                                   //行号
        private static int zw = 0, sw = 0, w = 0, offset = 0, tempi = 0, sd = 0, nw = 0;
        private static Stack<string> tool = new Stack<string>();  //表达式堆栈
        public static string yuyi = "";                               //语义
        public static string four = "";
        public static string errIn = "";                              //错误
        public static Hashtable fuhaoBiao = new Hashtable();          //符号表，用hash表储存
        public static void setInit()
        {
            zxin = "";
            name = ""; 
            fname = "";
            bxin = ""; 
            sxin = ""; 
            xin = "";  
            tempi = 0; 
            wy = "";   
            shuZuName = "";  
            Ez = ""; Fz = ""; Tz = ""; Rz = "";
            Le = ""; Lin = ""; wb = ""; we = "";
            p = ""; pq = ""; bj = "";            
            fhxin = "";                         
            canshuzu = "";                      
            hanshuname = "";                     
            line = 0; zw = 0; sw = 0;              
            w = 0;                              
            offset = 0;                         
            sd = 0; nw = 0;                      
            tool = new Stack<string>();     
            yuyi = "";                           
            errIn = "";                          
            fuhaoBiao = new Hashtable();        
        }
    


        private static string newTmp()
        {
            return ("t" + tempi++);
        }
       



        private static string getTmp(int o)
        {
            return ("t" + (tempi - o));
        }
        



        public static void run(int i, String n, string el)
        {

            switch (i)
            {
                case 1:            
                    zxin = "teshu";
                    zw = 3;
                    break;
                case 2:
                    name = n;        
                    break;
                case 3://char
                    bxin = "char";
                    w = 1;           
                    break;
                case 4://double
                    bxin = "double";
                    w = 8;          
                    break;
                case 5://float
                    bxin = "float";
                    w = 8;
                    break;
                case 6://int
                    bxin = "int";
                    w = 4;
                    break;
                case 7://long
                    bxin = "long";
                    w = 4;
                    break;
                case 8://short
                    bxin = "short";
                    w = 4;
                    break;
                case 9://指针
                    zxin = "pointer( " + zxin + " )";
                    zw = 4;                  
                    break;
                case 10://数组
                    sxin = sxin.Replace(bxin, "array(" + int.Parse(n) + ", " + bxin + ")");//敲定类型，int.Parse(n)是数组大小，bxin是数组类型
                    sw = int.Parse(n) * sw;   
                    break;
                case 11://数组
                    sxin = bxin;    //数组的类型，int，char等
                    sw = w;         //数组的宽度
                    break;
                case 12: 
                    xin = zxin.Replace("teshu", sxin); 
                    w = (4 - zw) * (sw - 3) + zw;  
                    if (fuhaoBiao[name] == null)  
                    {
                        fuhaoBiao[name] = new FuHao(name, offset, xin, w);
                        offset += w;
                    }
                    else    
                    {
                        w = 0;
                        xin = "";
                        errIn += "line " + el + "\t变量 " + name + " 重复声明\r\n";
                    }
                    break;
                case 13:   
                    zxin = "teshu";
                    zw = 3;
                    break;
                case 14:  
                    name = n;
                    sxin = bxin; 
                    sw = w;  

                    break;
                case 15:
                    xin = zxin.Replace("teshu", sxin); 
                    w = (4 - zw) * (sw - 3) + zw;    
                    if (fuhaoBiao[name] == null)
                    {
                        fuhaoBiao[name] = new FuHao(name, offset, xin, w);
                        offset += w;
                    }
                    else
                    {
                        w = 0;
                        xin = "";
                        errIn += "line " + el + "\t变量 " + name + " 重复声明\r\n";
                    }
                    break;
                case 16: 
                    four = "(=," + Tz + "," + "_," + fname + ")";
                    yuyi += "" + (++line) + "\t" + fname + " = " + Tz + "\t" + four + "\t" + "\r\n";

                    
                    break;
                case 17: 
                    fname = name;
                    break;
                case 18: 
                    break;
                case 19: 
                    break;
                case 20: 
                    break;
                case 21:
                    Ez = Tz;  
                    break;
                case 22:
                    string o = newTmp();
                    four = "(+," + Ez + "," + Tz + "," + o + ")";
                    yuyi += "" + (++line) + "\t" + o + " = " + Ez + " + " + Tz + "\t" + four + "\t" + "\r\n";
                    Tz = getTmp(1);
                    four = "(=," + o + "," + "_," + name + ")";
                    yuyi += "" + (++line) + "\t" + name + " = " + o + "\t" + four + "\t" + "\r\n";
                    break;
                case 23:
                    Ez = Tz;
                    break;
                case 24:
                    o = newTmp();
                    four = "(-," + Ez + "," + Tz + "," + o + ")";
                    yuyi += "" + (++line) + "\t" + o + " = " + Ez + " - " + Tz + "\t" + four + "\t" + "\r\n";
                    Tz = getTmp(1);
                    four = "(=," + o + "," + "_," + name + ")";
                    yuyi += "" + (++line) + "\t" + name + " = " + o + "\t" + four + "\t" + "\r\n";
                    break;
                case 25:
                    Tz = Fz;
                    break;
                case 26: 
                    Rz = Fz;
                    break;
                case 27:
                    o = newTmp();
                    four = "(*," + Ez + "," + Tz + "," + o + ")";
                    yuyi += "" + (++line) + "\t" + o + " = " + Ez + " * " + Tz + "\t" + four + "\t" + "\r\n";
                    Fz = getTmp(1);
                    four = "(=," + o + "," + "_," + name + ")";
                    yuyi += "" + (++line) + "\t" + name + " = " + o + "\t" + four + "\t" + "\r\n";

                    break;
                case 28:
                    Rz = Fz;
                    break;
                case 29:
                    o = newTmp();
                    four = "(/," + Ez + "," + Tz + "," + o + ")";
                    yuyi += "" + (++line) + "\t" + o + " = " + Ez + " / " + Tz + "\t" + four + "\t" + "\r\n";
                    Fz = getTmp(1);
                    four = "(=," + o + "," + "_," + name + ")";
                    yuyi += "" + (++line) + "\t" + name + " = " + o + "\t" + four + "\t" + "\r\n";
                    break;

                case 30:
                    Fz = n;
                    break;
                case 31:
                    Fz = n;
                    break;
                case 32: 
                    tool.Push(Ez);
                    tool.Push(Tz);
                    tool.Push(Fz);
                    tool.Push(Rz);
                    break;
                case 33: 
                    Tz = tool.Pop();
                    Ez = tool.Pop();
                    Rz = tool.Pop();
                    Fz = tool.Pop();
                    Fz = getTmp(1);
                    break;
                case 34:  

                    sd = 0;
                    break;
                case 35:
                    
                    Fz = getTmp(1);
                    break;
                case 36:
                    tool.Push(Ez);
                    tool.Push(Tz);
                    tool.Push(Fz);
                    tool.Push(Rz);
                    
                    sd++;
                    break;
                case 37: 
                    Rz = tool.Pop();
                    Fz = tool.Pop();
                    Tz = tool.Pop();
                    Ez = tool.Pop();
                    FuHao f = (FuHao)fuhaoBiao[shuZuName];
                    String[] s = f.type.Split(", ".ToCharArray());
                    try
                    {
                        String ss = s[2 * sd - 2];   
                        ss = ss.Substring(6, ss.Length - 6);
                        nw = nw / int.Parse(ss); 



                        yuyi += "" + (++line) + "\t" + newTmp() + " = " + getTmp(2) + " * " + nw + "\r\n";
                        if (sd > 1) 
                        {
                            yuyi += "" + (++line) + "\t" + newTmp() + " = " + getTmp(2) + " + " + wy + "\r\n";
                        }
                        wy = getTmp(1);
                    }
                    catch
                    {
                        nw = 0;
                        errIn += "line " + el + "\t变量 " + shuZuName + " 数组引用错误\r\n";
                    }
                    break;
                case 38:;
                    if (fuhaoBiao[n] == null)
                    {

                        errIn += "line " + el + "\t变量 " + " 没有声明即使用\r\n";
                    }
                    shuZuName = n;
                    try
                    {
                        nw = ((FuHao)fuhaoBiao[n]).width;
                    }
                    
                    catch (NullReferenceException e)
                    {

                    }
                    break;
                case 39:
                    
                    if (pq == getTmp(1))
                    {
                        MessageBox.Show(pq + "  " + name + "  " + getTmp(1) + Tz);
                        four = "(j" + bj + "," + pq + "," + Tz + "," + (line + 3) + ")";

                        p = pq + bj + Tz;
                    }
                    else
                    {
                        four = "(j" + bj + "," + pq + "," + getTmp(1) + "," + (line + 3) + ")";
                    }
                    yuyi += "" + (++line) + "\tif " + p + " goto " + (line + 2) + "\t" + four + "\t" + "\r\n";
                    four = "(j,_,_,_, Le )";
                   
                    yuyi += "" + (++line) + "\tgoto Le " + "\t" + four + "\t" + "\r\n";
                    break;
                case 40:       
                    four = "(j,_,_,_, Lin )";
                    
                    yuyi += "" + (++line) + "\tgoto Lin" + "\t" + four + "\t" + "\r\n";
                    break;
                case 41:      
                    Le = "" + (line + 1);
                    yuyi = yuyi.Replace("Le", Le);
                    Lin = "" + (line + 1);
                    yuyi = yuyi.Replace("Lin", Lin);
                    break;
                case 42:     
                    Le = "" + (line + 1);
                    yuyi = yuyi.Replace("Le", Le);
                    Lin = "" + (line + 1);
                    yuyi = yuyi.Replace("Lin", Lin);
                    break;
                case 43: 
                    if (pq == getTmp(1))
                    {
                        MessageBox.Show(pq + "  " + name + "  " + getTmp(1) + Tz);
                        four = "(j" + bj + "," + pq + "," + Tz + "," + (line + 3) + ")";

                        p = pq + bj + Tz;
                    }
                    else
                    {
                        
                        four = "(j" + bj + "," + pq + "," + getTmp(1) + "," + (line + 3) + ")";
                    }
                    yuyi += "" + (++line) + "\tif " + p + " goto " + (line + 2) + "\t" + four + "\t" + "\r\n";//条件正确的话去那一行
                    four = "(j,_,_,_, We )";
                  

                    yuyi += "" + (++line) + "\tgoto We" + "\t" + four + "\t" + "\r\n";                            //条件错误的话去那一行
                    tool.Push(wb);
                    break;
                case 44:
                    wb = tool.Pop();
                    four = "(j,_,_,_," + wb + ")";
                   
                    yuyi += "" + (++line) + "\tgoto " + wb + "\t" + four + "\t" + "\r\n"; 
                    we = "" + (line + 1);
                    yuyi = yuyi.Replace("We", we);                   
                    break;
                case 45:   

                    wb = "" + (line + 1);
                    tool.Push(wb);  
                    break;
                case 46:  
                    wb = tool.Pop();
                    four = "(j,_,_,_," + wb + ")";
                    
                    yuyi += "" + (++line) + "\tif " + p + " goto " + wb + "\t" + four + "\t" + "\r\n"; //while（）中的条件，正确的话跳转到wb
                    break;
                case 47: 

                    p = "" + getTmp(1) + "!=0";
                    break;
                case 48:
                    pq = getTmp(1);
                    break;
                case 49: 
                    p = pq + bj + getTmp(1);
                    break;
                case 50: //比较符号
                    bj = n;
                    break;
                case 51: //比较
                    bj = n;
                    break;
                case 52: //比较
                    bj = n;
                    break;
                case 53: //比较
                    bj = n;
                    break;
                case 54: //比较
                    bj = n;
                    break;
                case 55: //比较
                    bj = n;
                    break;
                case 56:
                    Le = "" + (line + 1);
                    yuyi = yuyi.Replace("Le", Le); 
                    break;
                case 57:  
                    wb = "" + (line + 1);
                    break;
                case 58:   
                    fname = name;
                    break;
                case 59:
                    if (sd > 0)
                    { 

                        o = newTmp();
                        four = "(=," + shuZuName + "[" + wy + "]" + "," + "_," + o + ")";
                       
                        yuyi += "" + (++line) + "\t" + o + " = " + shuZuName + "[" + wy + "]" + "\t" + four + "\t" + "\r\n";
                      
                        FuHao ff = (FuHao)fuhaoBiao[shuZuName];

                        String[] sss = ff.type.Split(", ".ToCharArray()); 
                        String ss22;

                        try
                        {
                            ss22 = sss[2 * sd]; 
                        }
                        catch
                        {
                            errIn += "line " + el + "\t变量 " + shuZuName + " 引用错误\r\n";
                            return;
                        }

                        if (ss22.Substring(0, 3).Equals("int") || ss22.Substring(0, 5).Equals("float"))
                        {
                        }
                        else
                        {
                            errIn += "line " + el + "\t变量 " + shuZuName + " 未引用到基本类型\r\n";
                        }


                    }
                    else
                    {
                        o = newTmp();
                        four = "(=," + shuZuName + "," + "_," + o + ")";
                        yuyi += "" + (++line) + "\t" + o + " = " + shuZuName + "\t" + four + "\t" + "\r\n";
                    }
                    break;
                case 60:         
                    hanshuname = name;
                    fhxin = zxin.Replace("teshu", bxin);
                    canshuzu = "";

                    break;
                case 61:        
                    zxin = "teshu";
                    break;
                case 62:       
                    sxin = bxin;
                    break;
                case 63:        
                    String canshu = zxin.Replace("teshu", sxin);
                    if (canshuzu.Equals("")) 
                    {
                        canshuzu = canshu;
                    }
                    else                   
                    {
                        canshuzu = canshuzu + "X" + canshu;
                    }
                    break;
                case 64:
                    if (fuhaoBiao[hanshuname] == null)
                    {
                        if (canshuzu.Equals(""))  
                        {
                            canshuzu = "空";
                        }
                        fuhaoBiao[hanshuname] = new FuHao(hanshuname, offset, canshuzu + "->" + fhxin, 4);
                        offset += 4;
                    }
                    else 
                    {
                        errIn += "line " + el + "\t变量 " + name + " 重复声明\r\n";
                    }
                    break;
                default:
                    break;
            }


        }
    }
}
