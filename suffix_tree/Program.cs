using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace suffix_tree1
{
    public class WordCount : IComparable
    {
        public string Word;
        public long Count;
        public WordCount(string w, long c)
        {
            Word = w;
            Count = c;
        }
        public void Increase(int count)
        {
            Count += count;
        }
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (obj is WordCount)
                return -Count.CompareTo(((WordCount)obj).Count);
            else
                throw new InvalidOperationException();
        }
        public override string ToString()
        {
            return Word + " - " + Count;
        }
    }

    public class SuffixTree
    {
        List<string> DiffrentWords = new List<string>(MAXLEN);
        int[] width = new int[1000];// for Tostring()
        int[] height = new int[1000];// for Tostring()
        const int MAXLEN = 600000;
        string s;
        int[] count = new int[MAXLEN];
        int[] pos = new int[MAXLEN];
        int[] gt = new int[MAXLEN];
        int[] len = new int[MAXLEN];
        int[] par = new int[MAXLEN];
        Dictionary<char, int>[] to = new Dictionary<char, int>[MAXLEN];
        Dictionary<char, int>[] link = new Dictionary<char, int>[MAXLEN];
        int sz = 2;
        int[] path = new int[MAXLEN];
        public SuffixTree(string st)
        {
            for (int i = 0; i < MAXLEN; i++)
            {
                to[i] = new Dictionary<char, int>();
                link[i] = new Dictionary<char, int>();
            }
            len[1] = 1; pos[1] = 0; par[1] = 0;
            for (int c = 0; c < 1104; c++)
                (link[0]).Add((char)c, 1);
            s = st;
        }
        void attach(int child, int parent, char c, int child_len)
        {
            if (to[parent].Keys.Contains(c))
                to[parent][c] = child;
            else
                to[parent].Add(c, child);
            len[child] = child_len;
            par[child] = parent;
        }
        public void extend(int i)
        {
            int v, vlen = s.Length - i, old = sz - 1, pstk = 0;
            for (v = old; !link[v].ContainsKey(s[i]); v = par[v])
            {
                vlen -= len[v];
                path[pstk++] = v;
            }
            int w = link[v][s[i]];
            if (to[w].ContainsKey(s[i + vlen]))
            {
                int u = to[w][s[i + vlen]];
                for (pos[sz] = pos[u] - len[u]; s[pos[sz]] == s[i + vlen]; pos[sz] += len[v])
                {
                    v = path[--pstk];
                    vlen += len[v];
                }
                attach(sz, w, s[pos[u] - len[u]], len[u] - (pos[u] - pos[sz]));
                attach(u, sz, s[pos[sz]], pos[u] - pos[sz]);
                w = link[v][s[i]] = sz++;
            }
            link[old][s[i]] = sz;
            attach(sz, w, s[i + vlen], s.Length - (i + vlen));
            pos[sz++] = s.Length;
        }
        public string ToString(int peak)
        {
            string str = "";
            string text = "";
            if (peak > 1)
                text = Program.Slice(s, pos[peak] - len[peak], pos[peak] - 1);
            text += string.Format("({0})", peak);
            str += text;
            int size = (int)to[peak].Values.LongCount();
            width[peak] += text.Length - 1;
            height[width[peak]] = 1;
            int i = 0;
            foreach (char k in to[peak].Keys)
            {
                if (i != size - 1)
                {
                    str += "\n";
                    for (int j = 0; j < width[peak]; j++)
                    {
                        if (height[j] == 0)
                            str += " ";
                        else
                            str += "│";
                    }
                    width[to[peak][k]] += width[peak] + 1;
                    str += "├" + ToString(to[peak][k]);
                }
                else
                {
                    str += "\n";
                    for (int j = 0; j < width[peak]; j++)
                    {
                        if (height[j] == 0)
                            str += " ";
                        else
                            str += "│";
                    }
                    width[to[peak][k]] += width[peak] + 1;
                    height[width[peak]] = 0;
                    str += "└" + ToString(to[peak][k]);
                }
                i++;
            }
            height[width[peak]] = 0;
            return str;
        }
        public int Search(string k)
        {
            int i = 0;
            int n = 1;
            if (k.Length != 0)
                while (to[n].ContainsKey(k[i]))
                {
                    n = to[n][k[i]];
                    for (int j = pos[n] - len[n]; j < pos[n]; j++, i++)
                    {
                        if (k[i] != s[j])
                            return -1;
                        if (i == k.Length - 1)
                        {
                            return n;
                        }
                    }
                }
            return -1;
            throw new ArgumentException("argument(string) empty");
        }
        public int SearchCount(string k)
        {
            int i = 0;
            int n = 1;
            if (k.Length != 0)
                while (to[n].ContainsKey(k[i]))
                {
                    n = to[n][k[i]];
                    for (int j = pos[n] - len[n]; j < pos[n]; j++, i++)
                    {
                        if (k[i] != s[j])
                            return 0;
                        if (i == k.Length - 1)
                        {
                            int buff = 1;
                            CountLists(ref buff, n);
                            return buff;
                        }
                    }
                }
            return 0;
            throw new ArgumentException("argument(string) empty");
        }
        private void Build()
        {
            string str = "";
            for (int i = s.Length - 1; i >= 0; i--)
            {
                if (s[i] != ' ')
                {
                    str = s[i] + str;
                    if (i == 0 || s[i - 1] == ' ')
                    {
                        int n = Search(str);
                        extend(i);
                        if (n == -1)
                        {
                            DiffrentWords.Add(str);
                            count[sz - 1] = 1;
                        }
                        else
                            count[n] += 1;
                        str = "";
                        continue;
                    }
                }
                extend(i);
            }
        }
        public void CountLists(ref int buff, int root)
        {
            foreach (int k in to[root].Values)
            {
                if (to[k].Keys.Count == 0)
                    buff++;
                else
                {
                    buff++;
                    CountLists(ref buff, k);
                }
            }
        }
        public void Option1()
        {

            int j = 0;
            DateTime t1 = DateTime.Now;
            Build();
            Console.WriteLine("ПОСТРОЕНИЕ ДЕРЕВА ЗА {0}", DateTime.Now - t1);
            t1 = DateTime.Now;
            Console.WriteLine(ToString(1));
            Console.WriteLine("ВЫВОД ДЕРЕВА ЗА {0}", DateTime.Now - t1);
            foreach (var i in DiffrentWords)
            {
                Console.WriteLine(i);
            }
            foreach (var i in DiffrentWords)
            {
                Console.WriteLine("{0} - {1}", i, count[Search(i)]);
            }
        }
    }
    class Program
    {
        static public string Slice(string s, int a, int b)
        {
            string str = "";
            if (a <= b && s.Length > b)
            {
                for (int i = a; i <= b; i++)
                    str += s[i];
                return str;
            }
            else
                throw new ArgumentException("must a<=b and len(s)>b ");
        }
        static void Main(string[] args)
        {
            string s = "мойка пол садовод должен как сад садовод должен как новый садовод должен как садовод должен как";
            DateTime t1 = DateTime.Now;
            SuffixTree a = new SuffixTree(s);
            a.Option1();
        }
    }
}
