using Compile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LEX
{
    class Help
    {
        public Help()
        {


        }

        //八进制判断
        public int eight(String nowWord)
        {


            int i = 0;
            char[] words = nowWord.ToCharArray();
            if (!words[0].Equals('0'))
                return 0;

            else if (words.Length >= 2 && words[1] <= '7')
            {
                for (i = 2; i < words.Length; i++)
                {
                    if (words[i] > '7')
                        return 0;

                }
                return 1;
            }
            return 0;
        }



        public Boolean isSingleBorders(char c)
        {
            if (',' == c || '.' == c || ';' == c || '[' == c || '{' == c || '}' == c || ']' == c)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Boolean isBorders(char c)
        {
            if (33 <= c && 47 >= c || 58 <= c && 64 >= c || 91 <= c && 96 >= c || 123 <= c && 126 >= c)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean isChar(char c)
        {
            if (' ' == c || '\t' == c || '\n' == c || '\r' == c)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Boolean isLetter(char c)
        {
            if (c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z')
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Boolean isNumber(char c)
        {
            if (c <= '9' && c >= '0')
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public String error(int state, int n = 0)
        {
            switch (state)
            {
                case 1:
                    return "错误：请按规则构造标识符";



                case 2:
                    if (0 == n)
                    {
                        return "错误：找不到可识别的字符";
                    }
                    else
                    {
                        return "错误：找不到可识别的字符，且字符常量中的字符太多";
                    }


                case 3:
                    if (0 == n)
                    {
                        return "错误：找不到右引号";
                    }
                    else
                    {
                        return "错误：字符常量中的字符太多";
                    }
                case 5:
                    return "错误：整数拼写错误";
                case 7:
                    return "错误：数字拼写错误";
                case 8:
                    return "错误：数字拼写错误";
                case 9:
                    return "错误：数字拼写错误";
                case 19:
                    return "错误：不可识别的单界符";
                case 20:
                    return "错误：不可识别的双界符";
                case 21:
                    return "错误：缺少对应的双引号";
                case 22:
                    return "错误：缺少对应的双引号";
                default:
                    return "错误：系统不可识别的错误";
            }



            //return "系统不可识别错误";
        }

    }
}
namespace Compile
{
    //产生式类
    class Product
    {
        String left;//左部
        ArrayList right;//右部
        ArrayList first;//first集
        ArrayList follow;//follow集
        ArrayList select;//select集
        public Product(String left, ArrayList right)
        {
            this.left = left;
            this.right = right;
            first = new ArrayList();
            follow = new ArrayList();
            select = new ArrayList();
        }
        public String getLeft()
        {
            return left;
        }
        public ArrayList getRight()
        {
            return right;
        }
        public void addSelect(ArrayList s)
        {
            foreach (Object o in s)
            {
                if (!this.select.Contains(o))
                {
                    this.select.Add(o);
                }
            }
        }
        public ArrayList getSelect()
        {
            return select;
        }
    }
    class WForm
    {
        //格式化终结符与非终结符
        public static String name(WordStruct w)
        {
            if (w.getId() < 300)
            {
                return w.getName();
            }
            else
            {
                return "<" + w.getName() + ">";
            }
        }
    }


}
