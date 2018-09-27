using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
namespace LEX
{
    struct TokenStruct
    {
        String word;       
        int id;            
        String descri;
        public String getWord()
        {
            return word;
        }
        public int getId()
        {
            return id;
        }
        public String getDecri()
        {
            return descri;
        }
        public TokenStruct(String word, int id)
        {
            this.word = word;
            this.id = id;
            this.descri = "";
        }

        public TokenStruct(String word, int id, String descri)
        {
            this.word = word;
            this.id = id;
            this.descri = descri;
        }
    }

    //词法分析类
    class Lex
    {
        Help help;
        TokenStruct charConst;                     //字符常量
        TokenStruct stringConst;                   //字符串常量
        TokenStruct floatConst;                    //浮点数常量
        TokenStruct note;
        TokenStruct eighth;
        TokenStruct sixth;
        TokenStruct[] singleDelimiters = new TokenStruct[30];
        TokenStruct[] dualDelimiters = new TokenStruct[50];
        TokenStruct identify;                       //标识符
        TokenStruct[] keyword = new TokenStruct[50]; //关键字
        TokenStruct intConst;                      //整型常量
        
        
        

        //词法分析类构造方法
        public Lex()
        {

            

            identify = new TokenStruct("IDN", 1, "identify");   
            intConst = new TokenStruct("INT", 2, "int");
            charConst = new TokenStruct("CHAR", 3, "char");


            floatConst = new TokenStruct("FLOAT", 4, "float");
            stringConst = new TokenStruct("STRING", 5, "string");
            note = new TokenStruct("Note", 0, "注释");


            eighth = new TokenStruct("Eight", -88, "八进制");
            sixth = new TokenStruct("Six", -66, "十六进制");
            help = new Help();


            StreamReader r = new StreamReader(@"..\\..\\tool\\keyword.txt");

            String temp;
            for (int i = 0; i < 50; i++)
            {
                temp = r.ReadLine();
                if (temp == null || temp.Equals(""))
                {
                    break;
                }
                keyword[i] = new TokenStruct(temp, 10 + i, temp); 
            }
            r.Close();

            

            r = new StreamReader(@"..\\..\\tool\\singleDelimiters.txt");
            String[] s;
            for (int i = 0; i < 30; i++)
            {
                temp = r.ReadLine();
                if (temp == null || temp.Equals(""))
                {
                    break;
                }
               
                s = temp.Split(new char[] { '\t' });
                singleDelimiters[i] = new TokenStruct(s[0], 60 + i, s[1]); 
            }
            r.Close();

           
             
            r = new StreamReader(@"..\\..\\tool\\dualDelimiters.txt");
            for (int i = 0; i < 50; i++)
            {
                temp = r.ReadLine();
                if (temp == null || temp.Equals(""))
                {
                    break;
                }

                s = temp.Split(new char[] { '\t' });
                dualDelimiters[i] = new TokenStruct(s[0], 90 + i, s[1]);
            }
            r.Close();

        }
                
        


        
        //词法分析
        public void lexexe(String code, out String phrase, out String errIn, out String dfa)
        {
            phrase = "";
            errIn = "";
            dfa = "";



            char[] chars = code.ToCharArray();
            int state = 0;
            String nowWord = "";
            int line = 1;



            for (int i = 0; i < chars.Length;)
            {
                if (i > 0 && '\n' == chars[i - 1])
                {
                    line++;
                }
                


                if (0 == state)
                {
                    if (help.isLetter(chars[i]) || '_' == chars[i])
                    {
                      
                        nowWord += chars[i];
                        i++;
                        state = 1;


                    }
                    else if ('"' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 21;


                    }
                    else if ('\'' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 2;

                    }
                   
                    else if (help.isNumber(chars[i]))
                    { 
                      
                            nowWord += chars[i];
                            i++;
                            state = 5;
                        
                    }
                    else if ('/' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 11;
                    }
                    else if (help.isChar(chars[i]))
                    {
                        i++;
                    }
                    else
                    {
                        if (help.isSingleBorders(chars[i]))
                        {
                            nowWord += chars[i];
                            i++;
                            int sKey = findSingleBorders(nowWord);
                            if (-1 != sKey)
                            {
                                phrase += getTokenOut(line, nowWord, singleDelimiters[sKey]);
                                dfa+=printdfa(line, nowWord, singleDelimiters[sKey]);
                            }
                            else           
                            {
                                errIn += getTokenOut(line, nowWord, help.error(state));
                            }
                            state = 0;
                            nowWord = "";


                        }
                        else
                        {
                            nowWord += chars[i];
                            i++;
                            state = 19;
                        }
                    }
                }



                else if (1 == state)      
                {
                    if (help.isNumber(chars[i]) || help.isLetter(chars[i]) || '_' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                    }
                    else                 
                    {
                        state = 0;
                        int keyI = isKeyWord(nowWord);



                        if (keyI == -1)
                        {
                            phrase += getTokenOut(line, nowWord, identify);
                            dfa +=printdfa(line, nowWord, identify);
                        }
                        else
                        {
                            phrase += getTokenOut(line, nowWord, keyword[keyI]);
                            dfa +=printdfa(line, nowWord, keyword[keyI]);
                        }
                        nowWord = "";
                    }
                }



                else if (2 == state)
                {
                    if (32 <= chars[i] && 126 >= chars[i] && 39 != chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 3;
                    }
                    else


                    {
                        


                        while (i < chars.Length && '\n' != chars[i] && '\'' != chars[i])
                        {
                            nowWord += chars[i];
                            i++;
                        }
                        if (i < chars.Length)
                        {
                            if ('\n' == chars[i])
                            {
                                i++;
                            }
                            else if ('\'' == chars[i])
                            {
                                nowWord += chars[i];
                                i++;
                            }
                        }
                        if (nowWord.Length <= 3)
                        {
                            errIn += getTokenOut(line, nowWord, help.error(state, 0));
                        }
                        else
                        {
                            errIn += getTokenOut(line, nowWord, help.error(state, 1));
                        }
                        state = 0;
                        nowWord = "";
                    }
                }


                else if (3 == state)
                {
                    if ('\'' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 4;
                    }




                    else
                    {
                        
                        while (i < chars.Length && '\n' != chars[i] && '\'' != chars[i])
                        {
                            nowWord += chars[i];
                            i++;
                        }
                        if (i < chars.Length)
                        {
                            if ('\n' == chars[i])
                            {
                                i++;
                                errIn += getTokenOut(line, nowWord, help.error(state));//直到换行也没找到单引号
                            }
                            else if ('\'' == chars[i])
                            {
                                nowWord += chars[i];
                                i++;
                                errIn += getTokenOut(line, nowWord, help.error(state, 1));//已找到单引号
                            }
                        }
                        nowWord = "";
                        state = 0;
                    }
                }





                else if (4 == state)
                {
                    phrase += getTokenOut(line, nowWord, charConst);
                    dfa +=printdfa(line, nowWord, charConst);
                    state = 0;
                    nowWord = "";
                }





                else if (5 == state)
                {

                    if (help.isNumber(chars[i]))
                    {
                        nowWord += chars[i];
                        i++;
                    }



                    else if ('.' == chars[i])

                    {
                        nowWord += chars[i];
                        i++;
                        state = 7;
                    }
                    else if ('e' == chars[i] || 'E' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 8;
                    }



                    else if (chars[i] == 'x' && nowWord[0] == '0')
                    {

                        nowWord += chars[i];
                        i++;
                        for (; i < chars.Length;)
                        {
                            if ((chars[i] <= '9' && '0' <= chars[i]) || (chars[i] <= 'F' && chars[i] >= 'A'))
                            {
                                nowWord += chars[i];
                                i++;
                            }
                            else
                            {
                                
                                    phrase += getTokenOut(line, nowWord, intConst);
                                    dfa += printdfa(line, nowWord, sixth);
                                    nowWord = "";
                                    state = 0;
                                    break;
                                
                            }                          
                        }

                    }





                    else if (help.isChar(chars[i]) || help.isBorders(chars[i]))
                    {
                        int k = 1;
                        if (nowWord[0] == '0')
                        k = help.eight(nowWord);
                        


                        if (k == 0&&nowWord!="0")
                        {
                            errIn += getTokenOut(line, nowWord, help.error(7));
                            state = 0;
                            nowWord = "";
                            
                        }
                      

                        if (k != 0||nowWord=="0")
                        {
                            
                            phrase += getTokenOut(line, nowWord, intConst);
                            if(nowWord[0]=='0')
                            dfa += printdfa(line, nowWord, eighth);
                            if (nowWord[0] != '0')
                            dfa += printdfa(line, nowWord, intConst);
                            //MessageBox.Show(""+k);
                            nowWord = "";
                            state = 0;
                        }
                    }





                    else
                    {
                        while (i < chars.Length && !help.isBorders(chars[i]) && !help.isChar(chars[i]))
                        {
                            nowWord += chars[i];
                            i++;
                        }

                        errIn += getTokenOut(line, nowWord, help.error(state));
                        nowWord = "";
                        state = 0;
                    }
                }


                else if (7 == state)
                {
                    if (help.isNumber(chars[i]))
                    {
                        nowWord += chars[i];
                        i++;
                    }
                    else if ('e' == chars[i] || 'E' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 8;
                    }
                    else if (help.isChar(chars[i]) || help.isBorders(chars[i]) && '.' != chars[i])//浮点数接收完毕
                    {
                        phrase += getTokenOut(line, nowWord, floatConst);
                        dfa +=printdfa(line, nowWord, floatConst);
                        nowWord = "";
                        state = 0;
                    }
                    else
                    {
                        while (i < chars.Length && !help.isBorders(chars[i]) && !help.isChar(chars[i]))
                        {
                            nowWord += chars[i];
                            i++;
                        }

                        errIn += getTokenOut(line, nowWord, help.error(state));
                        nowWord = "";
                        state = 0;
                    }
                }


                else if (8 == state)
                {
                    if (help.isNumber(chars[i]))
                    {
                        nowWord += chars[i];
                        i++;
                        state = 10;
                    }
                    else if ('+' == chars[i] || '-' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 9;
                    }
                    else
                    {
                        while (i < chars.Length && !help.isBorders(chars[i]) && !help.isChar(chars[i]))
                        {
                            nowWord += chars[i];
                            i++;
                        }

                        errIn += getTokenOut(line, nowWord, help.error(state));
                        nowWord = "";
                        state = 0;
                    }
                }
                else if (9 == state)
                {
                    if (help.isNumber(chars[i]))
                    {
                        nowWord += chars[i];
                        i++;
                        state = 10;
                    }
                    else
                    {
                        while (i < chars.Length && !help.isBorders(chars[i]) && !help.isChar(chars[i]))
                        {
                            nowWord += chars[i];
                            i++;
                        }

                        errIn += getTokenOut(line, nowWord, help.error(state));
                        nowWord = "";
                        state = 0;
                    }
                }
                else if (10 == state)
                {
                    if (help.isNumber(chars[i]))
                    {
                        nowWord += chars[i];
                        i++;
                    }
                    else if (help.isChar(chars[i]) || help.isBorders(chars[i]))


                   
                    {
                        phrase += getTokenOut(line, nowWord, floatConst);
                        dfa += printdfa(line, nowWord, floatConst);
                        nowWord = "";
                        state = 0;
                    }
                    else
                    {
                        while (i < chars.Length && !help.isBorders(chars[i]) && !help.isChar(chars[i]))
                        {
                            nowWord += chars[i];
                            i++;
                        }

                        errIn += getTokenOut(line, nowWord, help.error(state));
                        nowWord = "";
                        state = 0;
                    }
                }
                else if (11 == state)
                {
                    if ('/' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 12;
                    }
                    else if ('*' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 15;
                    }
                    else if (help.isBorders(chars[i]))
                    {
                        nowWord += chars[i];
                        i++;
                        state = 20;
                    }



                    else if (help.isNumber(chars[i]) || help.isLetter(chars[i]) || help.isChar(chars[i]))//为除号
                    {
                        int sKey = findSingleBorders("/");
                        phrase += getTokenOut(line, nowWord, singleDelimiters[sKey]);
                        dfa +=printdfa(line, nowWord, singleDelimiters[sKey]);
                        state = 0;
                        nowWord = "";
                    }
                }
                else if (12 == state)


                {
                    if ('\n' == chars[i])
                    {
                        state = 13;
                        i++;
                    }
                    else
                    {
                        nowWord += chars[i];
                        i++;
                        state = 12;
                    }
                }


                else if (13 == state)
                {
                    //phrase += "line " + line + "\t\t//注释\r\n";
                    nowWord = "";
                    state = 0;
                    //MessageBox.Show("phrase");
                    phrase += getTokenOut(line, nowWord, note);
                    dfa +=printdfa(line, nowWord, note);
                }


                else if (15 == state)

                {
                    if ('*' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 17;
                    }
                    else
                    {
                        nowWord += chars[i];
                        i++;
                    }
                }
                else if (17 == state)//等待*/中的/
                {
                    if ('/' == chars[i])// 结束注释*/
                    {
                        nowWord += chars[i];
                        i++;
                        state = 18;
                    }
                    else if ('*' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 17;
                    }
                    else
                    {
                        nowWord += chars[i];
                        i++;
                        state = 15;
                    }
                }
                else if (18 == state)
                {
                    //phrase += "line " + line + "\t\t结束注释*/\r\n";
                    nowWord = "";
                    state = 0;
                    //MessageBox.Show("phrase");
                    phrase += getTokenOut(line, nowWord, note);
                    dfa +=printdfa(line, nowWord, note);

                }
                else if (19 == state)



                {
                    if (help.isNumber(chars[i]) || help.isLetter(chars[i]) || help.isChar(chars[i]))//确定为单界符
                    {
                        int sKey = findSingleBorders(nowWord);
                        if (-1 != sKey)
                        {
                            phrase += getTokenOut(line, nowWord, singleDelimiters[sKey]);
                            dfa +=printdfa(line, nowWord, singleDelimiters[sKey]);
                        }
                        else


                        {
                            errIn += getTokenOut(line, nowWord, help.error(state));
                        }
                        state = 0;
                        nowWord = "";
                    }
                    else

                    {
                        nowWord += chars[i];
                        i++;
                        state = 20;
                    }
                }
                else if (20 == state)
                {
                    int dKey = findDualBorders(nowWord);
                    if (-1 != dKey)


                    {
                        phrase += getTokenOut(line, nowWord, dualDelimiters[dKey]);
                        dfa +=printdfa(line, nowWord, dualDelimiters[dKey]);
                    }
                    else


                    {
                        int sKey = findSingleBorders(nowWord.Substring(0, 1));
                        if (-1 != sKey)


                        {
                            phrase += getTokenOut(line, nowWord.Substring(0, 1), singleDelimiters[sKey]);
                            dfa +=printdfa(line, nowWord.Substring(0, 1), singleDelimiters[sKey]);
                        }
                        else


                        {
                            errIn += getTokenOut(line, nowWord.Substring(0, 1), help.error(state));
                        }
                        if ('"' == nowWord.ToCharArray()[1])
                        {
                            state = 21;
                            nowWord = "" + '"';
                            continue;
                        }
                        else if ('/' == nowWord.ToCharArray()[1])
                        {
                            state = 11;
                            nowWord = "" + '/';
                            continue;
                        }
                        else if ('\'' == nowWord.ToCharArray()[1])
                        {
                            state = 2;
                            nowWord = "'";
                            continue;
                        }
                        else
                        {
                            state = 19;
                            nowWord = "" + chars[i - 1];
                            continue;
                        }
                    }
                    state = 0;
                    nowWord = "";
                }
                else if (21 == state)
                {
                    if ('"' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 23;
                    }
                    else
                    {
                        nowWord += chars[i];
                        i++;
                        state = 22;
                    }
                }
                else if (22 == state)
                {
                    if ('"' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 23;
                    }
                    else if ('\n' == chars[i])
                    {
                        //nowWord += chars[i];
                        i++;
                        errIn += getTokenOut(line, nowWord, help.error(state));
                        state = 0;
                    }
                    else
                    {
                        nowWord += chars[i];
                        i++;
                    }
                }
                else if (23 == state)
                {
                    phrase += getTokenOut(line, nowWord, stringConst);
                    dfa += printdfa(line, nowWord, stringConst);
                    state = 0;
                    nowWord = "";
                }
                else
                {
                    state = 0;
                    nowWord = "";
                }

            }




            //处理剩余单词
            if (!nowWord.Equals(""))
            {
                if (0 == state)
                {
                    phrase += getTokenOut(line, nowWord, note);
                    dfa += printdfa(line, nowWord, note);
                    //MessageBox.Show("phrase");
                }
                if (1 == state)
                {
                    int keyI = isKeyWord(nowWord);
                    if (keyI == -1)
                    {
                        phrase += getTokenOut(line, nowWord, identify);
                        //MessageBox.Show("s");
                        dfa += printdfa(line, nowWord, identify);
                    }
                    else
                    {
                        phrase += getTokenOut(line, nowWord, keyword[keyI]);
                        dfa += printdfa(line, nowWord, keyword[keyI]);
                    }
                }
                else if (2 == state)
                {
                    errIn += getTokenOut(line, nowWord, help.error(state));
                }
                else if (3 == state)
                {
                    errIn += getTokenOut(line, nowWord, help.error(state));
                }
                else if (4 == state)
                {
                    phrase += getTokenOut(line, nowWord, charConst);
                    dfa += printdfa(line, nowWord, charConst);
                }
                else if (5 == state)
                {
                    int k = 1;
                    if (nowWord[0] == '0'&&nowWord.Length>=2&&nowWord[1]!='x')
                        k = help.eight(nowWord);
                    if (k == 0)
                    {
                        errIn += getTokenOut(line, nowWord, help.error(7));
                        state = 0;
                        nowWord = "";
                        //nowWord = "";
                    }


                    if (k != 0)
                    {

                        phrase += getTokenOut(line, nowWord, intConst);
                        if (nowWord[0] == '0'&&nowWord.Length>=2&&nowWord[1]!='x')
                            dfa += printdfa(line, nowWord, eighth);
                        else if(nowWord[0] == '0' && nowWord.Length>=2&&nowWord[1] == 'x')
                            dfa += printdfa(line, nowWord, sixth);
                        else
                            dfa += printdfa(line, nowWord, intConst);
                        //MessageBox.Show(""+k);
                        nowWord = "";
                        state = 0;
                    }
                }
                else if (7 == state)
                {
                    errIn += getTokenOut(line, nowWord, help.error(state));
                }
                else if (8 == state)
                {
                    errIn += getTokenOut(line, nowWord, help.error(state));
                }
                else if (9 == state)
                {
                    errIn += getTokenOut(line, nowWord, help.error(state));
                }
                else if (10 == state)
                {
                    phrase += getTokenOut(line, nowWord, floatConst);
                    dfa += printdfa(line, nowWord, floatConst);
                    //MessageBox.Show("s");
                }
                else if (11 == state)
                {
                    int sKey = findSingleBorders("/");
                    phrase += getTokenOut(line, nowWord, singleDelimiters[sKey]);
                    dfa += printdfa(line, nowWord, singleDelimiters[sKey]);
                }
                else if (12 == state)
                {
                    //phrase += "line " + line + "\t\t//注释\r\n";
                    phrase += getTokenOut(line, nowWord, note);
                    //MessageBox.Show("phrase");
                    dfa += printdfa(line, nowWord, note);
                }


                else if (13 == state)
                {
                    //phrase += "line " + line + "\t\t//注释\r\n";
                    phrase += getTokenOut(line, nowWord, note);
                    dfa += printdfa(line, nowWord, note);
                }
                else if (15 == state)
                {
                    errIn += "line " + line + "\t\t未找到对应*/就结束了\r\n";
                }
                else if (17 == state)
                {
                    errIn += "line " + line + "\t\t未找到对应*/就结束了\r\n";
                }
                else if (18 == state)
                {
                    //phrase += "line " + line + "\t\t结束注释*/\r\n";
                    phrase += getTokenOut(line, nowWord, note);
                    dfa += printdfa(line, nowWord, note);
                }
                else if (19 == state)
                {
                    int sKey = findSingleBorders(nowWord);
                    if (-1 != sKey)
                    {
                        phrase += getTokenOut(line, nowWord, singleDelimiters[sKey]);
                        dfa += printdfa(line, nowWord, singleDelimiters[sKey]);
                    }
                    else


                    {
                        errIn += getTokenOut(line, nowWord, help.error(state));
                    }
                }
                else if (20 == state)
                {
                    int dKey = findDualBorders(nowWord);
                    if (-1 != dKey)

                    {
                        phrase += getTokenOut(line, nowWord, dualDelimiters[dKey]);
                        dfa += printdfa(line, nowWord, dualDelimiters[dKey]);
                    }
                    else

                    {
                        int sKey = findSingleBorders(nowWord.Substring(0, 1));
                        if (-1 != sKey)
                        {
                            phrase += getTokenOut(line, nowWord.Substring(0, 1), singleDelimiters[sKey]);
                            dfa += printdfa(line, nowWord.Substring(0, 1), singleDelimiters[sKey]);
                        }
                        else

                        {
                            errIn += getTokenOut(line, nowWord.Substring(0, 1), help.error(state));
                        }
                        sKey = findSingleBorders(nowWord.Substring(1, 1));
                        if (-1 != sKey)

                        {
                            phrase += getTokenOut(line, nowWord.Substring(1, 1), singleDelimiters[sKey]);
                            dfa += printdfa(line, nowWord.Substring(1, 1), singleDelimiters[sKey]);
                        }
                        else

                        {
                            errIn += getTokenOut(line, nowWord.Substring(1, 1), help.error(state));
                        }
                    }
                }
                else if (21 == state)
                {
                    errIn += getTokenOut(line, nowWord, help.error(state));
                }
                else if (22 == state)
                {
                    errIn += getTokenOut(line, nowWord, help.error(state));
                }
                else if (23 == state)
                {
                    phrase += getTokenOut(line, nowWord, stringConst);
                    dfa += printdfa(line, nowWord, stringConst);
                }
                else
                {
                }
                
            }
        }      
       
        
        String getTokenOut(int line, String nowWord, TokenStruct w)
        {
            if(w.getId()==-88||w.getId()==-66)
                return "line " + line + "\t" + nowWord + "\t( " + w.getWord() + ", " + w.getId() + ", " + w.getDecri() + ")\r\n";

            else if (w.getId() == 0)
                return "line " + line + "\t" + "注释" + "\t( " + w.getWord() + ", " + w.getId() + ", " + w.getDecri() + ")\r\n";
            else if (w.getId() <= 5)
                return "line " + line + "\t" + nowWord + "\t( " + w.getWord() + ", " + w.getId() + ", " + w.getDecri() + ")\r\n";
            else
                return "line " + line + "\t" + nowWord + "\t( " + w.getWord() + ", " + w.getId() + ", " + w.getDecri() + ")\r\n";
        }
        String printdfa(int line, String nowWord, TokenStruct w)
        {
            String df ="";
            int id = w.getId();
            if (id == -88)
                df = "< 0,0,7 >< 7,1 - 7,8 >< 8,0 - 7,8 >";
            else if (id == -66)
                df = "< 0,0,7 >< 7,x,9 >< 9,1 - 9a - f,10 >< 10,0 - 9a - f,10 >";
            else if (id == 0)
                df = "< 0,/,3 >< 3,*,4 >< 4,其他,4 >< 4,*,5 >< 5,其他,4 >< 5,*,5 >< 5,/,6 >";
            else if (id == 1 || ((id >= 10) && (id <= 59)))
                df = "< 0,letter_,1 >< 1,letter_,1 >< 1,digit,1 >";
            else if (id >= 90 && id <= 139)
                df = "< 0,op,20 >< 1,op,21 >";
            else if (id >= 60 && id <= 89)
            {
                int iso = 0;
                iso = isOperate(nowWord);
                if (iso != -1)
                    df = "< 0,op,20 >< 1,op,21 >";
                else
                    df = "< 0,界符,2 >";

            }
            else if (id == 2 || id == 4)
                df = "<0,digit,11><11,digit,11><11,.,12><11,E,14><12,digit,13><13,digit,13><13,E,14><14,+-,15><14,digit,16><15,digit,16><16,digit,16>";
            else if (id == 3)
                df = "<0,',17><17,letter_,18><18,',19>";
            else
                df = "";
            String dfa = "line " + line + "\t" + nowWord + "\t" + df + "\r\n";
            return dfa;
        }
        String getTokenOut(int line, String nowWord, String error)
        {
            return "line " + line + "\t" + nowWord + "\t" + error + "\r\n";
        }
        int isOperate(String word)
        {
            int t = 0;
            for (int i = 0; i < 12; i++)
            {
                try
                {
                    t = singleDelimiters[i].getWord().CompareTo(word);
                }
                catch (NullReferenceException)
                {
                    return -1;
                }
                if (0 == t)
                {
                    return i;
                }
            }
            return -1;
        }
        //查找关键字
        int isKeyWord(String word)
        {
            int t = 0;
            for (int i = 0; i < 50; i++)
            {
                try
                {
                    t = keyword[i].getWord().CompareTo(word);
                }
                catch (NullReferenceException)
                {
                    return -1;
                }
                if (0 == t)
                {
                    return i;
                }
                else if (0 < t || keyword[i].getWord().Equals(""))
                {
                    return -1;
                }
            }
            return -1;
        }
        //查找双界符
        int findDualBorders(String word)
        {
            int t = 0;
            for (int i = 0; i < 30; i++)
            {
                try
                {
                    t = dualDelimiters[i].getWord().CompareTo(word);
                }
                catch (NullReferenceException)
                {
                    return -1;
                }
                if (0 == t)
                {
                    return i;
                }
            }
            return -1;
        }
        //查找单界符
        int findSingleBorders(String word)
        {
            int t = 0;
            for (int i = 0; i < 30; i++)
            {
                try
                {
                    t = singleDelimiters[i].getWord().CompareTo(word);
                }
                catch (NullReferenceException)
                {
                    return -1;
                }
                if (0 == t)
                {
                    return i;
                }
            }
            return -1;
        }

    }
}
