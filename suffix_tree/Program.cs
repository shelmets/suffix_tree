using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace suffix_tree
{
    public class SuffixTree
    {
        int[] width = new int[1000];// for Tostring()
        int[] height = new int[1000];// for Tostring()
        const int MAXLEN = 600000;
        string s;
        int[] pos = new int[MAXLEN];
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
            for (int c = 0; c < 256; c++)
                (link[0]).Add((char)c,1);
            s = st;
        }
        void attach(int child, int parent, char c, int child_len)
        {
            if (to[parent].Keys.Contains(c))
                to[parent][c] = child;
            else
                to[parent].Add(c,child);
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
            if (peak>1)
                foreach (var ch in s.Skip(pos[peak] - len[peak]).Take(len[peak]))
                {
                    text += ch;
                }
            text+= string.Format("({0})", peak);
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
    }
    class Program
    {
        static void Main(string[] args)
        {
            string s = "abababasdsdfasdf";
            SuffixTree a = new SuffixTree(s);
            for (int i = s.Length - 1; i >= 0; i--)
                a.extend(i);
            Console.WriteLine("Суффиксное дерево строки '{0}'\n",s);
            Console.WriteLine(a.ToString(1));
        }
    }
}
